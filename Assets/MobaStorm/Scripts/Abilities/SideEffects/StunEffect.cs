using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StunEffect : SideEffect  {

    public StunEffect(StunEffect sideEffect) : base(sideEffect) 
    {
    }

    public override SideEffect Clone()
    {
        return new StunEffect(this);
    }


    public override void ApplyEffect(Ability ability, MobaEntity attacker, MobaEntity reciever)
    {
        base.ApplyEffect(ability, attacker, reciever);
        reciever.StunEntity(true);
    }

    public override void ProcessEffect()
    {
        base.ProcessEffect();
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        m_reciever.StunEntity(false);
    }

    public override int DrawEffect(int xPos, int yPos, GUISkin skin)
    {  
        yPos = base.DrawEffect(xPos, yPos, skin);
#if UNITY_EDITOR
       
#endif
        return yPos;
    }

   
}
