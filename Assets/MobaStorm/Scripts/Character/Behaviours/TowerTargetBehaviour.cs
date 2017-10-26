using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class TowerTargetBehaviour : Behaviour
{
    private AIEntity m_attacker;
    private MobaEntity m_target;

    public TowerTargetBehaviour(AIEntity attacker, MobaEntity target,  Ability ability)
    {
        m_target = target;
        m_attacker = attacker;
        m_ability = ability;
    }

    public override void OnFinish()
    {

    }

    public override void OnStart()
    {
    }

    public override void Process()
    {
        if (m_target == null)
        {
            m_attacker.EntityBehaviour.StopAllBehaviours();
            return;
        }

        if (!m_target.Dead)
        {

            if (m_ability.IsOnRange(m_attacker, m_target.Position))
            {
                if (TryToCastAbility(m_target, m_ability))
                {
                    Debug.Log("Casting Ability");
                }
            }
            else
            {
                m_attacker.EntityBehaviour.StopAllBehaviours();
                m_attacker.Target = null;
            }
        }
        else
        {
            m_attacker.EntityBehaviour.StopAllBehaviours();
            m_attacker.Target = null;
        }
        
    }

    public bool TryToCastAbility(MobaEntity target, Ability ability)
    {

        if (ability.AbilityBase.RequiresTarget)
        {
            //Target Caculation
            if (ability.IsOnRange(m_attacker, m_target.Position) && ability.CoolDown <= 0)
            {
                Debug.Log("<color=blue>Moba Storm : </color> Casting Ability :" + ability.AbilityBase.Identifier);
                m_attacker.EntityBehaviour.StopAllBehaviours();
                m_attacker.EntityBehaviour.AddBehaviour(new AbilitySimpleTarget(m_attacker, new MobaEntity[] { target}, ability));
                return true;
            }
            return false;
                
        }

        return false;     

    }
}
