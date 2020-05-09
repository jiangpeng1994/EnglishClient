using UnityEngine;

/// <summary>
/// 下载组件。
/// </summary>
[DisallowMultipleComponent]
public class DownloadComponent : MonoBehaviour
{
    /// <summary>
    /// 下载管理器。
    /// </summary>
    private DownloadManager m_DownloadManager = null;

    /// <summary>
    /// 获取下载是否被暂停。
    /// </summary>
    public bool Paused
    {
        get
        {
            return m_DownloadManager.Paused;
        }
    }

    /// <summary>
    /// 获取当前下载速度。
    /// </summary>
    public float CurrentSpeed
    {
        get
        {
            return m_DownloadManager.CurrentSpeed;
        }
    }

    /// <summary>
    /// 获取或设置下载超时时长，以秒为单位。
    /// </summary>
    public float Timeout
    {
        get
        {
            return m_DownloadManager.Timeout;
        }
        set
        {
            m_DownloadManager.Timeout = value;
        }
    }

    /// <summary>
    /// 获取或设置将缓冲区写入磁盘的临界大小。
    /// </summary>
    public int FlushSize
    {
        get
        {
            return m_DownloadManager.FlushSize;
        }
        set
        {
            m_DownloadManager.FlushSize = value;
        }
    }

    /// <summary>
    /// 下载组件初始化。
    /// </summary>
    private void Awake()
    {
        m_DownloadManager = new DownloadManager();
        m_DownloadManager.DownloadStart += OnDownloadStart;
        m_DownloadManager.DownloadUpdate += OnDownloadUpdate;
        m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
        m_DownloadManager.DownloadFailure += OnDownloadFailure;
    }

    /// <summary>
    /// 下载组件循环。
    /// </summary>
    private void Update()
    {
        if (m_DownloadManager == null)
        {
            return;
        }

        m_DownloadManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    /// <summary>
    /// 创建下载代理。
    /// </summary>
    public void CreateDownloadAgent(int tpye)
    {
        if (tpye == 1)
        {
            // 创建队列下载型的代理(多个任务依次下载，下载完成一个后再开始下一个)
            m_DownloadManager.CreateQueueDownloadAgent(transform);
        } else
        {
            // 创建并行下载型的代理(多个任务并行下载)
        }
    }

    /// <summary>
    /// 增加下载任务。
    /// </summary>
    /// <param name="downloadUrl">原始下载地址。</param>
    /// <param name="downloadPath">下载后存放路径。</param>
    /// <returns>新增下载任务的序列编号。</returns>
    public int AddDownloadTask(string downloadUrl, string downloadPath)
    {
        return m_DownloadManager.AddDownloadTask(Ipv6Utility.FinalUrl(downloadUrl), downloadPath);
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
        return m_DownloadManager.AddDownloadTask(Ipv6Utility.FinalUrl(downloadUrl), downloadPath, md5);
    }

    /// <summary>
    /// 移除下载任务。
    /// </summary>
    /// <param name="serialId">要移除下载任务的序列编号。</param>
    public void RemoveDownloadTask(int serialId)
    {
        m_DownloadManager.RemoveDownloadTask(serialId);
    }

    /// <summary>
    /// 移除所有下载任务。
    /// </summary>
    public void RemoveAllDownloadTasks()
    {
        m_DownloadManager.RemoveAllDownloadTasks();
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    public void StartDownload()
    {
        m_DownloadManager.StartDownload();
    }

    /// <summary>
    /// 暂停下载
    /// </summary>
    public void PauseDownload()
    {
        m_DownloadManager.PauseDownload();
    }

    /// <summary>
    /// 应用层的回调：任务开始下载。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadStart(object sender, DownloadEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_DOWNLOAD_START", e);
    }

    /// <summary>
    ///  应用层的回调：任务下载的数据更新。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadUpdate(object sender, DownloadEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_DOWNLOAD_UPDATE", e);
    }

    /// <summary>
    ///  应用层的回调：任务下载完成。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadSuccess(object sender, DownloadEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_DOWNLOAD_SUCCESS", e);
    }

    /// <summary>
    /// 应用层的回调：任务下载失败。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnDownloadFailure(object sender, DownloadEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_DOWNLOAD_FAILURE", e);
    }
}