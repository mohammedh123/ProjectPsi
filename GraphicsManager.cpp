#include "GraphicsManager.h"

#include "VertexTypes.h"
#include "DDSTextureLoader.h"

using namespace DirectX;

GraphicsManager::GraphicsManager() : m_hInst(nullptr), m_hWnd(nullptr), 
    m_d3dDevice(nullptr), m_ImmediateContext(nullptr), m_SwapChain(nullptr), 
    m_RenderTargetView(nullptr), m_DepthStencil(nullptr), m_DepthStencilView(nullptr),
    m_TextureRV1(nullptr), m_TextureRV2(nullptr), m_BatchInputLayout(nullptr),
    m_DriverType(D3D_DRIVER_TYPE_NULL), m_FeatureLevel(D3D_FEATURE_LEVEL_11_0)
{}

bool GraphicsManager::Initialize( HINSTANCE hInstance, int nCmdShow )
{
    if(FAILED(InitWindow(hInstance, nCmdShow)))
        return false;

    if(FAILED(InitDevice()))
    {
        CleanupDevice();
        return false;
    }

    return true;
}

HRESULT GraphicsManager::InitWindow( HINSTANCE hInstance, int nCmdShow, WNDPROC loopFunc, const char* windowTitle )
{
    // Register class
    WNDCLASSEX wcex;
    wcex.cbSize = sizeof( WNDCLASSEX );
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = loopFunc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInstance;
    wcex.hCursor = LoadCursor( nullptr, IDC_ARROW );
    wcex.hbrBackground = ( HBRUSH )( COLOR_WINDOW + 1 );
    wcex.lpszMenuName = nullptr;
    wcex.lpszClassName = "AppWindow";
    if( !RegisterClassEx( &wcex ) )
        return E_FAIL;

    // Create window
    m_hInst = hInstance;
    RECT rc = { 0, 0, 640, 480 };
    AdjustWindowRect( &rc, WS_OVERLAPPEDWINDOW, FALSE );
    m_hWnd = CreateWindow( "AppWindow", windowTitle, WS_OVERLAPPEDWINDOW,
                           CW_USEDEFAULT, CW_USEDEFAULT, rc.right - rc.left, rc.bottom - rc.top, nullptr, nullptr, hInstance,
                           nullptr );
    if( !m_hWnd )
        return E_FAIL;

    ShowWindow( m_hWnd, nCmdShow );

    return S_OK;
}

HRESULT GraphicsManager::InitDevice()
{    
    HRESULT hr = S_OK;

    RECT rc;
    GetClientRect( m_hWnd, &rc );
    UINT width = rc.right - rc.left;
    UINT height = rc.bottom - rc.top;

    UINT createDeviceFlags = 0;
#ifdef _DEBUG
    createDeviceFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    D3D_DRIVER_TYPE driverTypes[] =
    {
        D3D_DRIVER_TYPE_HARDWARE,
        D3D_DRIVER_TYPE_WARP,
        D3D_DRIVER_TYPE_REFERENCE,
    };
    UINT numDriverTypes = ARRAYSIZE( driverTypes );

    D3D_FEATURE_LEVEL featureLevels[] =
    {
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
    };
    UINT numFeatureLevels = ARRAYSIZE( featureLevels );

    DXGI_SWAP_CHAIN_DESC sd;
    ZeroMemory( &sd, sizeof( sd ) );
    sd.BufferCount = 1;
    sd.BufferDesc.Width = width;
    sd.BufferDesc.Height = height;
    sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    sd.BufferDesc.RefreshRate.Numerator = 60;
    sd.BufferDesc.RefreshRate.Denominator = 1;
    sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    sd.OutputWindow = m_hWnd;
    sd.SampleDesc.Count = 1;
    sd.SampleDesc.Quality = 0;
    sd.Windowed = TRUE;

    for( UINT driverTypeIndex = 0; driverTypeIndex < numDriverTypes; driverTypeIndex++ )
    {
        m_DriverType = driverTypes[driverTypeIndex];
        hr = D3D11CreateDeviceAndSwapChain( nullptr, m_DriverType, nullptr, createDeviceFlags, featureLevels, numFeatureLevels,
                                            D3D11_SDK_VERSION, &sd, &m_SwapChain, &m_d3dDevice, &m_FeatureLevel, &m_ImmediateContext );
        if( SUCCEEDED( hr ) )
            break;
    }
    if( FAILED( hr ) )
        return hr;

    // Create a render target view
    ID3D11Texture2D* pBackBuffer = nullptr;
    hr = m_SwapChain->GetBuffer( 0, __uuidof( ID3D11Texture2D ), ( LPVOID* )&pBackBuffer );
    if( FAILED( hr ) )
        return hr;

    hr = m_d3dDevice->CreateRenderTargetView( pBackBuffer, nullptr, &m_RenderTargetView );
    pBackBuffer->Release();
    if( FAILED( hr ) )
        return hr;

    // Create depth stencil texture
    D3D11_TEXTURE2D_DESC descDepth;
    ZeroMemory( &descDepth, sizeof(descDepth) );
    descDepth.Width = width;
    descDepth.Height = height;
    descDepth.MipLevels = 1;
    descDepth.ArraySize = 1;
    descDepth.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
    descDepth.SampleDesc.Count = 1;
    descDepth.SampleDesc.Quality = 0;
    descDepth.Usage = D3D11_USAGE_DEFAULT;
    descDepth.BindFlags = D3D11_BIND_DEPTH_STENCIL;
    descDepth.CPUAccessFlags = 0;
    descDepth.MiscFlags = 0;
    hr = m_d3dDevice->CreateTexture2D( &descDepth, nullptr, &m_DepthStencil );
    if( FAILED( hr ) )
        return hr;

    // Create the depth stencil view
    D3D11_DEPTH_STENCIL_VIEW_DESC descDSV;
    ZeroMemory( &descDSV, sizeof(descDSV) );
    descDSV.Format = descDepth.Format;
    descDSV.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
    descDSV.Texture2D.MipSlice = 0;
    hr = m_d3dDevice->CreateDepthStencilView( m_DepthStencil, &descDSV, &m_DepthStencilView );
    if( FAILED( hr ) )
        return hr;

    m_ImmediateContext->OMSetRenderTargets( 1, &m_RenderTargetView, m_DepthStencilView );

    // Setup the viewport
    D3D11_VIEWPORT vp;
    vp.Width = (FLOAT)width;
    vp.Height = (FLOAT)height;
    vp.MinDepth = 0.0f;
    vp.MaxDepth = 1.0f;
    vp.TopLeftX = 0;
    vp.TopLeftY = 0;
    m_ImmediateContext->RSSetViewports( 1, &vp );

    // Create DirectXTK objects
    m_States.reset( new CommonStates( m_d3dDevice ) );
    m_Sprites.reset( new SpriteBatch( m_ImmediateContext ) );
    m_FXFactory.reset( new EffectFactory( m_d3dDevice ) );

    m_BatchEffect.reset( new BasicEffect( m_d3dDevice ) );
    m_BatchEffect->SetVertexColorEnabled(true);

    {
        void const* shaderByteCode;
        size_t byteCodeLength;

        m_BatchEffect->GetVertexShaderBytecode( &shaderByteCode, &byteCodeLength );

        hr = m_d3dDevice->CreateInputLayout( VertexPositionColor::InputElements,
                                              VertexPositionColor::InputElementCount,
                                              shaderByteCode, byteCodeLength,
                                              &m_BatchInputLayout );
        if( FAILED( hr ) )
            return hr;
    }

    m_Font.reset( new SpriteFont( m_d3dDevice, L"italic.spritefont" ) );
    
    // Load the Texture
    hr = CreateDDSTextureFromFile( m_d3dDevice, L"seafloor.dds", nullptr, &m_TextureRV1 );
    if( FAILED( hr ) )
        return hr;

    hr = CreateDDSTextureFromFile( m_d3dDevice, L"windowslogo.dds", nullptr, &m_TextureRV2 );
    if( FAILED( hr ) )
        return hr;

    // Initialize the world matrices
    m_World = XMMatrixIdentity();

    // Initialize the view matrix
    XMVECTOR Eye = XMVectorSet( 0.0f, 3.0f, -6.0f, 0.0f );
    XMVECTOR At = XMVectorSet( 0.0f, 1.0f, 0.0f, 0.0f );
    XMVECTOR Up = XMVectorSet( 0.0f, 1.0f, 0.0f, 0.0f );
    m_View = XMMatrixLookAtLH( Eye, At, Up );

    m_BatchEffect->SetView( m_View );

    // Initialize the projection matrix
    m_Projection = XMMatrixPerspectiveFovLH( XM_PIDIV4, width / (FLOAT)height, 0.01f, 100.0f );

    m_BatchEffect->SetProjection( m_Projection );

    return S_OK;
}

void GraphicsManager::CleanupDevice()
{
    if( m_ImmediateContext ) m_ImmediateContext->ClearState();

    if ( m_BatchInputLayout ) m_BatchInputLayout->Release();

    if( m_TextureRV1 ) m_TextureRV1->Release();
    if( m_TextureRV2 ) m_TextureRV2->Release();

    if( m_DepthStencilView ) m_DepthStencilView->Release();
    if( m_DepthStencil ) m_DepthStencil->Release();
    if( m_RenderTargetView ) m_RenderTargetView->Release();

    if( m_SwapChain )
    {
        m_SwapChain->Release();        
		m_SwapChain->SetFullscreenState(false, NULL);
    }
    
    if( m_ImmediateContext ) m_ImmediateContext->Release();
    if( m_d3dDevice ) m_d3dDevice->Release();
}

void GraphicsManager::ClearBuffers()
{
    //
    // Clear the back buffer
    //
    m_ImmediateContext->ClearRenderTargetView( m_RenderTargetView, Colors::Black );

    //
    // Clear the depth buffer to 1.0 (max depth)
    //
    m_ImmediateContext->ClearDepthStencilView( m_DepthStencilView, D3D11_CLEAR_DEPTH, 1.0f, 0 );
}