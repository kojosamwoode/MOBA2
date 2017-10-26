using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AbilityComponent : MonoBehaviour , IPooled {

    public enum EProjectileType
    {
        EntityTarget,
        PositionTarget,
        DirectionTarget,
    }

    private MobaEntity m_attacker;
    public MobaEntity Attacker
    {
        get { return m_attacker; }
        set { m_attacker = value; }
    }
    private MobaEntity m_target;
    public MobaEntity Target
    {
        get { return m_target; }
        set { m_target = value; }
    }
    private Vector3 m_targetPos;
    public Vector3 TargetPos
    {
        get { return m_targetPos; }
        set { m_targetPos = value; }
    }
    private EEntityTransform m_targetEntityTransform;
    public EEntityTransform TargetEntityTranform
    {
        get { return m_targetEntityTransform; }
        set { m_targetEntityTransform = value; }
    }

    private EntityTransform m_targetTransform;
    public EntityTransform TargetTransform
    {
        get { return m_targetTransform; }
        set { m_targetTransform = value; }
    }


    private Ability m_ability;
    public Ability Ability
    {
        get { return m_ability; }
        set { m_ability = value; }
    }

    private bool m_expended = false;
    public bool Expended
    {
        get { return m_expended; }
        set { m_expended = value; }
    }

    private string m_serverInstanceId;
    public string ServerInstanceId
    {
        get { return m_serverInstanceId; }
        set { m_serverInstanceId = value; }
    }

    public delegate void OnProjectileExpended(AbilityComponent ability, bool abilityHit);
    public event OnProjectileExpended onProjectileExpended;

    private ETeam m_targetTeam;

    //private HashSet<MobaEntity> m_collisionEntities = new HashSet<MobaEntity>();
    //public HashSet<MobaEntity> CollisionEntities
    //{
    //    get { return m_collisionEntities; }
    //    set { m_collisionEntities = value; }
    //}


    public Vector2 Position
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    protected void AbilityMiss()
    {
        if (onProjectileExpended != null)
        {
            onProjectileExpended(this, false);
        }
        m_expended = true;
    }

    protected virtual void ProjectileHit(MobaEntity target)
    {
        if (onProjectileExpended != null)
        {
            onProjectileExpended(this, true);
        }
        m_expended = true;
        if (m_attacker.isClient)
        {
            return;
        }
        switch (m_ability.AbilityBase.DamageLogic)
        {
            case EDamageLogic.DamageTargetLogic:
                if (target != null)
                {
                    DamageDirect(target);
                }
                break;
            case EDamageLogic.DamageAoeLogic:
                DamageAOE();
                break;
            case EDamageLogic.DamageCustomLogic:
                DamageCustom(m_ability, target);
                break;
            default:
                break;
        }
    }

    protected void DamageDirect(MobaEntity target)
    {
        new DamageProcess(m_attacker, target, m_ability);
    }
    protected void DamageAOE()
    {
        if (m_attacker.isServer)
        {
            //m_collisionEntities = GameManager.instance.MobaEntities(targetTeam);
            foreach (MobaEntity entity in GameManager.instance.TeamEntities[m_targetTeam])
            {
                if (entity.EntityData is TowerData && !m_ability.AbilityBase.CanDamageTowers)
                {
                    continue;
                }
                if (Utils.IsOnRange(Position, entity.Position, m_ability.AbilityBase.AoeRange))
                {
                    new DamageProcess(m_attacker, entity, m_ability);
                }
            }
        }
        
    }
    protected virtual void DamageCustom(Ability ability, MobaEntity target = null)
    {

    }

    protected virtual void FixedUpdate()
    {
        if (Expended)
            return;
        MobaEntity entity = null;
        if (Utils.IsOnRange(Position, GameManager.instance.TeamEntities[m_targetTeam], m_ability.AbilityBase.DetectionRange, out entity))
        {
            if (entity.EntityData is TowerData && !m_ability.AbilityBase.CanDamageTowers)
            {
                return;
            }
            ProjectileHit(entity);
        }
       
    }



    public  void OnInstantiate()
    {
        m_attacker = null;
        m_target = null;
        m_targetPos = Vector3.zero;
        m_targetTransform = null;
        m_ability = null;
        m_expended = false;
    }

    public void Initialize(MobaEntity attacker, MobaEntity target, Ability ability, OnProjectileExpended expendedCallback = null)
    {
        m_expended = false;
        m_attacker = attacker;
        m_target = target;
        m_targetTransform = target.GetTransformPosition(EEntityTransform.Center);
        m_ability = ability;
        onProjectileExpended = expendedCallback;
        m_targetTeam = m_attacker.GetAbilityTargetTeam(m_ability.AbilityBase.Allegiance);
        OnStart();
    }
    public void Initialize(MobaEntity attacker, Vector3 targetPos, Ability ability, OnProjectileExpended expendedCallback = null)
    {
        m_expended = false;
        m_attacker = attacker;
        m_targetPos = targetPos;
        m_ability = ability;
        onProjectileExpended = expendedCallback;
        m_targetTeam = m_attacker.GetAbilityTargetTeam(m_ability.AbilityBase.Allegiance);

        OnStart();

    }

    public virtual void OnStart()
    {

    }

    public void OnUnSpawn()
    {
    }
}
