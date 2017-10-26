using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class UiLeftPanel : MonoBehaviour {
    [Header("Portraits")]
    [SerializeField]
    private Image m_charPortraitImage;

    [Header("Experience Bars")]
    [SerializeField]
    private Image m_expBar;
    [SerializeField]
    private Text m_expText;

    [Header("Character Data")]
    [SerializeField]
    private Text m_baseAdText;
    [SerializeField]
    private Text m_baseApText;
    [SerializeField]
    private Text m_armorText;
    [SerializeField]
    private Text m_magicResText;
    [SerializeField]
    private Text m_speedText;

    [SerializeField]
    private Text m_goldText;

    [Header("Inventory View")]
    [SerializeField]
    private InventoryView m_inventoryView;
    

    // Use this for initialization
    public void Initialize(CharacterEntity entity)
    {
        SetCharacterPortrait(entity.Icon);
        CharacterDataChanged(entity);
        entity.OnDataChanged += CharacterDataChanged;
        entity.EntityInventory.onCurrencyUpdated += CurrencyChanged;
        if (m_inventoryView != null)
        {
            m_inventoryView.InitializeInventoryView(entity, entity.EntityInventory.ClientInventoryItemClicked);
        }
        CurrencyChanged(entity.EntityInventory.Currency);
    }


    private void SetCharacterPortrait(Sprite sprite)
    {
        m_charPortraitImage.sprite = sprite;
    }

    public void CharacterDataChanged(MobaEntity entity)
    {
        m_expBar.fillAmount = (float)(entity.CurrentExperience - entity.CurentLevelExp) / (float)(entity.NextLevelExp - entity.CurentLevelExp);
        m_expText.text = (entity.CurrentExperience - entity.CurentLevelExp) + " / " + (entity.NextLevelExp - entity.CurentLevelExp);
        m_baseAdText.text = ((int)entity.EntityAbilities.BaseAttackDamage).ToString() + " + "+ ((int)entity.EntityAbilities.AttackDamageMod).ToString();
        m_baseApText.text = ((int)entity.EntityAbilities.BaseMagicDamage).ToString() + " + " + ((int)entity.EntityAbilities.MagicDamageMod).ToString();
        m_armorText.text = ((int)entity.EntityAbilities.Armor).ToString() + " + " + ((int)entity.EntityAbilities.ArmorMod).ToString() + " % " + ((int)entity.EntityAbilities.ArmorResPercentMod).ToString();
        m_magicResText.text = ((int)entity.EntityAbilities.MagicRes).ToString() + " + " + ((int)entity.EntityAbilities.MagicResMod).ToString() + " % " + ((int)entity.EntityAbilities.MagicResPercentMod).ToString();
        m_speedText.text = ((int)entity.Speed).ToString();

    }

    public void CurrencyChanged(Currency currency)
    {
        if (m_goldText)
        {
            m_goldText.text = currency.Ammount.ToString();
        }
    }
	
}
