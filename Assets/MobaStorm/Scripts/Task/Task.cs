using UnityEngine;
using System.Collections;

public abstract class Task {

    private int m_step;
    public Task(int step)
    {
        m_step = step;
    }

    public abstract void Run();

    public bool CanRunTask()
    {      
        if (NetworkTime.Instance.GetServerFixedStep() >= m_step)
        {
            return true;
        }
        return false;
    }

}
