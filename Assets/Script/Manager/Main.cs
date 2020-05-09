using System;
using System.IO;
using cn.bmob.api;
using Sproto;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
        WindowManager.instance.Initialize();
    }

    public void Start()
    {
        Init();
        CheckCanEnter();
    }

    private void CheckCanEnter()
    {
        BmobUnity bmobUnity = gameObject.GetComponent<BmobUnity>();
        bmobUnity.Get<CanLogin>("canLogin", "NcSl555G", (resp, exception) =>
        {
            if (exception != null)
            {
                return;
            }

            CanLogin canLogin = resp;
            if (canLogin.status.ToString().Equals("True"))
            {
                print("允许登录");
            }
            else
            {
                print("拒绝登录");
                WindowManager.instance.Open<Dont>();
            }
        });
    }

    private void Init()
    {
        NetCore.Init();
        NetReceiver.Init();
        NetSender.Init();
        LoadLoginPanel();
        LoadDownLoadInfo();
        //屏幕常亮
        Screen.sleepTimeout = -1;
        //设置安卓屏幕分辨率
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.SetResolution(1280, 720, false);
        }
    }

    private static void LoadLoginPanel()
    {
        GameObject guiPanel = GameObject.Find("GUI/GUICamera");
        DontDestroyOnLoad(GameObject.Find("GUI"));
        Transform _transform = (Instantiate(Resources.Load("UI/UILoginPanel")) as GameObject).transform;
        _transform.parent = guiPanel.transform;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
    }

    string url = "https://englishpal2019-1300493262.cos.ap-shanghai.myqcloud.com/SPACE%20TRAVEL%20unit%20resources/Resource/DownLoadInfo.txt";
    private void LoadDownLoadInfo()
    {
        string downLoadInfoPath = Path.Combine(Application.persistentDataPath, "DownLoadInfo.txt");
        if (File.Exists(downLoadInfoPath))
        {
            GetDownLoadInfo();
        } else
        {
            GameObject DownloadObj = new GameObject("DownloadComponent");
            DownloadComponent downloadComponent = DownloadObj.AddComponent<DownloadComponent>();
            downloadComponent.CreateDownloadAgent(1);
            int downloadTaskID = downloadComponent.AddDownloadTask(url, downLoadInfoPath);
            GlobalEvent.AddEvent("MSG_ON_DOWNLOAD_SUCCESS", OnDownloadSuccess);
            GlobalEvent.AddEvent("MSG_ON_DOWNLOAD_FAILURE", OnDownloadFailure);
            downloadComponent.StartDownload();
        }
    }

    private void GetDownLoadInfo()
    {
        try
        {
            string downLoadInfoPath = Path.Combine(Application.persistentDataPath, "DownLoadInfo.txt");
            using (FileStream sourceFileStream = File.OpenRead(downLoadInfoPath))
            {
                using (StreamReader sr = new StreamReader(sourceFileStream))
                {
                    string line;
                    // 从文件读取并显示行，直到文件的末尾
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] content = line.Split('|');
                        DownLoadInfo downLoadInfo = new DownLoadInfo(content[1], Convert.ToInt64(content[2]));
                        DataManager.GetInstance().downLoadInfoList.Add(content[0], downLoadInfo);
                    }
                }
            }
            Debug.Log("资源信息加载成功。");
        }
        catch (Exception e)
        {
            Debug.Log("资源信息加载失败。");
        }
    }

    private void OnDownloadSuccess(object[] args)
    {
        Debug.Log("资源信息下载成功。");
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_SUCCESS", OnDownloadSuccess);
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_FAILURE", OnDownloadFailure);
        GetDownLoadInfo();
    }

    private void OnDownloadFailure(object[] args)
    {
        Debug.Log("资源信息下载失败。");
        DownloadEventArgs downloadEventArgs = (DownloadEventArgs)args[0];
        Debug.LogWarning("DownloadFailure，ErrorCode：" + downloadEventArgs.ErrorCode + "  ErrorMessage：" + downloadEventArgs.ErrorMessage);
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_SUCCESS", OnDownloadSuccess);
        GlobalEvent.RemoveEvent("MSG_ON_DOWNLOAD_FAILURE", OnDownloadFailure);
    }

    public static bool isStart = false;
    public static bool isStartHeartBeat = false;
    void Update()
    {
        NetCore.Dispatch();
        WindowManager.instance.Update();

        //监听安卓返回键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameTools.Instance.MsgShow("是否退出游戏", "提示", () => { }, () => { NetCore.Disconnect(); Application.Quit(); }, true, "返回", "确定");
        }

        if (isStart)
        {
            if (!NetCore.connected)
            {
                CancelInvoke("HeartBeat");
                Debug.Log("断线，尝试重连");
                NetCore.Disconnect();
                NetCore.Init();
                NetReceiver.Init();
                NetSender.Init();
                NetCore.Connect(UILoginPanel.Host, UILoginPanel.Port, null);
                isStartHeartBeat = false;
            }

            if (!isStartHeartBeat)
            {
                isStartHeartBeat = true;
                InvokeRepeating("HeartBeat", 0, 20f);
            }
        }
    }

    private void HeartBeat()
    {
        Debug.Log("心跳包");
        NetSender.Send<ProtoProtocol.heartbeat>(null, OnHeartBeat);
    }

    private void OnHeartBeat(SprotoTypeBase rpcRsp)
    {
        Debug.Log("心跳包回包");
    }

    public SprotoTypeBase RoleOffline(SprotoTypeBase msg)
    {
        GameTools.Instance.MsgShow("您的账号已在其他设备登陆", "提示",
            null,
            () => {
                NetCore.Disconnect();
                Application.Quit();
            }, false, "重登", "退出");
        return null;
    }

    void OnDestroy()
    {
        /*GameObject go = GameObject.Find("GUICamera");
        for (int i = 0; i < go.transform.childCount; i++)
        {
            string name = go.transform.GetChild(i).gameObject.name;
            if (name != "LogPanel" && name != "WaitUI")
            {
                Destroy(go.transform.GetChild(i).gameObject);
            }
        }
        LoadLoginPanel();
        CheckCanEnter();*/
        QuitGame();
    }

    private void QuitGame()
    {
        NetCore.Disconnect();
        Application.Quit();
    }
}