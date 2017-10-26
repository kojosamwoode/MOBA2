using UnityEngine;
using System.Collections;
using UnityEditor;

public class CharacterListSection : BaseTemplate
{
    public static float m_listWidth = 260;
    private GUIStyle m_boxStyle;
    private Texture2D m_scrollBackgroundTex;
    private string m_selectionID;

    public override void Initialize()
    {
        base.Initialize();
        m_boxStyle = new GUIStyle(GUI.skin.box);
        m_boxStyle.normal.background = null;
        m_boxStyle.fixedHeight = 65;
    }

    public override void Render()
    {
        base.Render();
        DrawScrolleableRegionWithDefaultContent(0, 150, m_listWidth, m_windowHeight, m_defaultColor, m_listWidth, m_windowHeight - 60, true);
    }

    protected override void DefaultRegionContent()
    {
        base.DefaultRegionContent();

        if(CharacterEditorWindow.m_entityHolderPrefab == null)
        {
            Debug.LogError("Holder List Null");
            return;
        }
        
        DrawRegion(5, 5, m_listWidth - 10, 20, () => {

            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("Entity Library", CharacterEditorWindow.m_skin.label);

            if (GUILayout.Button("Refresh"))
            {
                (m_parent as CharacterEditorWindow).RefreshHoldersList();
            }

            GUILayout.Space(20);

            GUILayout.EndHorizontal();

        }, m_defaultColor, true);

        GUILayout.Space(50);

        foreach (MobaEntityData entityData in CharacterEditorWindow.m_gameDataManager.EntityData.Values)
        {
            DrawCharacterBox(entityData);
        }


        GUILayout.Space(100);
    }

    private void DrawCharacterBox(MobaEntityData entityData)
    {

        GUILayout.BeginVertical(m_boxStyle);
        HighlightGUI(entityData.m_dataIdentifier == m_selectionID);
        if (GUILayout.Button(entityData.m_dataIdentifier, CharacterEditorWindow.m_skin.button))
        {
            (m_parent as CharacterEditorWindow).LoadCharacterView(entityData);

            m_selectionID = entityData.m_dataIdentifier;
        }
        HighlightGUI(false);
        GUILayout.EndVertical();
    }

    private void HighlightGUI(bool value)
    {
        GUI.color = (value) ? Color.cyan : Color.white;
    }
}
