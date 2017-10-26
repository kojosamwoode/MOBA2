using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections.Generic;

public class MobaServerManager : MonoSingleton<MobaServerManager>
{
    //Server Variables
    [SerializeField]
    private int m_gameServerPort = 15000;

    [SerializeField]
    private GameObject m_entitySpawner;

    [SerializeField]
    private NetworkTime m_networkTime;

    NetworkServer myServer;
    // Use this for initialization
    private int m_connectionId = 0;

    [SerializeField]
    private string m_serverLevel = "Level_PitServer";

    /// <summary>
    /// List of connected players, KEY: Connection ID
    /// </summary>
    private Dictionary<int, MobaPlayer> m_connectedPlayers = new Dictionary<int, MobaPlayer>();

    public void SetupServer()
    {
        ConnectionConfig config = new ConnectionConfig();
        config.NetworkDropThreshold = 90;
        config.ReducedPingTimeout = 1000;
        config.PingTimeout = 1000;
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.AllCostDelivery);
        NetworkServer.Configure(config, 10);
        NetworkServer.RegisterHandler(MsgType.AddPlayer, OnAddPlayerServer);
        if (NetworkServer.Listen(m_gameServerPort))
        {
            Debug.Log("<color=blue>MOBA STORM:</color>------------Server Started------------");
            SpawnManager.instance.CachePool();
            NetworkServer.SpawnObjects();
            MenuManager.instance.ShowMenu<MenuPickSelection>();
            (MenuManager.instance.CurrentMenu as MenuPickSelection).InitializeServerInfo();
            NetworkServer.RegisterHandler(MsgType.Ready, OnPlayerReadyMessage);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnectMessage);
            GameManager.instance.LoadWorldScene(m_serverLevel);
            ServerChat.instance.InitializeServerChat();
        }
    }

    private void OnPlayerDisconnectMessage(NetworkMessage netMsg)
    {
        if (m_connectedPlayers.ContainsKey(netMsg.conn.connectionId))
        {
            GameManager.instance.EntityUnSpawned(m_connectedPlayers[netMsg.conn.connectionId].CharacterEntity);
            SpawnManager.instance.DestroyPool(m_connectedPlayers[netMsg.conn.connectionId].CharacterEntity.gameObject);
            NetworkServer.UnSpawn(m_connectedPlayers[netMsg.conn.connectionId].CharacterEntity.gameObject);
            Destroy(m_connectedPlayers[netMsg.conn.connectionId].CharacterEntity.gameObject);
            m_connectedPlayers.Remove(netMsg.conn.connectionId);
        }
        NetworkServer.DestroyPlayersForConnection(netMsg.conn);
    }

    // AddPlayer handler for the server side
    public void OnAddPlayerServer(NetworkMessage netMsg)
    {
        m_connectionId++;
        AddPlayerMessage msg = netMsg.ReadMessage<AddPlayerMessage>();
        GameObject newPlayer = SpawnManager.instance.InstantiatePool("Player", Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(netMsg.conn, newPlayer, msg.playerControllerId);
        MobaPlayer mobaPlayer = newPlayer.GetComponent<MobaPlayer>();
        mobaPlayer.PlayerName = "Player " + m_connectionId;
        mobaPlayer.PickSlot = (MenuManager.instance.CurrentMenu as MenuPickSelection).AssignPlayerToFreeSlot(mobaPlayer);
        m_connectedPlayers.Add(netMsg.conn.connectionId, mobaPlayer);
        Debug.Log("------------Player Joined with IP------------ " + netMsg.conn.address);
    }


    // On the server side
    public void OnPlayerReadyMessage(NetworkMessage conn) 
    {
        // This spawns the new player on all clients
        NetworkServer.SetClientReady(conn.conn);
    }

}
