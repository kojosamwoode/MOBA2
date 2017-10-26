/// <summary>
/// AI character controller.
/// Just A basic AI Character controller
/// will looking for a Target and moving to and Attacking
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using System;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Minion Data Class
/// This is the data container for all minions
/// </summary>
[System.Serializable]
public class MinionData : MobaEntityData
{
    public float m_aggroRange;
#if UNITY_EDITOR
    //Draw CharacterData in the editor window
    public override MobaEntityData DrawEditor(GUISkin skin)
    {
        MinionData definiton = base.DrawEditor(skin) as MinionData;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Aggro Range", skin.label);
        float aggroRange = EditorGUILayout.FloatField(m_aggroRange, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        definiton.m_aggroRange = aggroRange;
        GUILayout.EndHorizontal();
        return definiton;
    }
    public override MobaEntityData LoadEntityData(string path)
    {
        return Utils.Load<MinionData>(path);
    }

    public override string DataFileExtension()
    {
        return GameDataManager.m_minionExtension;
    }

#endif
}
/// <summary>
/// Minion Data Class
/// This is the data container for all Towers
/// </summary>
[System.Serializable]
public class TowerData : MobaEntityData
{
    //Detection rangue for towers
    public float m_detectionRange;
#if UNITY_EDITOR
    public override MobaEntityData DrawEditor(GUISkin skin)
    {
        TowerData definiton = base.DrawEditor(skin) as TowerData;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Detection Range", skin.label);
        float detectionRange = EditorGUILayout.FloatField(m_detectionRange, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        definiton.m_detectionRange = detectionRange;
        GUILayout.EndHorizontal();
        return definiton;
    }
    public override MobaEntityData LoadEntityData(string path)
    {
        return Utils.Load<TowerData>(path);
    }
    public override string DataFileExtension()
    {
        return GameDataManager.m_towerExtension;
    }
#endif
}

/// <summary>
/// Minion Data Class
/// This is the data container for all Nexus
/// </summary>
[System.Serializable]
public class NexusData : MobaEntityData
{
#if UNITY_EDITOR
    public override MobaEntityData LoadEntityData(string path)
    {
        return Utils.Load<NexusData>(path);
    }
    public override string DataFileExtension()
    {
        return GameDataManager.m_nexusExtension;
    }
#endif
}

/// <summary>
/// AiEntity
/// This is the implementation of Moba Entity used by all non characters in the game.
/// </summary>
[System.Serializable]
public class AIEntity : MobaEntity {

    private LineRenderer m_debugPathLineRend;

    #region UNITY_CALLS 
    public override void Awake()
    {
        base.Awake();
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
        if (isClient)
            return;
    }
    #endregion

    //Initialize the current entity, and load the entityData from its identifier
    public override void InitializeEntity(MobaEntityData entityData, string identifier)
    {
        base.InitializeEntity(entityData, identifier);
        //Initialize additional parameters
        ServerSetEntityTeam(entityData.m_team);
        m_entityData = entityData;
        InitializePathRenderer();
    }

    //Initialize the path renderer to show the debug path in the editor
    void InitializePathRenderer()
    {
        if (m_debugPathLineRend == null)
        m_debugPathLineRend = Utils.CreateLineRendererObject(this, true);
    }

    //Override method to calculate the path for the AI Entities
    public override MobaPath ServerCalculatePath(Vector2 target)
    {
        MobaPath path = base.ServerCalculatePath(target);
        if (path == null)
        {
            return null;
        }
        Vector3[] points = new Vector3[path.Corners.Length];
        for (int i = 0; i < path.Corners.Length; i ++)
        {
            points[i] = new Vector3(path.Corners[i].x, transform.position.y + 0.02f, path.Corners[i].y);
        }
        m_debugPathLineRend.numPositions = path.Corners.Length;
        m_debugPathLineRend.SetPositions(points);
        return path;
    }
}
