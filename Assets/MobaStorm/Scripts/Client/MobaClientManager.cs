 using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MobaClientManager : MonoSingleton<MobaClientManager>  {
    
    [SerializeField]
    private int m_gameServerPort = 15000;
    NetworkClient myClient;

    [SerializeField]
    private GameObject m_cameraParticle;

    [SerializeField]
    private Text m_ipText;

    [SerializeField]
    private string m_clientLevel = "Level_PitClient";


    public NetworkClient MyClient
    {
        get { return myClient; }
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Network Server Active: " + NetworkServer.active);
	}

    void Start()
    {
    }

    public void SetupClient(bool simulatedLatency)
    {
        ConnectionConfig config = new ConnectionConfig();
        //config.por
        //MyNetworkManager.singleton.StartServer();
        
        config.NetworkDropThreshold = 90;
        config.ReducedPingTimeout = 1000;
        config.PingTimeout = 1000;
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.AllCostDelivery);
        myClient = new NetworkClient();
        myClient.Configure(config, 10);
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        if (simulatedLatency)
        myClient.ConnectWithSimulator(m_ipText.text, m_gameServerPort, 150, 0);
        else
        myClient.Connect(m_ipText.text, m_gameServerPort);
    }

     //client function
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("<color=blue>MOBA STORM:</color>------------Connected to Server------------");
        SpawnManager.instance.CachePool();
        ClientScene.AddPlayer(netMsg.conn, 0);
        MenuManager.instance.ShowMenu<MenuPickSelection>();
        (MenuManager.instance.CurrentMenu as MenuPickSelection).InitializeClientInfo();
#if (UNITY_IOS || UNITY_ANDROID)
                m_clientLevel += "Mobile";

#endif
        //TODO add mobile
        GameManager.instance.LoadWorldScene(m_clientLevel);
        GameManager.instance.StartCoroutine(GenerateGrid());
        m_cameraParticle.SetActive(false);
        ClientChat.instance.InitializeClientChat(myClient);

    }

    private IEnumerator GenerateGrid()
    {
        while (Pathfinding.instance == null)
        {
            yield return 0;
        }
        Pathfinding.instance.CreateGrid();
    }
}
