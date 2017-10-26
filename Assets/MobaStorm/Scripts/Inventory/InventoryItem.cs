using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem {

    private BaseItemContainer m_itemContainer;
    public BaseItemContainer ItemContainer
    {
        get { return m_itemContainer; }
        //set { m_itemContainer = value; }
    }

    private int m_stacks = 0;
    public int Stacks
    {
        get { return m_stacks; }
        set { m_stacks = value; }
    }

    private int m_charges;
    public int Charges
    {
        get { return m_charges; }
        set { m_charges = value; }
    }

    private int m_slot;
    public int Slot
    {
        get { return m_slot; }
        set { m_slot = value; }
    }

    private CharacterEntity m_entity;
    public CharacterEntity Entity
    {
        get { return m_entity; }
    }

    private List<SideEffect> m_activeSideEffect;
    public List<SideEffect> ActiveSideEffect
    {
        get { return m_activeSideEffect; }
        set { m_activeSideEffect = value; }
    }



    public InventoryItem(CharacterEntity entity, BaseItemContainer itemContainer)
    {
        m_activeSideEffect = new List<SideEffect>();
        m_entity = entity;
        m_itemContainer = itemContainer;
        m_charges = itemContainer.Charges;
        m_stacks = 1;
    }

    public bool Stakeable
    {
        get { return m_itemContainer.MaxStacks > 1; }
    }

    
    public bool TryAddStack()
    {
        if (m_stacks < m_itemContainer.MaxStacks)
        {
            m_stacks++;
            return true;
        }
        return false;
    }
    public bool TryRemoveStack()
    {
        if (m_stacks > 1)
        {
            m_stacks--;
            return true;
        }
        return false;
    }

}
