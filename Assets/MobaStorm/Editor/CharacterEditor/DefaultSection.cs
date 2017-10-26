using UnityEngine;
using System.Collections;

public class DefaultSection : BaseTemplate
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Render()
    {
        base.Render();
        float sectionWidth = m_windowWidth - CharacterListSection.m_listWidth;
        DrawRegionWithDefaultContent(CharacterListSection.m_listWidth, 150, sectionWidth, 80, m_defaultColor, true);
    }

    protected override void DefaultRegionContent()
    {
        base.DefaultRegionContent();

        GUILayout.BeginHorizontal();

        GUILayout.Space(20);

        CharacterEditorWindow.m_skin.customStyles[4].alignment = TextAnchor.MiddleLeft;

        GUILayout.Label("Create Entity", CharacterEditorWindow.m_skin.customStyles[4], GUILayout.Height(40));

        CharacterEditorWindow.m_skin.customStyles[4].alignment = TextAnchor.UpperLeft;

        GUILayout.Space(10);

        if (GUILayout.Button(new GUIContent(""), CharacterEditorWindow.m_skin.customStyles[1], GUILayout.Width(40), GUILayout.Height(40)))
        {
            CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.CreatingHolder;
        }

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
    }
}
