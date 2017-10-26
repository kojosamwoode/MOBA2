using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryManagerEditor : EditorWindow {

    public const string m_inventoryAssetPath = "Assets/MobaStorm/Scripts/Inventory/InventoryAssets/";
    static List<BaseItemContainer> m_itemList = null;
    private static InventoryManagerEditor m_window;
    private static InventoryManager m_inventoryManager;


    static int initX = 10;
    static int initY = 80;
    static int textFieldWidth = 100;
    static int textFieldHeight = 18;
    Vector2 scrollPos;
    Vector2 abilityScrollPos;
    static int m_abilityOffsetX = 350;


    static GUIStyle m_boldStyle;
    static int selectionID = 1;

    public static GUISkin m_skin;
    // Use this for initialization

    static string m_newItemIdentifier;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [MenuItem("MobaStorm/Inventory Editor")]
    static void Init()
    {
        m_window = (InventoryManagerEditor)EditorWindow.GetWindow(typeof(InventoryManagerEditor));
        EditorWindow.GetWindow(typeof(InventoryManagerEditor));

        m_inventoryManager = (InventoryManager)AssetDatabase.LoadAssetAtPath("Assets/MobaStorm/Prefabs/Managers/InventoryManager.prefab", typeof(InventoryManager));
        if (m_inventoryManager == null)
        {
            Debug.Log("Cannot load AbilityManager");
        }
        else
        {
            m_itemList = m_inventoryManager.m_items;
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
        m_newItemIdentifier = EditorGUI.TextField(new Rect(initX + 10, initY + 60, 200, textFieldHeight), m_newItemIdentifier, m_skin.textField);

        //Add Item button
        if (GUI.Button(new Rect(initX + 215, initY + 55, textFieldHeight + 10, textFieldHeight + 10), "", m_skin.customStyles[1]))
        {
            if (m_newItemIdentifier != "")
            {
                foreach (BaseItemContainer currentItem in m_itemList)
                {
                    if (currentItem.name == m_newItemIdentifier)
                    {
                        Debug.LogError("Ability Identifier already Exist");
                        return;
                    }
                }
                BaseItemContainer item = ScriptableObjectUtility.CreateAsset<BaseItemContainer>(m_newItemIdentifier, m_inventoryAssetPath);
                //ability.name = m_newItemIdentifier;
                m_itemList.Add(item);
                EditorUtility.SetDirty(m_inventoryManager);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                selectionID = 0;
                return;
            }

        }

        if (m_itemList.Count < 1)
        {
            return;
        }
        EditorGUI.LabelField(new Rect(m_abilityOffsetX, yPos, 300, 20), new GUIContent("Item Configuration"), m_skin.GetStyle("BoldText"));
        Rect visibleRect = new Rect(m_abilityOffsetX, 100, m_window.position.width - m_abilityOffsetX, m_window.position.height - 100 - 5);
        Rect itemListRect = new Rect(m_abilityOffsetX, 100, 1200, 800);
        abilityScrollPos = GUI.BeginScrollView(new Rect(m_abilityOffsetX, 100, visibleRect.width, visibleRect.height), abilityScrollPos, itemListRect, m_skin.horizontalScrollbar, m_skin.verticalScrollbar);

        m_itemList[selectionID].DrawItem(m_abilityOffsetX, yPos + 40,  m_skin);
        GUI.EndScrollView();
    }

    private void ItemNameList()
    {
        int tmpY = initY + 100;
        Rect visibleRect = new Rect(initX, tmpY, m_window.position.width - initX, m_window.position.height - tmpY - 5);
        Rect itemListRect = new Rect(initX, tmpY, 285, 45 * m_itemList.Count);
        scrollPos = GUI.BeginScrollView(new Rect(initX, tmpY, itemListRect.width + 25, visibleRect.height), scrollPos, itemListRect, m_skin.horizontalScrollbar, m_skin.verticalScrollbar);

        GUI.color = new Color(.8f, .8f, .8f, 1f);
        GUI.color = Color.white;

        ShowItemList();
        GUI.EndScrollView();
    }

    void ShowItemList()
    {

        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (m_itemList[i] == null)
            {
                continue;
            }
            int offsetY = (45 * i) + 150;
            if (GUI.Button(new Rect(initX + 10, (initY / 2) + offsetY, textFieldWidth * 2f, textFieldHeight * 2f), "ID:" + m_itemList[i].name, m_skin.button))
            {
                selectionID = i;
            }


            m_itemList[i].Icon = (Sprite)EditorGUI.ObjectField(new Rect(initX + 10 + textFieldWidth * 2f, (initY / 2) + offsetY, textFieldHeight * 2f, textFieldHeight * 2f), m_itemList[i].Icon, typeof(Sprite), false);



            if (selectionID == i && GUI.Button(new Rect(initX + 15 + (textFieldWidth * 2f) + (textFieldHeight * 2f), (initY / 2) + offsetY, textFieldHeight * 1.5f, textFieldHeight * 1.5f), "", m_skin.GetStyle("RemoveButton")))
            {
                //if (m_itemList[selectionID].SideEffects.Count > 0)
                //{
                //    Debug.LogError("Remove all side effects before removing the current ability");
                //    return;
                //}
                string path = AssetDatabase.GetAssetPath(m_itemList[selectionID]);
                AssetDatabase.DeleteAsset(path);
                m_itemList.RemoveAt(selectionID);
                selectionID = 0;
                EditorUtility.SetDirty(m_inventoryManager);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }
        }

    }

    void OnDestroy()
    {
        EditorUtility.SetDirty(m_inventoryManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}
