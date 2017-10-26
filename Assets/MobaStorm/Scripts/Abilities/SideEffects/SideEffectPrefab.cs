using UnityEngine;
using System.Collections;

public class SideEffectPrefab  : MonoBehaviour{

    private string m_serverInstanceId;
    public string ServerInstanceId
    {
        get { return m_serverInstanceId; }
    }

    private CustomAudioSource m_audioSource;
    public CustomAudioSource AudioSource
    {
        get { return m_audioSource; }
    }

    public void Initialize(string serverInstanceId, CustomAudioSource audiosource = null)
    {
        m_serverInstanceId = serverInstanceId;
        m_audioSource = audiosource;
    }
	
}
