using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryView
{

    [Header("Item Container")]
    [SerializeField]
    private RectTransform m_itemContainer;


    [Header("UI ItemButton Prefab")]
    [SerializeField]
    private ItemButton m_buttonPrefab;

    private CharacterEntity m_entity;



    private Dictionary<int, ItemButton> m_buttonsDict = new Dictionary<int, ItemButton>();

    public Dictionary<int, ItemButton> ButtonsDict
    {
        get { return m_buttonsDict; }
        set { m_buttonsDict = value; }
    }

    private Action<ItemButton> m_onItemButtonClicked;

    public void InitializeInventoryView(CharacterEntity entity, Action<ItemButton> onItemButtonClicked = null)
    {
        m_entity = entity;
        entity.EntityInventory.onInventoryUpdated += UpdateInventory;
        m_onItemButtonClicked = onItemButtonClicked;
        for (int i = 0; i < GameDataManager.instance.GlobalConfig.m_maxItemSlots; i++)
        {
            ItemButton newButton = GameObject.Instantiate<ItemButton>(m_buttonPrefab);
            newButton.transform.SetParent(m_itemContainer);
            newButton.Slot = i;
            newButton.UpdateItemContainer(null);
            m_buttonsDict.Add(i, newButton);
        }
    }

    public void UpdateInventory(EntityInventory inventory)
    {
        foreach (EntityInventory.InventorySlot itemSlot in inventory.IntentorySlots)
        {
            m_buttonsDict[itemSlot.SlotNumber].UpdateInventoryItem(itemSlot.Item, ItemClicked);
        }
    }
    private void ItemClicked(ItemButton itemButton)
    {
        if (m_onItemButtonClicked != null)
        {
            m_onItemButtonClicked(itemButton);
        }
    }
}