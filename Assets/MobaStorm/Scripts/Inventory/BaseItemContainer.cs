using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BaseItemContainer : ScriptableObject {

    public enum EItemType
    {
        Consumable,
        Equipable,
    }

    public enum EItemUseType
    {
        Passive,
        Active,
    }

    [SerializeField]
    private EItemType m_itemType;
    public EItemType ItemType
    {
        get { return m_itemType; }
        set { m_itemType = value; }
    }

    [SerializeField]
    private EItemUseType m_itemUseType;
    public EItemUseType ItemUseType
    {
        get { return m_itemUseType; }
        set { m_itemUseType = value; }
    }

    public string Identifier
    {
        get { return this.name; }
    }

    [SerializeField]
    private string m_name;
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    [SerializeField]
    private int m_maxStacks = 1;
    public int MaxStacks
    {
        get { return m_maxStacks; }
        set { m_maxStacks = value; }
    }    

    [SerializeField]
    private Currency m_cost;
    public Currency Cost
    {
        get { return m_cost; }
        set { m_cost = value; }
    }

    [SerializeField]
    private int m_charges;
    public int Charges
    {
        get { return m_charges; }
        set { m_charges = value; }
    }


    [SerializeField]
    private string m_description;
    public string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }

    [SerializeField]
    private Sprite m_icon;
    public Sprite Icon
    {
        get { return m_icon; }
        set { m_icon = value; }
    }

    [SerializeField]
    private AbilityBase m_abilityBase;
    public AbilityBase AbilityBase
    {
        get { return m_abilityBase; }
        set { m_abilityBase = value; }
    }

    [SerializeField]
    private List<SideEffect> m_passiveEffects = new List<SideEffect>();
    public List<SideEffect> PassiveEffects
    {
        get { return m_passiveEffects; }
        set { m_passiveEffects = value; }
    }

    [SerializeField]
    private List<BaseItemContainer> m_purchaseRequeriments;
    public List<BaseItemContainer> PurchaseRequeriment
    {
        get { return m_purchaseRequeriments; }
        set { m_purchaseRequeriments = value; }
    }

    private string[] m_effectClasses;
    private int m_effectClassIndex;


    public int DrawItem(int posX, int yPos, GUISkin skin)
    {


        int initPos = yPos;
#if UNITY_EDITOR
        //EditorStyles.textField.fontStyle = FontStyle.Bold;

        m_icon = (Sprite)EditorGUI.ObjectField(new Rect(posX, yPos, 70, 70), m_icon, typeof(Sprite), false);

        yPos += 70;

        EditorGUI.LabelField(new Rect(posX + 150, yPos += 20, 150, 20), this.name, skin.textField);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Identifier", "Item identifier used to identify and load item"), skin.label);

        m_description = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_description, skin.textField);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Description Name: ", "Item description to show"), skin.label);

        int ammount = EditorGUI.IntField(new Rect(posX + 150, yPos += 20, 150, 20), m_cost.Ammount, skin.textField);
        m_cost.SetAmmount(ammount);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Cost: ", "Item Cost"), skin.label);

        //m_charges = EditorGUI.IntField(new Rect(posX + 150, yPos += 20, 150, 20), m_charges, skin.textField);
        //EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Charges: ", "Item Use Charges"), skin.label);

        m_maxStacks = EditorGUI.IntField(new Rect(posX + 150, yPos += 20, 150, 20), m_maxStacks, skin.textField);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Stacks: ", "Set this if the item is stackeable"), skin.label);

        m_itemType = (EItemType)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_itemType);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Type: ", "Select item type"), skin.label);

        m_itemUseType = (EItemUseType)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_itemUseType);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Item Use: ", "Select Use type"), skin.label);

        m_abilityBase = (AbilityBase)EditorGUI.ObjectField(new Rect(new Rect(posX + 150, yPos += 20, 150, 20)), m_abilityBase, typeof(AbilityBase), false);

        
        GUIStyle m_boldStyle = new GUIStyle();
        m_boldStyle.fontSize = 18;
        m_boldStyle.fontStyle = FontStyle.Bold;
        EditorGUI.LabelField(new Rect(posX + 400, yPos - 40, 300, 20), new GUIContent("Passive Effects"), skin.GetStyle("BoldText"));
        
        m_effectClasses = GetEffectClasses();
        
        m_effectClassIndex = EditorGUI.Popup(new Rect(posX + 400, yPos += 20, 300, 20), m_effectClassIndex, m_effectClasses);
        GUI.color = Color.green;
        if (GUI.Button(new Rect(posX + 400, yPos += 20, 300, 20), "Add Passive Effect"))
        {
            SideEffect effect = ScriptableObjectUtility.CreateAsset(m_effectClasses[m_effectClassIndex], this.name);
            effect.name = this.name + "-" + m_effectClasses[m_effectClassIndex];
            m_passiveEffects.Add(effect);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return 0;
        }
        GUI.color = Color.white;
        yPos += 30;
        for (int i = 0; i < m_passiveEffects.Count; i++)
        {
            //GUI.contentColor = Color.cyan;
        
            EditorGUI.LabelField((new Rect(posX + 400, yPos += 20, 300, 20)), new GUIContent("Effect Type: " + m_passiveEffects[i].GetType().ToString()), skin.label);
        
        
            int boxPosY = yPos;
            yPos = m_passiveEffects[i].DrawEffect(posX + 400, yPos, skin);
            //GUI.color = Color.cyan;
            GUI.Box(new Rect(posX + 390, boxPosY, 8, (yPos - boxPosY) + 20), "");
            //GUI.color = Color.red;
            EditorGUI.LabelField((new Rect(posX + 400, yPos += 20, 300, 20)), new GUIContent("Remove this effect "), skin.GetStyle("RedText"));
            if (GUI.Button(new Rect(posX + 550, yPos, 20, 20), "", skin.GetStyle("RemoveButton")))
            {
                string path = AssetDatabase.GetAssetPath(m_passiveEffects[i]);
                AssetDatabase.DeleteAsset(path);
                m_passiveEffects.RemoveAt(i);
                break;
            }
            //    if (GUI.Button(new Rect(posX + 400, yPos += 20, 300, 20), "Remove This Effect"))
            //{
            //   
            //}
            //GUI.color = Color.white;
            yPos += 20;
        }


        EditorUtility.SetDirty(this);
#endif
        return yPos - initPos;
    }

    public static string[] GetEffectClasses()
    {
        List<string> classes = new List<string>();
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var t in a.GetTypes())
            {
                if (t.IsSubclassOf(typeof(SideEffect)))
                {
                    classes.Add(t.ToString());
                }
            }
        }
        return classes.ToArray();
    }
}
