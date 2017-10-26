using UnityEngine;
using System.Collections;

public class CustomAudioSource : MonoBehaviour {

    public enum ESoundType
    {
        Sound2d,
        Sound3dFollow,
        Sound3dStatic
    }
    private ESoundType m_soundType = ESoundType.Sound2d;
    protected AudioSource m_audioSource;
    protected bool m_active = false;
    private GameObject m_objToFollow;
	// Use this for initialization
	void Awake () {
        m_audioSource = GetComponent<AudioSource>();
	}

    void Update()
    {
        if (!m_active)
            return;

        switch (m_soundType)
        {
            case ESoundType.Sound2d:
                break;
            case ESoundType.Sound3dFollow:
                if (m_objToFollow)
                {
                    transform.position = m_objToFollow.transform.position;
                }
                break;
            case ESoundType.Sound3dStatic:
                break;
            default:
                break;
        }
    }
	
    public virtual void PlaySound(AudioClip audio, int volume, bool looping)
    {
        m_audioSource.loop = looping;
        m_audioSource.volume = (float)volume / 100;
        m_audioSource.clip = audio;
        m_audioSource.spatialBlend = 0;
        m_audioSource.Play();
        if (!looping)
            AudioManager.instance.StartCoroutine(SoundStopCoroutine(audio.length));
        m_active = true;
    }

    public virtual void PlaySound3dFollow(AudioClip audio, int volume, GameObject obj, bool looping)
    {
        m_audioSource.loop = looping;
        m_audioSource.volume = (float)volume / 100;
        m_audioSource.clip = audio;
        m_audioSource.spatialBlend = 1;
        m_audioSource.Play();
        if (!looping)
            AudioManager.instance.StartCoroutine(SoundStopCoroutine(audio.length));
        m_objToFollow = obj;
        m_active = true;
    }

    public virtual void PlaySound3dStatic(AudioClip audio, int volume, Vector3 pos, bool looping)
    {
        m_audioSource.loop = looping;
        m_audioSource.volume = (float)volume / 100;
        m_audioSource.clip = audio;
        m_audioSource.spatialBlend = 1;
        m_audioSource.Play();
        if (!looping)
            AudioManager.instance.StartCoroutine(SoundStopCoroutine(audio.length));
        m_active = true;
        transform.position = pos;
        
    }

    public void StopSound()
    {
        m_audioSource.Stop();
        AudioManager.instance.AudioSourceEndedCallback(this);
    }

    IEnumerator SoundStopCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        m_active = false;
        AudioManager.instance.AudioSourceEndedCallback(this);
    }
}
