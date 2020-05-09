using System.Collections.Generic;
using UnityEngine;

public class DownloadUI : MonoBehaviour
{
    public GameObject downloadUI;
    public UISlider ProgressSlider;
    public UILabel ProgressLabel;

    private GameObject DownloadObj = null;
    private DownloadComponent downloadComponent = null;
    private GameObject UnZipObj = null;
    private UnZipComponent unZipComponent = null;
    private Dictionary<int, DownloadItem> DownloadTaskList = null;
    private Dictionary<int, DownloadItem> UnZipTaskList = null;

    private float DownloadProgressVaule = 0;
    private float UnzipProgressVaule = 0;
    private string DownloadProgressText = "正在为您准备下载资源包";
    private string UnzipProgressText = "正在为您解压资源包(此过程不消耗流量),请耐心等待";
    /// <summary>
    /// 0:未启用 1：下载中 2：解压中 3：完成
    /// </summary>
    private int DownloadStatus = 0;
    private long TotalNeedDownloadLength = 0;
    private string TotalNeedDownloadLengthStr = "";
    private long AlreadyDownloadLength = 0;
    private bool isAllDownloaded = false;
    private int autoRetryTimes = 0;
    private int maxAutoRetryTimes = 3;
    private long TotalNeedUnZipLength = 0;
    private long AlreadyUnZipLength = 0;

    void Awake()
    {
        //downloadUI.SetActive(false);
        DownloadTaskList = new Dictionary<int, DownloadItem>();
        UnZipTaskList = new Dictionary<int, DownloadItem>();
    }

    /// <summary>
    /// 添加下载任务。
    /// </summary>
    /// <param name="TaskId">任务编号。</param>
    /// <param name="downloadUrl">下载地址。</param>
    /// <param name="downloadPath">下载保存地址。</param>
    /// <param name="fileLength">文件大小。</param>
    /// <param name="isDownloaded">是否已下载。</param>
    /// <param name="unZipPath">解压保存地址。</param>
    /// <param name="isUnZiped">是否已解压。</param>
    public void AddDownloadTask(string TaskId, string downloadUrl, string downloadPath, long fileLength, bool isDownloaded, string unZipPath, bool isUnZiped)
    {
        DownloadItem downloadItem = new DownloadItem(TaskId, downloadUrl, downloadPath, fileLength, isDownloaded, unZipPath, isUnZiped);

        // 任务需要下载且没有创建下载器，则创建下载器。
        if(!isDownloaded && downloadComponent == null)
        {
            DownloadObj = new GameObject("DownloadComponent");
            downloadComponent = DownloadObj.AddComponent<DownloadComponent>();
            downloadComponent.CreateDownloadAgent(1);
        }

        //  任务需要解压且没有创建解压器，则创建解压器。
        if (!isUnZiped && unZipComponent == null)
        {
            UnZipObj = new GameObject("UnZipComponent");
            unZipComponent = UnZipObj.AddComponent<UnZipComponent>();
            unZipComponent.CreateUnZipAgent(1);
        }

        if (isDownloaded)
        {
            // 已经下载。
            if (!isUnZiped)
            {
                // 还未解压,添加解压任务。
                int unZipTaskID = unZipComponent.AddUnZipTask(downloadItem.downloadPath, downloadItem.unZipPath, 0, true);
                UnZipTaskList.Add(unZipTaskID, downloadItem);
            }
        }
        else
        {
            // 还未下载,添加下载任务。
            int downloadTaskID = downloadComponent.AddDownloadTask(downloadItem.downloadUrl, downloadItem.downloadPath);
            DownloadTaskList.Add(downloadTaskID, downloadItem);
        }
    }

    /// <summary>
    /// 开始下载。
    /// </summary>
    public void Download()
    {
        ReInit();
        foreach (DownloadItem item in DownloadTaskList.Values)
        {
            // 计算需要下载的总大小，待下载任务部分。
            TotalNeedDownloadLength = TotalNeedDownloadLength + item.fileLength;
            // 计算需要解压的总大小，待下载任务部分。
            TotalNeedUnZipLength = TotalNeedUnZipLength + item.fileLength;
        }

        foreach (DownloadItem item in UnZipTaskList.Values)
        {
            // 计算需要下载的总大小，已下载任务部分。
            TotalNeedDownloadLength = TotalNeedDownloadLength + item.fileLength;
            // 计算需要解压的总大小，已下载任务部分。
            TotalNeedUnZipLength = TotalNeedUnZipLength + item.fileLength;
            // 计算已经下载的大小，即已下载任务部分的大小。
            AlreadyDownloadLength = AlreadyDownloadLength + item.fileLength;
        }

        // 没有下载 / 解压的任务，直接终止。
        if (TotalNeedUnZipLength == 0)
        {
            UnInit();
            return;
        }

        TotalNeedDownloadLengthStr = DownloadTool.ByteUnitConversion(TotalNeedDownloadLength);
        if (AlreadyDownloadLength == TotalNeedDownloadLength)
        {
            // 全部下载完毕。
            isAllDownloaded = true;
        }

        // 开始下载
        ShowUI();
    }

    /// <summary>
    /// 显示下载进度ui。
    /// </summary>
    private void ShowUI()
    {
        if (!IsTheAvailableStorageSizeEnough())
        {
            return;
        }

        // 注册监听事件
        RegistEvent();

        // 展示进度界面。
        downloadUI.SetActive(true);

        if (isAllDownloaded)
        {
            // 全部下载完毕,直接开始解压。
            DownloadStatus = 2;
            StartUnZip();
        } else
        {
            // 开始下载。
            StartDownload();
        }
    }

    /// <summary>
    /// 注册监听事件。
    /// </summary>
    private void RegistEvent()
    {
        GlobalEvent.AddEvent("MSG_ON_DOWNLOAD_START", OnDownloadStart);
        GlobalEvent.AddEvent("MSG_ON_DOWNLOAD_UPDATE", OnDownloadUpdate);
        GlobalEvent.AddEvent("MSG_ON_DOWNLOAD_SUCCESS", OnDownloadSuccess);
        GlobalEvent.AddEvent("MSG_ON_DOWNLOAD_FAILURE", OnDownloadFailure);

        GlobalEvent.AddEvent("MSG_ON_UNZIP_START", OnUnZipStart);
        GlobalEvent.AddEvent("MSG_ON_UNZIP_UPDATE", OnUnZipUpdate);
        GlobalEvent.AddEvent("MSG_ON_UNZIP_SUCCESS", OnUnZipSuccess);
        GlobalEvent.AddEvent("MSG_ON_UNZIP_FAILURE", OnUnZipFailure);
    }

    /// <summary>
    /// 设备可用存储大小是否足够。
    /// </summary>
    /// <returns>设备可用存储大小是否足够</returns>
    private bool IsTheAvailableStorageSizeEnough()
    {
        if (!isAllDownloaded)
        {
            // 下载空间是否足够。
            if (!DownloadTool.IsTheAvailableStorageSizeEnough(TotalNeedUnZipLength * 2))
            {
                // 提示空间不足。
                GameTools.Instance.MsgShow("手机空间不足，本次下载需要"+ DownloadTool.ByteUnitConversion(TotalNeedUnZipLength) + "的存储空间，请清理后再尝试",
                "提示",
                null,
                () => {
                    {
                        Close();
                        Application.Quit();
                    }
                }, false, null, "取消");
                return false;
            } else
            {
                return true;
            }
        } else
        {
            // 解压空间是否足够。
            if (!DownloadTool.IsTheAvailableStorageSizeEnough(TotalNeedUnZipLength * 2))
            {
                // 提示空间不足。
                GameTools.Instance.MsgShow("手机空间不足，本次下载需要" + DownloadTool.ByteUnitConversion(TotalNeedUnZipLength) + "的存储空间，请清理后再尝试",
                "提示",
                null,
                () => {
                    {
                        Close();
                        Application.Quit();
                    }
                }, false, null, "取消");
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// 开始下载。
    /// </summary>
    private void StartDownload()
    {
        if (downloadComponent == null)
        {
            return;
        }

        downloadComponent.StartDownload();
    }

    /// <summary>
    /// 开始解压。
    /// </summary>
    private void StartUnZip()
    {
        if (unZipComponent == null)
        {
            return;
        }

        unZipComponent.StartUnZip();
    }

    /// <summary>
    /// 回调：任务开始下载。
    /// </summary>
    private void OnDownloadStart(params object[] objs)
    {
        DownloadEventArgs downloadEventArgs = (DownloadEventArgs)objs[0];
        Debug.Log("开始下载任务：" + downloadEventArgs.SerialId);
        DownloadStatus = 1;
    }

    /// <summary>
    /// 回调：任务下载的数据更新。
    /// </summary>
    private void OnDownloadUpdate(params object[] objs)
    {
        DownloadEventArgs downloadEventArgs = (DownloadEventArgs)objs[0];
        long AllDownloadLength = AlreadyDownloadLength + downloadEventArgs.SavedLength;
        DownloadProgressVaule = (float)AllDownloadLength / TotalNeedDownloadLength;
        DownloadProgressText = DownloadTool.ByteUnitConversion(AllDownloadLength) + "/" + TotalNeedDownloadLengthStr;
    }

    /// <summary>
    /// 回调：任务下载完成。
    /// </summary>
    private void OnDownloadSuccess(params object[] objs)
    {
        DownloadEventArgs downloadEventArgs = (DownloadEventArgs)objs[0];
        Debug.Log("任务下载完成,任务ID为：" + downloadEventArgs.SerialId);
        
        // 从下载任务列表中移除，再添加到解压任务列表中。
        DownloadItem downloadItem = DownloadTaskList[downloadEventArgs.SerialId];
        downloadItem.isDownloaded = true;
        DownloadTaskList.Remove(downloadEventArgs.SerialId);
        int unZipTaskID = unZipComponent.AddUnZipTask(downloadItem.downloadPath, downloadItem.unZipPath, 0, true);
        UnZipTaskList.Add(unZipTaskID, downloadItem);

        FinishRecord(downloadItem, 1);

        // 更新下载进度数据。
        AlreadyDownloadLength = AlreadyDownloadLength + downloadEventArgs.SavedLength;
        DownloadProgressVaule = AlreadyDownloadLength / TotalNeedDownloadLength;

        // 如果全部下载完成，则结束下载，开始解压。
        if (DownloadProgressVaule == 1)
        {
            downloadComponent.PauseDownload();
            DownloadStatus = 2;
            StartUnZip();
        }
    }

    /// <summary>
    /// 回调：任务下载失败。
    /// </summary>
    private void OnDownloadFailure(params object[] objs)
    {
        DownloadEventArgs downloadEventArgs = (DownloadEventArgs)objs[0];
        Debug.LogWarning("DownloadFailure，ErrorCode：" + downloadEventArgs.ErrorCode + "  ErrorMessage：" + downloadEventArgs.ErrorMessage);
        //  异常提示：1:下载请求异常(自动重试3次) 2:IO读写异常 3:请求超时异常(自动重试3次) 4:MD5校验不通过
        DownloadFailureTip(downloadEventArgs.ErrorCode);
    }

    /// <summary>
    /// 下载异常提示。
    /// </summary>
    /// <param name="errorCode">错误码。</param>
    private void DownloadFailureTip(int errorCode)
    {
        GameTools.Instance.MsgShow("下载异常，请检查网络状况，再进行尝试（错误码：" + errorCode + "）",
                "提示",
                () => {
                    {
                        Close();
                    }
                },
                () => {
                    {
                        StartDownload();
                    }
                }, false, "退出", "重试");
    }

    /// <summary>
    /// 回调：任务开始解压。
    /// </summary>
    private void OnUnZipStart(params object[] objs)
    {
        UnZipEventArgs unZipEventArgs = (UnZipEventArgs)objs[0];
        Debug.Log("开始解压任务：" + unZipEventArgs.SerialId);
    }

    /// <summary>
    /// 回调：任务解压的数据更新。
    /// </summary>
    private void OnUnZipUpdate(params object[] objs)
    {
        UnZipEventArgs unZipEventArgs = (UnZipEventArgs)objs[0];
        float AllUnZipLength = AlreadyUnZipLength + unZipEventArgs.UnZipProgress * UnZipTaskList[unZipEventArgs.SerialId].fileLength;
        UnzipProgressVaule = AllUnZipLength / TotalNeedUnZipLength;
    }

    /// <summary>
    /// 回调：任务解压完成。
    /// </summary>
    private void OnUnZipSuccess(params object[] objs)
    {
        UnZipEventArgs unZipEventArgs = (UnZipEventArgs)objs[0];
        Debug.Log("解压完成,任务ID为：" + unZipEventArgs.SerialId);

        // 从解压任务列表中移除。
        DownloadItem downloadItem = UnZipTaskList[unZipEventArgs.SerialId];
        downloadItem.isUnZiped = true;
        UnZipTaskList.Remove(unZipEventArgs.SerialId);

        FinishRecord(downloadItem, 2);

        // 更新下载进度数据。
        AlreadyUnZipLength = AlreadyUnZipLength + downloadItem.fileLength;
        UnzipProgressVaule = AlreadyUnZipLength / TotalNeedUnZipLength;

        // 如果全部解压完成，则结束解压。
        if (UnzipProgressVaule == 1)
        {
            unZipComponent.PauseUnZip();
            DownloadStatus = 3;
        }
    }

    /// <summary>
    /// 回调：任务解压失败。
    /// </summary>
    private void OnUnZipFailure(params object[] objs)
    {
        UnZipEventArgs unZipEventArgs = (UnZipEventArgs)objs[0];
        Debug.LogWarning("UnZipFailure，ErrorCode：" + unZipEventArgs.ErrorCode + "  ErrorMessage：" + unZipEventArgs.ErrorMessage);
        //  异常提示：1:没有解压源文件 2:解压失败
        if (unZipEventArgs.ErrorCode == 1)
        {
            UnZipFailureTip(unZipEventArgs.ErrorCode);
        }
        else if (unZipEventArgs.ErrorCode == 2)
        {
            // 该情况尝试自动重试。
            UnZipFailureTip(unZipEventArgs.ErrorCode);
        }
    }

    /// <summary>
    /// 解压异常提示。
    /// </summary>
    /// <param name="errorCode">错误码。</param>
    private void UnZipFailureTip(int errorCode)
    {
        GameTools.Instance.MsgShow("解压异常，是否进行重试(错误码：" + errorCode + ")",
                "提示",
                () => {
                    {
                        Close();
                    }
                },
                () => {
                    {
                        StartUnZip();
                    }
                }, false, "退出", "重试");
    }

    /// <summary>
    /// 完成记录。
    /// </summary>
    /// <param name="item">下载任务。</param>
    /// <param name="type">完成类型。</param>
    private void FinishRecord(DownloadItem item, int type)
    {
        string typeStr = "";
        if (type == 1)
        {
            // 下载完成。
            typeStr = "IsDownload-";
        } else
        {
            // 解压完成。
            typeStr = "IsUnZip-";
        }
        GameDataManager.SetBool(typeStr + item.downloadId, true);
    }

    /// <summary>
    /// 界面更新。
    /// </summary>
    private void Update()
    {
        if (!downloadUI.activeSelf)
        {
            return;
        }

        switch (DownloadStatus)
        {
            case 0:
                // 未启动。
                ProgressSlider.value = DownloadProgressVaule;
                ProgressLabel.text = DownloadProgressText;
                break;
            case 1:
                // 下载中。
                ProgressSlider.value = DownloadProgressVaule;
                ProgressLabel.text = DownloadProgressText;
                break;
            case 2:
                // 解压中。
                ProgressSlider.value = UnzipProgressVaule;
                ProgressLabel.text = UnzipProgressText;
                break;
            case 3:
                // 完成。
                ProgressSlider.value = 1;
                ProgressLabel.text = "下载完毕，祝您使用愉快。";
                Close();
                break;
        }
    }

    /// <summary>
    /// 重置参数。
    /// </summary>
    private void ReInit()
    {
        DownloadProgressVaule = 0;
        UnzipProgressVaule = 0;
        DownloadProgressText = "正在为您准备下载资源包";
        UnzipProgressText = "正在为您解压资源包(此过程不消耗流量),请耐心等待";
        DownloadStatus = 0;
        TotalNeedDownloadLength = 0;
        TotalNeedDownloadLengthStr = "";
        AlreadyDownloadLength = 0;
        isAllDownloaded = false;
        autoRetryTimes = 0;
        maxAutoRetryTimes = 3;
        TotalNeedUnZipLength = 0;
        AlreadyUnZipLength = 0;
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    private void Close()
    {
        if (gameObject == null)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnInit();
    }

    /// <summary>
    /// 注销时反初始化。
    /// </summary>
    private void UnInit()
    {
        ReInit();
        if (downloadComponent != null)
        { 
            downloadComponent.PauseDownload();
            downloadComponent.RemoveAllDownloadTasks();
            downloadComponent = null;
        }
        if (DownloadObj != null)
        {
            Destroy(DownloadObj);
            DownloadObj = null;
        }

        if (unZipComponent != null)
        {
            unZipComponent.PauseUnZip();
            unZipComponent.RemoveAllUnZipTasks();
            unZipComponent = null;
        }
        if (UnZipObj != null)
        {
            Destroy(UnZipObj);
            UnZipObj = null;
        }

        DownloadTaskList.Clear();
        UnZipTaskList.Clear();

        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_START", OnDownloadStart);
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_UPDATE", OnDownloadUpdate);
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_SUCCESS", OnDownloadSuccess);
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_FAILURE", OnDownloadFailure);

        GlobalEvent.RemoveEvent("MSG_ON_UNZIP_START", OnUnZipStart);
        GlobalEvent.RemoveEvent("MSG_ON_UNZIP_UPDATE", OnUnZipUpdate);
        GlobalEvent.RemoveEvent("MSG_ON_UNZIP_SUCCESS", OnUnZipSuccess);
        GlobalEvent.RemoveEvent("MSG_ON_UNZIP_FAILURE", OnUnZipFailure);
    }
}

public class DownloadItem
{
    public string downloadId;
    public string downloadUrl;
    public string downloadPath;
    public long fileLength;
    public bool isDownloaded;
    public string unZipPath;
    public bool isUnZiped;

    public DownloadItem(string downloadId, string downloadUrl, string downloadPath, long fileLength, bool isDownloaded, string unZipPath, bool isUnZiped)
    {
        this.downloadId = downloadId;
        this.downloadUrl = downloadUrl;
        this.downloadPath = downloadPath;
        this.fileLength = fileLength;
        this.isDownloaded = isDownloaded;
        this.unZipPath = unZipPath;
        this.isUnZiped = isUnZiped;
    }
}