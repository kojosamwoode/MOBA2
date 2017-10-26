using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class GlobalConfig  {
    //Range used by AI to detect enemies.
    public int m_maxItemSlots = 6;

    public int m_extraItemSlots = 2;

    //Range used by AI to detect enemies.
    public float m_minionAggroRange;
    //Max distance allowed for AI to chase enemies
    public float m_maxChasingDistance = 4;
    //Delay in seconds to perform a target path update.
    public float m_targetPathUpdateRate = 0.5f;
    //Delay in seconds to perform a target update.
    public float m_targetUpdateRate = 20;
    //Update ratio to calculate path  in seconds
    public float m_pathUpdateRate = 2;
    //Delay in seconds to start spawning minions.
    public float m_waveSpawnStartDelay = 5;
    //Delay in seconds between waves.
    public float m_waveSpawnDelay = 60;

    public int[] m_accumulativeExpTable;
#if UNITY_EDITOR
    public void DrawEditor()
    {
        GUILayout.Label("Global Parameters", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Minion Aggro Range");
        m_minionAggroRange = EditorGUILayout.FloatField(m_minionAggroRange, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Max Chasing Distance");
        m_maxChasingDistance = EditorGUILayout.FloatField(m_maxChasingDistance, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Path Update Rate");
        m_targetPathUpdateRate = EditorGUILayout.FloatField(m_targetPathUpdateRate, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Update Rate");
        m_targetUpdateRate = EditorGUILayout.FloatField(m_targetUpdateRate, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Path Update Rate");
        m_pathUpdateRate = EditorGUILayout.FloatField(m_pathUpdateRate, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Wave Spawn Start Delay");
        m_waveSpawnStartDelay = EditorGUILayout.FloatField(m_waveSpawnStartDelay, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Wave Spawn Delay");
        m_waveSpawnDelay = EditorGUILayout.FloatField(m_waveSpawnDelay, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.Label("Character Experience Table", EditorStyles.boldLabel);
        if (m_accumulativeExpTable == null)
        {
            m_accumulativeExpTable = new int[13];
        }
        for (int i = 0; i < m_accumulativeExpTable.Length; i ++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level " + i);
            m_accumulativeExpTable[i] = EditorGUILayout.IntField(m_accumulativeExpTable[i], GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }



    }
#endif
}
