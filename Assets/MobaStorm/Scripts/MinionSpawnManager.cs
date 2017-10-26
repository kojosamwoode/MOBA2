using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;

[System.Serializable]
public class WaveData
{
    [SerializeField]
    public string m_entityIdentifier;
    [SerializeField]
    public Transform m_position;
}

public class MinionSpawnManager : MonoBehaviour {

    [SerializeField]
    private List<WaveData> m_waveDataList = new List<WaveData>();
    public string team;
	public WaypointCircuit circuitScript;
	private AIEntity minionScript;
	private WaypointProgressTracker waypointScript;

    public static WaypointCircuit m_blueCircuitScript;
    public static WaypointCircuit m_redCirtcuitScript;
    private int waveNumber;
	public float health;
	public float healthRegen;
	public int lvl;
	public float expToGive;
	public float goldToGive;
	public float ad;
	public float ap;
	public float adRes;
	public float apRes;

	public float adFinal;
	public float apFinal;
	GameObject gameManager;

    private bool m_canSpawn;
    public bool CanSpawn
    {
        get { return m_canSpawn; }
        set { m_canSpawn = value; }
    }
    private float m_delayTime;

    void Start () {
        foreach (WaveData wave in m_waveDataList)
        {
            if (wave.m_position == null || string.IsNullOrEmpty(wave.m_entityIdentifier))
            {
                Debug.LogError("Error Loading Wave Data. Please check MinionSpawnManagerPrefab");
            }
        }

        if (GameDataManager.instance.GlobalConfig.m_waveSpawnDelay == 0)
        {
            Debug.LogError("Error Loading GlobalConfig.WaveSpawnDelay. Must be greater than 0");
        }
        if (team == "red")
        {
            m_redCirtcuitScript = circuitScript;
        }
        else
        {
            m_blueCircuitScript = circuitScript;
        }

        GameManager.instance.AddMinionSpawner(this);

    }

    public void StartSpawningMinions()
    {
        StartCoroutine(StartSpawning());
    }

	IEnumerator StartSpawning () {
		yield return new WaitForSeconds(GameDataManager.instance.GlobalConfig.m_waveSpawnStartDelay);
        m_canSpawn = !m_canSpawn;
    }
	// This method spawns each enemy wave for the teams
	// Sends data to each minion playerstats script
	void SpawnMinion (WaveData wave) {
        AIEntity minionEntity = ServerEntitySpawner.instance.SpawnMinion(wave.m_entityIdentifier, wave.m_position.position, transform.rotation);
		waypointScript = minionEntity.GetComponent<WaypointProgressTracker>();
		minionScript = minionEntity.GetComponent<AIEntity>();
		waypointScript.circuit = circuitScript;
		minionEntity.name = wave.m_entityIdentifier + waveNumber.ToString();
        (minionScript.EntityLogic as MinionWaveLogic).XOffset = wave.m_position.position.x;
		waveNumber++;
    }
    IEnumerator SpawnWave()
    {
        foreach (WaveData wave in m_waveDataList)
        {
            SpawnMinion(wave);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(15);
        m_canSpawn = true;
    }

    void Update () {

        if (!m_canSpawn || GameManager.instance.IsGameOver)
            return;
        
        m_delayTime -= Time.deltaTime;
        if (m_delayTime <= 0)
        {

            StartCoroutine(SpawnWave());
            m_delayTime = GameDataManager.instance.GlobalConfig.m_waveSpawnDelay;
        }

       
    }
}
