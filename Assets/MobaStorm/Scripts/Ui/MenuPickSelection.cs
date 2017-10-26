using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using System;

public class MenuPickSelection : Menu {

    [SerializeField]
    private CharacterUiCard m_characterUiCard;
    [SerializeField]
    private GameObject m_characterPickContainer;

    [SerializeField]
    private ReadyButton m_readyButton;

    [SerializeField]
    private GameObject m_serverRunningObj;

    List<PlayerSlots> m_slots = new List<PlayerSlots>();

	// Use this for initialization
	void Start () {
        
    }


    public override void OpenMenu()
    {
        base.OpenMenu();
        
        

    }

    public void InitializeServerInfo()
    {
        InitializeSlots();
        m_serverRunningObj.gameObject.SetActive(true);
    }

    public void InitializeClientInfo()
    {
        InitializeSlots();
        m_readyButton.gameObject.SetActive(true);
    }

    private void InitializeSlots()
    {
        //Initialize playerSlots
        PlayerSlots[] slots = GetComponentsInChildren<PlayerSlots>(true);
        m_slots = new List<PlayerSlots>(slots.OrderBy(x => x.m_slotNumber));

        //Load available characters
        foreach (string character in CharacterData.m_characterList)
        {
            CharacterUiCard card = Instantiate(m_characterUiCard) as CharacterUiCard;
            card.transform.SetParent(m_characterPickContainer.transform);
            card.SetCharacter(character);
            card.onCardClicked += CharacterCardClicked;     
        }
    }

    #region CLIENT METHODS
    public void CharacterCardClicked(CharacterUiCard card)
    {
        if (NetworkServer.active)
        {
            return;
        }
        MobaPlayer.Instance.CmdCharacterClicked(card.EntityData.m_dataIdentifier);

    }
    #endregion

    #region SERVER METHODS

    /// <summary>
    /// Method used to add a player to this available slot
    /// </summary>
    /// <param name="mobaPlayer"> Current player added to this slot</param>
    /// <returns></returns>
    public PlayerSlots AssignPlayerToFreeSlot(MobaPlayer mobaPlayer)
    {
        foreach (PlayerSlots slot in m_slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetPlayerToSlot(mobaPlayer);
                return slot;
            }
        }
        Debug.LogError("Error: Cant find a slot for this player!" + mobaPlayer);
        return null;
    }
    internal void SlotClickedOnClient(MobaPlayer mobaPlayer, int slotClicked)
    {
        if (mobaPlayer.IsReady)
        {
            return;
        }
        if (m_slots[slotClicked].IsEmpty)
        {
            mobaPlayer.PickSlot.RemovePlayerFromSlot();
            mobaPlayer.PickSlot = m_slots[slotClicked];
            m_slots[slotClicked].SetPlayerToSlot(mobaPlayer);
        }
    }

    internal void CharacterClickedOnClient(MobaPlayer mobaPlayer, string characterIdentifier)
    {
        if (mobaPlayer.IsReady)
        {
            return;
        }
        mobaPlayer.CharacterIdentifier = characterIdentifier;
        mobaPlayer.PickSlot.CharacterChanged(characterIdentifier);
    }

    //Called on the server when a player clicks on the ready button
    [Server]
    internal void ReadyClickedOnClient(MobaPlayer mobaPlayer)
    {
        if (mobaPlayer.IsReady)
        {
            return;
        }
        mobaPlayer.SetPlayerReady();
        mobaPlayer.PickSlot.SetReadyStatus(true);
    }
    #endregion
}
