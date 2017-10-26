using UnityEngine;
using System.Collections;
using System;

public enum EAbilityState
{
    MovingCharacter,
    CastingState,
    PlayingAnimation,
    LaunchProjectileDelayed,
    AbilityEnds,
}

public class AbilitySimplePosition : Behaviour
{


    private EAbilityState m_currentState = EAbilityState.MovingCharacter;
    private MobaEntity m_attacker;
    private EntityAnimator m_attackerAnimator;
    private EntityAbilities m_attackerAbilities;
    private Vector2 m_targetPos;
    private float m_animationTime;
    private float m_castingTime;
    private float m_launchTime;
    private bool m_abilityLaunched = false;

    public AbilitySimplePosition(MobaEntity attacker, Vector2 targetPos, Ability ability)
    {
        m_targetPos = targetPos;
        m_ability = ability;
        m_attacker = attacker;
        m_attacker.StopAgent(false);
        m_attackerAbilities = m_attacker.GetComponent<EntityAbilities>();
        m_attackerAnimator = m_attacker.GetComponent<EntityAnimator>();
        m_attacker.EntityAbilities.IsCasting = true;
    }

    
    public override void OnFinish()
    {

    }

    public override void OnStart()
    {
       
      
    }

    private void StartAbility()
    {
        Debug.Log("<color=blue>Moba Storm : </color> Processing Ability :" + m_ability.AbilityBase.Identifier);
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
        }

        //Send facing direction to the clients
        Vector3 direction;
        direction = Utils.Vector2ToVector3(m_targetPos) - Utils.Vector2ToVector3(m_attacker.Position);
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
                //m_animationTime = m_ability.AbilityBase.AnimDuration;
                m_launchTime = (m_animationTime * m_ability.AbilityBase.AnimationPercentLaunch) / 100;
                break;
            case EAbilityState.LaunchProjectileDelayed:
                break;
            case EAbilityState.AbilityEnds:
                m_attacker.EntityBehaviour.StopAllBehaviours();
                m_attackerAnimator.ChangeState(EEntityState.Idle);

                break;
            default:
                break;
        }
    }

    

    public bool TryToCastAbility(Vector2 targetPos)
    {
        //Target Caculation
        if (m_ability.IsOnRange(m_attacker, targetPos))
        {
            Debug.Log("<color=blue>Moba Storm : </color> Casting Ability :" + m_ability.AbilityBase.Identifier);
            m_attacker.StopAgent(false);
            StartAbility();
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void LaunchAbility()
    {
        if (!m_abilityLaunched)
        {
            m_abilityLaunched = true;
            m_attackerAbilities.ServerCoolDownAbility(m_ability);
            m_attacker.EntityAbilities.IsCasting = false;
            LaunchAbilityPosition();
            if (!string.IsNullOrEmpty(m_ability.AbilityBase.LaunchSoundIdentifier))
            {
                AudioManager.instance.ServerPlay3dSound(m_ability.AbilityBase.LaunchSoundIdentifier, m_ability.AbilityBase.LaunchSoundVolume, m_attacker.transform.position);
            }
        }
    }

    private void LaunchAbilityPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        GameObject projectileObj = null;
        switch (m_ability.AbilityBase.SkillShotType)
        {
            case ESkillShotType.FrontSkill:
                spawnPos = m_attacker.GetTransformPosition(m_ability.AbilityBase.LaunchPos).transform.position;
                break;
            case ESkillShotType.FloorSkill:
                if (m_ability.AbilityBase.LaunchPos == EEntityTransform.Floor)
                {
                    spawnPos = Utils.Vector2ToVector3(m_targetPos);
                }
                else
                {
                    spawnPos = m_attacker.GetTransformPosition(m_ability.AbilityBase.LaunchPos).transform.position;
                }
                break;
            default:
                break;
        }
        projectileObj = SpawnManager.instance.InstantiatePool(m_ability.AbilityBase.ProjectileIdentifier, spawnPos, m_attacker.GetTransformPosition(m_ability.AbilityBase.LaunchPos).transform.rotation);
        AbilityComponent projectile = projectileObj.GetComponent<AbilityComponent>();
        projectile.Initialize(m_attacker, Utils.Vector2ToVector3(m_targetPos), m_ability, m_attacker.EntityAbilities.ServerAbilityHit);
        int projectileInstanceID = projectile.gameObject.GetInstanceID();
        projectile.ServerInstanceId = m_ability.AbilityBase.ProjectileIdentifier + "_" + projectileInstanceID;
        m_attacker.EntityAbilities.CacheActiveAbility(projectile);
        m_attacker.EntityAbilities.RpcSpawnPositionProjectile(m_attacker.transform.rotation, m_ability.AbilityBase.ProjectileIdentifier, projectile.ServerInstanceId, projectile.Ability.AbilityBase.Identifier, Utils.Vector2ToVector3(m_targetPos), NetworkTime.Instance.ServerStep());

    }

    public override void Process()
    {
       
        switch (m_currentState)
        {
            case EAbilityState.MovingCharacter:
                if (TryToCastAbility(m_targetPos))
                {

                }
                else
                {
                    m_attacker.ServerCalculatePath(m_targetPos);
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
                m_animationTime -= Time.fixedDeltaTime;
                ProcessLaunchingTime(ELaunchingState.AnimationState);
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
}
