using UnityEngine;
using System.Collections;
using System;

public class AbilitySimpleTarget : Behaviour
{
    private EAbilityState m_currentState = EAbilityState.MovingCharacter;
    private MobaEntity m_attacker;
    private EntityAnimator m_attackerAnimator;
    private MobaEntity[] m_targets;
    private MobaEntity Target
    {
        get { return m_targets[0]; }
        set
        {
            m_targets = new MobaEntity[1];
            m_targets[0] = value;
        }
    }
    private EntityAbilities m_attackerAbilities;
    private float m_animationTime;
    private float m_castingTime;
    private float m_launchTime;
    private float m_launchParticleTime;
    private bool m_launchParticle = false;
    private bool m_abilityLaunched = false;
    private float m_updatePathRate;

    public AbilitySimpleTarget(MobaEntity attacker, MobaEntity[] targets, Ability ability)
    {
        m_targets = targets;
        m_ability = ability;
        m_attacker = attacker;
        m_attacker.StopAgent(false);
        m_attackerAbilities = m_attacker.GetComponent<EntityAbilities>();
        m_attackerAnimator = m_attacker.GetComponent<EntityAnimator>();
        //m_attacker.EntityAbilities.IsCasting = true;
    }

    public AbilitySimpleTarget(MobaEntity attacker, MobaEntity target, Ability ability)
    {
        m_ability = ability;
        m_attacker = attacker;
        m_attacker.StopAgent(false);
        m_attackerAbilities = m_attacker.GetComponent<EntityAbilities>();
        m_attackerAnimator = m_attacker.GetComponent<EntityAnimator>();
        Target = target;
    }

    public override void OnFinish()
    {
    }

    public override void OnStart()
    {
        
       
    }

    private void ProcessAbility()
    {
        m_attacker.EntityAbilities.IsCasting = true;
        if (m_ability.AbilityBase.PlaysCastAnimation)
        {
            ChangeState(EAbilityState.CastingState);
        }
        else if (m_ability.AbilityBase.PlayAbilityAnimation)
        {
            ChangeState(EAbilityState.PlayingAnimation);
        }
        else
        {
            LaunchAbility();
            ChangeState(EAbilityState.AbilityEnds);
        }

        //Send facing direction to the clients
        Vector3 direction;
        direction = m_targets[0].transform.position - m_attacker.transform.position;
        if (m_ability.AbilityBase.FaceTarget)
        {
            m_attacker.transform.rotation = Quaternion.LookRotation(direction);
            m_attacker.EntityAbilities.RpcFaceTarget(Quaternion.LookRotation(direction), NetworkTime.Instance.ServerStep());
        }
    }
    private void ChangeState(EAbilityState newState)
    {
        m_currentState = newState;
        switch (newState)
        {
            case EAbilityState.MovingCharacter:

                break;
            case EAbilityState.CastingState:
                m_castingTime = m_ability.AbilityBase.CastTime;
                m_attackerAnimator.ChangeState(m_ability.AbilityBase.CastingAnimation);
                break;
            case EAbilityState.PlayingAnimation:
                m_animationTime = m_attackerAnimator.ChangeState(m_ability.AbilityBase.Animation);
                m_launchTime = (m_animationTime * m_ability.AbilityBase.AnimationPercentLaunch) / 100;
                m_launchParticleTime = (m_animationTime * m_ability.AbilityBase.LaunchParticlePercentLaunch) / 100;
                break;
            case EAbilityState.LaunchProjectileDelayed:
                break;
            case EAbilityState.AbilityEnds:
                if (m_ability.AttackType == EAttackType.Basic)
                {
                    m_attacker.EntityBehaviour.StopAllBehaviours();
                    m_attackerAnimator.ChangeState(EEntityState.Idle);
                    //TODO: FIX THIS CODE BY CHECKING ON ENTITY IF HAVE ANY TARGET
                    if (m_attacker is CharacterEntity)
                    {
                        m_attacker.EntityBehaviour.AddBehaviour(new AbilitySimpleTarget(m_attacker, m_targets[0], m_ability));
                    }
                }
                else
                {
                    m_attacker.EntityBehaviour.StopAllBehaviours();
                    m_attackerAnimator.ChangeState(EEntityState.Idle);
                }
                break;
            default:
                break;
        }
    }  

    private void LaunchAbility()
    {
        if (!m_abilityLaunched)
        {
            m_abilityLaunched = true;
            m_attackerAbilities.ServerCoolDownAbility(m_ability);
            m_attacker.EntityAbilities.IsCasting = false;
            LaunchAbilityTarget();

            if (!string.IsNullOrEmpty(m_ability.AbilityBase.LaunchSoundIdentifier))
            {
                AudioManager.instance.ServerPlay3dSound(m_ability.AbilityBase.LaunchSoundIdentifier, m_ability.AbilityBase.LaunchSoundVolume, m_attacker.transform.position);
            }
        }
    }

   
    private void LaunchAbilityTarget()
    {
        switch (m_ability.AbilityBase.Processor)
        {
            case AbilityProcessorType.DamageImpact:
                foreach (MobaEntity entity in m_targets)
                {
                   
                    new DamageProcess(m_attacker, entity, m_ability);
                    if (m_ability.AbilityBase.ImpactParticleIdentifier != "")
                    {
                        EntityTransform impactTarget = entity.GetTransformPosition(m_ability.AbilityBase.LaunchPos);
                        SpawnManager.instance.InstantiatePool(m_ability.AbilityBase.ImpactParticleIdentifier, impactTarget.transform.position, impactTarget.transform.rotation);
                        entity.EntityAbilities.RpcSpawnImpactParticle(m_ability.AbilityBase.ImpactParticleIdentifier, (int)EEntityTransform.Center);                       
                    }
                    
                }
                break;
            case AbilityProcessorType.LaunchAbility:
                foreach (MobaEntity targetEntity in m_targets)
                {
                    EntityTransform spawnPos = m_attacker.GetTransformPosition(m_ability.AbilityBase.LaunchPos);
                    GameObject projectileObj = SpawnManager.instance.InstantiatePool(m_ability.AbilityBase.ProjectileIdentifier, spawnPos.transform.position, spawnPos.transform.rotation);
                    AbilityComponent projectile = projectileObj.GetComponent<AbilityComponent>();
                    projectile.Initialize(m_attacker, targetEntity, m_ability, m_attacker.EntityAbilities.ServerAbilityHit);
                    int projectileInstanceID = projectile.gameObject.GetInstanceID();
                    projectile.ServerInstanceId = m_ability.AbilityBase.ProjectileIdentifier + "_" + projectileInstanceID;
                    m_attacker.EntityAbilities.CacheActiveAbility(projectile);
                    m_attacker.EntityAbilities.RpcSpawnTargetProjectile(m_attacker.transform.rotation , m_ability.AbilityBase.ProjectileIdentifier, projectile.ServerInstanceId, projectile.Ability.AbilityBase.Identifier, targetEntity.InstanceId, NetworkTime.Instance.ServerStep());
                }
                break;
            case AbilityProcessorType.CastSideEffectsOnly:
                foreach (MobaEntity targetEntity in m_targets)
                {
                    targetEntity.EntityAbilities.ApplyAbilityEffects(m_ability, m_attacker);
                }
                break;
            default:
                break;
        }
    }

    public override void Process()
    {
        switch (m_currentState)
        {
            case EAbilityState.MovingCharacter:
                m_updatePathRate -= Time.fixedDeltaTime;
                if (!Target.Dead)
                {
                    Node node = Pathfinding.instance.GetNode(m_attacker.Position);
                    if (TryToCastAbility(m_targets, m_ability) && (node.walkable || !node.walkable && node.Resident == m_attacker))
                    {
                        ;
                    }
                    else
                    {
                        if ((node.walkable || !node.walkable && node.Resident != m_attacker) && !m_ability.IsOnRange(m_attacker, Target.Position))
                        {
                            if (m_updatePathRate <= 0)
                            {
                                if (!node.walkable && node.Resident != m_attacker)
                                {
                                    m_attacker.ServerCalculatePath(new Vector2(0, 0));
                                    m_updatePathRate = GameDataManager.instance.GlobalConfig.m_targetPathUpdateRate;
                                }
                                else
                                {
                                    m_attacker.ServerCalculatePath(Target.Position);
                                    m_updatePathRate = GameDataManager.instance.GlobalConfig.m_targetPathUpdateRate;
                                }

                            }

                        }
                        else
                        {
                            if (node.walkable && m_attacker.FollowingPath)
                            {
                                m_attacker.StopAgent(true);
                            }
                        }
                    }
                }
                else
                {
                    m_attacker.EntityBehaviour.StopAllBehaviours();
                }
                break;
            case EAbilityState.CastingState:
                ProcessLaunchingTime(ELaunchingState.CastingState);

                m_castingTime -= Time.fixedDeltaTime;
                if (m_castingTime <= 0)
                {
                    if (m_ability.AbilityBase.PlayAbilityAnimation)
                    {
                        ChangeState(EAbilityState.PlayingAnimation);
                    }
                    else
                    {
                        ChangeState(EAbilityState.AbilityEnds);
                    }
                }
                break;
            case EAbilityState.PlayingAnimation:
                ProcessLaunchingTime(ELaunchingState.AnimationState);
                if (m_launchParticle == false && m_ability.AbilityBase.LaunchParticleIdentifier != "")
                {
                    if (m_launchParticleTime <= 0)
                    {
                        m_launchParticle = true;
                        m_attacker.EntityAbilities.RpcSpawnImpactParticle(m_ability.AbilityBase.LaunchParticleIdentifier, (int)m_ability.AbilityBase.LaunchPos);
                    }
                }
                m_launchTime -= Time.fixedDeltaTime;
                m_animationTime -= Time.fixedDeltaTime;
                m_launchParticleTime -= Time.fixedDeltaTime;
                if (m_launchTime <= 0)
                {
                    LaunchAbility();
                }
                if (m_animationTime <= 0)
                {
                    ChangeState(EAbilityState.AbilityEnds);
                }
                break;
            case EAbilityState.LaunchProjectileDelayed:
                break;

            default:
                break;
        }

    }

    private void ProcessLaunchingTime(ELaunchingState state)
    {
        if (state == m_ability.AbilityBase.LaunchingType)
        {
            m_launchTime -= Time.fixedDeltaTime;
            if (m_launchTime <= 0)
            {
                LaunchAbility();
            }
        }
    }

    private bool TryToCastAbility(MobaEntity[] targets, Ability ability)
    {
        //Target Caculation
        if (ability.IsOnRange(m_attacker, Target.Position))
        {
            if (ability.CoolDown <= 0)
            {
                m_attacker.StopAgent(false);
                Debug.Log("<color=blue>Moba Storm : </color> Casting Ability :" + ability.AbilityBase.Identifier);
                ProcessAbility();
            }
            else
            {
                if (m_attackerAnimator.CurrentState != EEntityState.Idle)
                {
                    m_attacker.StopAgent(true);
                }
            }
            return true;
        }
        return false;
    }
}
