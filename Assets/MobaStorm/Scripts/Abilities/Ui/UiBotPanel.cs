using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UiBotPanel : MonoBehaviour {

    [Header("Button Holders")]
    [SerializeField]
    private Transform m_abilityButtonsHolder;
    private AbilityButton[] m_abilityButtons;

    private Dictionary<EAttackType, AbilityButton> m_abilityButtonsDic = new Dictionary<EAttackType, AbilityButton>();
    public Dictionary<EAttackType, AbilityButton> AbilityButtonsDic
    {
        get { return m_abilityButtonsDic; }
        set { m_abilityButtonsDic = value; }
    }

    [SerializeField]
    private Transform m_levelUpButtonsHolder;
    private LevelUpButton[] m_levelUpButtons;

    private Dictionary<EAttackType, LevelUpButton> m_levelUpButtonsDic = new Dictionary<EAttackType, LevelUpButton>();
    public Dictionary<EAttackType, LevelUpButton> LevelUpButtonsDic
    {
        get { return m_levelUpButtonsDic; }
        set { m_levelUpButtonsDic = value; }
    }

    [Header("Character Health / Mana")]
    [SerializeField]
    private Image m_healthImage;
    [SerializeField]
    private Text m_healthText;

    [SerializeField]
    private Image m_manaImage;
    [SerializeField]
    private Text m_manaText;

    CharacterEntity m_character;

    // Use this for initialization
    public void Initialize(CharacterEntity entity)
    {
        m_character = entity;
        //Setup and Cache all Ability Buttons to a dictionary
        m_abilityButtons = m_abilityButtonsHolder.GetComponentsInChildren<AbilityButton>(true);
        foreach (AbilityButton button in m_abilityButtons)
        {
            if (!m_abilityButtonsDic.ContainsKey(button.AttackType))
            {
                m_abilityButtonsDic.Add(button.AttackType, button);
                Ability ability = m_character.EntityAbilities.GetAbility(button.AttackType);
                if (ability != null)
                {
                    button.SetAbility(m_character, ability);
                }
            }
            else
            {
                Debug.LogError("Error Duplicated ability button attack type: " + button.AttackType);
            }
        }

        //Setup and Cache all Level Up buttons to a dictionary
        m_levelUpButtons = m_levelUpButtonsHolder.GetComponentsInChildren<LevelUpButton>(true);
        foreach (LevelUpButton button in m_levelUpButtons)
        {
            if (!m_levelUpButtonsDic.ContainsKey(button.AttackType))
            {
                m_levelUpButtonsDic.Add(button.AttackType, button);
                button.SetButonActive(false);
            }
            else
            {
                Debug.LogError("Error Duplicated ability button attack type: " + button.AttackType);
            }
        }

        CharacterDataChanged(entity);
        entity.OnDataChanged += CharacterDataChanged;
        m_character.EntityAbilities.NewAbilityPointCallback += ClientShowHideLevelUpButtons;


    }

    public void CharacterDataChanged(MobaEntity entity)
    {
        m_healthImage.fillAmount = (float)(entity.Health) / (float)(entity.HealthMax);
        m_manaImage.fillAmount = (float)(entity.Mana) / (float)(entity.ManaMax);
        m_healthText.text = (int)entity.Health + " / " + (int)entity.HealthMax;
        m_manaText.text = (int)entity.Mana + " / " + (int)entity.ManaMax;

    }


    public void ClientShowHideLevelUpButtons()
    {
        if (m_character.EntityAbilities.AvailablePoints > 0)
        {
            foreach (Ability ability in m_character.EntityAbilities.Abilities.Values)
            {
                if (!ability.CanLevelUp)
                    continue;
                ShowLevelUpButton(ability.AttackType);
            }
        }
        else
        {
            foreach (LevelUpButton button in m_levelUpButtons)
            {
                button.SetButonActive(false);
            }
        }
    }

    private void ShowLevelUpButton(EAttackType type)
    {
        Ability ability = m_character.EntityAbilities.GetAbility(type);

        if (ability != null)
        {
            if (m_levelUpButtonsDic.ContainsKey(type))
            {
                m_levelUpButtonsDic[type].SetButonActive(ability.CanLevelUp);
            }
            else
            {
                Debug.LogError("Error: level up button not exist: " + type);
            }
        }
        else
        {
            if (m_levelUpButtonsDic.ContainsKey(type))
            {
                m_levelUpButtonsDic[type].SetButonActive(false);
            }
            Debug.LogError("Error: Ability doesnt exist: " + type);
        }
    }

 
}
