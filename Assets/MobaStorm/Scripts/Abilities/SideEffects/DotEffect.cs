using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EDotType
{
    None = 0,
    AcidSpray = 1,
    Bleed = 2,
    Burning = 4,
    Poison = 8,
}

public class DotEffect : SideEffect
{
    [SerializeField]
    private EDotType m_dotType;
    public EDotType DotType
    {
        get { return m_dotType; }
        set { m_dotType = value; }
    }
    [SerializeField]
    private float m_damage;
    public float Damage
    {
        get { return m_damage; }
        set { m_damage = value; }
    }



    public DotEffect(DotEffect sideEffect) : base(sideEffect) 
    {
        m_dotType = sideEffect.m_dotType;
        m_damage = sideEffect.m_damage;
    }
    public override SideEffect Clone()
    {
        return new DotEffect(this);
    }

    public override void ApplyEffect(Ability ability, MobaEntity attacker, MobaEntity reciever)
    {
        base.ApplyEffect(ability, attacker, reciever);
    }

    public override void ProcessEffect()
    {
        base.ProcessEffect();
    }

    public override void ApplyTick()
    {
        base.ApplyTick();
        m_reciever.DealDamage(m_damage / (float)m_ticks, new DamageProcess(m_attacker, m_reciever, null));

    }
    public override void RemoveEffect()
    {
        base.RemoveEffect();
    }

    public override int DrawEffect(int xPos, int yPos, GUISkin skin)
    {

        yPos = base.DrawEffect(xPos, yPos, skin);
#if UNITY_EDITOR

        m_damage = EditorGUI.FloatField(new Rect(xPos + 150, yPos += 20, 200, 20),  m_damage);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Total Damage: "), skin.label);

        m_dotType = (EDotType)EditorGUI.EnumPopup(new Rect(xPos + 150, yPos += 20, 200, 20), m_dotType);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Effect Type: "), skin.label);

        EditorUtility.SetDirty(this);
#endif
        return yPos;

    }

}
