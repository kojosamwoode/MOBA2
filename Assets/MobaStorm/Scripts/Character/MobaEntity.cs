using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Diagnostics;

public enum ETeam
{
    none = 0,
    Neutral = 1,
    Blue = 2,
    Red = 4,
    BlueAndRed = Blue | Red,
    BlueAndNeutral = Blue | Neutral,
    RedAndNeutral = Red | Neutral,

}

public enum EEntityTransform
{
    Head,
    RightHand,
    LeftHand,
    Center,
    Floor,
    Sky,
    Model,
}

public enum EEntityRespawnType
{
    NoRespawn,
    NoRespawnAndDestroy,
    RespawnAndReset
}

public enum EStatMods
{
    Health,
    MaxHealth,
    Mana,
    MaxMana,
    SpeedMod,
    CoolDownPercentMod,
    HealthRegenerationMod,
    HealtRegenerationPercentMod,
    ArmorMod,
    ArmorPercentMod,
    MagicResMod,
    MagicResPercentMod,
    AttackDamageMod,
    AttackDamagePercentMod,
    MagicDamageMod,
    MagicDamagePercentMod,
    GoldBonusMod,
}

[RequireComponent(typeof(EntityAnimator), typeof(EntityAbilities))]
[NetworkSettings(channel = 0, sendInterval = 0.040f)]
public class MobaEntity : NetworkBehaviour , IPooled {

    public Queue<Task> m_tasks = new Queue<Task>();
    protected MobaEntityData m_entityData;
    public MobaEntityData EntityData
    {
        get
        {
            if (m_entityData == null)
            {
                m_entityData = GameDataManager.instance.GetEntityData(m_dataIdentifier);
            }
            return m_entityData;
        }
    }


    [SerializeField][SyncVar]
    private string m_dataIdentifier;
    public string DataIdentifier
    {
        get { return m_dataIdentifier; }
    }
    [SerializeField][SyncVar(hook = "InstanceIdChanged")]
    private string m_instanceId;
    private void InstanceIdChanged(string instanceId)
    {
        m_instanceId = instanceId;
    }
    [SerializeField][SyncVar]
    private ETeam m_team = ETeam.none;

    //HEALTH VARIABLES
    [SerializeField][SyncVar(hook = "HealthChanged")]
    private float m_health = 260;
    private void HealthChanged(float health)
    {
        m_health = health;
        EntityDataChanged();
    }
    [SerializeField][SyncVar(hook = "HealthMaxChanged")]
    private float m_healthMax = 260;
    private void HealthMaxChanged(float healthMax)
    {
        m_healthMax = healthMax;
        EntityDataChanged();
    }
    [SyncVar]
    private float m_healthRegenerate = 1.3f;

    [SerializeField]
    [SyncVar(hook = "HealthRegenerationModChanged")]
    private float m_healthRegenerationMod = 10;
    private void HealthRegenerationModChanged(float healthRegenMod)
    {
        m_healthRegenerationMod = healthRegenMod;
        EntityDataChanged();
    }

    [SerializeField]
    [SyncVar(hook = "HealthRegenerationPercentModChanged")]
    private float m_healthRegenerationPercentMod = 10;
    private void HealthRegenerationPercentModChanged(float healthRegenMod)
    {
        m_healthRegenerationPercentMod = healthRegenMod;
        EntityDataChanged();
    }

    //MANA VARIABLES
    [SerializeField][SyncVar(hook = "ManaChanged")]
    private float m_mana;
    private void ManaChanged(float mana)
    {
        m_mana = mana;
        EntityDataChanged();
    }
    [SyncVar(hook = "ManaMaxChanged")]
    private float m_manaMax = 150;
    private void ManaMaxChanged(float manaMax)
    {
        m_manaMax = manaMax;
        EntityDataChanged();
    }
    [SyncVar]
    private float m_manaRegeneration = 5f;

    //GENERAL VARIABLES
    private bool m_isLocked;
    public bool IsLocked
    {
        get { return m_isLocked; }
        set { m_isLocked = value; }
    }

    [SyncVar(hook = "SpeedModChanged")]
    private float m_speedMod = 0;
    private void SpeedModChanged(float value)
    {
        m_speedMod = value;
        m_entityAnimator.RefreshRunAnimationSpeed();
        EntityDataChanged();
    }


    [SyncVar(hook = "ExpChanged")]
    private int m_currentExperience = 0;
    private void ExpChanged(int exp)
    {
        m_currentExperience = exp;
        EntityDataChanged();
    }

    [SyncVar(hook = "GoldBonusChanged")]
    private int m_goldBonus = 0;
    private void GoldBonusChanged(int bonus)
    {
        m_goldBonus = bonus;
        EntityDataChanged();
    }

    [SyncVar(hook = "GoldBonusModChanged")]
    private int m_goldBonusMod = 0;
    private void GoldBonusModChanged(int bonus)
    {
        m_goldBonusMod = bonus;
        EntityDataChanged();
    }

    //GAMEPLAY VARIABLES
    [SyncVar]
    protected bool m_dead = false;
    [SyncVar]
    private bool m_isStun = false;
    public bool IsStun
    {
        get { return m_isStun; }
        set { m_isStun = value; }
    }

    private float m_regenerationTime = 0;

    private Vector2 m_originPosition;
    public Vector2 OriginPosition
    {
        get { return m_originPosition; }
    }

    protected MobaEntity m_target;
    public MobaEntity Target
    {
        get { return m_target; }
        set { m_target = value; }
    }

    private Node m_lockedNode;
    public Node LockedNode
    {
        get { return m_lockedNode; }
        set { m_lockedNode = value; }
    }

    private EntityTransform[] m_entityTranforms;

    private EntityCanvas m_entityLabel;
    GameObject m_deathParticle;
    private Vector2[] m_path;

    private bool m_followingPath;
    public bool FollowingPath
    {
        get { return m_followingPath; }
        set { m_followingPath = value; }
    }
    [SyncVar]
    private int m_level = 1;
    public int Level
    {
        get { return m_level; }
        set { m_level = value; }
    }

    public int CurentLevelExp
    {
        get { return GameDataManager.instance.GlobalConfig.m_accumulativeExpTable[m_level]; }
    }

    LineRenderer m_pathRenderer;

    private float serverGoldOverTime;

    public int NextLevelExp
    {
        get
        {
            if (m_level < GameDataManager.instance.GlobalConfig.m_accumulativeExpTable.Length -1)
            {
                return GameDataManager.instance.GlobalConfig.m_accumulativeExpTable[m_level + 1];
            }
            else
            {
                return -1;
            }
        }
    }

    public int MaxLevel
    {
        get { return GameDataManager.instance.GlobalConfig.m_accumulativeExpTable.Length - 1; }
    }

    private float reachDistance = 0.04f;
    private int currentNode = 0;

    public string InstanceId { get { return m_instanceId; } protected set { m_instanceId = value; } }
    public ETeam Team { get { return m_team; } } 
    public virtual string DisplayName { get { return EntityData.m_dataDisplayName; } }
    public float Health { get { return m_health; } }
    public float HealthMax { get { return m_healthMax; } set { m_healthMax = value; } }
    public float HealthRegeneration { get { return m_healthRegenerate; } }
    public float Mana { get { return m_mana; } set { m_mana = value; } }
    public float ManaMax { get { return m_manaMax; } set { m_manaMax = value; } }
    public float ManaRegeneration { get { return m_manaRegeneration; } }

    public float Speed { get { return (EntityData.m_speed + m_speedMod); } }
    public float SpeedMod { get { return m_speedMod; } set { m_speedMod = value; } }

    public bool Dead { get { return m_dead; } }

   
    public bool UsesPathFinding { get { return EntityData.m_usesPathFinfing; } }
    public float AdPerLevel { get { return EntityData.m_adPerLevel; } }
    public float AmorPerLevel { get { return EntityData.m_armorPerLevel; } }
    public string SpawnParticle { get { return EntityData.m_spawnParticle; } }
    public EEntityTransform SpawnParticlePosition { get { return EntityData.m_spawnParticlePosition; } }
    public string DeathParticlePrefab { get { return EntityData.m_deathPrefabParicle; } }
    public EEntityTransform DeathParticlePosition { get { return EntityData.m_deathParticlePosition; } }
    public EEntityRespawnType RespawnType { get { return EntityData.m_respawnType; } }
    public float RespawnTime { get { return EntityData.m_respawnTime; } }
    public int CurrentExperience  { get { return m_currentExperience; } }

    public float ExpToGive { get { return EntityData.m_expToGive; } }

    public int GoldToGive { get { return EntityData.m_goldToGive; } }

    public int GoldBonus { get { return EntityData.m_goldToGive; } }
    public int GoldBonusMod { get { return EntityData.m_goldToGive; } set { m_goldBonusMod = value; } }

    private EntityBehaviour m_entityBehaviour;
    public EntityBehaviour EntityBehaviour
    {
        get { return m_entityBehaviour; }
    }
    /// <summary>
    /// Component used to load and cast abilities
    /// </summary>
    private EntityAbilities m_entityAbilities;
    public EntityAbilities EntityAbilities
    {
        get { return m_entityAbilities; }
    }
    /// <summary>
    /// Controls all entity animations
    /// </summary>
    private EntityAnimator m_entityAnimator;
    public EntityAnimator EntityAnimator
    {
        get { return m_entityAnimator; }
    }
    /// <summary>
    /// Entity Canvas
    /// </summary>
    private EntityCanvas m_entityCanvas;
    public EntityCanvas EntityCanvas
    {
        get { return m_entityCanvas; }
    }
    /// <summary>
    /// Entity Logic
    /// </summary>
    private Logic m_entityLogic;
    public Logic EntityLogic
    {
        get { return m_entityLogic = m_entityLogic == null ? GetComponent<Logic>() : m_entityLogic; }
    }

    /// <summary>
    /// Entity Logic
    /// </summary>
    private EntityInventory m_entityInventory;
    public EntityInventory EntityInventory
    {
        get { return m_entityInventory = m_entityInventory == null ? GetComponent<EntityInventory>() : m_entityInventory; }
    }
    /// <summary>
    /// Renderer Used to show the outline
    /// </summary>
    private Renderer m_renderer;
    public Renderer Renderer
    {
        get { return m_renderer; }
    }


    //Entity public callbacks
    public Action<float> OnDamageDealed;
    public Action<MobaEntity> OnDataChanged;
    //public Action<MobaEntity> OnLevelChanged;
    //public Action<MobaEntity> OnExpChanged;
    //public Action<MobaEntity> OnHealthChanged;
    //public Action<MobaEntity> OnManaChanged;
    public Action<DamageProcess> OnDeathCallBack;
    //public Action<MobaEntity> OnDamageDataChanged;

    public Action<DamageProcess> OnAttackDamageProcessStart;
    public Action<DamageProcess> OnRecieveDamageProcessStart;

    public Vector2 Position
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    public Sprite Icon
    {
        get
        {
            return SpriteDatabaseManager.instance.GetSprite(EntityData.m_icon);
        }
    }
    /// <summary>
    /// Virtual Method  that Initializes a MobaEntity and load all configuration from the MobaEntity data
    /// This Method can be override by custom entities to load custom entity data
    /// </summary>
    /// <param name="data">MobaEntity Data</param>
    /// <param name="instanceId">Unique Instance Identifier for this Entity</param>
    public virtual void InitializeEntity(MobaEntityData data, string instanceId)
    {
        m_entityData = data;
        m_dataIdentifier = data.m_dataIdentifier;
        //GENERAL VARIABLES
        m_instanceId = instanceId;
        m_goldBonus = data.m_goldBonus;

        //OFFENSIVE VARIABLES
        EntityAbilities.BaseAttackDamage = data.m_baseAttackDamage;
        EntityAbilities.BaseMagicDamage = data.m_baseMagicDamage;

        //DEFENSIVE VARIABLES
        EntityAbilities.Armor = data.m_armor;
        EntityAbilities.MagicRes = data.m_magicRes;

        //HEALTH VARIABLES
        m_healthMax = data.m_healthMax;
        m_health = m_healthMax;
        m_healthRegenerate = data.m_healthRegenerate;

        //MANA VARIABLES
        m_manaMax = data.m_manaMax;
        m_mana = m_manaMax;
        m_manaRegeneration = data.m_manaRegeneration;

        m_dead = false;
        m_target = null;
    }


    public void EntityDataChanged()
    {
        if (OnDataChanged!= null)
        {
            OnDataChanged(this);
        }
    }

    public void OnInstantiate()
    {
    }

    public void OnUnSpawn()
    {
        if (EntityLogic)
        {
            EntityLogic.OnFinish();
        }
    }

    #region MONOBEHAVIOUR METHODS

    public virtual void Awake()
    {
        m_entityLogic = GetComponent<Logic>();
        m_entityBehaviour = GetComponent<EntityBehaviour>();
        m_entityAbilities = GetComponent<EntityAbilities>();
        m_entityAnimator = GetComponent<EntityAnimator>();
        m_entityCanvas = GetComponent<EntityCanvas>();
        m_entityTranforms = transform.GetComponentsInChildren<EntityTransform>(true);
        m_entityLabel = GetComponent<EntityCanvas>();
        m_pathRenderer = Utils.CreateLineRendererObject(this, true);
        Transform model = GetTransformPosition(EEntityTransform.Model).transform;
        m_renderer = model.GetComponent<Renderer>();
        if (m_renderer == null)
        {
            m_renderer = model.GetComponentInChildren<Renderer>(true);
        }
    }
    public virtual void Start()
    {

    }
    public virtual void Update()
    {
        if (m_entityLogic != null && m_entityLogic.Initialized)
        {
            m_entityLogic.Process();
        }

        if (UsesPathFinding && m_path != null && m_path.Length > 0)
        {
            MoveOnPath();
        }

        if (m_tasks.Count > 0)
        {
            if (m_tasks.Peek().CanRunTask())
            {
                m_tasks.Dequeue().Run();
            }
        }

        if (isServer && !m_dead)
        {
            m_regenerationTime += Time.deltaTime;
            //Each 0.5 seconds we calculate the regeneration
            float tickTime = 0.5f;
            //Regeneration is calculated based on 5 seconds
            float regenerationTickTime = 0.2f;
            if (m_regenerationTime >= tickTime)
            {
                m_regenerationTime = 0;
                m_health = Mathf.Clamp(m_health + (m_healthRegenerate * regenerationTickTime * tickTime), 0, m_healthMax);
                m_mana = Mathf.Clamp(m_mana + (m_manaRegeneration * regenerationTickTime * tickTime), 0, m_manaMax);
                if (EntityInventory)
                {
                    //Formula to calculate Gold Bonus Overtime
                    serverGoldOverTime += ((m_goldBonus + m_goldBonusMod )) * regenerationTickTime * tickTime;
                    if (serverGoldOverTime > 1)
                    {
                        serverGoldOverTime -= 1;
                        EntityInventory.Currency.AddCurrency(1);
                    }
                }
            }
        }
    }

    #endregion

    #region NETWORK BEHAVIOUR CALLBACKS
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (m_entityAbilities != null)
            m_entityAbilities.LoadAbilities();
        m_entityCanvas.Initialize(this);
        if (!string.IsNullOrEmpty(SpawnParticle))
        {
            ParticleManager.Instance.ClientSpawnParticle(this, SpawnParticle, SpawnParticlePosition);
        }
        InitializeLogic();

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (m_entityAbilities != null)
            m_entityAbilities.LoadAbilities();
        m_originPosition = Position;
        m_entityCanvas.Initialize(this);
        InitializeLogic();

    }

    public void InitializeLogic()
    {

        if (m_entityLogic)
        {
            m_entityLogic.Initialize(this);
        }
        else
        {
            UnityEngine.Debug.LogError("Logic component not found on entity prefab: " + EntityData.m_prefab);
        }
    }
    #endregion

    #region GAMEPLAY METHODS
    [Server]
    public void ServerSetEntityTeam(ETeam team)
    {
        m_team = team;
    }

    [Server]
    public void ServerAddModifiers(EStatMods statMod, int modifier)
    {
        switch (statMod)
        {
            case EStatMods.Health:
                ServerAddHealth(modifier);
                break;
            case EStatMods.MaxHealth:
                m_healthMax += modifier;
                break;
            case EStatMods.Mana:
                ServerAddMana(modifier);
                break;
            case EStatMods.MaxMana:
                m_manaMax += modifier;
                break;
            case EStatMods.SpeedMod:
                m_speedMod += modifier;
                break;
            case EStatMods.CoolDownPercentMod:
                m_entityAbilities.CoolDownPercentMod += modifier;
                break;
            case EStatMods.HealthRegenerationMod:
                m_healthRegenerationMod += modifier;
                break;
            case EStatMods.HealtRegenerationPercentMod:
                m_healthRegenerationPercentMod += modifier;
                break;
            case EStatMods.ArmorMod:
                EntityAbilities.ArmorMod += modifier;
                break;
            case EStatMods.ArmorPercentMod:
                EntityAbilities.ArmorResPercentMod += modifier;
                break;
            case EStatMods.MagicResMod:
                EntityAbilities.MagicResMod += modifier;
                break;
            case EStatMods.MagicResPercentMod:
                EntityAbilities.MagicResPercentMod += modifier;
                break;
            case EStatMods.AttackDamageMod:
                EntityAbilities.AttackDamageMod += modifier;
                break;
            case EStatMods.AttackDamagePercentMod:
                EntityAbilities.AttackDamagePercentMod += modifier;
                break;
            case EStatMods.MagicDamageMod:
                EntityAbilities.MagicDamageMod += modifier;
                break;
            case EStatMods.MagicDamagePercentMod:
                EntityAbilities.MagicDamagePercentMod += modifier;
                break;
            case EStatMods.GoldBonusMod:
                m_goldBonusMod += modifier;
                break;
            default:
                break;
        }
    }

    [Server]
    public void ServerGiveExperience(int exp)
    {
        if (m_level < MaxLevel)
        {
            m_currentExperience = Mathf.Clamp(m_currentExperience + exp, 0, GameDataManager.instance.GlobalConfig.m_accumulativeExpTable[MaxLevel]);
            if (m_currentExperience >= NextLevelExp)
            {
                EntityAbilities.Armor += AmorPerLevel;
                EntityAbilities.BaseAttackDamage += AdPerLevel;
                m_level++;
                EntityAbilities.GiveAbilityPoint();
                RpcLevelUp();
            }
        }             
    }
    [Server]
    public void ServerAddHealth(int healthToAdd)
    {
        m_health = Mathf.Clamp(m_health + healthToAdd, 0, m_healthMax);
        if (m_entityLabel != null && healthToAdd > 0)
        {
            m_entityLabel.ShowFloatingText(FloatingText.FloatingTextType.HealFloatingText, healthToAdd.ToString());
        }
    }
    [Server]
    public void ServerAddMana(int manaToAdd)
    {
        m_mana = Mathf.Clamp(m_mana + manaToAdd, 0, m_manaMax);
        if (m_entityLabel != null && manaToAdd > 0)
        {
            m_entityLabel.ShowFloatingText(FloatingText.FloatingTextType.HealFloatingText, manaToAdd.ToString());
        }
    }

    [Client]
    public void ClientAddEntityTask(Task action)
    {
        m_tasks.Enqueue(action);
    }

    public EAllegiance GetTargetAllegiance(MobaEntity entity)
    {
        if (entity.Team == ETeam.Neutral)
        {
            return EAllegiance.Hostile;
        }
        if (m_team == ETeam.Blue && entity.Team == ETeam.Red)
        {
            return EAllegiance.Hostile;
        }
        if (m_team == ETeam.Red && entity.Team == ETeam.Blue)
        {
            return EAllegiance.Hostile;
        }
        if (m_team == entity.Team)
        {
            return EAllegiance.Allied;
        }
        return EAllegiance.Hostile;

    }

    public ETeam GetAbilityTargetTeam(EAllegiance abilityAllegiance)
    {
        switch (m_team)
        {
            case ETeam.Neutral:

            case ETeam.Blue:
                if (abilityAllegiance == EAllegiance.Allied)
                {
                    return ETeam.Blue;
                }
                else
                {
                    return ETeam.Red;
                }
            case ETeam.Red:
                if (abilityAllegiance == EAllegiance.Allied)
                {
                    return ETeam.Red;
                }
                else
                {
                    return ETeam.Blue;
                }
            default:
                return ETeam.Neutral;
        }
    }

    public EntityTransform GetTransformPosition(EEntityTransform launchPos)
    {
        EntityTransform anyEntity = null;
        foreach (EntityTransform entityTransform in m_entityTranforms)
        {
            if (anyEntity == null)
            {
                anyEntity = entityTransform;
            }
            if (entityTransform.EntityTransformType == launchPos)
            {
                return entityTransform;
            }
        }
        //If the character doesnt have any entityTransform we create one
        if (anyEntity == null)
        {
            GameObject objToSpawn = new GameObject("Center");
            anyEntity = objToSpawn.AddComponent<EntityTransform>();
            anyEntity.EntityTransformType = EEntityTransform.Center;
            objToSpawn.transform.SetParent(transform);
            objToSpawn.transform.localPosition = new Vector3(0, 0.4f, 0);
        }
        return anyEntity;
    }
    #endregion

    #region PATHFINDING

    private void MoveOnPath()
    {
        float dist = Vector2.Distance(m_path[currentNode], Position);

        Vector3 destination = new Vector3(m_path[currentNode].x, transform.position.y, m_path[currentNode].y);

        Vector3 dir = destination - transform.position;

        transform.Translate(dir.normalized * Time.deltaTime * (Speed /5), Space.World);
        Quaternion  lookRotation = Quaternion.LookRotation(dir);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);

        if (dist <= reachDistance)
        {
            currentNode++;
        }

        if (currentNode >= m_path.Length)
        {
            StopAgent(true);
            
        }
    }
   
    public void SetPath(Vector2[] path)
    {
        if (path== null || path.Length == 0)
        {
            UnityEngine.Debug.Log("PathNull");
            return;
        }
        if (path.Length > 1)
        {
            currentNode = 1;
        }
        else
        {
            currentNode = 0;
        }

        m_path = path;
        
        if (isServer)
        {
            m_pathRenderer.numPositions = m_path.Length;
            Vector3[] pathVector3 = new Vector3[m_path.Length];
            for (int i = 0; i < m_path.Length; i++)
            {
                pathVector3[i] = Utils.Vector2ToVector3(m_path[i]);
            }
            m_pathRenderer.SetPositions(pathVector3);
            m_followingPath = true;
            m_entityAnimator.ChangeState(EEntityState.Run);
        }
    }
    
    public void ClientSetNavMeshPath(Vector2[] pathArray)
    {
        SetPath(pathArray);
    }

    public void ClearLockedNodes()
    {
        if (m_lockedNode != null)
        {
            m_lockedNode.walkable = true;
            m_lockedNode = null;
        }
    }
    public void WarpEntityOnServer(Vector2 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.y);
        RpcWarpEntity(position);
    }
    public virtual MobaPath ServerCalculatePath(Vector2 target)
    {
        if (IsStun)
            return null;

        MobaPath path = Pathfinding.instance.FindPath(Position, target);
        if (path == null)
        {
            //UnityEngine.Debug.Log("Path is null");
            return null;
        }
        RpcClientCalculatePath(Position, target, NetworkTime.Instance.ServerStep());

        SetPath(path.Corners);
        //m_entityAbilities.IsCasting = false;
        
        return path;
    }
    
    public void StopAgent(bool goToIdle)
    {
        m_followingPath = false;
        m_path = null;
        //m_rigidBody.velocity = Vector3.zero;
        if (isServer)
        {
            if (goToIdle)
            m_entityAnimator.ChangeState(EEntityState.Idle);
            m_entityAbilities.IsCasting = false;        
            RpcStopAgent(NetworkTime.Instance.ServerStep(), goToIdle);
        }
        //else
        //{
        //    if (goToIdle)
        //        m_entityAnimator.ClientAnimationChange(EEntityState.Idle);
        //}
    }

#endregion

    #region DAMAGE METHODS

    public void StunEntity(bool isStun)
    {
        UnityEngine.Debug.Log("Entity Stunned: " + isStun);
        m_isStun = isStun;
        if (isStun)
        {
            m_entityAnimator.ChangeState(EEntityState.Stun);
            m_entityBehaviour.StopAllBehaviours();
            StopAgent(false);
        }
        else
        {
            m_entityAnimator.ChangeState(EEntityState.Idle);
        }
    }

    public virtual void DealDamage(float damage, DamageProcess damageProcess)
    {
        if (Dead)
        {
            return;
        }
        m_health = Mathf.Clamp(m_health - damage, 0, m_healthMax);

        if (m_health == 0)
        {
            Die(damageProcess);
            if (this is CharacterEntity)
            {
                GameEvents.Instance.ServerSendSlainEvent(damageProcess);
            }
            if (damageProcess.Attacker is CharacterEntity)
            {
                CharacterEntity attacker = damageProcess.Attacker as CharacterEntity;
                attacker.ServerGiveExperience((int)damageProcess.Target.ExpToGive);
                attacker.EntityInventory.Currency.AddCurrency(damageProcess.Target.GoldToGive);
                if (m_entityLabel != null)
                {
                    m_entityLabel.ShowFloatingText(FloatingText.FloatingTextType.GoldFloatingText, Mathf.Ceil(damageProcess.Target.GoldToGive).ToString());
                }
            }
        }
        else
        {
            if (m_entityLabel != null)
            {
                m_entityLabel.ShowFloatingText(FloatingText.FloatingTextType.DamageFloatingText, Mathf.Ceil(damage).ToString());
            }
        }

        EntityDataChanged();
        if (OnDamageDealed != null)
        {
            OnDamageDealed(damage);
        }
    }

    public virtual void Die(DamageProcess damageProcess)
    {
        if (OnDeathCallBack != null)
        {
            OnDeathCallBack(damageProcess);
        }
        m_entityAbilities.RemoveAllSideEffects();
        EntityBehaviour.StopAllBehaviours();
        StopAgent(false);
        m_dead = true;
        EntityAnimator.ChangeState(EEntityState.Dead);
        if (!string.IsNullOrEmpty(DeathParticlePrefab))
        {
            RpcSpawnDeathParticle();
            m_deathParticle = SpawnManager.instance.InstantiatePool(DeathParticlePrefab, Vector3.zero, Quaternion.identity);
            EntityTransform canvasTrasnform = GetTransformPosition(DeathParticlePosition);
            m_deathParticle.transform.SetParent(canvasTrasnform.transform);
            m_deathParticle.transform.localPosition = Vector3.zero;
        }
        switch (RespawnType)
        {
            case EEntityRespawnType.NoRespawn:
                break;
            case EEntityRespawnType.NoRespawnAndDestroy:
                StartCoroutine(NoRespawnAndDestroyCoroutine());
                break;
            case EEntityRespawnType.RespawnAndReset:
                StartCoroutine(RespawnAndResetCoroutine());
                break;
            default:
                break;
        }




    }
    IEnumerator RespawnAndResetCoroutine()
    {
        yield return new WaitForSeconds(RespawnTime);
        if (m_lockedNode != null)
        {
            m_lockedNode.Resident = null;
        }
        if (m_deathParticle != null)
        {
            RpcDestroyDeathParticle();
            SpawnManager.instance.DestroyPool(m_deathParticle);
        }

        ResetEntityData();
        WarpEntityOnServer(m_originPosition);

    }

    void ResetEntityData()
    {
        StopAgent(true);
        m_dead = false;
        m_target = null;
        //HEALTH VARIABLES
        m_health = m_healthMax;

        //MANA VARIABLES
        m_mana = m_manaMax;
        m_entityCanvas.Initialize(this);
        RpcIntializeCanvas();
    }

    IEnumerator NoRespawnAndDestroyCoroutine()
    {
        yield return new WaitForSeconds(5);
        if (m_deathParticle != null)
        {
            RpcDestroyDeathParticle();
            SpawnManager.instance.DestroyPool(m_deathParticle);
        }
        if (m_lockedNode != null)
        {
            m_lockedNode.Resident = null;
        }
        RpcDestroyEntity(true);
        GameManager.instance.EntityDestroyed(this);
        NetworkServer.UnSpawn(gameObject);
        SpawnManager.instance.DestroyPool(gameObject);
    }
    #endregion

    #region CLIENT RPCS METHODS
    [ClientRpc]
    public void RpcLevelUp()
    {
        EntityTransform entityTransform = GetTransformPosition(EEntityTransform.Floor);
        GameObject sideEffectObj = SpawnManager.instance.InstantiatePool("FX_LvlUp", entityTransform.transform.position, entityTransform.transform.rotation);
        Particle prefab = sideEffectObj.GetComponent<Particle>();
        prefab.Initialize("", this, null, false);
    }
    [ClientRpc]
    public void RpcIntializeCanvas()
    {
        m_entityCanvas.CharacterCanvas.CanvasInfoHolder.SetActive(true);
    }
   
    //[ClientRpc]
    //public void RpcPathRecieved(Vector2[] pathArray, int timeStamp)
    //{
    //    NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Path recieved");
    //    AddEntityTask(new SetPathTask(this, pathArray, timeStamp));
    //}

    [ClientRpc]
    public void RpcClientCalculatePath(Vector2 startPos, Vector2 destinationPos, int timeStamp)
    {
        NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Calculate Path");
        ClientAddEntityTask(new CalculatePathTask(this, startPos, destinationPos, timeStamp));
    }
    [ClientRpc]
    public void RpcStopAgent(int timeStamp, bool goToIdle)
    {
        NetworkTime.Instance.CheckTimeStepOffset(timeStamp, "Stop Agent");
        ClientAddEntityTask(new StopPathTask(this, timeStamp, goToIdle));
    }

    [ClientRpc]
    public void RpcDestroyEntity(bool dead)
    {
        if (dead)
        {
            GameManager.instance.EntityDestroyed(this);
        }
    }

    [ClientRpc]
    public void RpcSpawnDeathParticle()
    {
        m_deathParticle = SpawnManager.instance.InstantiatePool(DeathParticlePrefab, Vector3.zero, Quaternion.identity);
        EntityTransform canvasTrasnform = GetTransformPosition(DeathParticlePosition);
        m_deathParticle.transform.SetParent(canvasTrasnform.transform);
        m_deathParticle.transform.localPosition = Vector3.zero;
    }

    [ClientRpc]
    public void RpcDestroyDeathParticle()
    {
        if (m_deathParticle != null)
        {
            SpawnManager.instance.DestroyPool(m_deathParticle);
        }
    }

    [ClientRpc]
    public void RpcWarpEntity(Vector2 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.y);
    }
    #endregion
}


