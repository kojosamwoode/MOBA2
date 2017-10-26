using UnityEngine;
using System.Collections;
using System;

public class HeaderSection : BaseTemplate
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Render()
    {
        base.Render();
        DrawRegionWithDefaultContent(0, 0, m_windowWidth, 150, m_defaultColor, true);
    }

    protected override void DefaultRegionContent()
    {
        base.DefaultRegionContent();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUI.Button(new Rect(10, 0, 250, 150), "", CharacterEditorWindow.m_skin.customStyles[0]))
        {
            Application.OpenURL("www.jmgdigital.com");
        }
        GUILayout.FlexibleSpace();
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
    }
}
