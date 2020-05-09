using System;
using System.Collections.Generic;

public sealed class DownloadCounter
{
    private readonly LinkedList<DownloadCounterNode> m_DownloadCounterNodes;

    private float m_UpdateInterval;

    private float m_RecordInterval;
    private float m_Accumulator;

    private float m_TimeLeft;

    public float UpdateInterval
    {
        get
        {
            return m_UpdateInterval;
        }
        set
        {
            if (value <= 0f)
            {
                throw new Exception("Update interval is invalid.");
            }
            m_UpdateInterval = value;
            Reset();
        }
    }

    public float RecordInterval
    {
        get
        {
            return m_RecordInterval;
        }
        set
        {
            if (value <= 0f)
            {
                throw new Exception("Record interval is invalid.");
            }
            m_RecordInterval = value;
            Reset();
        }
    }

    public float CurrentSpeed { get; private set; }

    public DownloadCounter(float updateInterval, float recordInterval)
    {
        if (updateInterval <= 0f)
        {
            throw new Exception("Update interval is invalid.");
        }
        if (recordInterval <= 0f)
        {
            throw new Exception("Record interval is invalid.");
        }
        m_DownloadCounterNodes = new LinkedList<DownloadCounterNode>();
        m_UpdateInterval = updateInterval;
        m_RecordInterval = recordInterval;
        Reset();
    }

    public void Shutdown()
    {
        Reset();
    }

    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        if (m_DownloadCounterNodes.Count > 0)
        {
            m_Accumulator += realElapseSeconds;
            if (m_Accumulator > m_RecordInterval)
            {
                m_Accumulator = m_RecordInterval;
            }
            m_TimeLeft -= realElapseSeconds;
            foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
            {
                downloadCounterNode.Update(elapseSeconds, realElapseSeconds);
            }
            while (m_DownloadCounterNodes.Count > 0)
            {
                DownloadCounterNode value = m_DownloadCounterNodes.First.Value;
                if (value.ElapseSeconds < m_RecordInterval)
                {
                    break;
                }
                m_DownloadCounterNodes.RemoveFirst();
            }
            if (m_DownloadCounterNodes.Count <= 0)
            {
                Reset();
            }
            else if (m_TimeLeft <= 0f)
            {
                int num = 0;
                foreach (DownloadCounterNode downloadCounterNode2 in m_DownloadCounterNodes)
                {
                    num += downloadCounterNode2.DownloadedLength;
                }
                CurrentSpeed = ((m_Accumulator > 0f) ? ((float)num / m_Accumulator) : 0f);
                m_TimeLeft += m_UpdateInterval;
            }
        }
    }

    public void RecordDownloadedLength(int downloadedLength)
    {
        if (downloadedLength > 0)
        {
            if (m_DownloadCounterNodes.Count > 0)
            {
                DownloadCounterNode value = m_DownloadCounterNodes.Last.Value;
                if (value.ElapseSeconds < m_UpdateInterval)
                {
                    value.AddDownloadedLength(downloadedLength);
                    return;
                }
            }
            m_DownloadCounterNodes.AddLast(DownloadCounterNode.Create(downloadedLength));
        }
    }

    private void Reset()
    {
        m_DownloadCounterNodes.Clear();
        CurrentSpeed = 0f;
        m_Accumulator = 0f;
        m_TimeLeft = 0f;
    }
}