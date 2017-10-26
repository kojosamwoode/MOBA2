using UnityEngine;
using System.Collections;
using System;

public class TargetAbilityTask : Task {

    public Quaternion m_entityRotation;
    string m_projectileIdentifier;
    string m_instanceId;
    string m_abilityIdentifier;
    string m_targetIdentifier;
    MobaEntity m_attacker;

    public TargetAbilityTask(MobaEntity attacker, Quaternion entityRotation, string projectileIdentifier, string instanceId, string abilityIdentifier, string targetIdentifier, int time) : base(time)
    {
        m_attacker = attacker;
        m_projectileIdentifier = projectileIdentifier;
        m_entityRotation = entityRotation;
        m_instanceId = instanceId;
        m_abilityIdentifier = abilityIdentifier;
        m_targetIdentifier = targetIdentifier;
    }

    public override void Run()
    {
        //Debug.Log("Target Ability step" + NetworkTime.Instance.ServerStep());
        m_attacker.transform.rotation = m_entityRotation;
        AbilityBase abilityBase = AbilityManager.instance.GetAbilityBase(m_abilityIdentifier);
        Ability ability = m_attacker.EntityAbilities.GetAbility(abilityBase.AttackType);

        EntityTransform launchPos = m_attacker.GetTransformPosition(abilityBase.LaunchPos);
        GameObject projectileObj = SpawnManager.instance.InstantiatePool(m_projectileIdentifier + "Client", launchPos.transform.position, launchPos.transform.rotation);
        AbilityComponent abilityComponent = projectileObj.GetComponent<AbilityComponent>();
        abilityComponent.ServerInstanceId = m_instanceId;
        MobaEntity target = GameManager.instance.GetMobaEntity(m_targetIdentifier);
        abilityComponent.Initialize(m_attacker, target, ability);
        m_attacker.EntityAbilities.CacheActiveAbility(abilityComponent);
    }

}
