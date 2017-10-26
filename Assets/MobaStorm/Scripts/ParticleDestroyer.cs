using UnityEngine;
using System.Collections;
using System;

public class ParticleDestroyer : MonoBehaviour, IPooled
{

    public float m_timeToDestroy = 2;
    public void OnInstantiate()
    {
        m_timeToDestroy = 2;
    }

    public void OnUnSpawn()
    {
    }

    void Update()
    {
        m_timeToDestroy -= Time.deltaTime;
        if (m_timeToDestroy <= 0)
        {
            SpawnManager.instance.DestroyPool(this.gameObject);
        }
    }
}
