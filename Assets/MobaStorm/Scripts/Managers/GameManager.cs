using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class GameManager : MonoSingleton<GameManager>
{

    private MobaPlayer m_localPlayer;
    /// <summary>
    /// Current Client Local Player
    /// </summary>
    public MobaPlayer LocalPlayer
    {
        get { return m_localPlayer; }
        set { m_localPlayer = value; }
    }

    private ECursors m_currentCursor;

    bool m_sceneSpawnerLoaded = false;

    private Dictionary<string, MobaEntity> m_gameEntities = new Dictionary<string, MobaEntity>();
    public Dictionary<string, MobaEntity> GameEntities
    {
        get { return m_gameEntities; }
    }

    private Dictionary<ETeam, HashSet<MobaEntity>> m_teamEntities = new Dictionary<ETeam, HashSet<MobaEntity>>()
    {
        {ETeam.Blue, new HashSet<MobaEntity>()}, {ETeam.Red, new HashSet<MobaEntity>()}, {ETeam.Neutral, new HashSet<MobaEntity>()}
    };
    public Dictionary<ETeam, HashSet<MobaEntity>> TeamEntities
    {
        get { return m_teamEntities; }
    }
    private CustomAudioSource m_musicAudioSource;
    public void PlayMusic(string clip, int volume)
    {
        if (m_musicAudioSource)
        {
            m_musicAudioSource.StopSound();
        }
        m_musicAudioSource = AudioManager.instance.Play2dSound(clip, volume, true);
       
    }

    private bool m_isGameOver;
    public bool IsGameOver
    {
        get { return m_isGameOver; }
        set { m_isGameOver = value; }
    }


    [SerializeField]
    private LayerMask m_mask;
    public LayerMask Mask
    {
        get { return m_mask; }
    }
    [SerializeField]
    private LayerMask m_floorMask;
    public LayerMask FloorMask
    {
        get { return m_mask; }
    }

    [SerializeField]
    private GameObject m_blueWinScreen;

    [SerializeField]
    private GameObject m_redWinScreen;

    private List<MinionSpawnManager> m_minionSpawners = new List<MinionSpawnManager>();

    private bool m_minionWaveStarted = false;

    public void AddMinionSpawner(MinionSpawnManager minionSpawner)
    {
        m_minionSpawners.Add(minionSpawner);
    }

    public void StartSpawningMinions()
    {
        m_minionWaveStarted = true;
        foreach (MinionSpawnManager minionSpawner in m_minionSpawners)
        {
            minionSpawner.StartSpawningMinions();
        }
    }

    private void GameOver()
    {
        m_isGameOver = true;
    }

    public void NexusDestroyed(MobaEntity entity)
    {
        CameraController.instance.sources.target = entity.transform;
        GameOver();
        if (m_localPlayer != null && m_localPlayer.isClient)
        {
            if (m_localPlayer.CharacterEntity.Team != entity.Team)
            {
                AudioManager.instance.Play2dSound("MobaStorm_YouWin", 100);
            }
            else
            {
                AudioManager.instance.Play2dSound("MobaStorm_YouLose", 100);
            }
            if (entity.Team == ETeam.Blue)
            {
                GameManager.instance.TeamWin(ETeam.Blue);
            }
            else
            {
                GameManager.instance.TeamWin(ETeam.Red);
            }
        }
    }


    public bool IsLocalCharacter(MobaEntity characterEntity)
    {
        return characterEntity == m_localPlayer.CharacterEntity;
    }

    public void StopMusic()
    {
        if (m_musicAudioSource != null)
        {
            m_musicAudioSource.StopSound();
        }
    }

    public void SetCursor(ECursors cursor)
    {
        if (m_currentCursor != cursor)
        {
            m_currentCursor = cursor;
            switch (cursor)
            {
                case ECursors.Cursor_Default:
                    Cursor.SetCursor(SpriteDatabaseManager.instance.GetTexture(cursor.ToString()), Vector2.zero, CursorMode.Auto);
                    break;
                case ECursors.Cursor_Attack:
                    //new Vector2(15, 15)
                    Cursor.SetCursor(SpriteDatabaseManager.instance.GetTexture(cursor.ToString()), Vector2.zero, CursorMode.Auto);

                    break;
                default:
                    break;
            }
        }    
    }

    /// <summary>
    /// Adds a MobaEntity to all entity list
    /// </summary>
    /// <param name="entity">Entity Spawned</param>
    public void EntitySpawned(MobaEntity entity)
    {
        if (entity is CharacterEntity)
        {
            if (entity.isServer)
            {
                if (!m_minionWaveStarted)
                {
                    StartSpawningMinions();
                }
            }

        }
        if (!m_gameEntities.ContainsKey(entity.InstanceId))
        {
            m_gameEntities.Add(entity.InstanceId, entity);
        }
        else
        {
            Debug.LogError("Entity ID already exist: " + entity.InstanceId);
        }

        m_teamEntities[entity.Team].Add(entity);
    }

    /// <summary>
    /// Adds a MobaEntity to all entity list
    /// </summary>
    /// <param name="entity">Entity Spawned</param>
    public void EntityUnSpawned(MobaEntity entity)
    {      
        if (m_gameEntities.ContainsKey(entity.InstanceId))
        {
            m_gameEntities.Remove(entity.InstanceId);
        }
        else
        {
            Debug.LogError("Entity ID already exist: " + entity.InstanceId);
        }

        m_teamEntities[entity.Team].Remove(entity);
    }
    /// <summary>
    /// Removes a MobaEntity from the all entity lists
    /// </summary>
    /// <param name="entity">Entity destroyed</param>
    public void EntityDestroyed(MobaEntity entity)
    {
        if (m_gameEntities.ContainsKey(entity.InstanceId))
        {
            m_gameEntities.Remove(entity.InstanceId);
        }
        else
        {
            Debug.LogError("Entity ID doesnt exist: " + entity.netId);
        }

        m_teamEntities[entity.Team].Remove(entity);

    }
    /// <summary>
    /// Finds a MobaEntity using the Instance Identifier
    /// </summary>
    /// <param name="instanceId">Moba Entity InstanceIdentifier</param>
    /// <returns>Returns MobaEntity</returns>
    public MobaEntity GetMobaEntity(string instanceId)
    {
        if (m_gameEntities.ContainsKey(instanceId))
        {
            return m_gameEntities[instanceId];
        }
        else
        {
            Debug.LogError("Entity ID diesnt exist: " + instanceId);
        }
        return null;
    }

    public void CharacterCreated(CharacterEntity characterEntity)
    {
        //If the character instantiate is the main character    
        if (LocalPlayer != null && characterEntity.InstanceId == LocalPlayer.PlayerName)
        {
            GameEvents.Instance.PlayGreetingsAudio();
            m_localPlayer.CharacterEntity = characterEntity;
            CameraController.instance.sources.target = characterEntity.transform;
            CameraController.instance.config.cameraActive = true;
            CameraController.instance.config.cameraLocked = true;
            UiManager.instance.LoadGameUI(characterEntity);
        }      
    }

    public void MinionKilled(DamageProcess damageProcess)
    {
        if (damageProcess.Attacker is CharacterEntity)
        {
            switch (damageProcess.Target.Team)
            {
                case ETeam.Blue:
                    ScoreBoardManager.Instance.RedMinionScore++;
                    break;
                case ETeam.Red:
                    ScoreBoardManager.Instance.BlueMinionScore++;
                    break;
            }
        }
        
    }
    public void TowerKilled(DamageProcess damageProcess)
    {
        switch (damageProcess.Target.Team)
        {
            case ETeam.Blue:
                ScoreBoardManager.Instance.RedTowerScore++;
                break;
            case ETeam.Red:
                ScoreBoardManager.Instance.BlueTowerScore++;
                break;
        }    
    }

    public void LoadWorldScene(string scene)
    {
        StartCoroutine(AsynchronousLoad(scene));
    }

    public void TeamWin(ETeam team)
    {
        switch (team)
        {
            case ETeam.Red:
                m_redWinScreen.SetActive(true);
                break;
            case ETeam.Blue:
                m_blueWinScreen.SetActive(true);
                break;
            default:
                break;
        }
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        yield return null;

        AsyncOperation ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            //float progress = Mathf.Clamp01(ao.progress / 0.9f);
            //Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Loading completed
            if (ao.progress == 0.9f)
            {
                //Debug.Log("Press a key to start");
                //if (Input.anyKey)
                ao.allowSceneActivation = true;
                if (NetworkServer.active && !m_sceneSpawnerLoaded)
                {

                }
            }

            yield return null;
        }

        while (!ServerEntitySpawner.instance)
        {
            yield return null;
        }
        ServerEntitySpawner.instance.SpawnSceneObjects();
        m_sceneSpawnerLoaded = true;

    }
    
}
