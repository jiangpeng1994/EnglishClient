using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 队列下载代理。
/// </summary>
public class QueueDownloadAgent : IDisposable
{
    /// <summary>
    /// 使用 UnityWebRequest 实现的下载代理辅助器。
    /// </summary>
    private readonly DownloadHelper m_Helper;

    /// <summary>
    /// 下载任务队列。
    /// </summary>
    private Queue<DownloadTask> m_TaskQueue;

    /// <summary>
    /// 下载文件存入本地的文件流。
    /// </summary>
    private FileStream m_FileStream;

    /// <summary>
    /// 文件流中等待被Flush的数据大小。
    /// </summary>
    private int m_WaitFlushSize;

    /// <summary>
    /// 当前执行的下载任务。
    /// </summary>
    public DownloadTask CurTask { get; private set; }

    /// <summary>
    /// 当前任务在下载过程中请求数据的等待时间。
    /// </summary>
    public float WaitTime { get; private set; }

    /// <summary>
    /// 当前任务在本次下载中，开始下载的数据位置。
    /// </summary>
    public int StartLength { get; private set; }

    /// <summary>
    /// 当前任务在本次下载中，已经下载的数据大小。
    /// </summary>
    public int DownloadedLength { get; private set; }

    /// <summary>
    /// 当前任务已经存入本地的数据大小。
    /// </summary>
    public int SavedLength { get; private set; }
   
    /// <summary>
    /// 获取或设置下载是否被暂停。
    /// </summary>
    public bool Paused { get; set; }

    /// <summary>
    /// 事件回调：任务开始下载。
    /// </summary>
    public Action<QueueDownloadAgent> DownloadAgentStart;
    
    /// <summary>
    /// 事件回调：下载数据更新。
    /// </summary>
    public Action<QueueDownloadAgent, int, float> DownloadAgentUpdate;

    /// <summary>
    /// 事件回调：下载任务完成。
    /// </summary>
    public Action<QueueDownloadAgent, int> DownloadAgentSuccess;

    /// <summary>
    /// 事件回调：下载任务失败。
    /// </summary>
    public Action<QueueDownloadAgent, int, string> DownloadAgentFailure;

    private bool m_Disposed;

    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="downloadAgentHelper">下载代理辅助器</param>
    public QueueDownloadAgent(DownloadHelper downloadAgentHelper)
    {
        if (downloadAgentHelper == null)
        {
            Debug.LogWarning("Download serial agent helper is invalid.");
        }
        m_Helper = downloadAgentHelper;
        m_TaskQueue = new Queue<DownloadTask>();
        m_FileStream = null;
        m_WaitFlushSize = 0;
        CurTask = null;
        WaitTime = 0f;
        StartLength = 0;
        DownloadedLength = 0;
        SavedLength = 0;
        Paused = true;
        DownloadAgentStart = null;
        DownloadAgentUpdate = null;
        DownloadAgentSuccess = null;
        DownloadAgentFailure = null;
        m_Disposed = false;
    }

    /// <summary>
    /// 初始化回调。
    /// </summary>
    public void Initialize()
    {
        m_Helper.DownloadAgentHelperUpdateBytes += OnDownloadHelperUpdateBytes;
        m_Helper.DownloadAgentHelperComplete += OnDownloadHelperComplete;
        m_Helper.DownloadAgentHelperError += OnDownloadHelperError;
    }

    /// <summary>
    /// 增加下载任务到下载队列中。
    /// </summary>
    /// <param name="downloadTask">下载任务</param>
    public void AddDownloadTask(DownloadTask downloadTask)
    {
        m_TaskQueue.Enqueue(downloadTask);
    }

    /// <summary>
    /// 移除下载任务。
    /// </summary>
    /// <param name="serialId">要移除下载任务的序列编号。</param>
    public bool RemoveTask(int serialId)
    {
        return false;
    }

    /// <summary>
    /// 移除所有下载任务。
    /// </summary>
    public void RemoveAllTasks()
    {
        m_TaskQueue.Clear();
        Reset();
    }

    /// <summary>
    /// 队列下载代理循环。
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
    /// 依次执行队列中的下载任务。
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
            case DownloadTaskStatus.Todo:
                // 如果当前下载任务还没开始，则开始执行
                StartCurTask();
                break;
            case DownloadTaskStatus.Doing:
                // 如果当前下载任务正在下载，则检测当前下载任务执行过程中是否超时
                CheckIsTimeout(realElapseSeconds);
                break;
            case DownloadTaskStatus.Done:
                //  如果当前下载任务已经完成下载，则重置下载队列代理，并移除当前下载任务
                Reset();
                m_TaskQueue.Dequeue();
                break;
            case DownloadTaskStatus.Error:
                //  如果当前下载任务出错，则重置下载队列代理，并暂停下载
                PauseDownload();
                break;
        }
    }

    /// <summary>
    /// 开始执行当前下载任务。
    /// </summary>
    private void StartCurTask()
    {
        if (CurTask == null || m_Helper == null)
        {
            return;
        }

        CurTask.Status = DownloadTaskStatus.Doing;
        string path = string.Format("{0}.download", CurTask.DownloadPath);
        try
        {
            // 本地存在该文件（存在本地却没有下完，即下载了部分到本地，则需要断点续传）
            if (File.Exists(path))
            {
                m_FileStream = File.OpenWrite(path);
                // 移动本地文件流中的指针到末尾，接着上次下载写入到的断点，接着继续写入本地
                m_FileStream.Seek(0L, SeekOrigin.End);
                StartLength = (SavedLength = (int)m_FileStream.Length);
                DownloadedLength = 0;
            }
            // 本地不存在该文件（该文件还未开始下）
            else
            {
                string directoryName = Path.GetDirectoryName(CurTask.DownloadPath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                m_FileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                StartLength = (SavedLength = (DownloadedLength = 0));
            }

            if (DownloadAgentStart != null)
            {
                // 事件回调管理层：任务开始下载
                DownloadAgentStart(this);
            }

            if (StartLength > 0)
            {
                // 断点下载
                m_Helper.Download(CurTask.DownloadUrl, StartLength, CurTask.UserData);
            }
            else
            {
                // 从头下载
                m_Helper.Download(CurTask.DownloadUrl, CurTask.UserData);
            }
        }
        catch (Exception ex)
        {
            DownloadHelperEventArgs downloadAgentHelperErrorEventArgs = DownloadHelperEventArgs.Create(2, ex.ToString());
            OnDownloadHelperError(this, downloadAgentHelperErrorEventArgs);
        }
    }

    /// <summary>
    /// 检测当前下载任务执行过程中是否超时。
    /// </summary>
    /// <param name="realElapseSeconds"></param>
    private void CheckIsTimeout(float realElapseSeconds)
    {
        WaitTime += realElapseSeconds;
        if (WaitTime >= CurTask.Timeout)
        {
            DownloadHelperEventArgs downloadAgentHelperErrorEventArgs = DownloadHelperEventArgs.Create(3, "Timeout");
            OnDownloadHelperError(this, downloadAgentHelperErrorEventArgs);
        }
    }

    /// <summary>
    ///  代理层的回调：当前任务下载的数据更新。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadHelperUpdateBytes(object sender, DownloadHelperEventArgs e)
    {
        WaitTime = 0f;
        try
        {
            m_FileStream.Write(e.GetBytes(), e.Offset, e.Length);
            m_WaitFlushSize += e.Length;
            DownloadedLength += e.Length;
            SavedLength += e.Length;
            if (m_WaitFlushSize >= CurTask.FlushSize)
            {
                m_FileStream.Flush();
                m_WaitFlushSize = 0;
            }

            if (DownloadAgentUpdate != null)
            {
                // 事件回调管理层：下载数据更新
                DownloadAgentUpdate(this, e.Length, e.DownloadProgress);
            }
        }
        catch (Exception ex)
        {
            DownloadHelperEventArgs downloadAgentHelperErrorEventArgs = DownloadHelperEventArgs.Create(2, ex.ToString());
            OnDownloadHelperError(this, downloadAgentHelperErrorEventArgs);
        }
    }

    /// <summary>
    ///  代理层的回调：当前任务下载完成。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadHelperComplete(object sender, DownloadHelperEventArgs e)
    {
        WaitTime = 0f;
        DownloadedLength = e.Length;
        if (SavedLength != StartLength + DownloadedLength)
        {
            Debug.LogWarning("Internal download error.");
        }

        m_FileStream.Close();
        m_FileStream = null;
        if (File.Exists(CurTask.DownloadPath))
        {
            File.Delete(CurTask.DownloadPath);
        }
        File.Move(string.Format("{0}.download", CurTask.DownloadPath), CurTask.DownloadPath);

        // 如果下载任务设置了MD5校验值，则进行MD5校验
        if(CurTask.MD5 != null)
        {
            // 校验不通过
            if(!DownloadTool.CheckFileMd5(CurTask.DownloadPath, CurTask.MD5))
            {
                DownloadHelperEventArgs downloadAgentHelperErrorEventArgs = DownloadHelperEventArgs.Create(4, "md5 verification failed.");
                OnDownloadHelperError(this, downloadAgentHelperErrorEventArgs);
                return;
            }
        }

        CurTask.Status = DownloadTaskStatus.Done;

        if (DownloadAgentSuccess != null)
        {
            // 事件回调管理层：任务下载完成。
            DownloadAgentSuccess(this, e.Length);
        }
    }

    /// <summary>
    /// 重置下载队列代理。
    /// </summary>
    public void Reset()
    {
        m_Helper.Reset();
        if (m_FileStream != null)
        {
            m_FileStream.Close();
            m_FileStream = null;
        }
        m_WaitFlushSize = 0;
        if (CurTask != null)
        {
            CurTask.Status = DownloadTaskStatus.Todo;
            CurTask = null;
        }
        WaitTime = 0f;
        StartLength = 0;
        DownloadedLength = 0;
        SavedLength = 0;
    }

    /// <summary>
    /// 代理层的回调：当前任务下载失败。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadHelperError(object sender, DownloadHelperEventArgs e)
    {
        m_Helper.Reset();
        if (m_FileStream != null)
        {
            m_FileStream.Close();
            m_FileStream = null;
        }
        CurTask.Status = DownloadTaskStatus.Error;

        if (DownloadAgentFailure != null)
        {
            // 事件回调管理层：任务下载失败。
            DownloadAgentFailure(this, e.ErrorCode, e.ErrorMessage);
        }
    }

    /// <summary>
    /// 暂停下载。
    /// </summary>
    public void PauseDownload()
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
        m_Helper.DownloadAgentHelperUpdateBytes -= OnDownloadHelperUpdateBytes;
        m_Helper.DownloadAgentHelperComplete -= OnDownloadHelperComplete;
        m_Helper.DownloadAgentHelperError -= OnDownloadHelperError;
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

        if (disposing && m_FileStream != null)
        {
            m_FileStream.Dispose();
            m_FileStream = null;
        }
        m_Disposed = true;
    }
}