using UnityEngine;
using System.Collections;
using System;

public class Ability
{
    public Ability(AbilityBase abilityBase, EAttackType type, MobaEntity entity)
    {
        m_abilityBase = abilityBase;
        m_mobaEntity = entity;
        //Only basic abilities starts at level 1
        m_attackType = type;
        Level = m_attackType == EAttackType.Basic ? 1 : 0;     
    }
    private MobaEntity m_mobaEntity;
    public MobaEntity MobaEntity { get { return m_mobaEntity; } }

    private AbilityBase m_abilityBase;
    public AbilityBase AbilityBase
    {
        get { return m_abilityBase; }
        set { m_abilityBase = value; }
    }

    public Action<Ability> m_onDataUpdated;


    private int m_level = 0;
    public int Level
    {
        get { return m_level; }
        set
        {
            m_level = value;
            if (m_level == 0)
                return;
            AbilityBase.AbilityDamageData damageData = m_abilityBase.GetAbilityDamageData(m_level);
            m_baseDamage = damageData.BaseAdDamage;
            m_baseAbilityPower = damageData.BaseApDamage;
            m_range = damageData.Range;
            m_coolDownTotal = damageData.ColdDownSeconds;
            m_manaCost = damageData.ManaCost;
            DataChanged();
        }
    }

    public bool CanLevelUp
    {
        get
        {
            return m_level < AbilityManager.instance.MaxAbilityLevels && m_abilityBase.CanLevelUp;
        }
    }


    [SerializeField]
    private EAttackType m_attackType;
    public EAttackType AttackType
    {
        get { return m_attackType; }
        set { m_attackType = value; }
    }

    private float m_coolDown;
    public float CoolDown
    {
        get { return m_coolDown; }
        set
        {
            m_coolDown = value;
            DataChanged();
        }
    }

    private float m_coolDownMultiplier = 1;
    public float CoolDownMultiplier
    {
        get { return m_coolDownMultiplier; }
        set { m_coolDownMultiplier = value; }
    }
    private float m_baseDamage;
    public float BaseDamage
    {
        get { return m_baseDamage; }
    }

    private float m_baseAbilityPower;
    public float BaseAbilityPower
    {
        get { return m_baseAbilityPower; }
    }

    private float m_range;
    public float Range
    {
        get { return m_range; }
    }

    private float m_coolDownTotal;
    public float CoolDownTotal
    {
        get { return m_coolDownTotal; }
    }
    private float m_manaCost;
    public float ManaCost
    {
        get { return m_manaCost; }
    }

    private void DataChanged()
    {
        if (m_onDataUpdated != null)
        {
            m_onDataUpdated(this);
        }
    }

    public void UpdateCdr(bool isClient)
    {
        //CoolDown = totalCoolDown;
        if (CoolDown > 0)
        {
            float totalCoolDown = CoolDownTotal;
            CoolDown -= Time.fixedDeltaTime * CoolDownMultiplier;
            if (isClient && m_mobaEntity == GameManager.instance.LocalPlayer.CharacterEntity)
            {

            }
            ///yield return new WaitForEndOfFrame();
        }
    }

    public bool IsOnRange(MobaEntity attacker, Vector2 targetPos)
    {
        float distanceToTarget = Vector2.Distance(attacker.Position, targetPos);
        //
        if (distanceToTarget < m_range)
        {
            return true;
        }
        return false;
    }

}
