using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PoolData
{
    //public string m_gameObjectIdentifier;
    public bool m_foldOut = true;
    public int m_quantity;
    public bool m_register;
    public GameObject m_obj;
}

public class PrefabHolder : MonoBehaviour {

    public List<PoolData> m_poolDataList = new List<PoolData>();

    public GameObject GetPrefabFromHolder(string prefabName)
    {
        foreach (PoolData poolData in m_poolDataList)
        {
            if (!poolData.m_obj)
            {
                continue;
            }
            if (prefabName == poolData.m_obj.name)
            {
                return poolData.m_obj;
            }
        }
        Debug.LogError("Cant find the prefab on this holder: " + prefabName);
        return null;
    }

    public bool RemovePrefab(string prefabName)
    {
        for (int i = 0; i < m_poolDataList.Count; i++)
        {
            if (!m_poolDataList[i].m_obj)
            {
                continue;
            }
            if (prefabName == m_poolDataList[i].m_obj.name)
            {
                m_poolDataList.RemoveAt(i);
                return true;
            }
        }
        Debug.LogError("Cant find the prefab on this holder: " + prefabName);
        return false;
    }

    public void AddPrefab(GameObject prefabName, bool registerUnet = false)
    {
        PoolData newPool = new PoolData();
        newPool.m_obj = prefabName;
        newPool.m_register = registerUnet;
        m_poolDataList.Add(newPool);
    }
}


