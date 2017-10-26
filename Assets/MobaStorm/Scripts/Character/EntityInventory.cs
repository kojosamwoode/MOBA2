using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class EntityInventory : NetworkBehaviour {

    public class InventorySlot
    {
        private int m_slot;
        public int SlotNumber
        {
            get { return m_slot; }
            set { m_slot = value; }
        }
        private InventoryItem m_item;
        public InventoryItem Item
        {
            get { return m_item; }
            set { m_item = value; }
        }
        public InventorySlot(int slot)
        {
            m_slot = slot;
        }
        public void AddItem(InventoryItem item)
        {
            m_item = item;
        }
        public void RemoveItem()
        {
            m_item = null;
        }

    }

    private CharacterEntity m_entity;
    public CharacterEntity Entity
    {
        get { return m_entity; }
        set { m_entity = value; }
    }

    private Currency m_currency;
    public Currency Currency
    {

        get
        {
            if (m_currency == null)
            {
                m_currency = new Currency(0);
            }
            return m_currency;
        }
        //set { m_currency = value; }
    }

    [SyncVar(hook = "hookSyncCurrency")]
    private int m_syncCurrency;


    void hookSyncCurrency(int syncCurrency)
    {
        m_syncCurrency = syncCurrency;
        m_currency.SetAmmount(syncCurrency);
    }

    private List<InventorySlot> m_inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> IntentorySlots
    {
        get { return m_inventorySlots; }
        set { m_inventorySlots = value; }
    }

    public delegate void OnInventoryUpdated(EntityInventory inventory);
    public OnInventoryUpdated onInventoryUpdated;

    public delegate void OnCurrencyUpdated(Currency currency);
    public OnCurrencyUpdated onCurrencyUpdated;

    private void Awake()
    {
        m_entity = GetComponent<CharacterEntity>();
        m_currency = new Currency(0);
        for (int i =0; i< GameDataManager.instance.GlobalConfig.m_maxItemSlots; i++)
        {
            m_inventorySlots.Add(new InventorySlot(i));
        }
    }

    public void Start()
    {
        //m_characterCurrency.Callback = CurrencyChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        m_currency.m_onCurrencyUpdated += ClientOnCurrencyUpdated;
        m_currency.SetAmmount(m_syncCurrency);

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        m_currency.m_onCurrencyUpdated += ServerOnCurrencyUpdated;
        m_currency.SetAmmount(500);
        m_syncCurrency = m_currency.Ammount;
    }
    [Server]
    private void ServerOnCurrencyUpdated(Currency currency)
    {
        m_syncCurrency = m_currency.Ammount;
        //RpcServerUpdateCurrency(currency.Ammount);
        if (onCurrencyUpdated != null)
        {
            onCurrencyUpdated(m_currency);
        }
    }

    [Client]
    private void ClientOnCurrencyUpdated(Currency currency)
    {
        if (onCurrencyUpdated != null)
        {
            onCurrencyUpdated(m_currency);
        }
    }

    [Client]
    public void ClientInventoryItemClicked(ItemButton itemButton)
    {
        if (itemButton.InventoryItem != null)
        {
            BaseItemContainer baseItem = itemButton.InventoryItem.ItemContainer;
            if (baseItem != null && baseItem.ItemUseType == BaseItemContainer.EItemUseType.Active)
            {
                if (baseItem.AbilityBase)
                {
                    CharacterLogic characterLogic = m_entity.EntityLogic as CharacterLogic;
                    EAttackType attackType = (EAttackType)Enum.Parse(typeof(EAttackType), "ItemSlot" + itemButton.Slot);
                    characterLogic.TryToCastAbilityOnClient(attackType, false);
                }
            }
            
        }
    }

    public bool HasSlots
    {
        get
        {
            foreach(InventorySlot itemSlot in m_inventorySlots)
            {
                if (itemSlot.Item == null)
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Client]
    public void TryPurchaseItem(BaseItemContainer itemContainer)
    {
        CmdServerPurchaseItem(itemContainer.Identifier);
    }

    [Client]
    public void TrySellItem(int slot)
    {
        CmdServerSellItem(slot);
    }

    [Server]
    public bool ServerTryRemoveItem(int slot)
    {
        if (slot < 0 || slot > m_inventorySlots.Count)
        {
            return false;
        }
        InventoryItem item = m_inventorySlots[slot].Item;
        if (item == null)
        {
            return false;
        }

        if (TryRemoveStackItem(item))
        {
            //m_currency.AddCurrency(item.ItemContainer.Cost.Ammount / 2);
            return true;
        }
        //Remove Ability from the character
        EAttackType attackType = (EAttackType)Enum.Parse(typeof(EAttackType), "ItemSlot" + item.Slot);
        m_entity.EntityAbilities.RemoveAbility(attackType);

        foreach (SideEffect effect in item.ActiveSideEffect)
        {
            m_entity.EntityAbilities.RemoveSideEffect(effect);
        }  

        m_inventorySlots[slot].RemoveItem();
        RpcUpdateSlot(slot, "", 0, 0);
        return true;
    }

    [Server]
    public void ItemUsed(EAttackType slot)
    {
        string slotString = slot.ToString().Replace("ItemSlot", "");
        //InventorySlot inventorySlot = GetInventorySlot(int.Parse(slotString));
        ServerTryRemoveItem(int.Parse(slotString));
    }

    private InventorySlot GetInventorySlot(int slot)
    {
        return m_inventorySlots[slot];
    }
   
    [Server]
    public bool ServerTryPurchaseItem(BaseItemContainer itemContainer)
    {
        InventoryItem stackedItem;
        if (!m_currency.CanAfford(itemContainer.Cost))
        {
            return false;
        }

        //Check if the we have the item in our inventory
        if (HasInventoryItem(itemContainer.Identifier, out stackedItem))
        {
            if (stackedItem.Stakeable)
            {
                if (TryStackItem(stackedItem))
                {
                    m_currency.SubstractCurrency(itemContainer.Cost);
                    return true;
                }
                else
                {
                    //TODO: Notify stack is full. Only one stack allowed. Cant add more stacked items.
                    return false;
                }
            }    
            else
            {
                return ServerTryAddNewInventoryItem(itemContainer);
            }     
        }
        else
        {
            return ServerTryAddNewInventoryItem(itemContainer);
        }
    }

    [Server]
    private bool ServerTryAddNewInventoryItem(BaseItemContainer itemContainer)
    {
        InventoryItem newItem = new InventoryItem(m_entity, itemContainer);
        if (ServerAddItemToIventory(newItem))
        {
            m_currency.SubstractCurrency(itemContainer.Cost);
            foreach (SideEffect effect in itemContainer.PassiveEffects)
            {
                //SideEffect cloneEffect = effect.Clone();
                SideEffect cloneEffect = m_entity.EntityAbilities.TryApplySideEffect(null, effect, m_entity, true);
                newItem.ActiveSideEffect.Add(cloneEffect);
            }
            return true;

        }
        else
        {
            //TODO: Notify No Stlots Available
            return false;
        }
    }


    [Server]
    private bool TryStackItem(InventoryItem item)
    {
        if (item.TryAddStack())
        {
            RpcUpdateSlot(item.Slot, item.ItemContainer.Identifier, item.Charges, item.Stacks);
            return true;
        }
        return false;
    }

    [Server]
    private bool TryRemoveStackItem(InventoryItem item)
    {
        if (item.TryRemoveStack())
        {
            RpcUpdateSlot(item.Slot, item.ItemContainer.Identifier, item.Charges, item.Stacks);
            return true;
        }
        return false;
    }

    [Server]
    public bool ServerAddItemToInventory(string baseItemIdentifier)
    {
        BaseItemContainer itemContainer = InventoryManager.Instance.GetItem(baseItemIdentifier);
        if (itemContainer)
        {
            return ServerAddItemToIventory(new InventoryItem(m_entity, itemContainer));
        }
        return false;
    }
    [Server]
    public bool ServerAddItemToIventory(InventoryItem item)
    {
        if (HasSlots)
        {
            int emptySlotIndex = GetFreeSlot();
            if (emptySlotIndex != -1)
            {
                item.Slot = emptySlotIndex;
                m_inventorySlots[emptySlotIndex].AddItem(item);
                if (item.ItemContainer.AbilityBase != null)
                {
                    EAttackType attackType = (EAttackType)Enum.Parse(typeof(EAttackType), "ItemSlot" + emptySlotIndex);
                    m_entity.EntityAbilities.AddAbility(attackType, item.ItemContainer.AbilityBase.Identifier, 1);
                }
                RpcUpdateSlot(item.Slot, item.ItemContainer.Identifier, item.Charges, item.Stacks);
                return true;
            }
        }
        return false;
    }

    [Server]
    public int GetFreeSlot()
    {
        for (int i = 0; i < m_inventorySlots.Count; i++)
        {
            if (m_inventorySlots[i].Item == null)
            {
                return m_inventorySlots[i].SlotNumber;
            }
        }
        
        return -1;
    }



    [Server]
    public bool HasInventoryItem(string itemIdentifier, out InventoryItem item)
    {
        item = null;
        //foreach(KeyValuePair<int, InventoryItem> key in m_inventoryItems)
        foreach(InventorySlot slot in m_inventorySlots)
        {
            if (slot.Item == null)
            {
                continue;
            }
            if (slot.Item.ItemContainer.Identifier == itemIdentifier)
            {
                item = slot.Item;
                return true;
            }          
        }

        return false;
    }
    

    [ClientRpc]
    public void RpcUpdateSlot(int slot, string itemIdentifier, int charges, int stacks)
    {
        if (!string.IsNullOrEmpty(itemIdentifier))
        {
            BaseItemContainer itemContainer = InventoryManager.Instance.GetItem(itemIdentifier);
            m_inventorySlots[slot].AddItem(new InventoryItem(m_entity, itemContainer));
            m_inventorySlots[slot].Item.Charges = charges;
            m_inventorySlots[slot].Item.Stacks = stacks;
            m_inventorySlots[slot].Item.Slot = slot;
            if (itemContainer.AbilityBase != null)
            {
                EAttackType attackType = (EAttackType)Enum.Parse(typeof(EAttackType), "ItemSlot" + slot);
                m_entity.EntityAbilities.AddAbility(attackType, itemContainer.AbilityBase.Identifier, 1);
            }
        }
        else
        {
            m_inventorySlots[slot].RemoveItem();
        }
       
        onInventoryUpdated(this);
    }

    //[ClientRpc]
    //public void RpcServerUpdateCurrency(int ammount)
    //{
    //    m_currency.SetAmmount(ammount);
    //}
    [Command]
    public void CmdServerPurchaseItem(string itemIdentifier)
    {
        BaseItemContainer itemContainer = InventoryManager.Instance.GetItem(itemIdentifier);
        if (itemContainer)
        {
            ServerTryPurchaseItem(itemContainer);
        }
    }

    [Command]
    public void CmdServerSellItem(int slot)
    {
        if (slot < 0 || slot > m_inventorySlots.Count)
        {
            return;
        }
        InventoryItem item = m_inventorySlots[slot].Item;
        BaseItemContainer itemContainer = item.ItemContainer;
        if (ServerTryRemoveItem(slot))
        {
            m_currency.AddCurrency(itemContainer.Cost.Ammount / 2);
        }
    }

    

}
