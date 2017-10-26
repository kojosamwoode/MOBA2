using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

//[CustomEditor(typeof(AbilityManager))]
public class AbilityManagerEditor : EditorWindow {

    static List<AbilityBase> m_abilityList = null;
    private static AbilityManagerEditor m_window;
    private static AbilityManager m_abilityManager;
    static int initX = 10;
    static int initY = 80;
    Vector2 scrollPos;
    Vector2 abilityScrollPos;

    static int textFieldWidth = 100;
    static int textFieldHeight = 18;
    static int selectionID = 1;

    static int m_abilityOffsetX = 350;
    static GUIStyle m_boldStyle;
    static string m_newAbilityIdentifier;

    public static GUISkin m_skin;

    [MenuItem("MobaStorm/Ability Editor")]
    static void Init()
    {
        m_window = (AbilityManagerEditor)EditorWindow.GetWindow(typeof(AbilityManagerEditor));
        EditorWindow.GetWindow(typeof(AbilityManagerEditor));
        
        m_abilityManager = (AbilityManager)AssetDatabase.LoadAssetAtPath("Assets/MobaStorm/Prefabs/Managers/AbilityManager.prefab", typeof(AbilityManager));
        if (m_abilityManager == null)
        {
            Debug.Log("Cannot load AbilityManager");
        }
        else
        {
            m_abilityList = m_abilityManager.m_abilities;
        }

        m_skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/MobaStorm/Resources/EditorSkin.guiskin", typeof(GUISkin));
        m_boldStyle = new GUIStyle();
        m_boldStyle.fontSize = 18;
        m_boldStyle.fontStyle = FontStyle.Bold;
        selectionID = 0;
    }


    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, m_window.position.width, m_window.position.height), "", m_skin.window);
        if (GUI.Button(new Rect(0, 0, 250, 125), "", m_skin.customStyles[0]))
        {
            Application.OpenURL("www.jmgdigital.com");
        }

        ItemNameList();
        int yPos = 60;
                
        EditorGUI.LabelField(new Rect(initX + 10, initY + 40, 150, textFieldHeight), "Add Ability: ", m_skin.label);
        m_newAbilityIdentifier = EditorGUI.TextField(new Rect(initX + 10, initY + 60, 200, textFieldHeight), m_newAbilityIdentifier, m_skin.textField);
        
        //Add ability button
        if (GUI.Button(new Rect(initX + 215, initY + 55, textFieldHeight + 10, textFieldHeight + 10), "", m_skin.customStyles[1]))
        {
            if (m_newAbilityIdentifier != "")
            {
                foreach (AbilityBase currentAbility in m_abilityList)
                {
                    if (currentAbility.Identifier == m_newAbilityIdentifier)
                    {
                        Debug.LogError("Ability Identifier already Exist");
                        return;
                    }
                }
                AbilityBase ability = ScriptableObjectUtility.CreateAsset<AbilityBase>(m_newAbilityIdentifier, "Assets/MobaStorm/Scripts/Abilities/AbilityAssets/");
                ability.Identifier = m_newAbilityIdentifier;
                m_abilityList.Add(ability);
                EditorUtility.SetDirty(m_abilityManager);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                selectionID = 0;
                return;
            }
            
        }

        if (m_abilityList.Count <1)
        {  
            return;
        }
        EditorGUI.LabelField(new Rect(m_abilityOffsetX, yPos, 300, 20), new GUIContent("Ability Configuration"), m_skin.GetStyle("BoldText"));
        Rect visibleRect = new Rect(m_abilityOffsetX, 100, m_window.position.width - m_abilityOffsetX, m_window.position.height - 100 - 5);
        Rect itemListRect = new Rect(m_abilityOffsetX, 100, 1200, 800);
        abilityScrollPos = GUI.BeginScrollView(new Rect(m_abilityOffsetX, 100, visibleRect.width, visibleRect.height), abilityScrollPos, itemListRect, m_skin.horizontalScrollbar, m_skin.verticalScrollbar);

        m_abilityList[selectionID].DrawAbility(m_abilityOffsetX, yPos + 40, m_abilityManager.MaxAbilityLevels, m_skin);
        GUI.EndScrollView();
    }
    void ItemNameList()
    {
        int tmpY = initY + 100;
        Rect visibleRect = new Rect(initX, tmpY, m_window.position.width - initX, m_window.position.height - tmpY - 5);
        Rect itemListRect = new Rect(initX, tmpY, 285, 45 * m_abilityList.Count);
        scrollPos = GUI.BeginScrollView(new Rect(initX, tmpY, itemListRect.width + 25, visibleRect.height), scrollPos, itemListRect, m_skin.horizontalScrollbar, m_skin.verticalScrollbar);

        GUI.color = new Color(.8f, .8f, .8f, 1f);
        GUI.color = Color.white;

        ShowItemList();
        GUI.EndScrollView();
    }

    void ShowItemList()
    {

        for (int i = 0; i < m_abilityList.Count; i++)
        {
            if (m_abilityList[i] == null)
            {
                continue;
            }
            int offsetY = (45 * i) + 150;
            if (GUI.Button(new Rect(initX + 10, (initY / 2) + offsetY, textFieldWidth * 2f, textFieldHeight * 2f), "ID:" + m_abilityList[i].Identifier, m_skin.button))
            {
                selectionID = i;
            }


            m_abilityList[i].Icon = (Sprite)EditorGUI.ObjectField(new Rect(initX + 10 + textFieldWidth * 2f, (initY / 2) + offsetY, textFieldHeight * 2f, textFieldHeight * 2f), m_abilityList[i].Icon, typeof(Sprite), false);


            if (selectionID == i && GUI.Button(new Rect(initX + 15 + (textFieldWidth * 2f) + (textFieldHeight * 2f), (initY / 2) + offsetY, textFieldHeight * 1.5f, textFieldHeight * 1.5f), "", m_skin.GetStyle("RemoveButton")))
            {
                if (m_abilityList[selectionID].SideEffects.Count > 0)
                {
                    Debug.LogError("Remove all side effects before removing the current ability");
                    return;
                }
                string path = AssetDatabase.GetAssetPath(m_abilityList[selectionID]);
                AssetDatabase.DeleteAsset(path);
                m_abilityList.RemoveAt(selectionID);
                selectionID = 0;
                EditorUtility.SetDirty(m_abilityManager);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }
        }

    }

 
    void OnDestroy()
    {
        EditorUtility.SetDirty(m_abilityManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }
  
}
