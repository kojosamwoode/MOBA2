using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteDatabaseManager : MonoSingleton<SpriteDatabaseManager> {
    [SerializeField]
    private List<Sprite> m_sprites = new List<Sprite>();
    [SerializeField]
    private List<Texture2D> m_textures = new List<Texture2D>();


    private Dictionary<string, Sprite> m_spriteDatabase = new Dictionary<string, Sprite>();
    private Dictionary<string, Texture2D> m_textureDatabase = new Dictionary<string, Texture2D>();


    public Sprite GetSprite(string sprite)
    {
        if (m_spriteDatabase.ContainsKey(sprite))
        {
            return m_spriteDatabase[sprite];
        }
        return m_spriteDatabase["NotAvailable_Sprite"];
    }

    public Texture2D GetTexture(string texture)
    {
        if (m_textureDatabase.ContainsKey(texture))
        {
            return m_textureDatabase[texture];
        }
        return m_textureDatabase["NotAvailable_Texture"];
    }

    // Use this for initialization
    void Awake () {
        foreach (Sprite sprite in m_sprites)
        {
            if (!m_spriteDatabase.ContainsKey(sprite.name))
            {
                m_spriteDatabase.Add(sprite.name, sprite);
            }
        }

        foreach (Texture2D texture in m_textures)
        {
            if (!m_textureDatabase.ContainsKey(texture.name))
            {
                m_textureDatabase.Add(texture.name, texture);
            }
        }

    }
	
}
