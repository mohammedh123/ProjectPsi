//--------------------------------------------------------------------------------------
// File: SimpleSample.cpp
//
// This is a simple Win32 desktop application showing use of DirectXTK
//
// http://go.microsoft.com/fwlink/?LinkId=248929
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

#include <d3d11.h>

#include <directxmath.h>

#include "GraphicsManager.h"

using namespace DirectX;

HRESULT InitWindow( HINSTANCE hInstance, int nCmdShow );
HRESULT InitDevice();
void CleanupDevice();
LRESULT CALLBACK    WndProc( HWND, UINT, WPARAM, LPARAM );
void Render();

std::unique_ptr<GraphicsManager> GraphicsMan;

int WINAPI wWinMain( HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow )
{
    UNREFERENCED_PARAMETER( hPrevInstance );
    UNREFERENCED_PARAMETER( lpCmdLine );
    
    GraphicsMan.reset(new GraphicsManager());
    GraphicsMan->Initialize(hInstance, nCmdShow);

    // Main message loop
    MSG msg = {0};
    while( WM_QUIT != msg.message )
    {
        if( PeekMessage( &msg, nullptr, 0, 0, PM_REMOVE ) )
        {
            TranslateMessage( &msg );
            DispatchMessage( &msg );
        }
        else
        {
            Update();
            Render();
        }
    }
    
    GraphicsMan->CleanupDevice();

    return ( int )msg.wParam;
}

void Update()
{

}

void Render()
{
    auto sb = GraphicsMan->GetSpriteBatch();

    // Draw sprite
    sb->Begin( SpriteSortMode_Deferred );
    sb->Draw( g_pTextureRV2, XMFLOAT2(10, 75 ), nullptr, Colors::White );

    g_Font->DrawString( g_Sprites.get(), L"DirectXTK Simple Sample", XMFLOAT2( 100, 10 ), Colors::Yellow );
    g_Sprites->End();

    // Draw 3D object
    XMMATRIX local = XMMatrixMultiply( g_World, XMMatrixTranslation( -2.f, -2.f, 4.f ) );
    g_Shape->Draw( local, g_View, g_Projection, Colors::White, g_pTextureRV1 );

    XMVECTOR qid = XMQuaternionIdentity();
    const XMVECTORF32 scale = { 0.01f, 0.01f, 0.01f};
    const XMVECTORF32 translate = { 3.f, -2.f, 4.f };
    XMVECTOR rotate = XMQuaternionRotationRollPitchYaw( 0, XM_PI/2.f, XM_PI/2.f );
    local = XMMatrixMultiply( g_World, XMMatrixTransformation( g_XMZero, qid, scale, g_XMZero, rotate, translate ) );
    g_Model->Draw( g_pImmediateContext, *g_States, local, g_View, g_Projection );
    //
    // Present our back buffer to our front buffer
    //
    g_pSwapChain->Present( 0, 0 );
}

LRESULT CALLBACK WndProc( HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam )
{
    PAINTSTRUCT ps;
    HDC hdc;

    switch( message )
    {
        case WM_PAINT:
            hdc = BeginPaint( hWnd, &ps );
            EndPaint( hWnd, &ps );
            break;

        case WM_DESTROY:
            PostQuitMessage( 0 );
            break;

        default:
            return DefWindowProc( hWnd, message, wParam, lParam );
    }

    return 0;
}
