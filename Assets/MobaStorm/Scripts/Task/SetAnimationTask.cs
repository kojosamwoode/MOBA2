using UnityEngine;
using System.Collections;
using System;

public class SetAnimationTask : Task {

    Vector3[] m_pathArray;
    MobaEntity m_entity;
    EEntityState m_animation;

    public SetAnimationTask(MobaEntity entity, EEntityState animation, int time) : base(time)
    {
        m_entity = entity;
        m_animation = animation;
    }

    public override void Run()
    {
        m_entity.EntityAnimator.ClientAnimationChange(m_animation);
    }

}
