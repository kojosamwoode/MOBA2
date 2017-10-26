using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;

[System.Serializable]
public class TowerLogic : Logic {

    private AIEntity m_entity;
    private float m_targetUpdateRate;
    private Vector2 m_detectionPosition;
    private LineRenderer m_debugRangeLineRend;
    private LineRenderer m_debugTargetLineRend;
    [SyncVar]
    private float m_rotationX;
    [SyncVar]
    private float m_rotationY;
    [SyncVar]
    private float m_rotationZ;
    private float MaxChaseDistance { get { return GameDataManager.instance.GlobalConfig.m_maxChasingDistance; } }

    private float m_xoffset;
    public float XOffset
    {
        get { return m_xoffset; }
        set { m_xoffset = value; }
    }

    private float m_detectionRange;

    private Ability m_ability;

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
        m_ability = m_entity.EntityAbilities.GetAbility(EAttackType.Basic);
        if (m_entity.isServer)
        {
            ChangeState(IaState.idle);
            TowerData iaDef = m_entity.EntityData as TowerData;
            m_detectionRange = iaDef.m_detectionRange;
            m_rotationX = transform.rotation.eulerAngles.x;
            m_rotationY = transform.rotation.eulerAngles.y;
            m_rotationZ = transform.rotation.eulerAngles.z;
            InitializeLineRenderer();
            Initialized = true;
            m_entity.OnDeathCallBack += TowerKilled;

        }
        else
        {
            transform.eulerAngles = new Vector3(m_rotationX, m_rotationY, m_rotationZ);
        }
    }
    private void TowerKilled(DamageProcess damageProcess)
    {
        GameManager.instance.TowerKilled(damageProcess);
    }


    public void InitializeLineRenderer()
    {
        if (m_debugRangeLineRend == null)
            m_debugRangeLineRend = Utils.CreateLineRendererObject(m_entity, false);

        if (m_debugTargetLineRend == null)
            m_debugTargetLineRend = Utils.CreateLineRendererObject(m_entity, true);

        Vector3[] circlePoints = Utils.GenerateCirclePoints(m_entity.transform.position, m_detectionRange, 20);
        m_debugRangeLineRend.numPositions = circlePoints.Length;
        m_debugRangeLineRend.SetPositions(circlePoints);
    }


    public override void Process()
    {
        if (m_entity.isServer)
        {
            ServerProcessState();
        }
    }

    public void ServerProcessState()
    {
        m_targetUpdateRate -= Time.fixedDeltaTime;
        switch (m_state)
        {
            case IaState.idle:
                if (!m_entity.Target)
                {
                    if (EnemyDetected())
                    {
                        ChangeState(IaState.target);
                    }

                }
                break;
            case IaState.target:
                if (!m_entity.Target)
                {
                    ChangeState(IaState.idle);
                    break;
                }
                else
                {
                    m_debugTargetLineRend.numPositions= 2;
                    Vector3[] positions = new Vector3[] { m_entity.GetTransformPosition(EEntityTransform.RightHand).transform.position, m_entity.Target.GetTransformPosition(EEntityTransform.Center).transform.position };
                    m_debugTargetLineRend.SetPositions(positions);
                }
                if (m_entity.Dead)
                {
                    ChangeState(IaState.dead);
                    m_entity.EntityBehaviour.StopAllBehaviours();
                    return;
                }

                if (!m_entity.EntityBehaviour.HasBehaviours && m_entity.EntityAbilities.GetAbility(EAttackType.Basic) != null)
                {
                    m_entity.EntityBehaviour.AddBehaviour(new TowerTargetBehaviour(m_entity, m_entity.Target, m_ability));
                }
                break;
            case IaState.attacking:
                break;
            case IaState.retreat:

                if (EnemyDetected())
                {
                    ChangeState(IaState.target);
                }
                else
                {
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
            if (Utils.IsOnRangeWithPriority(m_entity.Position, GameManager.instance.TeamEntities[m_targetTeam], "AIEntity", m_detectionRange, out target))
            {
                m_entity.Target = target;
                m_detectionPosition = m_entity.Position;
                return true;
            }
        }
        return false;
    }

    public void ChangeState(IaState state)
    {
        m_state = state;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        m_entity.OnDeathCallBack -= TowerKilled;

    }

    public override void SetMobaEntity(GameObject target)
    {
        base.SetMobaEntity(target);
        target.AddComponent<AIEntity>();
    }

    public override MobaEntityData GetMobaEntityData()
    {
        return new TowerData();
    }
}
