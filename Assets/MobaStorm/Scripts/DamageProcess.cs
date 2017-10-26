using UnityEngine;
using System.Collections;

public class DamageProcess  {

    MobaEntity m_attacker;
    public MobaEntity Attacker
    {
        get { return m_attacker; }
    }
    MobaEntity m_target;
    public MobaEntity Target
    {
        get { return m_target; }
    }
    Ability m_ability;

    private float m_attackDamage;
    public float AttackDamage
    {
        get { return m_attackDamage; }
        set { m_attackDamage = value; }
    }
    private float m_abilityDamage;
    public float AbilityDamage
    {
        get { return m_abilityDamage; }
        set { m_abilityDamage = value; }
    }

    //Attack Damage Before Resistance Modifier
    private float m_adBResMod = 1;
    public float AdBResMod
    {
        get { return m_adBResMod; }
        set { m_adBResMod = value; }
    }
    //Ability Power Before Resistance Modifier
    private float m_apBResMod = 1;
    public float ApBResMod
    {
        get { return m_apBResMod; }
        set { m_apBResMod = value; }
    }

    //Attack Damage After Resistance Modifier
    private float m_adAResMod = 1;
    public float AdAResMod
    {
        get { return m_adAResMod; }
        set { m_adAResMod = value; }
    }
    //Ability Power After Resistance Modifier
    private float m_apAResMod = 1;
    public float ApAResMod
    {
        get { return m_apAResMod; }
        set { m_apAResMod = value; }
    }

    public DamageProcess(MobaEntity attacker, MobaEntity target, Ability ability)
    {
        m_attacker = attacker;
        m_target = target;
        m_ability = ability;
        if (m_attacker != null && m_target != null && m_ability != null)
        {
            RunProcess();
        }
    }

    void RunProcess()
    {
        if (m_attacker.OnAttackDamageProcessStart != null)
        {
            m_attacker.OnAttackDamageProcessStart(this);
        }
        if (m_target.OnRecieveDamageProcessStart != null)
        {
            m_target.OnRecieveDamageProcessStart(this);
        }

        //Calculate Damage from the ability parameters
        CalculateTotalDamage(out m_attackDamage, out m_abilityDamage);
        if (m_attackDamage != 0 || m_abilityDamage != 0)
        {
            //Apply Damage Modifiers Before Resistance
            m_attackDamage *= m_adBResMod;
            m_abilityDamage *= m_apBResMod;

            //Apply Damage Resistances
            m_attackDamage = CalculateDamageAfterResistance(m_attackDamage);
            m_abilityDamage = CalculateAbilityAfterResistance(m_abilityDamage);

            //Calculate final Damage and apply Damage Modifiers After Resistance
            m_target.DealDamage((m_attackDamage * m_adAResMod) + (m_abilityDamage * m_apAResMod), this);

        }

        //Apply side effects
        if (!m_target.Dead)
        {
            m_target.EntityAbilities.ApplyAbilityEffects(m_ability, m_attacker);
        }

    }
    public void CalculateTotalDamage(out float baseAttackDamage, out float baseAbilityDamage)
    {
        baseAttackDamage = 0;
        baseAbilityDamage = 0;

        //If the Ability doesnt deal damage return 0
        if (m_ability.AbilityBase.DescriptionParamList.Count == 0)
        {
            return;
        }

        foreach (EDescriptionParameter damageType in m_ability.AbilityBase.DescriptionParamList)
        {
            switch (damageType)
            {
                case EDescriptionParameter.Entity_BaseDamage:
                    baseAttackDamage += m_attacker.EntityAbilities.BaseAttackDamage;
                    break;
                case EDescriptionParameter.Entity_BaseAbility:
                    baseAbilityDamage += m_attacker.EntityAbilities.BaseMagicDamage;
                    break;
                case EDescriptionParameter.Ability_AttackDamage:
                    baseAttackDamage += m_ability.BaseDamage;
                    break;
                case EDescriptionParameter.Ability_AbilityPower:
                    baseAbilityDamage += m_ability.BaseAbilityPower;
                    break;
                default:
                    break;
            }
        }
        //TODO: ADD ITEM DAMAGE MOD
        baseAttackDamage += m_attacker.EntityAbilities.AttackDamageMod;
        baseAbilityDamage += m_attacker.EntityAbilities.MagicDamageMod;

        //Add DamageModifiers
        float damageMod = (baseAttackDamage * m_attacker.EntityAbilities.AttackDamagePercentMod) / 100;
        baseAttackDamage += damageMod;
        float abilityMod = (baseAbilityDamage * m_attacker.EntityAbilities.MagicDamagePercentMod) / 100;
        baseAbilityDamage += abilityMod;

    }

    public float CalculateDamageAfterResistance(float baseAttackDamage)
    {
        float baseArmor = m_target.EntityAbilities.Armor + m_target.EntityAbilities.ArmorMod;
        float totalArmor = baseArmor + (baseArmor * (m_target.EntityAbilities.ArmorResPercentMod / 100));
        float totalReductionMultiplier = totalArmor / (totalArmor + 100);
        float totalDamageReduced = baseAttackDamage * totalReductionMultiplier;
        //Debug.Log("<color=green>Moba Storm : </color> Attack :" + baseAttackDamage + "Attack Reduced :" + totalDamageReduced);
        return baseAttackDamage - totalDamageReduced;
    }

    public float CalculateAbilityAfterResistance(float baseAbilityDamage)
    {
        float baseMagicRes = m_target.EntityAbilities.MagicRes + m_target.EntityAbilities.MagicResMod;
        float totalMagicRes = baseMagicRes + (baseMagicRes * (m_target.EntityAbilities.MagicResPercentMod / 100));
        float totalReductionMultiplier = totalMagicRes / (totalMagicRes + 100);
        float totalMagicReduced = baseAbilityDamage * totalReductionMultiplier;
        //Debug.Log("<color=green>Moba Storm : </color> Magic :" + baseAbilityDamage + "Magic Reduced :" + totalMagicReduced);
        return baseAbilityDamage - totalMagicReduced;
    }
}
