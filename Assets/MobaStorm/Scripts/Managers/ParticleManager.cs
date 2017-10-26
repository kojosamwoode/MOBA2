using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ParticleManager : NetworkBehaviour {

    private Dictionary<string, Particle> m_particles = new Dictionary<string, Particle>();
    private static ParticleManager m_instance;
    public static ParticleManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }
    /// <summary>
    /// Call this method to spawn network particles on a MobaEntity
    /// </summary>
    /// <param name="entity">Moba Entity</param>
    /// <param name="particlePrefab">Particle Prefab Name</param>
    /// <param name="position">EntityTransform Position</param>
    /// <returns></returns>
    [Server]
    public Particle ServerSpawnParticle(MobaEntity entity, string particlePrefab, EEntityTransform position)
    {
        EntityTransform entityTransform = entity.GetTransformPosition(position);
        GameObject sideEffectObj = SpawnManager.instance.InstantiatePool(particlePrefab, entityTransform.transform.position, entityTransform.transform.rotation);
        int projectileInstanceID = sideEffectObj.GetInstanceID();
        Particle prefab = sideEffectObj.GetComponent<Particle>();
        prefab.Initialize(particlePrefab + "_" + projectileInstanceID, entity);
        m_particles.Add(prefab.ServerInstanceId, prefab);
        RpcSpawnParticle(entity.InstanceId ,particlePrefab, prefab.ServerInstanceId, (int)position);
        return prefab;
    }
    /// <summary>
    /// Call this method to spawn particles on a Locally On the client over a Entity
    /// </summary>
    /// <param name="entity">Moba Entity</param>
    /// <param name="particlePrefab">Particle Prefab Name</param>
    /// <param name="position">EntityTransform position</param>
    /// <returns></returns>
    [Client]
    public Particle ClientSpawnParticle(MobaEntity entity, string particlePrefab, EEntityTransform position)
    {
        EntityTransform entityTransform = entity.GetTransformPosition(position);
        GameObject sideEffectObj = SpawnManager.instance.InstantiatePool(particlePrefab, entityTransform.transform.position, entityTransform.transform.rotation);
        int projectileInstanceID = sideEffectObj.GetInstanceID();
        Particle prefab = sideEffectObj.GetComponent<Particle>();
        prefab.Initialize(particlePrefab + "_" + projectileInstanceID, entity);
        m_particles.Add(prefab.ServerInstanceId, prefab);
        return prefab;
    }
    /// <summary>
    /// Unspawn particles on the server
    /// </summary>
    /// <param name="prefab">Particle Prefab</param>
    [Server]
    public void ServerUnspawnParticle(Particle prefab)
    {
        SpawnManager.instance.DestroyPool(prefab.gameObject);
        m_particles.Remove(prefab.ServerInstanceId);
        RpcUnSpawnParticle(prefab.ServerInstanceId);
    }
    /// <summary>
    /// Unspawn particles on the client
    /// </summary>
    /// <param name="prefab">Particle Prefab</param>
    [Client]
    public void ClientUnspawnParticle(Particle prefab)
    {
        SpawnManager.instance.DestroyPool(prefab.gameObject);
        m_particles.Remove(prefab.ServerInstanceId);
    }

    /// <summary>
    /// RPC method to spawn particles on the clients
    /// </summary>
    /// <param name="entityInstanceId">Moba Entity Instance ID </param>
    /// <param name="particlePrefab">Particle PrefabName</param>
    /// <param name="serverInstanceId">Particle Instance ID</param>
    /// <param name="position">EntityTrnasform Position to spawn the particle</param>
    [ClientRpc]
    private void RpcSpawnParticle(string entityInstanceId, string particlePrefab, string serverInstanceId, int position)
    {
        MobaEntity entity = GameManager.instance.GetMobaEntity(entityInstanceId);
        if (entity == null)
        {
            Debug.LogError("Entity not found");
            return;
        }
        EntityTransform entityTransform = entity.GetTransformPosition((EEntityTransform)position);
        GameObject sideEffectObj = SpawnManager.instance.InstantiatePool(particlePrefab, entityTransform.transform.position, entityTransform.transform.rotation);
        Particle prefab = sideEffectObj.GetComponent<Particle>();
        prefab.Initialize(serverInstanceId, entity);
        m_particles.Add(prefab.ServerInstanceId, prefab);
    }
    [ClientRpc]
    private void RpcUnSpawnParticle(string serverInstanceId)
    {
        SpawnManager.instance.DestroyPool(m_particles[serverInstanceId].gameObject);
        m_particles.Remove(serverInstanceId);
    }
}
