using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System;

[NetworkSettings(channel = 0, sendInterval = 0.040f)]
public class NetworkTime : NetworkBehaviour
{
    private static NetworkTime m_instance;

    public static NetworkTime Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    public Text m_networkTimeText;
    int lastReceivedDelayMS = 0;
    NetworkConnection m_connection;
    int m_timeStamp = 0;
    float m_timeOffset;

    int m_fixedStep;
    int m_fixedStepOffset;

    int m_currentSamples;
    int m_totalSamples = 10; // decrease to get more precise values

    int[] m_latencySamples;
    int m_averageIndex = 0;

    int m_latencyAverage;

    bool m_improveOffsetSteps = true;
    int m_timeStepDiff = 100;

    void Awake()
    {        
        m_instance = this;
    }

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {        
        m_timeStamp = NetworkTransport.GetNetworkTimestamp();
        writer.Write(m_timeStamp);
        writer.Write(m_fixedStep); 
        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
       
        int timeStamp = reader.ReadInt32();
        int fixedStep = reader.ReadInt32();

        //Check for a max allowed diff bettween client step and server step
        if (Mathf.Abs(fixedStep - m_fixedStep) > 15)
        {
            m_fixedStep = fixedStep;
        }

        if (MobaClientManager.instance == null)
        {
            return;
        }
        NetworkClient client = MobaClientManager.instance.MyClient;
       
        m_connection = client.connection;
        byte error;
        lastReceivedDelayMS = NetworkTransport.GetRemoteDelayTimeMS(
            m_connection.hostId,
            m_connection.connectionId,
            timeStamp,
            out error);

        CalculateLatencyAverage();
        CheckTimeStepOffset(fixedStep, "Serialize TimeStamp");

        if (m_currentSamples >= m_totalSamples)
        {
            m_currentSamples = 0;
            if (m_improveOffsetSteps)
            {
                if (m_timeStepDiff > 5)
                {
                    DecrementFixedStepOffset(5);
                    m_timeStepDiff = 100;
                }
                else
                {
                    DecrementFixedStepOffset(1);
                }
            }
            m_improveOffsetSteps = true;
        }
    }

    private void CalculateLatencyAverage()
    {
        if (m_latencySamples == null)
        {
            m_latencySamples = new int[m_totalSamples];
        }
        m_latencySamples[m_averageIndex] = lastReceivedDelayMS;
        m_averageIndex++;
        if (m_averageIndex >= m_latencySamples.Length)
        {
            m_averageIndex = 0;
        }
        int latencyAverage = 0;
        foreach (int i in m_latencySamples)
        {
            latencyAverage += i;
        }
        m_latencyAverage = latencyAverage / m_latencySamples.Length;
    }

    void FixedUpdate()
    {
        if (isClient)
        {            
            m_fixedStep++;
            m_networkTimeText.text = "Fixed Step: " + m_fixedStep  + " Latency  " + m_latencyAverage + "Step Offset: " + m_fixedStepOffset;
        }
        else
        {
            m_networkTimeText.text = "Fixed Step: " + m_fixedStep;
            m_fixedStep++;
            SetDirtyBit(1);
        }
        
    }

    //public int ServerTime()
    //{
    //    return (int)(m_timeOffset + (Time.time * 1000));
    //}
    public int ServerStep()
    {
        return m_fixedStep;
    }

    //[ClientRpc]
    //void RpcSyncFixedStep(int timestamp, int fixedStep)
    //{
    //    byte error;
    //    NetworkClient client = MobaClientManager.instance.MyClient;
    //    m_connection = client.connection;
    //    m_timeStamp = timestamp;
    //    //var serverConnection = connectionToServer;
    //    lastReceivedDelayMS = NetworkTransport.GetRemoteDelayTimeMS(
    //        m_connection.hostId,
    //        m_connection.connectionId,
    //        timestamp,
    //        out error);
    //    if (!m_fixedStepSet)
    //    {
    //        if (lastReceivedDelayMS > 0)
    //        {
    //            m_fixedStepSet = true;
    //            m_fixedStep = fixedStep;
    //            Debug.Log("Initial offset: " + (lastReceivedDelayMS / 20) + "Last Delay MS: " + lastReceivedDelayMS);
    //        }           
    //    }
    //
    //    m_latencySamples[m_averageIndex] = lastReceivedDelayMS;
    //    m_averageIndex++;
    //    if (m_averageIndex >= m_latencySamples.Length)
    //    {
    //        m_averageIndex = 0;
    //    }
    //    int latencyAverage = 0;
    //    foreach (int i in m_latencySamples)
    //    {
    //        latencyAverage += i;
    //    }
    //    lastReceivedDelayMS = latencyAverage / m_latencySamples.Length;
    //
    //    m_fixedStepOffset = ((int)lastReceivedDelayMS / 20);
    //}

    //void SetTimeOffset()
    //{
    //    m_timeOffset = GetServerTimeStamp() - (Time.time * 1000);
    //}

    //public float GetServerTimeStamp()
    //{
    //    return m_timeStamp + lastReceivedDelayMS;
    //}

    public int GetServerFixedStep()
    {
        return m_fixedStep - m_fixedStepOffset;
    }

    public void CheckTimeStepOffset(int timeStep, string process)
    {
        m_currentSamples++;
        if (NetworkTime.Instance.GetServerFixedStep() > timeStep + 2)
        {
            Debug.LogError("Fixed Step passed out by: " + (NetworkTime.Instance.GetServerFixedStep() - timeStep) + " Steps");
            NetworkTime.Instance.IncrementFixedStepOffset();
        }
        else
        {
            //Debug.LogWarning("Fixed Step is ok by: " + (timeStep - NetworkTime.Instance.GetServerFixedStep()) + " Steps");
            if (m_improveOffsetSteps)
            {
                if (timeStep - GetServerFixedStep() < m_timeStepDiff)
                {
                    m_timeStepDiff = timeStep - GetServerFixedStep();
                }
                if (timeStep - NetworkTime.Instance.GetServerFixedStep() <= 1)
                {
                    m_improveOffsetSteps = false;
                }
            }
           
        }
    }
    public void IncrementFixedStepOffset()
    {
        m_fixedStepOffset++;
    }

    public void DecrementFixedStepOffset(int ammount)
    {
        Debug.Log("Decreasing Fixed Step Offset " + ammount);
        m_fixedStepOffset -= ammount;
    }

}