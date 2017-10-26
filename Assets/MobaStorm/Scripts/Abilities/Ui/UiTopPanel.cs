using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UiTopPanel : MonoBehaviour {
    [Header("Timer")]
    [SerializeField]
    private Text m_gameTime;

    [Header("Score")]
    [SerializeField]
    private Text m_blueScore;

    [SerializeField]
    private Text m_redScore;

    [Header("Towers Destroyed")]
    [SerializeField]
    private Text m_blueTowerScore;

    [SerializeField]
    private Text m_redTowerScore;

    // Use this for initialization
    public void Initialize()
    {
        ScoreChanged();
        ScoreBoardManager.Instance.OnScoreChanged += ScoreChanged;
    }

    private void ScoreChanged()
    {
        m_blueScore.text = ScoreBoardManager.Instance.BlueMinionScore.ToString();
        m_redScore.text = ScoreBoardManager.Instance.RedMinionScore.ToString();
        m_blueTowerScore.text = ScoreBoardManager.Instance.BlueTowerScore.ToString();
        m_redTowerScore.text = ScoreBoardManager.Instance.RedTowerScore.ToString();
    }

    void Update()
    {

    }

	
}
