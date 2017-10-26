using UnityEngine;
using System.Collections;
using System;

public class AbilityHitTask : Task {

    string m_instanceId;
    MobaEntity m_attacker;
    bool m_abilityHit = false;

    public AbilityHitTask(MobaEntity attacker, string instanceId, int time, bool abilityHit) : base(time)
    {
        m_attacker = attacker;
        m_instanceId = instanceId;
        m_abilityHit = abilityHit;
    }

    public override void Run()
    {
        m_attacker.EntityAbilities.ClientAbilityHit(m_instanceId, m_abilityHit);
    }

}
