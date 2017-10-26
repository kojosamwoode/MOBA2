using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EntityBehaviour : MonoBehaviour {

    public string m_currentBehaviourDebug;

    public enum EInsertOrder
    {
        Next,
        Last,
    }

    private MobaEntity m_entity;
    public MobaEntity Entity
    {
        get { return m_entity; }
        set { m_entity = value; }
    }

    private Behaviour m_currentBehaviour;
    public Behaviour CurrentBehaviour
    {
        get { return m_currentBehaviour; }
        set { m_currentBehaviour = value; }
    }

    public bool HasBehaviours
    {
        get { return m_currentBehaviour != null; }
    }

    void Awake()
    {
        m_entity = GetComponent<MobaEntity>();
    }

    void FixedUpdate()
    {
        if (m_currentBehaviour != null)
        {
            m_currentBehaviourDebug = m_currentBehaviour.GetType().ToString();
            if (m_currentBehaviour.IsFinish)
            {
                m_currentBehaviour.OnFinish();
                m_currentBehaviour = null;
                return;
            }
            else
            {
                m_currentBehaviour.Process();
            }
        }

    }

    public void AddBehaviour(Behaviour newBehaviour, EInsertOrder position = EInsertOrder.Last)
    {
        if (m_entity.IsStun)
            return;

        if (HasBehaviours && m_currentBehaviour.BehaviourAttacktype != EAttackType.Basic)
            return;

        m_currentBehaviour = newBehaviour;
        newBehaviour.OnStart();
    }



    public void StopAllBehaviours()
    {
        if (m_currentBehaviour != null)
        {
            m_currentBehaviour.OnFinish();
            m_currentBehaviour = null;
        }
    }
}
