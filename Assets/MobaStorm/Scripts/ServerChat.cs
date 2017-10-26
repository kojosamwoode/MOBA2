using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

//
// Summary:
//     ///
//     User message types.
//     ///
public enum MessageType
{
    MessageChat = 80,
    //
    // Summary:
    //     ///
    //     Warning message.
    //     ///
    MessageConnected = 81,
    //
    // Summary:
    //     ///
    //     Error message.
    //     ///
    MessageDisconnected = 82
}

public class ChatMessage : MessageBase
{
    public string m_name;
    public string m_message;
}

public class ServerChat : MonoSingleton<ServerChat>
{
    public void InitializeServerChat()
    {
        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        NetworkServer.RegisterHandler((short)MessageType.MessageChat, OnMessageRecieved);
        NetworkServer.RegisterHandler((short)MessageType.MessageConnected, OnClientConnected);
        NetworkServer.RegisterHandler((short)MessageType.MessageDisconnected, OnClientDesconnected);
    }
    private void OnClientConnected(NetworkMessage netMsg)
    {
        ChatMessage packet = netMsg.ReadMessage<ChatMessage>();
        string newMessage = "Player " + packet.m_name + " Conected";
        packet.m_message = newMessage;
        NetworkServer.SendToAll((short)MessageType.MessageChat, packet);
    }

    private void OnClientDesconnected(NetworkMessage netMsg)
    {
        ChatMessage packet = netMsg.ReadMessage<ChatMessage>();
        string newMessage = "Player " + packet.m_name + " Disconnected";
        packet.m_message = newMessage;
        NetworkServer.SendToAll((short)MessageType.MessageChat, packet);
    }

    private void OnMessageRecieved(NetworkMessage netMsg)
    {
        ChatMessage packet = netMsg.ReadMessage<ChatMessage>();
        NetworkServer.SendToAll((short)MessageType.MessageChat, packet);
    }

   

}
