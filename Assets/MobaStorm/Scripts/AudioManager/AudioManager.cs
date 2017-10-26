using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AudioManager : NetworkBehaviour {
    [SerializeField]
    private AudioClip[] m_audioClips;

    private static AudioManager m_instance;
    public static AudioManager instance
    {
        get { return m_instance; }
    }

    [SerializeField]
    private CustomAudioSource m_audioSourcePrefab;
    private Queue<CustomAudioSource> m_audioSourcesQueue = new Queue<CustomAudioSource>();

    private Dictionary<string, AudioClip> m_audioClipsDict = new Dictionary<string, AudioClip>();

    private CustomAudioSource m_musicAudiosource;
    public CustomAudioSource MusicAudioSource
    {
        get { return m_musicAudiosource; }
        set { m_musicAudiosource = value; }
    }


    // Use this for initialization
    void Awake() {
        if (m_instance == null)
        {
            m_instance = this;
        }
        for (int i = 0; i < 20; i++)
        {
            CreateAudioSource();
        }
        foreach (AudioClip audio in m_audioClips)
        {
            m_audioClipsDict.Add(audio.name, audio);
        }

    }

    void CreateAudioSource()
    {
        CustomAudioSource source = Instantiate(m_audioSourcePrefab, transform.position, Quaternion.identity) as CustomAudioSource;
        m_audioSourcesQueue.Enqueue(source);
        source.transform.SetParent(transform);
    }

    public CustomAudioSource Play2dSound(string soundKey, int volume, bool looping = false)
    {
        if (m_audioClipsDict.ContainsKey(soundKey))
        {
            if (m_audioSourcesQueue.Count > 0)
            {
                CustomAudioSource audioSource = m_audioSourcesQueue.Dequeue();
                audioSource.PlaySound(m_audioClipsDict[soundKey], volume, looping);
                return audioSource;
            }
        }
        else
        {
            Debug.Log("Sound key not found: " + soundKey);
        }
        return null;
    }
    public CustomAudioSource Play3dSound(string soundKey, int volume,  GameObject sourceObj, bool looping = false)
    {
        if (m_audioClipsDict.ContainsKey(soundKey))
        {
            if (m_audioSourcesQueue.Count > 0)
            {
                CustomAudioSource audioSource = m_audioSourcesQueue.Dequeue();
                audioSource.PlaySound3dFollow(m_audioClipsDict[soundKey], volume, sourceObj, looping);
                return audioSource;
            }
        }
        else
        {
            Debug.Log("Sound key not found: " + soundKey);
        }

        return null;
    }

    public CustomAudioSource Play3dSound(string soundKey, int volume, Vector3 position, bool looping = false)
    {
        if (m_audioClipsDict.ContainsKey(soundKey))
        {
            if (m_audioSourcesQueue.Count > 0)
            {
                CustomAudioSource audioSource = m_audioSourcesQueue.Dequeue();
                audioSource.PlaySound3dStatic(m_audioClipsDict[soundKey], volume, position, looping);
                return audioSource;
            }
        }
        else
        {
            Debug.Log("Sound key not found: " + soundKey);
        }

        return null;
    }

    public void AudioSourceEndedCallback(CustomAudioSource audioSource)
    {
        m_audioSourcesQueue.Enqueue(audioSource);
    }
    [Server]
    public void ServerPlay3dSound(string soundKey, int volume, Vector3 position)
    {
        RpcPlay3dSound(soundKey, volume, position);
    }

    [ClientRpc]
    public void RpcPlay3dSound(string soundKey, int volume, Vector3 position)
    {
        Play3dSound(soundKey, volume, position);
    }



}
