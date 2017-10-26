using UnityEngine;
using System.Collections;
using System;

public class StopPathTask : Task {

    MobaEntity m_attacker;
    bool m_goToIdle;

    public StopPathTask(MobaEntity attacker, int time, bool goToIdle) : base(time)
    {
        m_attacker = attacker;
        m_goToIdle = goToIdle;
    }

    public override void Run()
    {
        m_attacker.StopAgent(m_goToIdle);
    }

}
