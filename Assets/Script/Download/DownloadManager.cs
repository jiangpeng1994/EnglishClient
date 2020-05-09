using System;
using UnityEngine;

/// <summary>
/// 下载管理器。
/// </summary>
public class DownloadManager
{
    /// <summary>
    /// 下载速度计算器。
    /// </summary>
    private readonly DownloadCounter m_DownloadCounter = null;

    /// <summary>
    /// 队列下载代理。
    /// </summary>
    private QueueDownloadAgent m_QueueDownloadAgent = null;

    /// <summary>
    /// 获取或设置下载是否被暂停。
    /// </summary>
    public bool Paused
    {
        get
        {
            return m_QueueDownloadAgent.Paused;
        }
        set
        {
            m_QueueDownloadAgent.Paused = value;
        }
    }

    /// <summary>
    /// 获取当前下载速度。
    /// </summary>
    public float CurrentSpeed
    {
        get
        {
            return m_DownloadCounter.CurrentSpeed;
        }
    }

    /// <summary>
    /// 获取或设置下载超时时长，以秒为单位。
    /// </summary>
    public float Timeout { get; set; }

    /// <summary>
    /// 获取或设置将缓冲区写入磁盘的临界大小。
    /// </summary>
    public int FlushSize { get; set; }

    private EventHandler<DownloadEventArgs> m_DownloadStartEventHandler;

    private EventHandler<DownloadEventArgs> m_DownloadUpdateEventHandler;

    private EventHandler<DownloadEventArgs> m_DownloadSuccessEventHandler;

    private EventHandler<DownloadEventArgs> m_DownloadFailureEventHandler;

    public event EventHandler<DownloadEventArgs> DownloadStart
    {
        add
        {
            m_DownloadStartEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Combine(m_DownloadStartEventHandler, value);
        }
        remove
        {
            m_DownloadStartEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Remove(m_DownloadStartEventHandler, value);
        }
    }

    public event EventHandler<DownloadEventArgs> DownloadUpdate
    {
        add
        {
            m_DownloadUpdateEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Combine(m_DownloadUpdateEventHandler, value);
        }
        remove
        {
            m_DownloadUpdateEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Remove(m_DownloadUpdateEventHandler, value);
        }
    }

    public event EventHandler<DownloadEventArgs> DownloadSuccess
    {
        add
        {
            m_DownloadSuccessEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Combine(m_DownloadSuccessEventHandler, value);
        }
        remove
        {
            m_DownloadSuccessEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Remove(m_DownloadSuccessEventHandler, value);
        }
    }

    public event EventHandler<DownloadEventArgs> DownloadFailure
    {
        add
        {
            m_DownloadFailureEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Combine(m_DownloadFailureEventHandler, value);
        }
        remove
        {
            m_DownloadFailureEventHandler = (EventHandler<DownloadEventArgs>)Delegate.Remove(m_DownloadFailureEventHandler, value);
        }
    }

    /// <summary>
    /// 构造函数。
    /// </summary>
    public DownloadManager()
    {
        m_DownloadCounter = new DownloadCounter(1f, 10f);
        FlushSize = (int)DownloadTool.OneMegaBytes;
        Timeout = 10f;
        m_DownloadStartEventHandler = null;
        m_DownloadUpdateEventHandler = null;
        m_DownloadSuccessEventHandler = null;
        m_DownloadFailureEventHandler = null;
    }

    /// <summary>
    /// 创建队列下载代理。
    /// </summary>
    public void CreateQueueDownloadAgent(Transform parent)
    {
        DownloadHelper downloadAgentHelper = new GameObject("Download Agent Helper").AddComponent<DownloadHelper>();
        downloadAgentHelper.transform.SetParent(parent);
        downloadAgentHelper.transform.localScale = Vector3.one;

        m_QueueDownloadAgent = new QueueDownloadAgent(downloadAgentHelper);
        QueueDownloadAgent downloadAgent2 = m_QueueDownloadAgent;
        downloadAgent2.DownloadAgentStart = (Action<QueueDownloadAgent>)Delegate.Combine(downloadAgent2.DownloadAgentStart, new Action<QueueDownloadAgent>(OnDownloadAgentStart));
        QueueDownloadAgent downloadAgent3 = m_QueueDownloadAgent;
        downloadAgent3.DownloadAgentUpdate = (Action<QueueDownloadAgent, int, float>)Delegate.Combine(downloadAgent3.DownloadAgentUpdate, new Action<QueueDownloadAgent, int, float>(OnDownloadAgentUpdate));
        QueueDownloadAgent downloadAgent4 = m_QueueDownloadAgent;
        downloadAgent4.DownloadAgentSuccess = (Action<QueueDownloadAgent, int>)Delegate.Combine(downloadAgent4.DownloadAgentSuccess, new Action<QueueDownloadAgent, int>(OnDownloadAgentSuccess));
        QueueDownloadAgent downloadAgent5 = m_QueueDownloadAgent;
        downloadAgent5.DownloadAgentFailure = (Action<QueueDownloadAgent, int, string>)Delegate.Combine(downloadAgent5.DownloadAgentFailure, new Action<QueueDownloadAgent, int, string>(OnDownloadAgentFailure));
        m_QueueDownloadAgent.Initialize();
    }

    /// <summary>
    /// 增加下载任务。
    /// </summary>
    /// <param name="downloadUrl">原始下载地址。</param>
    /// <param name="downloadPath">下载后存放路径。</param>
    /// <returns>新增下载任务的序列编号。</returns>
    public int AddDownloadTask(string downloadUrl, string downloadPath)
    {
        return AddDownloadTask(downloadUrl, downloadPath, null);
    }

    /// <summary>
    /// 增加下载任务。
    /// </summary>
    /// <param name="downloadUrl">原始下载地址。</param>
    /// <param name="downloadPath">下载后存放路径。</param>
    /// <param name="md5">下载文件的校验MD5。</param>
    /// <returns>新增下载任务的序列编号。</returns>
    public int AddDownloadTask(string downloadUrl, string downloadPath, string md5)
    {
        if (string.IsNullOrEmpty(downloadUrl))
        {
            Debug.LogWarning("Download Url is invalid.");
        }
        if (string.IsNullOrEmpty(downloadPath))
        {
            Debug.LogWarning("Download Path is invalid.");
        }
        if (m_QueueDownloadAgent == null)
        {
            Debug.LogWarning("Download agent is invalid.");
        }

        DownloadTask downloadTask = DownloadTask.Create(downloadUrl ,downloadPath, md5, FlushSize, Timeout, null);
        m_QueueDownloadAgent.AddDownloadTask(downloadTask);
        return downloadTask.SerialId;
    }

    /// <summary>
    /// 移除下载任务。
    /// </summary>
    /// <param name="serialId">要移除下载任务的序列编号。</param>
    public bool RemoveDownloadTask(int serialId)
    {
        return m_QueueDownloadAgent.RemoveTask(serialId);
    }

    /// <summary>
    /// 移除所有下载任务。
    /// </summary>
    public void RemoveAllDownloadTasks()
    {
        m_QueueDownloadAgent.RemoveAllTasks();
    }

    /// <summary>
    /// 开始下载。
    /// </summary>
    public void StartDownload()
    {
        m_QueueDownloadAgent.Paused = false;
    }

    /// <summary>
    /// 暂停下载。
    /// </summary>
    public void PauseDownload()
    {
        m_QueueDownloadAgent.PauseDownload();
    }

    /// <summary>
    /// 下载管理器循环。
    /// </summary>
    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        if (m_QueueDownloadAgent == null || m_DownloadCounter == null)
        {
            return;
        }

        m_QueueDownloadAgent.Update(elapseSeconds, realElapseSeconds);
        m_DownloadCounter.Update(elapseSeconds, realElapseSeconds);
    }

    /// <summary>
    /// 关闭。
    /// </summary>
    public void Shutdown()
    {
        m_QueueDownloadAgent.Shutdown();
        m_DownloadCounter.Shutdown();
    }

    /// <summary>
    /// 管理层的回调：任务开始下载。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    private void OnDownloadAgentStart(QueueDownloadAgent sender)
    {
        if (m_DownloadStartEventHandler != null)
        {
            DownloadEventArgs downloadStartEventArgs = DownloadEventArgs.CreateStartEventArgs(sender.CurTask.SerialId, sender.CurTask.DownloadUrl, sender.CurTask.DownloadPath, sender.SavedLength, sender.CurTask.UserData);
            m_DownloadStartEventHandler(this, downloadStartEventArgs);
        }
    }

    /// <summary>
    /// 管理层的回调：任务下载的数据更新。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="lastDownloadedLength">本次更新下载数据大小</param>
    /// <param name="downloadProgress">下载进度</param>
    private void OnDownloadAgentUpdate(QueueDownloadAgent sender, int lastDownloadedLength, float downloadProgress)
    {
        m_DownloadCounter.RecordDownloadedLength(lastDownloadedLength);
        if (m_DownloadUpdateEventHandler != null)
        {
            DownloadEventArgs downloadUpdateEventArgs = DownloadEventArgs.CreateUpdateEventArgs(sender.CurTask.SerialId, sender.CurTask.DownloadUrl, sender.CurTask.DownloadPath, sender.SavedLength, downloadProgress, sender.CurTask.UserData);
            m_DownloadUpdateEventHandler(this, downloadUpdateEventArgs);
        }
    }

    /// <summary>
    /// 管理层的回调：任务下载完成。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="lastDownloadedLength">本次更新下载数据大小</param>
    private void OnDownloadAgentSuccess(QueueDownloadAgent sender, int lastDownloadedLength)
    {
        if (m_DownloadSuccessEventHandler != null)
        {
            DownloadEventArgs downloadSuccessEventArgs = DownloadEventArgs.CreateSuccessEventArgs(sender.CurTask.SerialId, sender.CurTask.DownloadUrl, sender.CurTask.DownloadPath, sender.SavedLength, sender.CurTask.MD5, sender.CurTask.UserData);
            m_DownloadSuccessEventHandler(this, downloadSuccessEventArgs);
        }
    }

    /// <summary>
    ///  管理层的回调：任务下载失败。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="errorCode">错误码</param>
    /// <param name="errorMessage">错误消息</param>
    private void OnDownloadAgentFailure(QueueDownloadAgent sender, int errorCode, string errorMessage)
    {
        if (m_DownloadFailureEventHandler != null)
        {
            DownloadEventArgs downloadFailureEventArgs = DownloadEventArgs.CreateFailureEventArgs(sender.CurTask.SerialId, sender.CurTask.DownloadUrl, sender.CurTask.DownloadPath, sender.SavedLength, errorCode, errorMessage, sender.CurTask.UserData);
            m_DownloadFailureEventHandler(this, downloadFailureEventArgs);
        }
    }
}