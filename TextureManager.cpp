#include "TextureManager.h"

#include "WICTextureLoader.h"

using namespace DirectX;
using namespace std;

map<string, unique_ptr<ID3D11ShaderResourceView> > m_textureMap;

TextureManager::TextureManager(ID3D11Device* device) : m_device(device)
{}

void TextureManager::AddTexture(const char* filename)
{    
    //CreateWICTextureFromFile(device, devCtx,
}

inline const ID3D11ShaderResourceView* TextureManager::GetTexture(const string& filename)
{
    return m_textureMap.at(filename).get();
}