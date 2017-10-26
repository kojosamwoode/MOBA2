using UnityEngine;
using System.Collections;

public abstract class Behaviour {
    protected Ability m_ability;
    public abstract void OnStart();
    public abstract void OnFinish();
    public abstract void Process();

    private bool m_isFinish;
    public bool IsFinish
    {
        get { return m_isFinish; }
        set { m_isFinish = value; }
    }

    public EAttackType BehaviourAttacktype
    {
        get
        {
            return m_ability.AttackType;
        }
    }

}
