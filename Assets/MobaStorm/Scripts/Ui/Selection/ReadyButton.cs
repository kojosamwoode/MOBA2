using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class ReadyButton : MonoBehaviour {
    public static ReadyButton instance;
    Button m_button;
	void Start () {
        instance = this;
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClick);
	}

    private void OnClick()
    {
        MobaPlayer.Instance.CmdReadyClicked();
    }

    public void SetButtonActive(bool active)
    {
        m_button.interactable = active;
    }
}
