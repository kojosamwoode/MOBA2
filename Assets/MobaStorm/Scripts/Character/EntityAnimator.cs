using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

public enum EEntityState
{
    Idle = 1,
    Run = 2,
    Basic = 3,
    AbilityQ = 4,
    AbilityW = 5,
    AbilityE = 6,
    AbilityR = 7,
    Dead = 8,
    Recall = 9,
    Emote = 10,
    CastingQ = 11,
    CastingW = 12,
    CastingE = 13,
    CastingR = 14,
    Stun = 15,
    Custom1 = 16,
    Custom2 = 17,
}
[NetworkSettings(channel = 0, sendInterval = 0.040f)]
public class EntityAnimator : NetworkBehaviour {

    [SerializeField][SyncVar]
    private EEntityState m_currentState = EEntityState.Idle;
    public EEntityState CurrentState { get { return m_currentState; } }

    private MobaEntity m_entity;
    [SerializeField]
    private Animation m_animation;

    [SerializeField]
    private Animator m_animator;
    public Action<EEntityState> OnAnimationStateChanged;

    private Dictionary<EEntityState, AnimationData> m_animationDataDict;

    private Dictionary<EEntityState, AnimationClip> m_animatorClipsDict;

    void Awake()
    {
        m_entity = GetComponent<MobaEntity>();
        if (m_animation == null)
        {
            m_animation = GetComponentInChildren<Animation>();
        }
        if (m_animator == null)
        {
            m_animator = GetComponentInChildren<Animator>();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        LoadAnimationData();
        ChangeState(EEntityState.Idle);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        LoadAnimationData();
        ClientAnimationChange(m_currentState);
    }

    public void LoadAnimationData()
    {
        if (m_animation == null && m_animator == null)
        {
            return;
        }

        //Load Legacy Data
        if (m_animation != null)
        {
            if (m_animationDataDict == null)
            {
                m_animationDataDict = new Dictionary<EEntityState, AnimationData>();
                //If using Legacy
                foreach (AnimationData animationData in m_entity.EntityData.m_animationDataList)
                {
                    if (!m_animationDataDict.ContainsKey(animationData.m_animation))
                    {
                        m_animationDataDict.Add(animationData.m_animation, animationData);
                    }
                    else
                    {
                        Debug.LogError("Error: Animation Data Duplicated: " + animationData.m_animation.ToString() + " In character: " + m_entity.DataIdentifier);
                    }
                }
            }
            return;
        }
        
        //Load Animator Data
        if (m_animator != null)
        {
            if (m_animatorClipsDict == null)
            {
                m_animatorClipsDict = new Dictionary<EEntityState, AnimationClip>();
            
                foreach (AnimationClip ac in m_animator.runtimeAnimatorController.animationClips)
                {
                    EEntityState animation = (EEntityState)Enum.Parse(typeof(EEntityState), ac.name);
                    // look at all the animation clips here!
                    if (!m_animatorClipsDict.ContainsKey(animation))
                    {
                        m_animatorClipsDict.Add(animation, ac);
                    }
                    else
                    {
                        Debug.LogError("Error: Animation Data Duplicated: " + ac.name + " In character: " + m_entity.DataIdentifier);
                    }
                }
            }
        }
       
    }

    public void ClientAnimationChange(EEntityState newState)
    {
        //If using legacy animation
        if (m_animation != null)
        {
            if (newState == EEntityState.Stun)
            {
                if (m_animation.isPlaying)
                {
                    m_animation.Stop();
                }
            }

            if (OnAnimationStateChanged != null)
            {
                OnAnimationStateChanged(newState);
            }
            if (newState == EEntityState.Run)
            {
                m_animation[newState.ToString()].speed = m_animationDataDict[newState].m_animationSpeed * (1 + (m_entity.SpeedMod / 10));
                m_animation.Play(newState.ToString());

            }
            else
            {
                AnimationClip clip = m_animation.GetClip(newState.ToString());
                if (clip != null)
                {
                    m_animation[newState.ToString()].speed = m_animationDataDict[newState].m_animationSpeed;
                    m_animation.Play(newState.ToString());
                }
            }
        }

        //If using Animator
        if (m_animator != null)
        {
            if (newState == EEntityState.Stun)
            {
                m_animator.StopPlayback();
            }

            if (OnAnimationStateChanged != null)
            {
                OnAnimationStateChanged(newState);
            }
            Debug.Log("Playing animation Mecanim " + newState);

            if (newState == EEntityState.Run)
            {
                m_animator.Play(newState.ToString());
                //m_animator.speed = (1 + m_entity.SpeedMod);
            }
            else
            {
                m_animator.Play(newState.ToString());
                //m_animator.speed = 1;
            }
        }

    }

    public void RefreshRunAnimationSpeed()
    {
        if (m_animation != null)
        {
            AnimationClip clip = m_animation.GetClip("Run");
            if (clip != null)
            {
                m_animation["Run"].speed = m_animationDataDict[EEntityState.Run].m_animationSpeed * (1 + (m_entity.SpeedMod / 10));
                Debug.Log("Final Speed" + m_animation["Run"].speed);
            }
        }
        if (m_animator != null)
        {
            m_animator.speed = (1 + (m_entity.SpeedMod / 10));
        }
    }

    public float ChangeState(EEntityState newState)
    {
        //Legacy Animation
        if (m_animation != null)
        {
            if (m_currentState != newState)
            {
                if (OnAnimationStateChanged != null)
                {
                    OnAnimationStateChanged(newState);
                }
                RpcAnimationChange((int)newState, NetworkTime.Instance.ServerStep());
                m_currentState = newState;
                AnimationClip clip = m_animation.GetClip(newState.ToString());
                if (clip != null)
                {
                    return clip.length / m_animationDataDict[newState].m_animationSpeed;
                }
                return 0;

            }
        }
        //Animator
        if (m_animator != null)
        {
            
            if (m_currentState != newState)
            {
                if (OnAnimationStateChanged != null)
                {
                    OnAnimationStateChanged(newState);
                }
                RpcAnimationChange((int)newState, NetworkTime.Instance.ServerStep());
                m_currentState = newState;
                m_animator.Play(newState.ToString());
                //m_animator.SetTrigger(newState.ToString());
                Debug.Log("Changing animation Trigger: " + newState);
                if (m_animatorClipsDict.ContainsKey(m_currentState))
                {
                    return m_animatorClipsDict[newState].length / m_animator.speed;
                }
            }
        }

        return 0;
    }


    [ClientRpc]
    public void RpcAnimationChange(int newState, int time)
    {
        m_entity.ClientAddEntityTask(new SetAnimationTask(m_entity, (EEntityState)newState, time));
    }
}
