#ifndef _GRAPHICS_MANAGER_H
#define _GRAPHICS_MANAGER_H

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX

#include <windows.h>
#include <d3d11.h>
#include <directxmath.h>

#include "CommonStates.h"
#include "SpriteBatch.h"
#include "SpriteFont.h"
#include "Effects.h"

class GraphicsManager
{
            D3D_DRIVER_TYPE             m_DriverType;
            D3D_FEATURE_LEVEL           m_FeatureLevel;

	        HINSTANCE                   m_hInst;
	        HWND                        m_hWnd;                
            ID3D11Device*               m_d3dDevice;
            ID3D11DeviceContext*        m_ImmediateContext;
            IDXGISwapChain*             m_SwapChain;
            ID3D11RenderTargetView*     m_RenderTargetView;
            ID3D11Texture2D*            m_DepthStencil;
            ID3D11DepthStencilView*     m_DepthStencilView;
            ID3D11ShaderResourceView*   m_TextureRV1;
            ID3D11ShaderResourceView*   m_TextureRV2;
            ID3D11InputLayout*          m_BatchInputLayout;

            std::unique_ptr<DirectX::CommonStates>  m_States;
            std::unique_ptr<DirectX::SpriteBatch>   m_Sprites;
            std::unique_ptr<DirectX::SpriteFont>    m_Font;
            std::unique_ptr<DirectX::BasicEffect>   m_BatchEffect;
            std::unique_ptr<DirectX::EffectFactory> m_FXFactory;
            
            DirectX::XMMATRIX           m_World;
            DirectX::XMMATRIX           m_View;
            DirectX::XMMATRIX           m_Projection;

	HRESULT InitWindow( HINSTANCE hInstance, int nCmdShow, WNDPROC loopFunc, const char* windowTitle );
    HRESULT InitDevice();
    void    Render();
public:
            GraphicsManager();
    bool    Initialize( HINSTANCE hInstance, int nCmdShow );
    void    CleanupDevice();
    void    ClearBuffers();

    DirectX::SpriteBatch* GetSpriteBatch() const { return m_Sprites.get(); }
    
};

#endif