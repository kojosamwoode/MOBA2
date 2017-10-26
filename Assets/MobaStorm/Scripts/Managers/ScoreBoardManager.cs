using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ScoreBoardManager : NetworkBehaviour {

    public static ScoreBoardManager Instance;

    [SyncVar(hook = "BlueMinionScoreChanged")]
    private int m_blueMinionScore;
    public int BlueMinionScore { get { return m_blueMinionScore; } set { m_blueMinionScore = value; } }
    public void BlueMinionScoreChanged(int score)
    {
        m_blueMinionScore = score;
        if (OnScoreChanged != null)
        {
            OnScoreChanged();
        }
    }

    [SyncVar(hook = "RedMinionScoreChanged")]
    private int m_redMinionScore;
    public int RedMinionScore { get { return m_redMinionScore; } set { m_redMinionScore = value; } }
    public void RedMinionScoreChanged(int score)
    {
        m_redMinionScore = score;
        if (OnScoreChanged != null)
        {
            OnScoreChanged();
        }
    }

    [SyncVar(hook = "BlueTowerScoreChanged")]
    private int m_blueTowerScore;
    public int BlueTowerScore { get { return m_blueTowerScore; } set { m_blueTowerScore = value; } }
    public void BlueTowerScoreChanged(int score)
    {
        m_blueTowerScore = score;
        if (OnScoreChanged != null)
        {
            OnScoreChanged();
        }
    }

    [SyncVar(hook = "RedTowerScoreChanged")]
    private int m_redTowerScore;
    public int RedTowerScore { get { return m_redTowerScore; } set { m_redTowerScore = value; } }
    public void RedTowerScoreChanged(int score)
    {
        m_redTowerScore = score;
        if (OnScoreChanged != null)
        {
            OnScoreChanged();
        }
    }

    public Action OnScoreChanged;
    // Use this for initialization
    void Awake () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
