using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class UiManager : MonoSingleton<UiManager> {

    [SerializeField]
    public UiLeftPanel m_leftPanel;

    [SerializeField]
    public UiBotPanel m_botPanel;

    [SerializeField]
    public UiTopPanel m_topPanel;

    [SerializeField]
    public UiStorePanel m_storePanel;

    public void LoadGameUI(CharacterEntity character)
    {
        //Set Ui Panels
        m_leftPanel.Initialize(character);
        m_botPanel.Initialize(character);
        m_topPanel.Initialize();
        m_storePanel.Initialize(character);

    }

    public void ShowStore()
    {
        m_storePanel.ShowStore();
    }

}
