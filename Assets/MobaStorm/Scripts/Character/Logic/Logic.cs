using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

[NetworkSettings(channel = 0, sendInterval = 0.040f)]
public class Logic : NetworkBehaviour
{
    public bool Initialized = false;
    public virtual void Initialize(MobaEntity entity)
    {

    }
    public virtual void Process()
    {

    }
    //public virtual void OnStart()
    //{
    //    Initialized = true;
    //}
    //public abstract void OnInstantiate();
    
    public virtual void SetMobaEntity(GameObject target)
    {

    }

    public virtual MobaEntityData GetMobaEntityData()
    {
        return new MobaEntityData();
    }
   
    public virtual void OnFinish()
    {
        Initialized = false;
    }

    public virtual bool IsLocalPlayerAuthority()
    {
        return false;
    }

}
