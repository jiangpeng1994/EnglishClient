using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

public class ThreadDownLoad : MonoBehaviour
{
    /// <summary>
    /// 下载线程
    /// </summary>
    private Thread _downLoadThread;
    /// <summary>
    /// 锁对象
    /// </summary>
    private static readonly object _lockObj = new object();
    /// <summary>
    /// 下载文件队列
    /// </summary>
    private Queue<RemoteFileInfo> _downLoadFileQueue = new Queue<RemoteFileInfo>();
    /// <summary>
    /// 下载数据变化回调
    /// </summary>
    private Action<RemoteFileInfo> _downLoadUpdateCallBack;
    private bool isDownLoadUpdate = false;

    /// <summary>
    /// 添加下载文件到下载队列中
    /// </summary>
    public void AddDownLoadFile(RemoteFileInfo remoteFileInfo, Action<RemoteFileInfo> callBack)
    {
        lock (_lockObj)
        {
            _downLoadFileQueue.Enqueue(remoteFileInfo);
            _downLoadUpdateCallBack = callBack;
        }
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    public void StartDownLoad()
    {
        isDownLoadUpdate = true;
        _downLoadThread = new Thread(DownLoadUpdate);
        _downLoadThread.Start();
    }

    /// <summary>
    /// 下载线程的方法：下载循环
    /// </summary>
    void DownLoadUpdate()
    {
        Debug.Log("下载线程开启");
        while (isDownLoadUpdate)
        {
            lock (_lockObj)
            {
                if (_downLoadFileQueue.Count > 0)
                {
                    RemoteFileInfo downLoadFileInfo = _downLoadFileQueue.Dequeue();
                    try
                    {
                        HttpDownLoad(downLoadFileInfo);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            Thread.Sleep(1);
        }

        ClearEvent();
        Debug.Log("下载线程结束");
    }

    /// <summary>
    /// Http下载
    /// </summary>
    /// <param name="remoteFileInfo">当前需要下载的文件信息</param>
    private void HttpDownLoad(RemoteFileInfo remoteFileInfo)
    {
        try
        {
            // 该文件没有下载完毕
            if (!remoteFileInfo.isDownLoadFinish)
            {
                ClearDownLoadFileInfoStuff(remoteFileInfo);

                //string requestUrl =  Ipv6Utility.FinalUrl(remoteFileInfo.remoteUrl);
                remoteFileInfo.httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(remoteFileInfo.remoteUrl));

                // 本地存在该文件（存在本地却没有下完，即下载了部分到本地，则需要断点续传）
                if (File.Exists(remoteFileInfo.localUrl))
                {
                    remoteFileInfo.fileStream = File.OpenWrite(remoteFileInfo.localUrl);
                    // 移动本地文件流中的指针到末尾，接着上次下载写入到的断点，接着继续写入本地
                    remoteFileInfo.fileStream.Seek(remoteFileInfo.fileStream.Length, SeekOrigin.Current);
                    // 请求远程文件中需要下载的数据，是从上次下载到的断点开始，接着继续下载
                    remoteFileInfo.httpWebRequest.AddRange((int)remoteFileInfo.fileStream.Length);
                }
                // 本地不存在该文件（该文件还未开始下）
                else
                {
                    remoteFileInfo.fileStream = File.Open(remoteFileInfo.localUrl, FileMode.Create);
                }

                remoteFileInfo.httpWebRequest.BeginGetResponse(new AsyncCallback(HttpDownLoadCallBack), remoteFileInfo);
            }
        }
        catch (Exception ex)
        {
            ClearDownLoadFileInfoStuff(remoteFileInfo);
        }
    }

    private const int bufferbytes = 1024;
    /// <summary>
    /// Http下载回调
    /// </summary>
    private void HttpDownLoadCallBack(IAsyncResult asyncResult)
    {
        lock(_lockObj)
        {
            RemoteFileInfo remoteFileInfo = (RemoteFileInfo)asyncResult.AsyncState;
            try
            {
                remoteFileInfo.httpWebResponse = (HttpWebResponse)(remoteFileInfo.httpWebRequest.EndGetResponse(asyncResult));
                remoteFileInfo.responseStream = remoteFileInfo.httpWebResponse.GetResponseStream();
                remoteFileInfo.totalSize = remoteFileInfo.responseStream.Length;
                Debug.Log("远程大小："+ remoteFileInfo.responseStream.Length);
                byte[] nbytes = new byte[bufferbytes];
                int nReadSize = 0;
                nReadSize = remoteFileInfo.responseStream.Read(nbytes, 0, bufferbytes);
                while (nReadSize > 0)
                {
                    if (isDownLoadUpdate && remoteFileInfo.fileStream != null)
                    {
                        // 写入本地
                        remoteFileInfo.fileStream.Write(nbytes, 0, nReadSize);
                        remoteFileInfo.downLoadSize += nReadSize;
                        nReadSize = remoteFileInfo.responseStream.Read(nbytes, 0, bufferbytes);
                        OnSyncEvent(remoteFileInfo);
                    }
                }
                ClearDownLoadFileInfoStuff(remoteFileInfo);

                if (remoteFileInfo.downLoadSize >= remoteFileInfo.totalSize)
                {
                    remoteFileInfo.isDownLoadFinish = true;
                }
                OnSyncEvent(remoteFileInfo);
                isDownLoadUpdate = false;
            }
            catch (Exception ex)
            {
                ClearDownLoadFileInfoStuff(remoteFileInfo);
            }
        }
    }

    private void ClearDownLoadFileInfoStuff(RemoteFileInfo remoteFileInfo)
    {
        if (remoteFileInfo == null)
        {
            return;
        }

        if (remoteFileInfo.httpWebRequest != null)
        {
            remoteFileInfo.httpWebRequest.Abort();
            remoteFileInfo.httpWebRequest = null;
        }

        if (remoteFileInfo.httpWebResponse != null)
        {
            remoteFileInfo.httpWebResponse.Close();
            remoteFileInfo.httpWebResponse = null;
        }

        if (remoteFileInfo.responseStream != null)
        {
            remoteFileInfo.responseStream.Close();
            remoteFileInfo.responseStream.Dispose();
            remoteFileInfo.responseStream = null;
        }

        if (remoteFileInfo.fileStream != null)
        {
            remoteFileInfo.fileStream.Close();
            remoteFileInfo.fileStream.Dispose();
            remoteFileInfo.fileStream = null;
        }
    }

    /// <summary>
    /// 异步事件：通知表现层下载数据发生变化
    /// </summary>
    /// <param name="remoteFileInfo">当前下载数据</param>
    private void OnSyncEvent(RemoteFileInfo remoteFileInfo)
    {
        _downLoadUpdateCallBack?.Invoke(remoteFileInfo);
    }

    public void ClearEvent()
    {
        lock (_lockObj)
        {
            _downLoadFileQueue.Clear();
        }
    }

    public void StopDownLoad()
    {
        isDownLoadUpdate = false;
    }

    void OnDestroy()
    {
        isDownLoadUpdate = false;
        _downLoadThread = null;
    }
}

public class RemoteFileInfo
{
    public string remoteUrl;
    public string localUrl;
    public long size;
    public long downLoadSize = 0;
    public long totalSize;
    public string md5;
    public bool isDownLoadFinish = false;

    public HttpWebRequest httpWebRequest = null;
    public HttpWebResponse httpWebResponse = null;
    public Stream responseStream = null;
    public Stream fileStream = null;
}