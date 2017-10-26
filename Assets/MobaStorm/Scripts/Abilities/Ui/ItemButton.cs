using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour {

    private InventoryItem m_item;
    public InventoryItem InventoryItem
    {
        get { return m_item; }
    }

    private BaseItemContainer m_baseItemContainer;
    public BaseItemContainer BaseItemContainer
    {
        get { return m_baseItemContainer; }
    }

    [SerializeField]
    private int m_slot;
    public int Slot
    {
        get { return m_slot; }
        set { m_slot = value; }
    }
    [SerializeField]
    private Sprite m_emptySlotIcon;

    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private Text m_cost;

    [SerializeField]
    private Text m_name;

    [SerializeField]
    private Text m_stacks;

    [SerializeField]
    private Text m_description;

    [SerializeField]
    private Image m_cdrImage;

    [SerializeField]
    private RectTransform m_requerimentsContainer;

    private List<ItemButton> m_requerimentButtons = new List<ItemButton>();
    public List<ItemButton> RequerimentButtons
    {
        get { return m_requerimentButtons; }
        set { m_requerimentButtons = value; }
    }

    private Action<ItemButton> m_onItemClicked;

    private Ability m_ability;

    private RectTransform m_rect;


    private void Awake()
    {
        m_rect = GetComponent<RectTransform>();
        Button button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(ItemClicked);
        }

        if (m_stacks)
        {
            m_stacks.gameObject.SetActive(false);
        }
    }

    public void UpdateInventoryItem(InventoryItem item,  Action<ItemButton> onItemClicked = null)
    {
      
        if (item != null)
        {
            //If the ability is active subscribe to update CDR
            if (item.ItemContainer.AbilityBase)
            {
                EAttackType attackType = (EAttackType)Enum.Parse(typeof(EAttackType), "ItemSlot" + item.Slot);
                Ability ability = item.Entity.EntityAbilities.GetAbility(attackType);
                if (ability != null)
                {
                    ability.m_onDataUpdated += UpdateAbility;
                    m_ability = ability;
                }
                else
                {
                    Debug.LogError("Error. Ability is null??");
                }

            }

            UpdateItemContainer(item.ItemContainer, onItemClicked);
            if (m_stacks)
            {
                m_stacks.text = item.Stacks.ToString();
                if (item.Stacks == 1)
                {
                    m_stacks.gameObject.SetActive(false);
                }
                else
                {
                    m_stacks.gameObject.SetActive(true);
                }
            }
            m_slot = item.Slot;
        }
        else
        {
            if (m_ability != null)
            {
                m_ability.m_onDataUpdated -= UpdateAbility;
                m_cdrImage.fillAmount = 0;
                m_ability.MobaEntity.EntityAbilities.RemoveAbility(m_ability.AttackType);
            }
            UpdateItemContainer(null, onItemClicked);

            if (m_stacks)
            {
                m_stacks.gameObject.SetActive(false);
            }
        }
        m_item = item;
    }


    public void UpdateItemContainer(BaseItemContainer item, Action<ItemButton> onItemClicked = null)
    {
        m_baseItemContainer = item;
        m_onItemClicked = onItemClicked;

        if (item == null)
        {
            m_icon.sprite = m_emptySlotIcon;
            return;
        }
        if (m_name)
        {
            m_name.text = item.Name;
        }
        if (m_cost)
        {
            m_cost.text = item.Cost.Ammount.ToString();
        }
        if (m_icon)
        {
            m_icon.sprite = item.Icon;
        }
        
        if (m_description)
        {
            m_description.text = item.Description.Replace("NEWL", "\n");
        }
        if (m_requerimentsContainer)
        {
            foreach (BaseItemContainer current in item.PurchaseRequeriment)
            {
                GameObject newButton = SpawnManager.instance.InstantiatePool("ItemSlot", Vector3.zero, Quaternion.identity);
                if (newButton)
                {
                    ItemButton button = newButton.GetComponent<ItemButton>();
                    button.UpdateItemContainer(current);
                    m_requerimentButtons.Add(button);
                    button.transform.SetParent(m_requerimentsContainer);
                }
            }
        }    
    }

    private void UpdateAbility(Ability ability)
    {
        UpdateCdrVisuals(ability);
    }

    private void UpdateCdrVisuals(Ability ability)
    {
        float fillValue = ability.CoolDown / ability.CoolDownTotal;
        float timeInSeconds = ability.CoolDown * ability.CoolDownMultiplier;

        if (m_cdrImage)
        {
            m_cdrImage.fillAmount = fillValue;
        }
    }


    public void ItemClicked()
    {
        if (m_onItemClicked != null)
        {
            m_onItemClicked(this);
        }
    }

    public void ShowToolTip()
    {
        if (m_baseItemContainer != null)
        {
            GameEvents.Instance.ShowToolTip(transform, m_baseItemContainer.Name, m_baseItemContainer.Description, 0, m_rect.transform.localPosition.x + 100, m_rect.transform.localPosition.y + 200);
        }
    }

    public void HideToolTip()
    {
        GameEvents.Instance.HideToolTip();
    }

}
