using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CharacterData : MobaEntityData
{
    //This is the character entity list.
    //Add all character Entity Identifiers to this array to show the new character at the pick selection
    public static string[] m_characterList = new string[] { "Entity_Galilei", "Entity_Allycra", "Entity_Jakei" };
#if UNITY_EDITOR
    public override string DataFileExtension()
    {
        return GameDataManager.m_characterExtension;
    }
    //Draw CharacterData in the editor window
    public override MobaEntityData DrawEditor(GUISkin skin)
    {
        CharacterData characterData = base.DrawEditor(skin) as CharacterData;
        GUILayout.BeginHorizontal();

        GUILayout.EndHorizontal();
        return characterData;
    }

    public override MobaEntityData LoadEntityData(string path)
    {
        return Utils.Load<CharacterData>(path);
    }


#endif
}

/// <summary>
/// Implementation of the Base Moba Entity
/// Contains the base methods and parameters for all characters
/// </summary>
public class CharacterEntity : MobaEntity {
    //Unet Connection ID
    [SyncVar]
    private int m_ownerConnectionId = 0;
    public int OwnerConnectionId
    {
        get { return m_ownerConnectionId; }
        set { m_ownerConnectionId = value; }
    }

    //Characters Display Name
    public override string DisplayName
    {
        get
        {
            return InstanceId;
        }
    }

    #region UNITY CALLS
    //Awake
    public override void Awake()
    {
        base.Awake();
    }

    //Start
    public override void Start()
    {
        base.Start();

        if (NetworkClient.active)
        {
            GameManager.instance.CharacterCreated(this);
        }
    }
    //Unet OnStart Client
    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.instance.EntitySpawned(this);
        
    }
    //Unet OnStart Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.instance.EntitySpawned(this);
    }

    public override void Update()
    {
        base.Update();

    }

    #endregion

    //Initialize the current entity, and load the entityData from its identifier
    public override void InitializeEntity(MobaEntityData entityData, string identifier)
    {
        base.InitializeEntity(entityData, identifier);
        //Initialize additional parameters
    }

  
	
}
