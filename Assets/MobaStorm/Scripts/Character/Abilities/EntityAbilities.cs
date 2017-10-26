using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

[NetworkSettings(channel = 0, sendInterval = 0.040f)]
public class EntityAbilities : NetworkBehaviour {

    private MobaEntity m_mobaEntity;

    private bool m_isCasting;
    public bool IsCasting
    {
        get { return m_isCasting; }
        set { m_isCasting = value; }
    }

    private bool m_smartCast = false;
    public bool SmartCast
    {
        get { return m_smartCast; }
        set { m_smartCast = value; }
    }

    private List<SideEffect> m_appliedEffects;
    public List<SideEffect> SideEffects
    {
        get { return m_appliedEffects; }
        set { m_appliedEffects = value; }
    }

    private List<SideEffect> m_passiveEffects;
    public List<SideEffect> PassiveEffects
    {
        get { return m_passiveEffects; }
        set { m_passiveEffects = value; }
    }

    [SyncVar(hook = "AvailablePointsChanged")]
    private int m_availablePoints;
    public int AvailablePoints
    {
        get { return m_availablePoints; }
        set { m_availablePoints = value; }
    }

    public Action NewAbilityPointCallback;
    public Action<Ability> AbilityLevelUpCallback;

    [SyncVar(hook = "CoolDownPercentModChange")]
    private float m_coolDownPercentMod = 0;
    private void CoolDownPercentModChange(float value)
    {
        m_coolDownPercentMod = value;
        m_mobaEntity.EntityDataChanged();

    }
    [SyncVar(hook = "BaseAdChanged")]
    private float m_baseAd;
    private void BaseAdChanged(float value)
    {
        m_baseAd = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "AttackDamageModChanged")]
    private float m_attackDamageMod;
    private void AttackDamageModChanged(float value)
    {
        m_attackDamageMod = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "AttackDamagePercentModChanged")]
    private float m_attackDamagePercentMod;
    private void AttackDamagePercentModChanged(float value)
    {
        m_attackDamagePercentMod = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "BaseMagicDamageChanged")]
    private float m_baseMagicDamage;
    private void BaseMagicDamageChanged(float value)
    {
        m_baseMagicDamage = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "AbilityDamageModChanged")]
    private float m_magicDamageMod;
    private void AbilityDamageModChanged(float value)
    {
        m_magicDamageMod = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "PercentApModChanged")]
    private float m_magicDamagePercentMod;
    private void PercentApModChanged(float value)
    {
        m_magicDamagePercentMod = value;
        m_mobaEntity.EntityDataChanged();
    }

    [SyncVar(hook = "ArmorChanged")]
    private float m_armor;
    private void ArmorChanged(float value)
    {
        m_armor = value;
        m_mobaEntity.EntityDataChanged();
    }

    [SyncVar(hook = "ArmorModChanged")]
    private float m_armorMod;
    private void ArmorModChanged(float value)
    {
        m_armorMod = value;
        m_mobaEntity.EntityDataChanged();
    }

    [SyncVar(hook = "ArmorPercentModChanged")]
    private float m_armorPercentMod;
    private void ArmorPercentModChanged(float value)
    {
        m_armorPercentMod = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "MagicResChanged")]
    private float m_magicRes;
    private void MagicResChanged(float value)
    {
        m_magicRes = value;
        m_mobaEntity.EntityDataChanged();
    }
    [SyncVar(hook = "MagicResModChanged")]
    private float m_magicResMod;
    private void MagicResModChanged(float value)
    {
        m_magicResMod = value;
        m_mobaEntity.EntityDataChanged();
    }

    [SyncVar(hook = "MagicResPercentModChanged")]
    private float m_magicResPercentMod;
    private void MagicResPercentModChanged(float value)
    {
        m_magicResPercentMod = value;
        m_mobaEntity.EntityDataChanged();
    }

    public float BaseAttackDamage { get { return m_baseAd; } set { m_baseAd = value; } }
    public float AttackDamageMod { get { return m_attackDamageMod; } set { m_attackDamageMod = value; } }
    public float AttackDamagePercentMod { get { return m_attackDamagePercentMod; } set { m_attackDamagePercentMod = value; } }

    public float BaseMagicDamage { get { return m_baseMagicDamage; } set { m_baseMagicDamage = value; } }
    public float MagicDamageMod { get { return m_magicDamageMod; } set { m_magicDamageMod = value; } }
    public float MagicDamagePercentMod { get { return m_magicDamagePercentMod; } set { m_magicDamagePercentMod = value; } }

    public float CoolDownPercentMod { get { return m_coolDownPercentMod; } set { m_coolDownPercentMod = value; } }

    public float Armor { get { return m_armor; } set { m_baseAd = value; } }
    public float ArmorMod { get { return m_armorMod; } set { m_armorMod = value; } }
    public float ArmorResPercentMod { get { return m_armorPercentMod; } set { m_armorPercentMod = value; } }

    public float MagicRes { get { return m_magicRes; } set { m_magicRes = value; } }
    public float MagicResMod { get { return m_magicResMod; } set { m_magicResMod = value; } }
    public float MagicResPercentMod { get { return m_magicResPercentMod; } set { m_magicResPercentMod = value; } }

    /// <summary>
    /// Dictionary with all entity abilities
    /// </summary>
    private Dictionary<EAttackType, Ability> m_abilities = new Dictionary<EAttackType, Ability>();
    public Dictionary<EAttackType, Ability> Abilities
    {
        get { return m_abilities; }
    }

    private Dictionary<string, AbilityComponent> m_activeAbilities = new Dictionary<string, AbilityComponent>();

    private Dictionary<string, SideEffectPrefab> m_sideEffectPrefabList = new Dictionary<string, SideEffectPrefab>();
    void Awake()
    {
        m_mobaEntity = GetComponent<MobaEntity>();
        m_appliedEffects = new List<SideEffect>();
        m_passiveEffects = new List<SideEffect>();

    }


    void FixedUpdate()
    {
        foreach (Ability ability in m_abilities.Values)
        {
            ability.UpdateCdr(isClient);
        }
        for (int i = 0; i< m_appliedEffects.Count; i++)
        {
            SideEffect effect = m_appliedEffects[i];
            if (!effect.Isfinish)
            {
                effect.ProcessEffect();
            }
            else
            {
                m_appliedEffects.Remove(effect);
                effect.FinishEffect();
                return;
            }
        }

        for (int i = 0; i < m_passiveEffects.Count; i++)
        {
            SideEffect effect = m_passiveEffects[i];
            if (!effect.Isfinish)
            {
                effect.ProcessEffect();
            }
            else
            {
                m_passiveEffects.Remove(effect);
                effect.FinishEffect();
                return;
            }
        }
    }


    public void LoadAbilities()
    {
        //Cache all abilities to a dictionary
        m_abilities = AbilityManager.instance.GetEntityAbilities(m_mobaEntity);     
    }

    public Ability GetAbility(EAttackType type)
    {
        if (m_abilities.ContainsKey(type))
        {
            return m_abilities[type];
        }
        Debug.LogError("Ability identifier" + type + " for: not found on the Abilties");
        return null;
    }

    public void AddAbility(EAttackType type, string abilityIdentifier, int level = 0)
    {
        Ability ability = AbilityManager.instance.GenerateEntityAbility(abilityIdentifier, type, m_mobaEntity);
        ability.Level = level;
        if (ability != null)
        {
            if (m_abilities.ContainsKey(type))
            {
                Debug.LogError("Error: Ability AlreadyCreated");
            }
            else
            {
                m_abilities.Add(type, ability);
            }
        }
        else
        {
            Debug.LogError("Error: AbilityBase is Null");
        }
    }

    public void RemoveAbility(EAttackType type)
    {
        if (m_abilities.ContainsKey(type))
        {
            m_abilities.Remove(type);
        }
    }

    #region ABILITY COLDDOWN
    public void ResetAllColdDown(EAttackType abilityType)
    {
        foreach (Ability ability in m_abilities.Values)
        {
            ability.CoolDown = 0;
        }
    }


    public void CoolDownAbility(EAttackType abilityType)
    {
       m_abilities[abilityType].CoolDown = m_abilities[abilityType].CoolDownTotal - (m_abilities[abilityType].CoolDownTotal * (m_coolDownPercentMod / 100));
    }
    #endregion

    #region ABILITY LEVELING
    public void ServerTryLevelUpAbility(EAttackType abilityType)
    {
        if (m_abilities[abilityType].CanLevelUp && m_abilities.ContainsKey(abilityType))
        {
            if (m_availablePoints <= 0)
            {
                Debug.LogError("Error: Not available ability points: " + abilityType);
                return;
            }
            m_availablePoints--;
            m_abilities[abilityType].Level++;
            RpcSetAbilityLevel((int)abilityType, m_abilities[abilityType].Level);
            if (AbilityLevelUpCallback != null)
            {
                AbilityLevelUpCallback(m_abilities[abilityType]);
            }
        }
        else
        {
            Debug.LogError("Error: Ability type does not exist: " + abilityType);
        }
    }

    public void GiveAbilityPoint()
    {
        m_availablePoints++;
    }

    void AvailablePointsChanged(int points)
    {
        m_availablePoints = points;
        if (NewAbilityPointCallback != null)
        {
            NewAbilityPointCallback();
        }
    }

    public int GetAbilityLevel(EAttackType type)
    {
        if (m_abilities.ContainsKey(type))
        {
            return m_abilities[type].Level;
        }
        Debug.LogError("Ability identifier" + type + " for: not found on the Abilties");
        return 0;
    }
    #endregion

    #region ACTIVE ABILITIES HANDLING
    public void CacheActiveAbility(AbilityComponent projectile)
    {
        m_activeAbilities.Add(projectile.ServerInstanceId, projectile);
    }


    public void ServerAbilityHit(AbilityComponent projectile, bool abilityHit)
    {
        SpawnManager.instance.DestroyPool(projectile.gameObject);
        if (m_activeAbilities.ContainsKey(projectile.ServerInstanceId))
        {
            m_activeAbilities.Remove(projectile.ServerInstanceId);
        }
        RpcAbilityHitOnClient(projectile.ServerInstanceId, NetworkTime.Instance.ServerStep(), abilityHit);
    }

    public void ClientAbilityHit(string instanceId, bool abilityHit)
    {
        if (m_activeAbilities.ContainsKey(instanceId))
        {
            AbilityComponent AbilityComponent = m_activeAbilities[instanceId];
            AbilityBase abilityBase = AbilityComponent.Ability.AbilityBase;
            SpawnManager.instance.DestroyPool(AbilityComponent.gameObject);

            m_activeAbilities.Remove(AbilityComponent.ServerInstanceId);
            if (abilityHit && AbilityComponent.Ability.AbilityBase.ImpactParticleIdentifier != "")
            {
                SpawnManager.instance.InstantiatePool(abilityBase.ImpactParticleIdentifier, AbilityComponent.transform.position, AbilityComponent.transform.rotation);
            }
            if (abilityHit && !string.IsNullOrEmpty(abilityBase.ImpactSoundIdentifier))
            {
                AudioManager.instance.Play3dSound(abilityBase.ImpactSoundIdentifier, abilityBase.ImpactSoundVolume, AbilityComponent.transform.position);
            }
        }
    }
    #endregion

    #region SIDE EFFECT HANDLING
    public void ApplyAbilityEffects(Ability ability, MobaEntity attacker)
    {
        foreach (SideEffect effect in ability.AbilityBase.SideEffects)
        {
            TryApplySideEffect(ability, effect, attacker, false);
        }
    }

   

    public SideEffect TryApplySideEffect(Ability ability, SideEffect effect, MobaEntity attacker, bool isPasive)
    {
        //TODO: Check here if the effects can be stacked or not
        //for (int i = 0; i < m_appliedEffects.Count; i++)
        //{
        //    if (m_appliedEffects[i].GetType() == effect.GetType())
        //    {
        //        RemoveSideEffect(m_appliedEffects[i]);
        //        break;
        //    }
        //}
        
        SideEffect newEffect = effect.Clone();
        newEffect.ApplyEffect(ability, attacker, m_mobaEntity);
        if (isPasive)
        {
            m_passiveEffects.Add(newEffect);
        }
        else
        {
            m_appliedEffects.Add(newEffect);
        }
        return newEffect;
    }

    public void RemoveAllSideEffects()
    {
        for (int i = 0; i < m_appliedEffects.Count; i++)
        {
            m_appliedEffects[i].FinishEffect();
        }
        m_appliedEffects.Clear();
    }

    public void RemoveSideEffect(SideEffect effect)
    {
        if (m_appliedEffects.Contains(effect))
        {
            m_appliedEffects.Remove(effect);
        }
        else if (m_passiveEffects.Contains(effect))
        {
            m_passiveEffects.Remove(effect);
        }
        effect.FinishEffect();
    }

    public SideEffectPrefab SpawnSideEffectPrefab(SideEffect effect, EntityTransform entityTransform)
    {
        GameObject sideEffectObj = SpawnManager.instance.InstantiatePool(effect.Prefab, entityTransform.transform.position, entityTransform.transform.rotation);
        int projectileInstanceID = sideEffectObj.GetInstanceID();
        SideEffectPrefab prefab = sideEffectObj.GetComponent<SideEffectPrefab>();
        prefab.Initialize(effect.Prefab + "_" + projectileInstanceID);
        m_sideEffectPrefabList.Add(prefab.ServerInstanceId, prefab);
        int transformPosint = (int)entityTransform.EntityTransformType;
        RpcSpawnSideEffectPrefab(effect.EffectIdentifier, prefab.ServerInstanceId, transformPosint);
        return prefab;
    }

    public void UnSpawnSideEffectPrefab(SideEffectPrefab prefab)
    {
        SpawnManager.instance.DestroyPool(prefab.gameObject);
        m_sideEffectPrefabList.Remove(prefab.ServerInstanceId);
        RpcUnSpawnSideEffectPrefab(prefab.ServerInstanceId);
    }
    #endregion

    #region CLIENT RPC METHODS
    [ClientRpc]
    public void RpcSetAbilityLevel(int attackType, int level)
    {
        
        EAttackType type = (EAttackType)attackType;
        if (m_abilities.ContainsKey(type))
        {
            m_abilities[type].Level = level;
            if (AbilityLevelUpCallback != null)
            {
                AbilityLevelUpCallback(m_abilities[type]);
            }
        }
        else
        {
            Debug.LogError("Error: Ability type does not exist: " + type);
        }

    }

    [ClientRpc]
    public void RpcSpawnImpactParticle(string prefabName, int transformPos)
    {
        EntityTransform entityTransform = m_mobaEntity.GetTransformPosition((EEntityTransform)transformPos);
        GameObject particle = SpawnManager.instance.InstantiatePool(prefabName, entityTransform.transform.position, entityTransform.transform.rotation);
        particle.transform.SetParent(entityTransform.transform);
    }
    [ClientRpc]
    public void RpcSpawnSideEffectPrefab(string effectIdentifier, string serverInstanceId, int transformPos)
    {
        SideEffect effect = AbilityManager.instance.GetSideEffect(effectIdentifier);
        EntityTransform entityTransform = m_mobaEntity.GetTransformPosition((EEntityTransform)transformPos);
        GameObject sideEffectObj = SpawnManager.instance.InstantiatePool(effect.Prefab, entityTransform.transform.position, entityTransform.transform.rotation);
        sideEffectObj.transform.SetParent(entityTransform.transform);
        SideEffectPrefab prefab = sideEffectObj.GetComponent<SideEffectPrefab>();
        CustomAudioSource audioSource = null;
        if (!string.IsNullOrEmpty(effect.EffectSound))
        {
            audioSource = AudioManager.instance.Play3dSound(effect.EffectSound, effect.Volume, entityTransform.gameObject, effect.LoopSound);
        }

        prefab.Initialize(serverInstanceId, audioSource);
        m_sideEffectPrefabList.Add(prefab.ServerInstanceId, prefab);
    }
    [ClientRpc]
    public void RpcUnSpawnSideEffectPrefab(string serverInstanceId)
    {
        SpawnManager.instance.DestroyPool(m_sideEffectPrefabList[serverInstanceId].gameObject);
        if (m_sideEffectPrefabList[serverInstanceId].AudioSource != null)
        {
            m_sideEffectPrefabList[serverInstanceId].AudioSource.StopSound();
        }
        m_sideEffectPrefabList.Remove(serverInstanceId);
    }

    [ClientRpc]
    public void RpcAbilityHitOnClient(string instanceId, int timeStamp, bool abilityHit)
    {
        NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Ability Hit");
        m_mobaEntity.ClientAddEntityTask(new AbilityHitTask(m_mobaEntity, instanceId, timeStamp, abilityHit));
    }

    [ClientRpc]
    public void RpcFaceTarget(Quaternion rotation, int timeStamp)
    {
        NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Ability Face Target");
        m_mobaEntity.ClientAddEntityTask(new SetRotationTask(m_mobaEntity, rotation, timeStamp));
    }

    [ClientRpc]
    public void RpcSpawnTargetProjectile(Quaternion entityRotation, string projectileIdentifier, string instanceId, string abilityIdentifier, string targetIdentifier, int timeStamp)
    {
        NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Spawn Target Projectile");
        m_mobaEntity.ClientAddEntityTask(new TargetAbilityTask(m_mobaEntity, entityRotation, projectileIdentifier, instanceId, abilityIdentifier, targetIdentifier, timeStamp));
    }

    [ClientRpc]
    public void RpcSpawnPositionProjectile(Quaternion entityRotation, string projectileIdentifier, string instanceId, string abilityIdentifier, Vector3 targetPos, int timeStamp)
    {
        NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Spawn Position Projectile");
        m_mobaEntity.ClientAddEntityTask(new PositionAbilityTask(m_mobaEntity, entityRotation, projectileIdentifier, instanceId, abilityIdentifier, targetPos, timeStamp));
    }
    
    public void ServerCoolDownAbility(Ability ability)
    {
        //If an item is used send a notify to the EntityInventory
        if (ability.AttackType.ToString().Contains("ItemSlot"))
        {
            (m_mobaEntity as CharacterEntity).EntityInventory.ItemUsed(ability.AttackType);
        }
        //Check if the ability used was removed (In the cas of items)
        if (m_abilities.ContainsKey(ability.AttackType))
        {
            m_mobaEntity.Mana -= ability.ManaCost;
            RpcCoolDownOnClient((int)ability.AttackType);
            CoolDownAbility(ability.AttackType);
        }
    }

    [ClientRpc]
    public void RpcCoolDownOnClient(int type)
    {
        CoolDownAbility((EAttackType)type);
    }
    #endregion
}
