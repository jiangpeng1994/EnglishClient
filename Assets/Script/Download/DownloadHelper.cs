using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 使用 UnityWebRequest 实现的下载辅助器。
/// </summary>
public class DownloadHelper : MonoBehaviour, IDisposable
{
    private const int CachedBytesLength = 0x1000; //4096 [4kb]
    private readonly byte[] m_CachedBytes = new byte[CachedBytesLength];

    private UnityWebRequest m_UnityWebRequest = null;
    private bool m_Disposed = false;

    private EventHandler<DownloadHelperEventArgs> m_DownloadAgentHelperUpdateBytesEventHandler = null;
    private EventHandler<DownloadHelperEventArgs> m_DownloadAgentHelperCompleteEventHandler = null;
    private EventHandler<DownloadHelperEventArgs> m_DownloadAgentHelperErrorEventHandler = null;

    /// <summary>
    /// 下载代理辅助器更新数据流事件。
    /// </summary>
    public event EventHandler<DownloadHelperEventArgs> DownloadAgentHelperUpdateBytes
    {
        add
        {
            m_DownloadAgentHelperUpdateBytesEventHandler += value;
        }
        remove
        {
            m_DownloadAgentHelperUpdateBytesEventHandler -= value;
        }
    }

    /// <summary>
    /// 下载代理辅助器完成事件。
    /// </summary>
    public event EventHandler<DownloadHelperEventArgs> DownloadAgentHelperComplete
    {
        add
        {
            m_DownloadAgentHelperCompleteEventHandler += value;
        }
        remove
        {
            m_DownloadAgentHelperCompleteEventHandler -= value;
        }
    }

    /// <summary>
    /// 下载代理辅助器错误事件。
    /// </summary>
    public event EventHandler<DownloadHelperEventArgs> DownloadAgentHelperError
    {
        add
        {
            m_DownloadAgentHelperErrorEventHandler += value;
        }
        remove
        {
            m_DownloadAgentHelperErrorEventHandler -= value;
        }
    }

    /// <summary>
    /// 通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void Download(string downloadUri, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Debug.LogWarning("Download agent helper handler is invalid.");
            return;
        }

        m_UnityWebRequest = new UnityWebRequest(downloadUri);
        m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
        m_UnityWebRequest.Send();
    }

    /// <summary>
    /// 通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。(用于断点续传)</param>
    /// <param name="userData">用户自定义数据。</param>
    public void Download(string downloadUri, int fromPosition, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Debug.LogWarning("Download agent helper handler is invalid.");
            return;
        }

        m_UnityWebRequest = new UnityWebRequest(downloadUri);
        m_UnityWebRequest.SetRequestHeader("Range",string.Format("bytes={0}-", fromPosition.ToString()));
        m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
        m_UnityWebRequest.Send();
    }

    /// <summary>
    /// 通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="toPosition">下载数据结束位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void Download(string downloadUri, int fromPosition, int toPosition, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Debug.LogWarning("Download agent helper handler is invalid.");
            return;
        }

        m_UnityWebRequest = new UnityWebRequest(downloadUri);
        m_UnityWebRequest.SetRequestHeader("Range", string.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()));
        m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
        m_UnityWebRequest.Send();
    }

    /// <summary>
    /// 重置下载代理辅助器。
    /// </summary>
    public void Reset()
    {
        if (m_UnityWebRequest != null)
        {
            m_UnityWebRequest.Abort();
            m_UnityWebRequest.Dispose();
            m_UnityWebRequest = null;
        }

        Array.Clear(m_CachedBytes, 0, CachedBytesLength);
    }

    /// <summary>
    /// 下载器循环
    /// </summary>
    private void Update()
    {
        if (m_UnityWebRequest == null)
        {
            return;
        }

        if (!m_UnityWebRequest.isDone)
        {
            return;
        }

        bool isError = false;

        isError = m_UnityWebRequest.isNetworkError;

        if (isError)
        {
            DownloadHelperEventArgs downloadAgentHelperErrorEventArgs = DownloadHelperEventArgs.Create(1, m_UnityWebRequest.error);
            m_DownloadAgentHelperErrorEventHandler(this, downloadAgentHelperErrorEventArgs);
        }
        else
        {
            DownloadHelperEventArgs downloadAgentHelperCompleteEventArgs = DownloadHelperEventArgs.Create((int)m_UnityWebRequest.downloadedBytes);
            m_DownloadAgentHelperCompleteEventHandler(this, downloadAgentHelperCompleteEventArgs);
        }

        Reset();
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

        if (disposing && m_UnityWebRequest != null)
        {
            m_UnityWebRequest.Dispose();
            m_UnityWebRequest = null;
        }
        m_Disposed = true;
    }

    private sealed class DownloadHandler : DownloadHandlerScript
    {
        private readonly DownloadHelper m_Owner;

        public DownloadHandler(DownloadHelper owner)
            : base(owner.m_CachedBytes)
        {
            m_Owner = owner;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (m_Owner != null && m_Owner.m_UnityWebRequest != null && dataLength > 0)
            {
                DownloadHelperEventArgs downloadAgentHelperUpdateBytesEventArgs = DownloadHelperEventArgs.Create(data, 0, dataLength, m_Owner.m_UnityWebRequest.downloadProgress);
                m_Owner.m_DownloadAgentHelperUpdateBytesEventHandler(this, downloadAgentHelperUpdateBytesEventArgs);
            }

            return base.ReceiveData(data, dataLength);
        }
    }
}