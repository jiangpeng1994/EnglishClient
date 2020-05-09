using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

/// <summary>
/// 队列解压代理。
/// </summary>
public class QueueUnZipAgent : IDisposable
{
    private LZ4UnzipWorker m_UnzipWorker = null;

    /// <summary>
    /// 解压线程。
    /// </summary>
    private Thread m_UnZipThread = null;
   
    /// <summary>
    /// 解压任务队列。
    /// </summary>
    private Queue<UnZipTask> m_TaskQueue = null;

    /// <summary>
    /// 当前执行的解压任务。
    /// </summary>
    public UnZipTask CurTask { get; private set; }

    /// <summary>
    /// 获取或设置解压是否被暂停。
    /// </summary>
    public bool Paused { get; set; }

    /// <summary>
    /// 事件回调：任务开始解压。
    /// </summary>
    public Action<QueueUnZipAgent> UnZipAgentStart;
    
    /// <summary>
    /// 事件回调：解压进度更新。
    /// </summary>
    public Action<QueueUnZipAgent, float> UnZipAgentUpdate;

    /// <summary>
    /// 事件回调：解压任务完成。
    /// </summary>
    public Action<QueueUnZipAgent> UnZipAgentSuccess;

    /// <summary>
    /// 事件回调：解压任务失败。
    /// </summary>
    public Action<QueueUnZipAgent, int, string> UnZipAgentFailure;

    private bool m_Disposed;

    /// <summary>
    /// 构造函数。
    /// </summary>
    public QueueUnZipAgent()
    {
        m_UnzipWorker = null;
        m_UnZipThread = null;
        m_TaskQueue = new Queue<UnZipTask>();
        CurTask = null;
        Paused = true;
        UnZipAgentStart = null;
        UnZipAgentUpdate = null;
        UnZipAgentSuccess = null;
        UnZipAgentFailure = null;
        m_Disposed = false;
    }

    /// <summary>
    /// 增加解压任务到解压队列中。
    /// </summary>
    /// <param name="UnZipTask">解压任务</param>
    public void AddUnZipTask(UnZipTask UnZipTask)
    {
        m_TaskQueue.Enqueue(UnZipTask);
    }

    /// <summary>
    /// 移除解压任务。
    /// </summary>
    /// <param name="serialId">要移除解压任务的序列编号。</param>
    public bool RemoveTask(int serialId)
    {
        return false;
    }

    /// <summary>
    /// 移除所有解压任务。
    /// </summary>
    public void RemoveAllTasks()
    {
        m_TaskQueue.Clear();
        Reset();
    }

    /// <summary>
    /// 队列解压代理循环。
    /// </summary>
    /// <param name="elapseSeconds"></param>
    /// <param name="realElapseSeconds"></param>
    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        if (Paused)
        {
            return;
        }

        RunningTasks(elapseSeconds, realElapseSeconds);
    }

    /// <summary>
    /// 依次执行队列中的解压任务。
    /// </summary>
    /// <param name="elapseSeconds"></param>
    /// <param name="realElapseSeconds"></param>
    private void RunningTasks(float elapseSeconds, float realElapseSeconds)
    {
        if (m_TaskQueue.Count <= 0)
        {
            return;
        }

        if(CurTask == null)
        {
            CurTask = m_TaskQueue.Peek();
        }

        switch (CurTask.Status)
        {
            case UnZipTaskStatus.Todo:
                // 如果当前解压任务还没开始，则开始执行
                StartCurTask();
                break;
            case UnZipTaskStatus.Doing:
                // 如果当前解压任务正在解压，则更新解压进度
                RunningCurTask();
                break;
            case UnZipTaskStatus.Done:
                //  如果当前解压任务已经完成解压，则重置解压队列代理，并移除当前解压任务
                Reset();
                m_TaskQueue.Dequeue();
                break;
            case UnZipTaskStatus.Error:
                //  如果当前解压任务出错，则重置解压队列代理，并暂停解压
                PauseUnZip();
                break;
        }
    }

    /// <summary>
    /// 开始执行当前解压任务。
    /// </summary>
    private void StartCurTask()
    {
        if (CurTask == null)
        {
            return;
        }

        if (!File.Exists(CurTask.SourceFilePath))
        {
            // 解压文件不存在
            OnUnZipError(1, "SourceFilePath does not exist.");
            return;
        }

        CurTask.Status = UnZipTaskStatus.Doing;

        m_UnzipWorker = new LZ4UnzipWorker(CurTask.SourceFilePath, CurTask.UnZipPath, CurTask.UnZipType);
        m_UnZipThread = new Thread(new ThreadStart(m_UnzipWorker.StartUnzipByType));
        m_UnZipThread.Start();

        if (UnZipAgentStart != null)
        {
            // 事件回调管理层：任务开始解压
            UnZipAgentStart(this);
        }
    }

    /// <summary>
    /// 执行解压任务中。
    /// </summary>
    private void RunningCurTask()
    {
        if (CurTask == null || m_UnzipWorker == null)
        {
            return;
        }

        ZipResult zipResult = m_UnzipWorker.GetUnzipResult();
        if (zipResult.Errors)
        {
            // 解压异常
            OnUnZipError(2, "Zip Error");
            return;
        }

        if (zipResult.UnZipPercent < 1)
        {
            if (UnZipAgentUpdate != null)
            {
                // 事件回调管理层：解压进度更新
                UnZipAgentUpdate(this, zipResult.UnZipPercent);
            }
            return;
        }

        OnUnZipComplete();
    }

    /// <summary>
    /// 当前任务解压完成。
    /// </summary>
    private void OnUnZipComplete()
    {
        CurTask.Status = UnZipTaskStatus.Done;

        if (CurTask.IsDeleteSourceFileAfterUnZip)
        {
            if (File.Exists(CurTask.SourceFilePath))
            {
                File.Delete(CurTask.SourceFilePath);
            }
        }
        
        if (UnZipAgentSuccess != null)
        {
            // 事件回调管理层：任务解压完成。
            UnZipAgentSuccess(this);
        }
    }

    /// <summary>
    /// 当前任务解压失败。
    /// <param name="errorCode">错误码</param>
    /// <param name="errorMessage">错误消息</param>
    /// </summary>
    private void OnUnZipError(int errorCode, string errorMessage)
    {
        CurTask.Status = UnZipTaskStatus.Error;

        if (UnZipAgentFailure != null)
        {
            // 事件回调管理层：任务解压失败。
            UnZipAgentFailure(this, errorCode, errorMessage);
        }
    }

    /// <summary>
    /// 重置解压队列代理。
    /// </summary>
    public void Reset()
    {
        m_UnzipWorker = null;
        if (m_UnZipThread != null)
        {
            m_UnZipThread.Abort();
            m_UnZipThread = null;
        }

        if (CurTask != null)
        {
            CurTask.Status = UnZipTaskStatus.Todo;
            CurTask = null;
        }
    }

    /// <summary>
    /// 暂停解压。
    /// </summary>
    public void PauseUnZip()
    {
        Reset();
        Paused = true;
    }

    /// <summary>
    /// 关闭。
    /// </summary>
    public void Shutdown()
    {
        Dispose();
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <param name="disposing">释放资源标记。</param>
    private void Dispose(bool disposing)
    {
        if (m_Disposed)
        {
            return;
        }

        m_Disposed = true;
    }
}