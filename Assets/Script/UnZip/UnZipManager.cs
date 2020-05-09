using System;
using UnityEngine;

/// <summary>
/// 解压管理器。
/// </summary>
public class UnZipManager
{
    /// <summary>
    /// 队列解压代理。
    /// </summary>
    private QueueUnZipAgent m_QueueUnZipAgent = null;

    /// <summary>
    /// 获取或设置解压是否被暂停。
    /// </summary>
    public bool Paused
    {
        get
        {
            return m_QueueUnZipAgent.Paused;
        }
        set
        {
            m_QueueUnZipAgent.Paused = value;
        }
    }

    private EventHandler<UnZipEventArgs> m_UnZipStartEventHandler;

    private EventHandler<UnZipEventArgs> m_UnZipUpdateEventHandler;

    private EventHandler<UnZipEventArgs> m_UnZipSuccessEventHandler;

    private EventHandler<UnZipEventArgs> m_UnZipFailureEventHandler;

    public event EventHandler<UnZipEventArgs> UnZipStart
    {
        add
        {
            m_UnZipStartEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Combine(m_UnZipStartEventHandler, value);
        }
        remove
        {
            m_UnZipStartEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Remove(m_UnZipStartEventHandler, value);
        }
    }

    public event EventHandler<UnZipEventArgs> UnZipUpdate
    {
        add
        {
            m_UnZipUpdateEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Combine(m_UnZipUpdateEventHandler, value);
        }
        remove
        {
            m_UnZipUpdateEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Remove(m_UnZipUpdateEventHandler, value);
        }
    }

    public event EventHandler<UnZipEventArgs> UnZipSuccess
    {
        add
        {
            m_UnZipSuccessEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Combine(m_UnZipSuccessEventHandler, value);
        }
        remove
        {
            m_UnZipSuccessEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Remove(m_UnZipSuccessEventHandler, value);
        }
    }

    public event EventHandler<UnZipEventArgs> UnZipFailure
    {
        add
        {
            m_UnZipFailureEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Combine(m_UnZipFailureEventHandler, value);
        }
        remove
        {
            m_UnZipFailureEventHandler = (EventHandler<UnZipEventArgs>)Delegate.Remove(m_UnZipFailureEventHandler, value);
        }
    }

    /// <summary>
    /// 构造函数。
    /// </summary>
    public UnZipManager()
    {
        m_UnZipStartEventHandler = null;
        m_UnZipUpdateEventHandler = null;
        m_UnZipSuccessEventHandler = null;
        m_UnZipFailureEventHandler = null;
    }

    /// <summary>
    /// 创建队列解压代理。
    /// </summary>
    public void CreateQueueUnZipAgent()
    {
        m_QueueUnZipAgent = new QueueUnZipAgent();
        QueueUnZipAgent UnZipAgent2 = m_QueueUnZipAgent;
        UnZipAgent2.UnZipAgentStart = (Action<QueueUnZipAgent>)Delegate.Combine(UnZipAgent2.UnZipAgentStart, new Action<QueueUnZipAgent>(OnUnZipAgentStart));
        QueueUnZipAgent UnZipAgent3 = m_QueueUnZipAgent;
        UnZipAgent3.UnZipAgentUpdate = (Action<QueueUnZipAgent, float>)Delegate.Combine(UnZipAgent3.UnZipAgentUpdate, new Action<QueueUnZipAgent, float>(OnUnZipAgentUpdate));
        QueueUnZipAgent UnZipAgent4 = m_QueueUnZipAgent;
        UnZipAgent4.UnZipAgentSuccess = (Action<QueueUnZipAgent>)Delegate.Combine(UnZipAgent4.UnZipAgentSuccess, new Action<QueueUnZipAgent>(OnUnZipAgentSuccess));
        QueueUnZipAgent UnZipAgent5 = m_QueueUnZipAgent;
        UnZipAgent5.UnZipAgentFailure = (Action<QueueUnZipAgent, int, string>)Delegate.Combine(UnZipAgent5.UnZipAgentFailure, new Action<QueueUnZipAgent, int, string>(OnUnZipAgentFailure));
    }

    /// <summary>
    /// 增加解压任务。
    /// </summary>
    /// <param name="sourceFilePath">解压源文件。</param>
    /// <param name="unZipPath">解压后存放路径。</param>
    /// <returns>新增解压任务的序列编号。</returns>
    public int AddUnZipTask(string sourceFilePath, string unZipPath)
    {
        return AddUnZipTask(sourceFilePath, unZipPath, 0, false);
    }

    /// <summary>
    /// 增加解压任务。
    /// </summary>
    /// <param name="sourceFilePath">解压源文件。</param>
    /// <param name="unZipPath">解压后存放路径。</param>
    /// <param name="unZipType">解压类型。</param>
    /// <param name="isDeleteSourceFileAfterUnZip">解压后是否删除源文件。</param>
    /// <returns>新增解压任务的序列编号。</returns>
    public int AddUnZipTask(string sourceFilePath, string unZipPath, int unZipType, bool isDeleteSourceFileAfterUnZip)
    {
        if (string.IsNullOrEmpty(sourceFilePath))
        {
            Debug.LogWarning("UnZip Url is invalid.");
        }
        if (string.IsNullOrEmpty(unZipPath))
        {
            Debug.LogWarning("UnZip Path is invalid.");
        }
        if (m_QueueUnZipAgent == null)
        {
            Debug.LogWarning("UnZip agent is invalid.");
        }

        UnZipTask UnZipTask = UnZipTask.Create(sourceFilePath ,unZipPath, unZipType, isDeleteSourceFileAfterUnZip, null);
        m_QueueUnZipAgent.AddUnZipTask(UnZipTask);
        return UnZipTask.SerialId;
    }

    /// <summary>
    /// 移除解压任务。
    /// </summary>
    /// <param name="serialId">要移除解压任务的序列编号。</param>
    public bool RemoveUnZipTask(int serialId)
    {
        return m_QueueUnZipAgent.RemoveTask(serialId);
    }

    /// <summary>
    /// 移除所有解压任务。
    /// </summary>
    public void RemoveAllUnZipTasks()
    {
        m_QueueUnZipAgent.RemoveAllTasks();
    }

    /// <summary>
    /// 开始解压。
    /// </summary>
    public void StartUnZip()
    {
        m_QueueUnZipAgent.Paused = false;
    }

    /// <summary>
    /// 暂停解压。
    /// </summary>
    public void PauseUnZip()
    {
        m_QueueUnZipAgent.PauseUnZip();
    }

    /// <summary>
    /// 解压管理器循环。
    /// </summary>
    /// <param name="elapseSeconds"></param>
    /// <param name="realElapseSeconds"></param>
    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        if (m_QueueUnZipAgent == null)
        {
            return;
        }

        m_QueueUnZipAgent.Update(elapseSeconds, realElapseSeconds);
    }

    /// <summary>
    /// 关闭。
    /// </summary>
    public void Shutdown()
    {
        m_QueueUnZipAgent.Shutdown();
    }

    /// <summary>
    /// 管理层的回调：任务开始解压。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    private void OnUnZipAgentStart(QueueUnZipAgent sender)
    {
        if (m_UnZipStartEventHandler != null)
        {
            UnZipEventArgs UnZipStartEventArgs = UnZipEventArgs.CreateStartEventArgs(sender.CurTask.SerialId, sender.CurTask.SourceFilePath, sender.CurTask.UnZipPath, sender.CurTask.UnZipType, sender.CurTask.UserData);
            m_UnZipStartEventHandler(this, UnZipStartEventArgs);
        }
    }

    /// <summary>
    /// 管理层的回调：任务解压的数据更新。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="UnZipProgress">解压进度</param>
    private void OnUnZipAgentUpdate(QueueUnZipAgent sender, float UnZipProgress)
    {
        if (m_UnZipUpdateEventHandler != null)
        {
            UnZipEventArgs UnZipUpdateEventArgs = UnZipEventArgs.CreateUpdateEventArgs(sender.CurTask.SerialId, sender.CurTask.SourceFilePath, sender.CurTask.UnZipPath, sender.CurTask.UnZipType, UnZipProgress, sender.CurTask.UserData);
            m_UnZipUpdateEventHandler(this, UnZipUpdateEventArgs);
        }
    }

    /// <summary>
    /// 管理层的回调：任务解压完成。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    private void OnUnZipAgentSuccess(QueueUnZipAgent sender)
    {
        if (m_UnZipSuccessEventHandler != null)
        {
            UnZipEventArgs UnZipSuccessEventArgs = UnZipEventArgs.CreateSuccessEventArgs(sender.CurTask.SerialId, sender.CurTask.SourceFilePath, sender.CurTask.UnZipPath, sender.CurTask.UnZipType, sender.CurTask.UserData);
            m_UnZipSuccessEventHandler(this, UnZipSuccessEventArgs);
        }
    }

    /// <summary>
    ///  管理层的回调：任务解压失败。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="errorCode">错误码</param>
    /// <param name="errorMessage">错误消息</param>
    private void OnUnZipAgentFailure(QueueUnZipAgent sender, int errorCode, string errorMessage)
    {
        if (m_UnZipFailureEventHandler != null)
        {
            UnZipEventArgs UnZipFailureEventArgs = UnZipEventArgs.CreateFailureEventArgs(sender.CurTask.SerialId, sender.CurTask.SourceFilePath, sender.CurTask.UnZipPath, sender.CurTask.UnZipType, errorCode, errorMessage, sender.CurTask.UserData);
            m_UnZipFailureEventHandler(this, UnZipFailureEventArgs);
        }
    }
}