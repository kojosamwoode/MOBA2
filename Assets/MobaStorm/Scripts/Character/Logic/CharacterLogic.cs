using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;
[System.Serializable]
public class CharacterLogic : Logic {

    private Camera MainCamera
    {
        get { return CameraController.instance.sources.currentCamera; }
    }

    private CharacterEntity m_characterEntity;

    private Indicator m_indicator;
    public Indicator Indicator
    {
        get { return m_indicator; }
        set { m_indicator = value; }
    }

    private MobaEntity m_selectedEntity;
    public MobaEntity SelectedEntity
    {
        get { return m_selectedEntity; }
        set { m_selectedEntity = value; }
    }
    [HideInInspector]
    public bool m_keySelect;
    [HideInInspector]
    public bool m_keyAction;
    [HideInInspector]
    public bool m_keyQ;
    [HideInInspector]
    public bool m_keyW;
    [HideInInspector]
    public bool m_keyE;
    [HideInInspector]
    public bool m_keyR;
    [HideInInspector]
    public bool m_keyStop;
    [HideInInspector]
    public bool m_keyAutoAttack;

    private Vector2 m_lastClickPosition;

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdUpdateStartingCurrency();
    }

    public override void Initialize(MobaEntity entity)
    {
        m_characterEntity = entity as CharacterEntity;

        if (GameManager.instance.LocalPlayer != null && m_characterEntity.InstanceId == GameManager.instance.LocalPlayer.PlayerName)
        {
            InputManager.instance.OnGetKeyDown += GetKeyDownEvent;
            InputManager.instance.OnTap += OnTapEvent;
        }
        Initialized = true;
    }


    private void OnTapEvent(Touch touch)
    {
        m_lastClickPosition = touch.position;
        if (m_indicator != null)
        {
            if (TryToCastAbilityOnClient(m_indicator.AttackType, true))
            {
                SpawnManager.instance.DestroyPool(m_indicator.gameObject);
                m_indicator = null;
                return;
            }
        }
        else
        {
            MoveOrTarget();
        }
#if (UNITY_IOS || UNITY_ANDROID)
        
#else

#endif
    }

    private void GetKeyDownEvent(GameKey gameKey)
    {
        m_lastClickPosition = Input.mousePosition;
        switch (gameKey)
        {
            case GameKey.Select:
                m_keySelect = true;
                break;
            case GameKey.Action:
                m_keyAction = true;
                break;
            case GameKey.AbilityQ:
                m_keyQ = true;
                break;
            case GameKey.AbilityW:
                m_keyW = true;
                break;
            case GameKey.AbilityE:
                m_keyE= true;
                break;
            case GameKey.AbilityR:
                m_keyR = true;
                break;
            case GameKey.Stop:
                m_keyStop = true;
                break;
            default:
                break;
        }
    }

  

    public bool TryToCastAbilityOnClient(EAttackType attackType, bool useSmartCast)
    {
        Ability ability = m_characterEntity.EntityAbilities.GetAbility(attackType);
        if (ability == null || ability.Level < 1)
            return false;

        if (useSmartCast)
        {
            if (ability.AbilityBase.RequiresTarget)
            {
                switch (ability.AbilityBase.TargetType)
                {
                    case ETargetType.EntityTarget:
                        MobaEntity targetEntity = null;
                        if (MobaPlayer.RaycastHitEntity(m_characterEntity, ability.AbilityBase.Allegiance, GameManager.instance.Mask, m_lastClickPosition, out targetEntity))
                        {
                            CmdAbilityTargetOnServer(targetEntity.InstanceId, (int)attackType);
                            return true;
                        }
                        break;
                    case ETargetType.PositionTarget:

                        RaycastHit[] raycastArray = Physics.RaycastAll(MainCamera.ScreenPointToRay(m_lastClickPosition), Mathf.Infinity, GameManager.instance.FloorMask);
                        Vector2 floorPoint = Vector2.zero;
                        foreach (RaycastHit ray in raycastArray)
                        {
                            if (ray.transform.tag == "Floor")
                            {
                                CmdAbilityPositionOnServer(Utils.Vector3ToVector2(ray.point), (int)attackType);
                                return true;
                            }
                        }
                        break;
                    case ETargetType.SelfTarget:
                        CmdAbilityTargetOnServer(m_characterEntity.InstanceId, (int)attackType);
                        return true;
                    case ETargetType.RandomTarget:
                        break;
                    case ETargetType.AllTarget:
                        break;
                    default:
                        break;
                }

            }
            else
            {
                //TODO: IMPLEMENT CAST ABILITIES INSTANTLY
            }
            return false;

        }
        else
        {
            if (ability.AbilityBase.RequiresTarget)
            {
                Indicator oldIndicator = null;
                if (m_indicator)
                {
                    oldIndicator = m_indicator;
                }
                switch (ability.AbilityBase.TargetType)
                {
                    case ETargetType.EntityTarget:
                        GameObject indicatorObj = SpawnManager.instance.InstantiatePool(ability.AbilityBase.IndicatorPrefab, m_characterEntity.transform.position, m_characterEntity.transform.rotation);
                        m_indicator = indicatorObj.GetComponent<Indicator>();
                        m_indicator.Initialize(m_characterEntity, ability, attackType);

                        break;
                    case ETargetType.PositionTarget:
                        GameObject pindicatorObj = SpawnManager.instance.InstantiatePool(ability.AbilityBase.IndicatorPrefab, m_characterEntity.transform.position, m_characterEntity.transform.rotation);
                        m_indicator = pindicatorObj.GetComponent<Indicator>();
                        m_indicator.Initialize(m_characterEntity, ability, attackType);
                        break;
                    case ETargetType.SelfTarget:
                        CmdAbilityTargetOnServer(m_characterEntity.InstanceId, (int)attackType);
                        break;
                    case ETargetType.RandomTarget:
                        break;
                    case ETargetType.AllTarget:
                        break;
                    default:
                        break;
                }
                if (oldIndicator)
                    SpawnManager.instance.DestroyPool(oldIndicator.gameObject);
            }
            else
            {
                //TODO: IMPLEMENT CAST ABILITIES INSTANTLY
            }
            return true;
        }
    }


    public override void Process()
    {
        if (!m_characterEntity || m_characterEntity.Dead || m_characterEntity.isServer || !GameManager.instance.IsLocalCharacter(m_characterEntity))
        {
            return;
        }

        if (GameManager.instance.IsGameOver)
        {
            if (m_characterEntity.EntityBehaviour.HasBehaviours)
            {
                m_characterEntity.EntityBehaviour.StopAllBehaviours();
            }
            return;
        }


        if (m_keyAction)
        {
            m_keyAction = false;
            //If there an active indicator, deactivate
            if (m_indicator)
            {
                SpawnManager.instance.DestroyPool(m_indicator.gameObject);
                m_indicator = null;
                return;
            }
            else
            {
                MoveOrTarget();
                return;
            }

        }


        if (m_keySelect)
        {
            m_keySelect = false;
            if (m_indicator != null)
            {
                if (TryToCastAbilityOnClient(m_indicator.AttackType, true))
                {
                    SpawnManager.instance.DestroyPool(m_indicator.gameObject);
                    m_indicator = null;
                    return;
                }
            }

        }

        if (m_keyQ)
        {
            m_keyQ = false;
            if (TryToCastAbilityOnClient(EAttackType.Q, m_characterEntity.EntityAbilities.SmartCast))
            {
                return;
            }
        }
        if (m_keyW)
        {
            m_keyW = false;
            if (TryToCastAbilityOnClient(EAttackType.W, m_characterEntity.EntityAbilities.SmartCast))
            {
                return;
            }
        }
        if (m_keyE)
        {
            m_keyE = false;
            if (TryToCastAbilityOnClient(EAttackType.E, m_characterEntity.EntityAbilities.SmartCast))
            {
                return;
            }
        }
        if (m_keyR)
        {
            m_keyR = false;
            if (TryToCastAbilityOnClient(EAttackType.R, m_characterEntity.EntityAbilities.SmartCast))
            {
                return;
            }
        }

        if (m_keyStop)
        {
            m_keyStop = false;
            CmdStopKeyClicked();
        }

#if (!UNITY_IOS && !UNITY_ANDROID)
        RaycastHit hit;
        if (Physics.Raycast(CameraController.instance.sources.currentCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, GameManager.instance.Mask))
        {
            MobaEntity entityHit = hit.transform.GetComponent<MobaEntity>();
            if (entityHit != null)
            {
                if (m_selectedEntity == entityHit)
                    return;
                if (m_selectedEntity != null)
                {
                    m_selectedEntity.Renderer.material.SetColor("_OutlineColor", Color.black);
                }
                m_selectedEntity = entityHit;
                EAllegiance allegiance = m_characterEntity.GetTargetAllegiance(m_selectedEntity);
                Color allegianceColor = Color.black;
                switch (allegiance)
                {
                    case EAllegiance.Allied:
                        allegianceColor = Color.white;
                        GameManager.instance.SetCursor(ECursors.Cursor_Default);
                        break;
                    case EAllegiance.Hostile:
                        allegianceColor = Color.red;
                        GameManager.instance.SetCursor(ECursors.Cursor_Attack);

                        break;
                    default:
                        break;
                }
                entityHit.Renderer.material.SetColor("_OutlineColor", allegianceColor);
            }
            else
            {
                GameManager.instance.SetCursor(ECursors.Cursor_Default);
                if (m_selectedEntity != null)
                {
                    m_selectedEntity.Renderer.material.SetColor("_OutlineColor", Color.black);
                }
                m_selectedEntity = null;
            }
        }
#endif


    }

    public void MoveOrTarget()
    {
        RaycastHit[] raycastArray = Physics.RaycastAll(MainCamera.ScreenPointToRay(m_lastClickPosition), Mathf.Infinity, GameManager.instance.Mask);
        Vector2 floorPoint = Vector2.zero;
        bool floorHit = false;
        foreach (RaycastHit ray in raycastArray)
        {
            MobaEntity entity = ray.transform.GetComponent<MobaEntity>();
            if (entity)
            {
                //If the player clicks on any hostile entity, send auto attack command to the server
                if (m_characterEntity.GetTargetAllegiance(entity) == EAllegiance.Hostile)
                {
                    Debug.Log("Entity Clicked: " + entity.name);
                    CmdAttackEntityOnServer(entity.InstanceId);
                    return;
                }
            }
            //If the player clicks on the floor, send move command to the server
            if (ray.transform.tag == "Floor")
            {
                floorPoint = new Vector2(ray.point.x, ray.point.z);
                floorHit = true;
            }
        }
        if (floorHit)
        {
            CmdCalculatePathOnServer(floorPoint);
        }
    }

    
    public override void OnFinish()
    {
        base.OnFinish();
    }

    public void AbilityButtonClicked(AbilityButton button)
    {
        TryToCastAbilityOnClient(button.AttackType, m_characterEntity.EntityAbilities.SmartCast);
    }

    [Command]
    public void CmdAbilityInstantOnServer(int attackType)
    {
        Ability ability = m_characterEntity.EntityAbilities.GetAbility((EAttackType)attackType);
        if (ability == null || m_characterEntity.IsStun || m_characterEntity.Mana < ability.ManaCost)
            return;

        if (ability.CoolDown <= 0 && !m_characterEntity.EntityAbilities.IsCasting)
        {
            //TODO ADD INSTANT BEHAVIOUR
        }
    }

    [Command]
    public void CmdAbilityPositionOnServer(Vector2 point, int attackType)
    {
        Ability ability = m_characterEntity.EntityAbilities.GetAbility((EAttackType)attackType);
        if (ability == null || m_characterEntity.IsStun || m_characterEntity.Mana < ability.ManaCost)
            return;

        if (ability.CoolDown <= 0 && !m_characterEntity.EntityAbilities.IsCasting)
        {
            m_characterEntity.EntityBehaviour.AddBehaviour(new AbilitySimplePosition(m_characterEntity, point, ability));
        }
    }

    [Command]
    public void CmdAbilityTargetOnServer(string targetIdentifier, int attackType)
    {
        Ability ability = m_characterEntity.EntityAbilities.GetAbility((EAttackType)attackType);

        if (ability == null || m_characterEntity.Dead || m_characterEntity.IsStun || m_characterEntity.Mana < ability.ManaCost)
        {
            return;
        }
        if (ability.CoolDown <= 0 && !m_characterEntity.EntityAbilities.IsCasting)
        {
            if (ability.AbilityBase.RequiresTarget)
            {
                m_characterEntity.EntityBehaviour.AddBehaviour(new AbilitySimpleTarget(m_characterEntity, GameManager.instance.GetMobaEntity(targetIdentifier), ability));
            }
            else
            {
                Debug.LogError("Ability" + (EAttackType)attackType + " error: Tryng to cast ability on target and settings doesnt require a target:");
            }

        }
    }
    [Command]
    public void CmdAttackEntityOnServer(string entityId)
    {
        MobaEntity entity = GameManager.instance.GetMobaEntity(entityId);
        Ability ability = m_characterEntity.EntityAbilities.GetAbility(EAttackType.Basic);

        if (entity == null || ability == null || m_characterEntity.Dead || m_characterEntity.IsStun)
        {
            return;
        }
        EAllegiance targetAllegiance = m_characterEntity.GetTargetAllegiance(entity);
        switch (targetAllegiance)
        {
            case EAllegiance.Allied:

                break;
            case EAllegiance.Hostile:
                if (!m_characterEntity.EntityAbilities.IsCasting)
                {
                    if (!m_characterEntity.EntityBehaviour.HasBehaviours)
                    {
                        m_characterEntity.EntityBehaviour.AddBehaviour(new AbilitySimpleTarget(m_characterEntity, entity, ability));
                    }
                    else
                    {
                        if (m_characterEntity.EntityBehaviour.CurrentBehaviour.BehaviourAttacktype != EAttackType.Basic)
                        {
                            m_characterEntity.EntityBehaviour.StopAllBehaviours();
                            m_characterEntity.EntityBehaviour.AddBehaviour(new AbilitySimpleTarget(m_characterEntity, entity, ability));
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    [Command]
    public void CmdCalculatePathOnServer(Vector2 destination)
    {
        if (!m_characterEntity.Dead && !m_characterEntity.EntityAbilities.IsCasting && !m_characterEntity.IsStun)
        {
            m_characterEntity.EntityBehaviour.StopAllBehaviours();
            m_characterEntity.ServerCalculatePath(destination);
        }
    }

    [Command]
    public void CmdStopKeyClicked()
    {
        if (!m_characterEntity.EntityAbilities.IsCasting && !m_characterEntity.IsStun)
        {
            if (m_characterEntity.FollowingPath)
            {
                m_characterEntity.StopAgent(true);

            }
            if (m_characterEntity.EntityBehaviour.HasBehaviours)
            {
                m_characterEntity.EntityBehaviour.StopAllBehaviours();
            }
        }

    }

    [Command]
    public void CmdUpdateStartingCurrency()
    {
        m_characterEntity.EntityInventory.Currency.SetAmmount(m_characterEntity.EntityInventory.Currency.Ammount);
    }
    public override void SetMobaEntity(GameObject target)
    {
        base.SetMobaEntity(target);
        target.AddComponent<CharacterEntity>();
    }

    public override MobaEntityData GetMobaEntityData()
    {
        return new CharacterData();
    }

    public override bool IsLocalPlayerAuthority()
    {
        return true;
    }
}
