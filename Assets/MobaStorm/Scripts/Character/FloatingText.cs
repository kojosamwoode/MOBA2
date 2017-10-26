using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FloatingText : MonoBehaviour ,IPooled {
    public enum FloatingTextType
    {
        GenericFloatingText,
        DamageFloatingText,
        GoldFloatingText,
        SideEffectFloatingText,
        HealFloatingText
    }

    [SerializeField]
    private Text m_text;

    public Color m_damageColor;
    public Color m_goldColor;
    public Color m_sideEffectColor;
    public Color m_healEffectColor;

    private float m_time;
    // Use this for initialization
    public void ShowFloatingText(FloatingTextType type, string text) {
        m_text.text = text;
        switch (type)
        {
            case FloatingTextType.GenericFloatingText:
                m_text.color = Color.white;
                break;
            case FloatingTextType.DamageFloatingText:
                m_text.color = m_damageColor;
                break;
            case FloatingTextType.GoldFloatingText:
                m_text.color = m_goldColor;
                break;
            case FloatingTextType.SideEffectFloatingText:
                m_text.color = m_sideEffectColor;
                break;
            case FloatingTextType.HealFloatingText:
                m_text.color = m_healEffectColor;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (Time.time > m_time)
        {
            SpawnManager.instance.DestroyPool(gameObject);
        }
    }

    public void OnInstantiate()
    {
        m_time = Time.time + 1.5f;
    }

    public void OnUnSpawn()
    {
        
    }
}
