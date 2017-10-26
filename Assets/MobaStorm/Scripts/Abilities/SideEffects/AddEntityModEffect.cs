using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AddEntityModEffect : SideEffect
{

    [SerializeField]
    private int m_modifier;
    public int Modifier
    {
        get { return m_modifier; }
        set { m_modifier = value; }
    }

    [SerializeField]
    private EStatMods m_modType;
    public EStatMods ModType
    {
        get { return m_modType; }
        set { m_modType = value; }
    }

    public AddEntityModEffect(AddEntityModEffect sideEffect) : base(sideEffect) 
    {
        m_modifier = sideEffect.m_modifier;
        m_modType = sideEffect.m_modType;
    }
    public override SideEffect Clone()
    {
        return new AddEntityModEffect(this);
    }

    public override void ApplyEffect(Ability ability, MobaEntity attacker, MobaEntity reciever)
    {
        base.ApplyEffect(ability,attacker, reciever);
        reciever.ServerAddModifiers(m_modType, m_modifier);
    }

    public override void ProcessEffect()
    {
        base.ProcessEffect();
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        m_reciever.ServerAddModifiers(m_modType, -m_modifier);
    }



    public override int DrawEffect(int xPos, int yPos, GUISkin skin)
    {

        yPos = base.DrawEffect(xPos, yPos, skin);
#if UNITY_EDITOR
           
        m_modType = (EStatMods)EditorGUI.EnumPopup(new Rect(xPos + 150, yPos += 20, 300, 20), m_modType);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Mod Type: "), skin.label);

        m_modifier = EditorGUI.IntField(new Rect(xPos + 150, yPos += 20, 200, 20), m_modifier);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Modifier: "), skin.label);

        EditorUtility.SetDirty(this);
#endif
        return yPos;

    }

}
