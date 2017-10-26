using UnityEngine;
using System.Collections;
using System;

public class Particle : MonoBehaviour, IPooled{

    [SerializeField]
    private bool m_followEntity;
    [SerializeField]
    private float m_timeToDestroy = 2;

    private float m_currentTime = 0;
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

    private MobaEntity m_entity;
    public MobaEntity Entity
    {
        get { return m_entity; }
        set { m_entity = value; }
    }
    private bool m_isNetworkedParticle;

    public void Initialize(string serverInstanceId, MobaEntity entity ,CustomAudioSource audiosource = null, bool isNetworkParticle = false)
    {
        m_isNetworkedParticle = isNetworkParticle;
        m_entity = entity;
        m_serverInstanceId = serverInstanceId;
        m_audioSource = audiosource;
        if (m_followEntity)
        {
            transform.SetParent(m_entity.transform);
        }
    }

    public void OnInstantiate()
    {
        m_currentTime = m_timeToDestroy;
    }

    public void OnUnSpawn()
    {
        
    }

    void Update()
    {

        m_currentTime -= Time.deltaTime;
        if (m_currentTime <= 0)
        {
            if (m_isNetworkedParticle && m_entity.isServer)
            {
                ParticleManager.Instance.ServerUnspawnParticle(this);
            }
            else
            {
                ParticleManager.Instance.ClientUnspawnParticle(this);
            }
        }
    }
}
