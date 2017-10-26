using UnityEngine;
using System.Collections;
using System;

public class PositionAbilityTask : Task {

    public Quaternion m_entityRotation;
    string m_projectileIdentifier;
    string m_instanceId;
    string m_abilityIdentifier;
    Vector3 m_targetPos;
    MobaEntity m_attacker;

    public PositionAbilityTask(MobaEntity attacker, Quaternion entityRotation, string projectileIdentifier, string instanceId, string abilityIdentifier, Vector3 targetPos, int time) : base(time)
    {
        m_attacker = attacker;
        m_projectileIdentifier = projectileIdentifier;
        m_entityRotation = entityRotation;
        m_instanceId = instanceId;
        m_abilityIdentifier = abilityIdentifier;
        m_targetPos = targetPos;
    }

    public override void Run()
    {
        m_attacker.transform.rotation = m_entityRotation;
        AbilityBase abilityBase = AbilityManager.instance.GetAbilityBase(m_abilityIdentifier);
        Ability ability = m_attacker.EntityAbilities.GetAbility(abilityBase.AttackType);

        EntityTransform launchPos = m_attacker.GetTransformPosition(abilityBase.LaunchPos);
        GameObject projectileObj = SpawnManager.instance.InstantiatePool(m_projectileIdentifier + "Client", launchPos.transform.position, launchPos.transform.rotation);
        AbilityComponent abilityComponent = projectileObj.GetComponent<AbilityComponent>();
        abilityComponent.ServerInstanceId = m_instanceId;
        abilityComponent.Initialize(m_attacker, m_targetPos, ability);
        m_attacker.EntityAbilities.CacheActiveAbility(abilityComponent);
        
    }

}
