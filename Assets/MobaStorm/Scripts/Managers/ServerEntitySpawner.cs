using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using UnityStandardAssets.Utility;

/// <summary>
/// This class is resposible in spawn all Game Entities from the server.
/// </summary>
public class ServerEntitySpawner : MonoSingleton<ServerEntitySpawner> {
    [System.Serializable]
    public class EntitySceneObjects
    {
        public string m_def;
        public Transform m_spawnTransform;
    }
    [SerializeField]
    private List<EntitySceneObjects> m_sceneObjects = new List<EntitySceneObjects>();
    /// <summary>
    /// Spawn All Networked Scene Objects
    /// Only need to be called on the server side
    /// </summary>
    public void SpawnSceneObjects()
    {
        foreach (EntitySceneObjects entity in m_sceneObjects)
        {
            if (entity.m_def.Contains("Nexus"))
            {
                SpawnNexus(entity.m_def, entity.m_spawnTransform.position, entity.m_spawnTransform.rotation);
            }
            if (entity.m_def.Contains("Tower"))
            {
                SpawnTower(entity.m_def, entity.m_spawnTransform.position, entity.m_spawnTransform.rotation);
            }
        }
        Pathfinding.instance.CreateGrid();
    }
    /// <summary>
    /// Creates a nexus based on the Data Identifier parameters
    /// </summary>
    /// <param name="entityIdentifier">Entity Identifier</param>
    /// <param name="pos">Position</param>
    /// <param name="rot">Rotation</param>
    public void SpawnNexus(string entityIdentifier, Vector3 pos, Quaternion rot)
    {
        NexusData def = GameDataManager.instance.GetEntityData(entityIdentifier) as NexusData;
        GameObject nexusObj = SpawnManager.instance.InstantiatePool(def.m_prefab, pos, rot);
        AIEntity aientityComponent = nexusObj.GetComponent<AIEntity>();
        aientityComponent.InitializeEntity(def, Guid.NewGuid().ToString());
        NetworkServer.Spawn(nexusObj);
    }
    /// <summary>
    /// Spawn a Tower From its Identifier
    /// </summary>
    /// <param name="entityIdentifier">Entity Identifier to spawn</param>
    /// <param name="pos">Position</param>
    /// <param name="rot">Rotation</param>
    public void SpawnTower(string entityIdentifier, Vector3 pos, Quaternion rot)
    {
        TowerData def = GameDataManager.instance.GetEntityData(entityIdentifier) as TowerData;
        GameObject towerObj = SpawnManager.instance.InstantiatePool(def.m_prefab, pos, rot);
        AIEntity towerComponent = towerObj.GetComponent<AIEntity>();
        towerComponent.InitializeEntity(def, Guid.NewGuid().ToString());
        NetworkServer.Spawn(towerObj);
    }
    /// <summary>
    /// Spawn a Minion From its Identifier
    /// </summary>
    /// <param name="entityIdentifier">Entity Identifier to spawn</param>
    /// <param name="pos">Position</param>
    /// <param name="rot">Rotation</param>
    public AIEntity SpawnMinion(string entityIdentifier, Vector3 pos, Quaternion rot)
    {
        MinionData def = GameDataManager.instance.GetEntityData(entityIdentifier) as MinionData;
        GameObject obj = SpawnManager.instance.InstantiatePool(def.m_prefab, pos, rot);
        AIEntity iaComponent = obj.GetComponent<AIEntity>();
        iaComponent.InitializeEntity(def, Guid.NewGuid().ToString());
        NetworkServer.Spawn(obj);
        return iaComponent;
    }

    /// <summary>
    /// Spawn a Character From its Identifier
    /// </summary>
    /// <param name="player">Current Moba Player</param>
    /// <param name="pos">Position</param>
    /// <param name="rot">Rotation</param>
    /// <param name="connectionToClient">Network Connection To Client</param>
    /// <returns></returns>
    public CharacterEntity SpawnCharacter(MobaPlayer player, Vector3 pos, Quaternion rot, NetworkConnection connectionToClient)
    {
        CharacterData def = GameDataManager.instance.GetEntityData(player.CharacterIdentifier) as CharacterData;
        GameObject characterObj = SpawnManager.instance.InstantiatePool(def.m_prefab, pos, rot);
        CharacterEntity characterComponent = characterObj.GetComponent<CharacterEntity>();
        characterComponent.OwnerConnectionId = connectionToClient.connectionId;
        characterComponent.InitializeEntity(def, player.PlayerName);
        characterComponent.ServerSetEntityTeam(player.GetTeamFromSlot());
        NetworkServer.SpawnWithClientAuthority(characterObj, connectionToClient);
        //NetworkServer.Spawn(characterObj);
        return characterComponent;
    }


}
