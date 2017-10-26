using UnityEngine;
using System.Collections;
using UnityEditor;

public class BackgroundSection : BaseTemplate
{
    private Texture2D m_backgroundTexture;
    private GUIStyle m_backgroundStyle;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Render()
    {
        base.Render();
        DrawBox(CharacterEditorWindow.m_skin.window, new Rect(0, 0, m_windowWidth, m_windowHeight));
    }
}
