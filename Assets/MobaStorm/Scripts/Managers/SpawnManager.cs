using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public int m_ObjectPoolSize = 5;
    public GameObject m_Prefab_client;
    public GameObject m_Prefab_server;
    public GameObject[] m_Pool;

    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);

    public Dictionary<NetworkHash128, GameObject> m_prefabs = new Dictionary<NetworkHash128, GameObject>();

    public List<PrefabHolder> m_holderList = new List<PrefabHolder>();

    public Dictionary<string, List<GameObject>> m_poolObjects = new Dictionary<string, List<GameObject>>();

    public Dictionary<string, GameObject> m_prefabsDict = new Dictionary<string, GameObject>();

    public void CachePool()
    {
        foreach (PrefabHolder holder in m_holderList)
        {
            foreach (PoolData poolData in holder.m_poolDataList)
            {
                if (!poolData.m_obj)
                    continue;
                //Add all prefabs to a dictionary for direct access
                if (!m_prefabsDict.ContainsKey(poolData.m_obj.name))
                {
                    m_prefabsDict.Add(poolData.m_obj.name, poolData.m_obj);
                }
                else
                {
                    Debug.LogError("Error: Prefab Name Duplicated: " + poolData.m_obj.name);
                }

                if (!m_poolObjects.ContainsKey(poolData.m_obj.name))
                {
                    if (poolData.m_register)
                    {
                        RegisterPrefab(poolData.m_obj);
                    }
                    m_poolObjects.Add(poolData.m_obj.name, new List<GameObject>());
                    for (int i = 1; i <= poolData.m_quantity; i++)
                    {
                        GameObject currentObj = Instantiate(poolData.m_obj) as GameObject;
                        currentObj.SetActive(false);
                        currentObj.transform.parent = transform;
                        currentObj.name = poolData.m_obj.name;
                        m_poolObjects[poolData.m_obj.name].Add(currentObj);
                    }
                }
            }
              
        }
        
    }

    //This method is used when a prefab is not in the pool dictionary so we add a new one
    void CreatePool(GameObject prefab)
    {
        if (!m_poolObjects.ContainsKey(prefab.name))
        {
            m_poolObjects.Add(prefab.name, new List<GameObject>());
            for (int i = 1; i <= 1; i++)
            {
                GameObject currentObj = Instantiate(prefab) as GameObject;
                currentObj.SetActive(false);
                currentObj.transform.parent = transform;
                currentObj.name = prefab.name;
                m_poolObjects[prefab.name].Add(currentObj);
            }
        }
    }

    /// <summary>
    /// Use thid method to instantiate a gameobject using a String Prefab Name
    /// </summary>
    /// <param name="obj">  PREFAB NAME </param>
    /// <param name="pos">  Position    </param>
    /// <param name="rot">  Rotation    </param>
    /// <returns></returns>
    public GameObject InstantiatePool(string obj, Vector3 pos, Quaternion rot)
    {
        if (m_prefabsDict.ContainsKey(obj))
        {
            GameObject newObj = InstantiatePool(m_prefabsDict[obj], pos, rot);
            return newObj;
        }
        else
        {
            Debug.LogError("Prefab Name Not Found: " + obj);
        }

        return null;
    }

    /// <summary>
    /// Use thid method to instantiate a gameobject using a Gameobject reference
    /// </summary>
    /// <param name="obj">  PREFAB </param>
    /// <param name="pos">  Position    </param>
    /// <param name="rot">  Rotation    </param>
    /// <returns></returns>
    private GameObject InstantiatePool(GameObject obj, Vector3 pos, Quaternion rot)
    {
        if (!m_poolObjects.ContainsKey(obj.name))
        {
            CreatePool(obj);
        }
        foreach (GameObject poolObj in m_poolObjects[obj.name])
        {
            if (poolObj == null)
                continue;
            if (poolObj.activeSelf == false)
            {

                poolObj.transform.position = pos;
                poolObj.transform.rotation = rot;
                IPooled pooledObj = poolObj.GetComponent<IPooled>();
                if (pooledObj != null)
                {
                    pooledObj.OnInstantiate();
                }
                poolObj.SetActive(true);
                return poolObj;
            }
        }


        GameObject currentObj = Instantiate(obj) as GameObject;
        currentObj.transform.position = pos;
        currentObj.transform.rotation = rot;
        currentObj.name = obj.name;
        currentObj.SetActive(true);
        IPooled newpooledObj = currentObj.GetComponent<IPooled>();
        if (newpooledObj != null)
        {
            newpooledObj.OnInstantiate();
        }
        m_poolObjects[obj.name].Add(currentObj);
        return currentObj;
    }



    public void RegisterPrefab(GameObject prefab)
    {
        NetworkIdentity networkIdentity = prefab.GetComponent<NetworkIdentity>();
        if (networkIdentity == null)
        {
            Debug.LogError("Registered Prefab doesnt contains Network Identity Component: " + prefab.name);
            return;
        }
        NetworkHash128 assetId = networkIdentity.assetId;
        if (!m_prefabs.ContainsKey(assetId))
        {
            m_prefabs.Add(assetId, prefab);
        }
        ClientScene.RegisterSpawnHandler(prefab.GetComponent<NetworkIdentity>().assetId, SpawnObject, DestroyPool);
    }

    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
    {
        GameObject serverObj = m_prefabs[assetId];
        GameObject obj = InstantiatePool(serverObj.name + "Client", position, Quaternion.identity);
        ClientScene.RegisterSpawnHandler(obj.GetComponent<NetworkIdentity>().assetId, SpawnObject, DestroyPool);
        return obj;
    }

    public void DestroyPool(GameObject spawned)
    {
        IPooled pooledObj = spawned.GetComponent<IPooled>();
        if (pooledObj != null)
        {
            pooledObj.OnUnSpawn();
        }
        spawned.transform.SetParent(transform);
        spawned.SetActive(false);
    }
}