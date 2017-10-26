using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UiStorePanel : MonoBehaviour {
    [Header("Ui Buttons")]
    [SerializeField]
    private List<ItemButton> m_itemButtons = new List<ItemButton>();


    [SerializeField]
    private ItemButton m_itemButtonPrefab;

    [SerializeField]
    private RectTransform m_itemContainer;

   
    private bool m_initialized = false;


    [SerializeField]
    private Button m_buyButton;

    [SerializeField]
    private Button m_sellButton;

    [SerializeField]
    private ItemButton m_detailedItem;

    [Header("Inventory View")]
    [SerializeField]
    private InventoryView m_inventoryView;

    private void Awake()
    {
        if (m_buyButton)
        {
            m_buyButton.onClick.AddListener(BuyClicked);
        }
        if (m_sellButton)
        {
            m_sellButton.onClick.AddListener(SellClicked);
        }
    }

    private void BuyClicked()
    {
        if (m_detailedItem.BaseItemContainer)
        {
            GameManager.instance.LocalPlayer.CharacterEntity.EntityInventory.TryPurchaseItem(m_detailedItem.BaseItemContainer);
        }
    }
    private void SellClicked()
    {
        if (m_detailedItem.BaseItemContainer)
        {
            GameManager.instance.LocalPlayer.CharacterEntity.EntityInventory.TrySellItem(m_detailedItem.Slot);
            m_detailedItem.gameObject.SetActive(false);
        }
    }

    public void ShowStore()
    {
        m_detailedItem.gameObject.SetActive(false);
        gameObject.SetActive(true);
        
    }

    // Use this for initialization
    public void Initialize(CharacterEntity entity)
    {
        if (!m_initialized)
        {
            foreach (BaseItemContainer item in InventoryManager.Instance.m_items)
            {
                ItemButton newButton = Instantiate<ItemButton>(m_itemButtonPrefab);
                newButton.transform.SetParent(m_itemContainer);
                newButton.UpdateItemContainer(item, ShowStoreItem);

            }
            if (m_inventoryView != null)
            {
                m_inventoryView.InitializeInventoryView(entity, ShowInventoryItem);
            }
            m_initialized = true;
        }
    }

    public void HideStore()
    {
        gameObject.SetActive(false);
    }

    private void ShowStoreItem(ItemButton itemButton)
    {
        if (itemButton.BaseItemContainer)
        {
            m_detailedItem.UpdateItemContainer(itemButton.BaseItemContainer);
            m_buyButton.gameObject.SetActive(true);
            m_sellButton.gameObject.SetActive(false);
            m_detailedItem.gameObject.SetActive(true);

            //GameManager.instance.LocalPlayer.CharacterEntity.EntityInventory.TryPurchaseItem(itemButton.InventoryItem.ItemContainer);
        }
    }

    private void ShowInventoryItem(ItemButton itemButton)
    {
        if (itemButton.InventoryItem != null)
        {
            m_detailedItem.UpdateInventoryItem(itemButton.InventoryItem);
            m_buyButton.gameObject.SetActive(false);
            m_sellButton.gameObject.SetActive(true);
            m_detailedItem.gameObject.SetActive(true);

            //GameManager.instance.LocalPlayer.CharacterEntity.EntityInventory.TryPurchaseItem(itemButton.InventoryItem.ItemContainer);
        }
    }
}
