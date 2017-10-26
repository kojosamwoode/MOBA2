using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        //ClientScene.AddPlayer(conn,0);
        Debug.Log("------------Server Ready------------");
    }
    
    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("------------Client Ready------------");
    }
	 //Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
