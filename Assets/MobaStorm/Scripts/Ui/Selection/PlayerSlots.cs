using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerSlots : NetworkBehaviour {
	public Text NameBox;
	public Text CharNameBox;
	public Text StatusBox;
	public RectTransform NameBoxRect; 
	public Button slotButton;
	public Text stackTxt;
	public int m_slotNumber;
    MobaPlayer m_mobaPlayer;


	public bool m_isReady = false;

    [SyncVar(hook = "StatusChanged")]
    public string m_status;

    [SyncVar(hook = "CharacterChanged")]
    public string m_character = "";

    [SyncVar(hook = "NameChanged")]
    private string m_playerName = "";
    public string PlayerName
    {
        get { return m_playerName; }
        set
        {
            m_playerName = value;
            if (value == "")
            {
                NameBox.text = "Free Slot";
            }
            else
            {
                NameBox.text = m_playerName;
            }        
        }
    }

    public void NameChanged(string name)
    {
        PlayerName = name;
    }
    public void CharacterChanged(string characterIdentifier)
    {
        if (!string.IsNullOrEmpty(characterIdentifier))
        {
            MobaEntityData entityData = GameDataManager.instance.GetEntityData(characterIdentifier);
            Sprite sprite = SpriteDatabaseManager.instance.GetSprite(entityData.m_icon);
            slotButton.GetComponent<Image>().sprite = sprite;
            CharNameBox.text = entityData.m_dataDisplayName;
            m_character = characterIdentifier;
        }
        else
        {
            CharNameBox.text = "EmptySlot";
            Sprite sprite = SpriteDatabaseManager.instance.GetSprite("EmptySlot_Sprite");
            slotButton.GetComponent<Image>().sprite = sprite;
            m_character = "";
        }
    }

    public void StatusChanged(string status)
    {  
        StatusBox.text = status;
    }
	// Use this for initialization
	void Start () {  

        if (isClient)
        {
            CharacterChanged(m_character);
            NameChanged(m_playerName);
            StatusChanged(m_status);
        }
        else
        {
            //Debuggin purposes only
            CharacterChanged(m_character);
        }
	}
	
    public void RemovePlayerFromSlot()
    {    
        m_mobaPlayer = null;
        PlayerName = "";
        m_status = "";
        CharacterChanged("");
    }

    

	// Update is called once per frame
	public void SetPlayerToSlot (MobaPlayer mobaPlayer) {

        if (NetworkServer.active)
        {
            CharacterChanged(mobaPlayer.CharacterIdentifier);
            m_mobaPlayer = mobaPlayer;
            PlayerName = mobaPlayer.PlayerName;
            SetReadyStatus(false);
        }
        
	}
    public void SetReadyStatus(bool ready)
    {
        m_isReady = ready;
        if (ready)
        {
            m_status = "Ready"; 
        }
        else
        {
            m_status = "Picking";
        }
    }


    public bool IsEmpty
    {
        get
        {
            return m_mobaPlayer == null;
        }
    }
	

	public void OnClick()
	{
        if (isServer)
            return; 

        if (PlayerName == "")
        {
            MobaPlayer.Instance.CmdSlotClicked(m_slotNumber);
        }
	}

   
}
