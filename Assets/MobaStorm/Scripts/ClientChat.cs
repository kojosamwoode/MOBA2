using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ClientChat : MonoSingleton<ClientChat> {

	//public string chatText;
	public Text chatBoxText;
	public InputField ChatInputField;
	private string[] colorNames = new string[] {"cyan", "green", "lightblue","olive", "orange", "purple","red", "silver", "teal","yellow"};
	private string userNameColor;
    private NetworkClient m_client;

    public bool IsInputFieldOnFocus
    {
        get
        {
            return ChatInputField.GetComponent<InputField>().isFocused;
        }

    }

    // Use this for initialization
    void Start () {
        userNameColor = colorNames[UnityEngine.Random.Range(0, colorNames.Length)];

    }

    public void InitializeClientChat(NetworkClient client)
    {
        m_client = client;
        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        m_client.RegisterHandler((short)MessageType.MessageChat, OnMessageRecieved);
        
    }

    private void OnMessageRecieved(NetworkMessage netMsg)
    {
        ChatMessage chatMessage = netMsg.ReadMessage<ChatMessage>();
        ShowIncommingMessage(chatMessage.m_message);
    }

    public void ShowIncommingMessage(string text)
	{
        chatBoxText.text += text;
    }

	// Update is called once per frame
	void Update () {
			if (Input.GetKeyDown(KeyCode.Return) && IsInputFieldOnFocus)
			{
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.m_message = "<color=" + userNameColor + ">" + GameManager.instance.LocalPlayer.PlayerName + " -</color> " + ChatInputField.text;
                m_client.Send((short)MessageType.MessageChat, chatMessage);
				ChatInputField.text = ChatInputField.text.Remove(ChatInputField.text.Length - 1);
                ChatInputField.text = "";
			}
		
	}


}
