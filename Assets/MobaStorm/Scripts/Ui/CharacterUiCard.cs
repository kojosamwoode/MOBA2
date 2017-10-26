using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

[RequireComponent(typeof(Button))]
public class CharacterUiCard : MonoBehaviour {
	public Button charButton;
    MobaEntityData m_entityData;

    public MobaEntityData EntityData
    {
        get { return m_entityData; }
    }

    public Action<CharacterUiCard> onCardClicked;
	
	void Start ()
	{
        GetComponent<Button>().onClick.AddListener(OnClick);
	}

    public void SetCharacter(string entityIdentitifer)
    {
        m_entityData = GameDataManager.instance.GetEntityData(entityIdentitifer);
        Sprite sprite = SpriteDatabaseManager.instance.GetSprite(m_entityData.m_icon);
        charButton.GetComponent<Image>().sprite = sprite;
    }

	public void OnClick()
	{
	    if (NetworkClient.active)
        {
            if (onCardClicked != null)
            {
                onCardClicked(this);
            }
        }
	}

}
