using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataManager : MonoSingleton<GameDataManager> {

    #region FIELDS
    public static string m_gameDataPath =  "/GameData/";

    public const string m_globalExtension = "global";

    public const string m_characterExtension = "char";

    public const string m_towerExtension = "tower";

    public const string m_nexusExtension = "nexus";

    public const string m_minionExtension = "minion";

    bool m_show = false;
    public bool ShowLog
    {
        get { return m_show; }
        set { m_show = value; }
    }
    static string myLog = "";
    private string output = "";
    private string stack = "";
    [SerializeField]
    public GlobalConfig m_globalConfig; 
    [SerializeField]
    private List<CharacterData> m_characterData = new List<CharacterData>();
    public List<CharacterData> CharacterData
    {
        get { return m_characterData; }
        set { m_characterData = value; }
    }
    [SerializeField]
    private List<MinionData> m_minionData = new List<MinionData>();
    public List<MinionData> MinionData
    {
        get { return m_minionData; }
        set { m_minionData = value; }
    }
    [SerializeField]
    private List<NexusData> m_nexusData = new List<NexusData>();
    public List<NexusData> NexusData
    {
        get { return m_nexusData; }
        set { m_nexusData = value; }
    }
    [SerializeField]
    private List<TowerData> m_towerData = new List<TowerData>();
    public List<TowerData> TowerData
    {
        get { return m_towerData; }
        set { m_towerData = value; }
    }

    private Dictionary<string, MobaEntityData> m_entityData = new Dictionary<string, MobaEntityData>();
    public Dictionary<string, MobaEntityData> EntityData
    {
        get { return m_entityData; }
    }

    public GlobalConfig GlobalConfig
    {
        get { return m_globalConfig; }
        set { m_globalConfig = value; }
    }
    #endregion

    #region UNITY_CALLS
    void Awake()
    {
        LoadAllData();
    }
    void Start()
    {
        Application.logMessageReceived += LogHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!ClientChat.instance.IsInputFieldOnFocus)
            {
                m_show = !m_show;
            }
        }
    }

    void OnGUI()
    {
        if (m_show)
            myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
    }

    #endregion

    /// <summary>
    /// Cache all Entitiy Data available to a dictionary for fast access
    /// </summary>
    public void LoadAllData()
    {
        m_entityData.Clear();
        int dataGenericIdentifier = 0;
        foreach (NexusData def in m_nexusData)
        {
            if (!m_entityData.ContainsKey(def.m_dataIdentifier))
            {
                m_entityData.Add(def.m_dataIdentifier, def);
            }
            else
            {
                m_entityData.Add((dataGenericIdentifier += 1).ToString(), def);
                Debug.LogError("Data Identifier Duplicated: " + def.m_dataIdentifier);
            }
        }

        foreach (TowerData def in m_towerData)
        {
            if (!m_entityData.ContainsKey(def.m_dataIdentifier))
            {
                m_entityData.Add(def.m_dataIdentifier, def);
            }
            else
            {
                m_entityData.Add((dataGenericIdentifier += 1).ToString(), def);
                Debug.LogError("Data Identifier Duplicated: " + def.m_dataIdentifier);
            }
        }
        foreach (CharacterData def in m_characterData)
        {
            if (!m_entityData.ContainsKey(def.m_dataIdentifier))
            {
                m_entityData.Add(def.m_dataIdentifier, def);
            }
            else
            {
                m_entityData.Add((dataGenericIdentifier += 1).ToString(), def);
                Debug.LogError("Data Identifier Duplicated: " + def.m_dataIdentifier);
            }
        }
        foreach (MinionData def in m_minionData)
        {
            if (!m_entityData.ContainsKey(def.m_dataIdentifier))
            {
                m_entityData.Add(def.m_dataIdentifier, def);
            }
            else
            {
                m_entityData.Add((dataGenericIdentifier += 1).ToString(), def);
                Debug.LogError("Data Identifier Duplicated: " + def.m_dataIdentifier);
            }
        }
    }

    /// <summary>
    /// Get any MobaEntityData available
    /// </summary>
    /// <param name="entityDataIdentifier">Entity Data Identifier</param>
    /// <returns>Custom MobaEntity Data</returns>
    public MobaEntityData GetEntityData(string entityDataIdentifier)
    {
        if (m_entityData.ContainsKey(entityDataIdentifier))
        {
            return m_entityData[entityDataIdentifier];
        }
        
        Debug.LogError("Data Identifier doesnt exist: " + entityDataIdentifier);
        return null;
    }
    /// <summary>
    /// Remove current Moba Entity Data
    /// </summary>
    /// <param name="entityData">Entity Data To Remove</param>
    /// <returns>True if the entity data is removed</returns>
    public bool RemoveEntityData(MobaEntityData entityData)
    {
        for (var i = 0; i < m_nexusData.Count; i++)
        {
            if(m_nexusData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_nexusData.RemoveAt(i);
                return true;
            }
        }

        for (var i = 0; i < m_towerData.Count; i++)
        {
            if (m_towerData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_towerData.RemoveAt(i);
                return true;
            }
        }

        for (var i = 0; i < m_minionData.Count; i++)
        {
            if (m_minionData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_minionData.RemoveAt(i);
                return true;
            }
        }

        for (var i = 0; i < m_characterData.Count; i++)
        {
            if (m_characterData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_characterData.RemoveAt(i);
                return true;
            }
        }

        Debug.LogError("Cant find the Data Identifier to remove: ");
        return false;
    }


    /// <summary>
    /// Add a new Entity Data to the Data Manager
    /// </summary>
    /// <param name="entityData">Entity Data to add</param>
    public void AddEntityData(MobaEntityData entityData)
    {
        if(entityData is CharacterData)
        {
            m_characterData.Add(entityData as CharacterData);
        }
        else if(entityData is MinionData)
        {
            m_minionData.Add(entityData as MinionData);
        }
        else if(entityData is NexusData)
        {
            m_nexusData.Add(entityData as NexusData);
        }
        else if(entityData is TowerData)
        {
            m_towerData.Add(entityData as TowerData);
        }
    }
    /// <summary>
    /// Replaces a current Entity Data
    /// </summary>
    /// <param name="entityData">Entity Data To Remove</param>
    /// <param name="newEntityData">Entity Data To Add</param>
    /// <returns></returns>
    public bool ReplaceEntityData(MobaEntityData entityData, MobaEntityData newEntityData)
    {
        for (var i = 0; i < m_nexusData.Count; i++)
        {
            if (m_nexusData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_nexusData[i] = newEntityData as NexusData;
                return true;
            }
        }

        for (var i = 0; i < m_towerData.Count; i++)
        {
            if (m_towerData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_towerData[i] = newEntityData as TowerData;
                return true;
            }
        }

        for (var i = 0; i < m_minionData.Count; i++)
        {
            if (m_minionData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_minionData[i] = newEntityData as MinionData;
                return true;
            }
        }

        for (var i = 0; i < m_characterData.Count; i++)
        {
            if (m_characterData[i].m_dataIdentifier == entityData.m_dataIdentifier)
            {
                m_characterData[i] = newEntityData as CharacterData;
                return true;
            }
        }

        Debug.LogError("Cant find the Data Identifier to remove: ");
        return false;
    }


    private void LogHandler(string condition, string stackTrace, LogType type)
    {
        output = condition;
        stack = stackTrace;
        myLog += "\n" + output;
        if (condition.Contains("Null"))
        {
            myLog += "\n" + stack;
        }
    }

   

}
