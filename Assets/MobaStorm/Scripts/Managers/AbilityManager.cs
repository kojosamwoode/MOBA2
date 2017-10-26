using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AbilityManager : MonoSingleton<AbilityManager> {
    public List<AbilityBase> m_abilities = new List<AbilityBase>();
    // Use this for initialization

    private Dictionary<string, AbilityBase> m_abilitiesDic = new Dictionary<string, AbilityBase>();
    private Dictionary<string, SideEffect> m_sideEffectsDic = new Dictionary<string, SideEffect>();

    [SerializeField]
    private int m_maxAbilityLevel;
    public int MaxAbilityLevels
    {
        get { return m_maxAbilityLevel; }
        set { m_maxAbilityLevel = value; }
    }

    void Start () {
        foreach (AbilityBase ability in m_abilities)
        {
            m_abilitiesDic.Add(ability.Identifier, ability);
            foreach (SideEffect effect in ability.SideEffects)
            {
                //string effectIdentifier = ability.Identifier + "_" + effect.EffectIdentifier;
                if (!m_sideEffectsDic.ContainsKey(effect.EffectIdentifier))
                {
                    m_sideEffectsDic.Add(effect.EffectIdentifier, effect);
                }
                else
                {
                    Debug.LogError("Error... Effect Identifier duplicated: " + effect.EffectIdentifier);
                }
            }
        }
	
	}


    /// <summary>
    /// Returns a dictionary with all Entity Abilities available for this entity
    /// </summary>
    /// <param name="entity">Current Entity</param>
    /// <returns></returns>
    public Dictionary<EAttackType, Ability> GetEntityAbilities(MobaEntity entity)
    {
        Dictionary<EAttackType, Ability> abilities = new Dictionary<EAttackType, Ability>();
        foreach(string abilityName in entity.EntityData.m_characterAbilities)
        {
            AbilityBase abilityBase = GetAbilityBase(abilityName);
            if (abilityBase != null)
            {
                abilities.Add(abilityBase.AttackType, new Ability(abilityBase, abilityBase.AttackType, entity));
            }         
        }

        return abilities;
    }

    /// <summary>
    /// Get an ability from its Identifier
    /// </summary>
    /// <param name="identifier">Ability Identifier</param>
    /// <returns>AbilityBase</returns>
    public Ability GenerateEntityAbility(string identifier, EAttackType type, MobaEntity entity)
    {
        if (m_abilitiesDic.ContainsKey(identifier))
        {
            return new Ability(m_abilitiesDic[identifier],type, entity);
        }

        return null;
    }

    /// <summary>
    /// Get an ability from its Identifier
    /// </summary>
    /// <param name="identifier">Ability Identifier</param>
    /// <returns>AbilityBase</returns>
    public AbilityBase GetAbilityBase(string identifier)
    {
        if (m_abilitiesDic.ContainsKey(identifier))
        {
            return m_abilitiesDic[identifier];
        }

        return null;
    }

    /// <summary>
    /// Get a ability Side Effect from its Identifier
    /// </summary>
    /// <param name="sideEffectIdentifier">Side Effect Identifier</param>
    /// <returns></returns>
    public SideEffect GetSideEffect(string sideEffectIdentifier)
    {
        if (m_sideEffectsDic.ContainsKey(sideEffectIdentifier))
        {
            return m_sideEffectsDic[sideEffectIdentifier];
        }

        return null;
    }
}
