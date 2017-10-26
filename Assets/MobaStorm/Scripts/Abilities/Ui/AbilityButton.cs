using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class AbilityButton : MonoBehaviour {

    [SerializeField]
    private EAttackType m_attackType;
    public EAttackType AttackType
    {
        get { return m_attackType; }
    }

    private RectTransform m_rect;

    private Button m_button;
    [SerializeField]
    private Image m_cdrImage;
    [SerializeField]
    private Text m_levelText;

    private Ability m_ability;

    void Awake()
    {
        m_button = GetComponent<Button>();
        m_rect = GetComponent<RectTransform>();
    }

    public void SetAbility(CharacterEntity character, Ability ability)
    {
        m_ability = ability;
        m_button.image.sprite = m_ability.AbilityBase.Icon;
        if (character.isClient && character == GameManager.instance.LocalPlayer.CharacterEntity)
        {
            m_ability.m_onDataUpdated += UpdateAbility;
        }
        UpdateAbility(ability);
    }

   

    public void OnClick()
    {
        GameManager.instance.LocalPlayer.CharacterLogic.AbilityButtonClicked(this);
    }

    public void ShowToolTip()
    {
        if (m_ability != null)
        GameEvents.Instance.ShowToolTip(transform, m_ability.AbilityBase.AbilityName, m_ability.AbilityBase.GetAbilityDescription(), 0, m_rect.transform.localPosition.x + 100, m_rect.transform.localPosition.y + 200);

    }

    private void UpdateAbility(Ability ability)
    {

        UpdateAbilityLevel(ability);
        UpdateCdrVisuals(ability);
        
    }
    private void UpdateAbilityLevel(Ability ability)
    {
        if (m_levelText)
        {
            m_levelText.text = ability.Level.ToString();
        }
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

    

    public void HideToolTip()
    {
        if (m_ability != null)
            GameEvents.Instance.HideToolTip();

    }
}
