using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;
[System.Serializable]
public class NexusLogic : Logic {

    private AIEntity m_entity;

    [SyncVar]
    private float m_rotationX;
    [SyncVar]
    private float m_rotationY;
    [SyncVar]
    private float m_rotationZ;
  

    public enum IaState
    {
        idle,
        dead
    }

    public override void Initialize(MobaEntity entity)
    {
        m_entity = entity as AIEntity;
        if (m_entity.isServer)
        {
            //NexusData iaDef = m_entity.EntityData as NexusData;
            m_rotationX = transform.rotation.eulerAngles.x;
            m_rotationY = transform.rotation.eulerAngles.y;
            m_rotationZ = transform.rotation.eulerAngles.z;
            Initialized = true;
            m_entity.OnDeathCallBack += NexusDestroyed;
        }
        else
        {
            transform.eulerAngles = new Vector3(m_rotationX, m_rotationY, m_rotationZ);
        }
    }



    private void NexusDestroyed(DamageProcess damageProcess)
    {
        GameManager.instance.NexusDestroyed(m_entity);
        RpcNexusDestroyed();   
    }

    public override void Process()
    {     

    }

    

    public override void OnFinish()
    {
        base.OnFinish();
    }

    [ClientRpc]
    public void RpcNexusDestroyed()
    {
        GameManager.instance.NexusDestroyed(m_entity);
        
        
    }

    public override void SetMobaEntity(GameObject target)
    {
        base.SetMobaEntity(target);
        target.AddComponent<AIEntity>();
    }

    public override MobaEntityData GetMobaEntityData()
    {
        return new NexusData();
    }
}
