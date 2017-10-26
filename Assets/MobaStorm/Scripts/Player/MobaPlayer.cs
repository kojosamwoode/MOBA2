using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum EAllegiance
{
    Allied,
    Hostile,
}

public enum ECursors
{
    Cursor_Default,
    Cursor_Attack,
}

public class MobaPlayer : NetworkBehaviour
{

    #region FIELDS
    public Action<MobaPlayer> onPlayerDataChange;

    private Camera MainCamera
    {
        get { return CameraController.instance.sources.currentCamera; }
    }

    private static MobaPlayer m_instance;
    public static MobaPlayer Instance
    {
        get { return m_instance; }
    }

    private CharacterEntity m_characterEntity;
    public CharacterEntity CharacterEntity
    {
        get { return m_characterEntity; }
        set { m_characterEntity = value; }
    }

    private CharacterLogic m_characterLogic;
    public CharacterLogic CharacterLogic
    {
        get
        {
            if (m_characterLogic == null)
            {
                m_characterLogic = m_characterEntity.EntityLogic as CharacterLogic;
            }
            return m_characterLogic;
        }
        set { m_characterLogic = value; }
    }


    [SyncVar(hook = "NameChanged")]
    private string m_playerName = "";
    public string PlayerName
    {
        get { return m_playerName; }
        set { m_playerName = value; }
    }

    [SyncVar(hook = "CharacterChanged")]
    private string m_characterIdentifier = "";
    public string CharacterIdentifier
    {
        get { return m_characterIdentifier; }
        set { m_characterIdentifier = value; }
    }

    public bool m_isReady = false;
    public bool IsReady
    {
        get { return m_isReady; }
    }

    private PlayerSlots m_pickSlot;
    public PlayerSlots PickSlot
    {
        get { return m_pickSlot; }
        set { m_pickSlot = value; }
    }

    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private LayerMask floorMask;

    #endregion

    #region UNITY_CALLS
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {            
            GameManager.instance.PlayMusic("MobaStorm_Selection", 30);
            if (m_instance == null)
            {
                Debug.Log("Setting this GameObject instance as the current Player" + m_playerName);
                GameManager.instance.LocalPlayer = this;
                m_instance = this;
            }
            else
            {
                Debug.Log("Error Current Player Already Exist");
            }
        }
    }

   
    #endregion
    
    /// <summary>
    /// Cast a Ray to find a Entity Based on allegiance
    /// </summary>
    /// <param name="entity">Current Entity</param>
    /// <param name="allegiance">Allegiance</param>
    /// <param name="mask">Layer Mask</param>
    /// <param name="entityHit">Hit Entity</param>
    /// <returns></returns>
    public static bool RaycastHitEntity(MobaEntity entity, EAllegiance allegiance, LayerMask mask, Vector2 screenPosition,  out MobaEntity entityHit)
    {
        entityHit = null;
        RaycastHit[] raycastArray = Physics.RaycastAll(CameraController.instance.sources.currentCamera.ScreenPointToRay(screenPosition), Mathf.Infinity, mask);
        foreach (RaycastHit ray in raycastArray)
        {
            entityHit = ray.transform.GetComponent<MobaEntity>();
            if (entityHit)
            {
                //If the player clicks on any entity with the current allegiance, return true
                if (entity.GetTargetAllegiance(entityHit) == allegiance)
                {
                    Debug.Log("Entity Clicked: " + entityHit.name);
                    return true;
                }
            }
        }
        return false;
    }

    public void PlayerDataChanged()
    {
        if (onPlayerDataChange != null)
        {
            onPlayerDataChange(this);
        }
    }

    //Called on the server to get the current slot team
    [Server]
    public ETeam GetTeamFromSlot()
    {
        if (m_pickSlot.m_slotNumber < 5)
        {
            return ETeam.Blue;
        }
        return ETeam.Red;
    }


    //Called on the client side when a player is ready
    [Server]
    public void SetPlayerReady()
    {
        m_isReady = true;
        TargetPlayerReadyCallBack(connectionToClient, true);

    }

    //Called on the clients when a player changes his character
    [Client]
    public void CharacterChanged(string character)
    {
        m_characterIdentifier = character;
        PlayerDataChanged();
    }

    //Called on the clients when the Moba Player Name Change
    [Client]
    public void NameChanged(string name)
    {
        m_playerName = name;
        PlayerDataChanged();
    }

    //Called on the client when the player click on the ability level up button
    [Client]
    public void ClientLevelUpAbility(EAttackType type)
    {
        CmdLevelUpAbility((int)type);
    }

    //Cheat used to Add Experience to the current player
    [Command]
    public void CmdCheatGiveExp()
    {
        m_characterEntity.ServerGiveExperience(200);
    }

    [Command]
    public void CmdLevelUpAbility(int attackType)
    {
        EAttackType type = (EAttackType)attackType;
        m_characterEntity.EntityAbilities.ServerTryLevelUpAbility(type);
    }

    //Called on th server when a player click on any slot in the pick selection
    [Command]
    public void CmdSlotClicked(int slotClicked)
    {
        MenuPickSelection menuPick = MenuManager.instance.CurrentMenu as MenuPickSelection;
        menuPick.SlotClickedOnClient( this,slotClicked);
    }

    //Called on th server when a player select a character in the pick selection
    [Command]
    public void CmdCharacterClicked(string entityIdentifier)
    {
        MenuPickSelection menuPick = MenuManager.instance.CurrentMenu as MenuPickSelection;
        menuPick.CharacterClickedOnClient(this, entityIdentifier);
    }

    //Called on th server when a player click on the Ready button in the pick selection
    [Command]
    public void CmdReadyClicked()
    {
        MenuPickSelection menuPick = MenuManager.instance.CurrentMenu as MenuPickSelection;
        menuPick.ReadyClickedOnClient(this);
        Transform spawnTransform = SpawnPositionManager.instance.GetSocketSpawnPosition(m_pickSlot.m_slotNumber);
        m_characterEntity = ServerEntitySpawner.instance.SpawnCharacter(this, spawnTransform.position, spawnTransform.rotation, connectionToClient);
    }

    //Called on the clients when the player is ready
    //This method opens the Game Menu Window and play the In Game Music
    [TargetRpc]
    public void TargetPlayerReadyCallBack(NetworkConnection target, bool ready)
    {
        GameManager.instance.PlayMusic("MobaStorm_InGame", 60);
        ReadyButton.instance.SetButtonActive(!ready);
        MenuManager.instance.ShowMenu<MenuGame>();
    }
}
