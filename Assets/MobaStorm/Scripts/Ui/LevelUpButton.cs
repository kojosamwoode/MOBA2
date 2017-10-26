using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LevelUpButton : MonoBehaviour {

    [SerializeField]
    private EAttackType m_attackType;
    public EAttackType AttackType
    {
        get { return m_attackType; }
        set { m_attackType = value; }
    }

    Button m_button;

    public Action<LevelUpButton> OnLevelUpClicked;

    void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClick);
    }

    public void SetButonActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnClick()
    {
        if (OnLevelUpClicked != null)
        {
            OnLevelUpClicked(this);
        }
        GameManager.instance.LocalPlayer.ClientLevelUpAbility(m_attackType);
    }


}
