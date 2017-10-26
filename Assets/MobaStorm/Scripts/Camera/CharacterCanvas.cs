using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CharacterCanvas : MonoBehaviour {

	private Camera m_Camera;
    [SerializeField]
    private GameObject m_canvasInfoHolder;
    public GameObject CanvasInfoHolder
    {
        get { return m_canvasInfoHolder; }
    }
    [SerializeField]
    private Image m_health;
    [SerializeField]
    private Image m_healthBack;
    [SerializeField]
    private Image m_mana;
    [SerializeField]
    private Text m_lvlText;
    [SerializeField]
    private Text m_nameText;
    private int m_lvl;
    MobaEntity m_entity;
    private float m_healthBarLength;
    private float m_healthBarBackLength;
    private float m_manaBarLength;
    void Awake(){
        m_Camera = CameraController.instance.sources.currentCamera;
	}
	void Update()
	{
		transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
		                 m_Camera.transform.rotation * Vector3.up);
        if (m_healthBarLength < m_healthBarBackLength)
        {
            m_healthBarBackLength -= 0.4f * Time.deltaTime;
            m_healthBack.fillAmount = m_healthBarBackLength;
        }
        if (m_healthBarLength > m_healthBarBackLength)
        {
            m_healthBarBackLength = m_healthBarLength;
            m_healthBack.fillAmount = m_healthBarBackLength;
        }
	}

    public void Initialize(MobaEntity entity)
    {
        if (m_entity == null)
        {
            m_entity = entity;
            m_entity.OnDataChanged += UpdateCanvas;
        }
       
        m_canvasInfoHolder.SetActive(true);
        UpdateCanvas(m_entity);
    }


    public void UpdateCanvas(MobaEntity entity)
    {
        if (m_entity.Health <= 0)
        {
            m_canvasInfoHolder.SetActive(false);
        }
        else
        {
            m_canvasInfoHolder.SetActive(true);
        }
        if (m_nameText != null)
        {
            m_nameText.text = m_entity.DisplayName;
        }

        //update the text lvlText with the actual player level
        if (m_lvlText != null)
        {
            m_lvlText.text = m_entity.Level.ToString();
        }
        
        //update the health image fillamount with the actual player health
        if (m_health != null)
        {
            if (m_entity.Health <= 0)
            {
                m_healthBarLength = 0;
            }
            else
            {
                m_healthBarLength = (m_entity.Health / m_entity.HealthMax);
            }
            m_health.fillAmount = m_healthBarLength;
        }

        //update the mana image fillamount with the actual player mana
        if (m_mana != null)
        {
            if (m_entity.Mana <= 0)
            {
                m_manaBarLength = 0;
            }
            else
            {
                m_manaBarLength = (m_entity.Mana / m_entity.ManaMax);
            }
            m_mana.fillAmount = m_manaBarLength;
        }
    }
}
