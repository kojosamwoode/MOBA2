using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class AITargetBehaviour : Behaviour
{
    private MobaEntity m_attacker;
    private MobaEntity[] m_targets;
    
    private float m_updatePathRate;
    private float m_forcePathRate;
    private MobaEntity Target
    {
        get { return m_targets[0]; }
        set
        {
            m_targets = new MobaEntity[1];
            m_targets[0] = value;
        }
    }

    public AITargetBehaviour(MobaEntity attacker, Ability ability)
    {
        m_ability = ability;
        m_attacker = attacker;
        Target = attacker.Target;
    }

    public override void OnFinish()
    {

    }

    public override void OnStart()
    {
    }

    public override void Process()
    {
        m_updatePathRate -= Time.fixedDeltaTime;
        m_forcePathRate -= Time.fixedDeltaTime;
        if (!Target.Dead)
        {
            if (!m_ability.IsOnRange(m_attacker, Target.Position))
            {
                if (m_updatePathRate <= 0)
                {
                    if (m_attacker.LockedNode != null)
                    {
                        m_attacker.LockedNode.Resident = null;
                        m_attacker.LockedNode = null;
                    }
                    m_attacker.LockedNode = Pathfinding.instance.GeNearestNeighboursInRange(m_attacker, Target.Position, m_ability.Range);
                    m_attacker.ServerCalculatePath(m_attacker.LockedNode.m_position);
                    m_updatePathRate = GameDataManager.instance.GlobalConfig.m_pathUpdateRate;
                }
                
            }
            else
            {
                if (m_attacker.LockedNode == null)
                {
                    m_attacker.LockedNode = Pathfinding.instance.GeNearestNeighboursInRange(m_attacker, Target.Position, m_ability.Range);
                    m_attacker.ServerCalculatePath(m_attacker.LockedNode.m_position);
                    m_updatePathRate = GameDataManager.instance.GlobalConfig.m_pathUpdateRate;
                }
                if (m_forcePathRate <= 0) //&& Vector2.Distance(m_attacker.Position, Target.Position) < 0.5)
                {
                    if (m_forcePathRate <= 0)
                    {
                        if (m_attacker.LockedNode != null)
                        {
                            m_attacker.LockedNode.Resident = null;
                            m_attacker.LockedNode = null;
                        }
                        Node currentNode = Pathfinding.instance.GetNode(m_attacker.Position);
                        if (currentNode.Resident == null)
                        {
                            currentNode.Resident = m_attacker;
                            m_attacker.LockedNode = currentNode;
                            m_attacker.StopAgent(true);
                            m_forcePathRate = GameDataManager.instance.GlobalConfig.m_pathUpdateRate;
                        }
                        else
                        {
                            m_attacker.LockedNode = Pathfinding.instance.GeNearestNeighboursInRange(m_attacker, m_attacker.Position, m_ability.Range);
                            m_attacker.ServerCalculatePath(m_attacker.LockedNode.m_position);
                            m_forcePathRate = GameDataManager.instance.GlobalConfig.m_pathUpdateRate;
                        }
                        
                    }
                }
                
            }

            if (!m_attacker.FollowingPath)
            {
                if (TryToCastAbility(m_targets, m_ability))
                {
                }
            }           
        }
        else
        {
            m_attacker.EntityBehaviour.StopAllBehaviours();
        }
        
    }

    public bool TryToCastAbility(MobaEntity[] targets, Ability ability)
    {
        if (ability.AbilityBase.RequiresTarget)
        {
            //Target Caculation
            if (ability.IsOnRange(m_attacker, Target.Position) && m_ability.CoolDown <= 0)
            {
                //Debug.Log("<color=blue>Moba Storm : </color> Casting Ability :" + currentAbility.Identifier);
                m_attacker.EntityBehaviour.StopAllBehaviours();
                m_attacker.EntityBehaviour.AddBehaviour(new AbilitySimpleTarget(m_attacker, targets, ability));
                return true;
            }
            return false;
                
        }           

        return false;
    }
}
