using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;
[System.Serializable]
public class MinionWaveLogic : Logic {

    private AIEntity m_entity;
    private float m_updatePathRate;
    private float m_targetUpdateRate;
    private Vector2 m_detectionPosition;
    private LineRenderer m_debugRangeLineRend;

    private float MaxChaseDistance { get { return GameDataManager.instance.GlobalConfig.m_maxChasingDistance; } }

    private WaypointProgressTracker m_waypointTracker;
    public WaypointProgressTracker WaypointTracker { get { return m_waypointTracker; } }

    private Ability m_ability;

    private float m_xoffset;
    public float XOffset
    {
        get { return m_xoffset; }
        set { m_xoffset = value; }
    }

    private float m_aggroRange;

    public enum IaState
    {
        idle,
        target,
        attacking,
        retreat,
        dead
    }
    [SerializeField]
    private IaState m_state = IaState.idle;

    public override void Initialize(MobaEntity entity)
    {
        m_entity = entity as AIEntity;      
        if (m_entity.isServer)
        {
            MinionData iaDef = m_entity.EntityData as MinionData;
            m_aggroRange = iaDef.m_aggroRange;           
            InitializeCircuit();
            ChangeState(IaState.idle);
            Initialized = true;
            m_ability = entity.EntityAbilities.GetAbility(EAttackType.Basic);
            m_entity.OnDeathCallBack += MinionKilled;
        }
    }

    private void MinionKilled(DamageProcess damageProcess)
    {
        GameManager.instance.MinionKilled(damageProcess);
    }

    public void InitializeLineRenderer()
    {
        if (m_debugRangeLineRend == null)
            m_debugRangeLineRend = Utils.CreateLineRendererObject(m_entity, false);

        Vector3[] circlePoints = Utils.GenerateCirclePoints(m_entity.transform.position, m_aggroRange, 20);
        m_debugRangeLineRend.numPositions = circlePoints.Length;
        m_debugRangeLineRend.SetPositions(circlePoints);
    }

    public void InitializeCircuit()
    {
        
        if (m_waypointTracker == null)
            m_waypointTracker = gameObject.AddComponent<WaypointProgressTracker>();
        if (m_entity.Team == ETeam.Blue)
        {
            m_waypointTracker.circuit = MinionSpawnManager.m_blueCircuitScript;
        }
        else
        {
            m_waypointTracker.circuit = MinionSpawnManager.m_redCirtcuitScript;
        }
        m_waypointTracker.Reset();
    }

    public override void Process()
    {
        if (GameManager.instance.IsGameOver)
        {
            if (m_entity.EntityBehaviour.HasBehaviours)
            {
                m_entity.EntityBehaviour.StopAllBehaviours();
            }
            return;
        }

        if (m_entity.isServer && !m_entity.IsStun)
        {            
            ServerProcessState();
        }
    }

    public void ServerProcessState()
    {

        m_updatePathRate -= Time.fixedDeltaTime;
        m_targetUpdateRate -= Time.deltaTime;

        switch (m_state)
        {
            case IaState.idle:
                if (m_entity.Dead)
                {
                    ChangeState(IaState.dead);
                    return;
                }

                if (!m_entity.Target)
                {

                        if (EnemyDetected())
                        {
                            ChangeState(IaState.target);
                            return;
                        }
                    
                    
                    FollowPath();

                }
                break;
            case IaState.target:
                if (m_entity.Dead)
                {
                    ChangeState(IaState.dead);
                    return;
                }

                if (!m_entity.Target || m_entity.Target.Dead || Vector2.Distance(m_detectionPosition, m_entity.Position) > MaxChaseDistance)
                {
                    ChangeState(IaState.retreat);
                    m_entity.Target = null;
                    if (m_entity.EntityBehaviour.HasBehaviours && !m_entity.EntityAbilities.IsCasting)
                    {
                        m_entity.EntityBehaviour.StopAllBehaviours();
                    }
                    return;                    
                }
                
                if (!m_entity.EntityBehaviour.HasBehaviours)
                {
                    m_entity.EntityBehaviour.AddBehaviour(new AITargetBehaviour(m_entity, m_ability));
                }
                break;
            case IaState.attacking:
                break;
            case IaState.retreat:

                if (m_entity.Dead)
                {
                    ChangeState(IaState.dead);
                    return;
                }

                if (EnemyDetected())
                {
                    ChangeState(IaState.target);
                }
                else
                {
                    RetreatPath();
                    if (Vector2.Distance(m_entity.Position, m_detectionPosition) <= 2)
                    {
                        ChangeState(IaState.idle);
                    }
                }                
                break;
            case IaState.dead:
                
                break;
            default:
                break;
        }
    }

   private bool EnemyDetected()
    {
        if (m_targetUpdateRate <= 0)
        {
            m_targetUpdateRate = GameDataManager.instance.GlobalConfig.m_targetUpdateRate;
            MobaEntity target;
            ETeam m_targetTeam = m_entity.GetAbilityTargetTeam(EAllegiance.Hostile);
            if (Utils.IsOnRangeWithPriority(m_entity.Position, GameManager.instance.TeamEntities[m_targetTeam], "AIEntity", m_aggroRange, out target))
            {
                m_entity.Target = target;
                m_detectionPosition = m_entity.Position;
                return true;
            }
        }
        
        return false;
    }

    private void ChangeState(IaState state)
    {
        m_state = state;
        switch (state)
        {
            case IaState.idle:
                break;
            case IaState.target:
                break;
            case IaState.attacking:
                break;
            case IaState.retreat:
                break;
            case IaState.dead:
                m_entity.StopAgent(false);
                m_entity.EntityBehaviour.StopAllBehaviours();
                break;
            default:
                break;
        }
    }


    void FollowPath()
    {
        if (m_updatePathRate <= 0 || !m_entity.FollowingPath)
        {
            if (m_waypointTracker.Target != null)
            {
                if (m_entity.LockedNode != null)
                {
                    m_entity.LockedNode.Resident = null;
                    m_entity.LockedNode = null;
                }
                m_entity.ServerCalculatePath(new Vector2(m_waypointTracker.GridPosition.x + XOffset, m_waypointTracker.GridPosition.y));
                m_updatePathRate = GameDataManager.instance.GlobalConfig.m_pathUpdateRate;
            }
        }
    }

    void RetreatPath()
    {
        if (m_updatePathRate <= 0)
        {
            if (m_entity.LockedNode != null)
            {
                m_entity.LockedNode.Resident = null;
                m_entity.LockedNode = null;
            }
            m_entity.ServerCalculatePath(new Vector2(m_detectionPosition.x, m_detectionPosition.y));
            m_updatePathRate = GameDataManager.instance.GlobalConfig.m_pathUpdateRate;            
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        m_entity.OnDeathCallBack -= MinionKilled;

        if (m_waypointTracker != null)
        {
            Destroy(m_waypointTracker.Target.gameObject);
            Destroy(m_waypointTracker);
        }
    }

    public override void SetMobaEntity(GameObject target)
    {
        base.SetMobaEntity(target);
        target.AddComponent<AIEntity>();
    }

    public override MobaEntityData GetMobaEntityData()
    {
        return new MinionData();
    }
}
