#ifndef _TEXTURE_MANAGER_H
#define _TEXTURE_MANAGER_H

#include <d3d11.h>

#include <map>
#include <memory>
#include <string>

class TextureManager
{            
    ID3D11Device* m_device;
    static std::map<std::string, std::unique_ptr<ID3D11ShaderResourceView> > m_textureMap;

    TextureManager();
public:
    TextureManager(ID3D11Device* device);
    
    static void AddTexture(const char* filename);
    static inline const ID3D11ShaderResourceView* GetTexture(const string& filename);
};

#endif