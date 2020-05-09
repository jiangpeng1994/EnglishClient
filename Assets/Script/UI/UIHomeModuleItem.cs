using ProtoSprotoType;
using UnityEngine;

public class UIHomeModuleItem : MonoBehaviour {

    public SelectCoursePanel m_Home;
    public GameObject[] m_Liang = new GameObject[3];
    public UISprite m_MoudleName;
    public UISprite m_BG;
    public UISprite m_Suo;
    public BoxCollider m_SuoBoxCollider;
    public GameObject m_cur;
    public GameObject m_Pass;
    public GameObject m_SuoIcon;

    private int m_MoudleID;
    private int m_StarNum = 3;//星星数量

    /// <summary>
    /// 初始化模块Item
    /// </summary>
    /// <param name="difficultyInfo"></param>
    public void Init(int moudleID, bool isOpen, int curNum, int starNum)
    {
        m_MoudleID = moudleID;
        SetModuleStar(starNum);

        if (isOpen)
        {
            if (moudleID < curNum)
            {
                m_SuoIcon.gameObject.SetActive(false);
                //m_cur.gameObject.SetActive(false);
                m_Suo.gameObject.SetActive(false);
                m_SuoBoxCollider.enabled = true;
            }
            else if (moudleID == curNum)
            {
                m_SuoIcon.gameObject.SetActive(false);
                //m_cur.gameObject.SetActive(true);
                m_Suo.gameObject.SetActive(false);
                m_SuoBoxCollider.enabled = true;
            }
            else
            {
                m_SuoIcon.gameObject.SetActive(true);
                //m_cur.gameObject.SetActive(false);
                m_Suo.gameObject.SetActive(true);
                m_SuoBoxCollider.enabled = false;
            }
        } else
        {
            m_SuoIcon.gameObject.SetActive(false);
            m_Pass.gameObject.SetActive(false);
            //m_cur.gameObject.SetActive(false);
            m_Suo.gameObject.SetActive(true);
            m_SuoBoxCollider.enabled = false;
        }

        if (moudleID == 6)
        {
            m_BG.spriteName = "bg1";
            m_MoudleName.spriteName = "name" + moudleID.ToString();
            m_MoudleName.MakePixelPerfect();
        }
        else
        {
            m_BG.spriteName = "nimibg" + moudleID.ToString();
            m_MoudleName.spriteName = "name" + moudleID.ToString();
        }
        
        UIEventListener.Get(gameObject).onClick = OnClickStartStudy;
        gameObject.GetComponent<UIButton>().defaultColor = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
    }

    /// <summary>
    /// 设置模块的星级
    /// </summary>
    /// <param name="starNum">星级</param>
    public void SetModuleStar(int starNum)
    {
        m_StarNum = starNum;
        for (int i = 0; i < m_Liang.Length; i++)
        {
            if (i < m_StarNum)
            {
                m_Liang[i].SetActive(true);
            }
            else
            {
                m_Liang[i].SetActive(false);
            }
        }

        if (starNum >= 3)
        {
            m_Pass.gameObject.SetActive(true);
        } else
        {
            m_Pass.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 选择模块
    /// </summary>
    /// <param name="btn"></param>
    public void OnClickStartStudy(GameObject btn)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：模块" + m_MoudleID + "的学习数据");

        GetMoudleInfo.request msg = new GetMoudleInfo.request();
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit
        };
        
        if (DataManager.GetInstance().roleData.curLevel == "4")
        {
            switch (m_MoudleID)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".1");
                    break;
                case 4:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".2");
                    break;
                case 5:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".3");
                    break;
            }
        } else
        {
            switch (m_MoudleID)
            {
                case 1:
                    break;
                case 2:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".2");
                    break;
                case 3:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".3");
                    break;
                case 4:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".4");
                    break;
                case 5:
                    moudle.moudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".5");
                    break;
            }
        }

        if (m_MoudleID == 1)
        {
            //string isDownLoad = GameDataManager.GetString(GetVideoName());
            //Debug.Log("读取GetVideoName():" + GetVideoName() + ":" + isDownLoad);
            //if (isDownLoad != null && isDownLoad != "" && isDownLoad == "1")
            //{
                // 已下载视频
                DataManager.GetInstance().roleData.curMoudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".1");
                WindowManager.instance.Open<UIWordGamePanel>().Init(TeachType.Animation, null, 0);
            //}
            //else
            //{
                // 未下载视频
                //GameTools.Instance.MsgShow("该单元动画需要下载，是否现在下载？", "提示", () => { }, () => { StartDownLoad(); }, false, "取消", "确定");
            //}
        }
        else
        {
            msg.moudleBase = moudle;
            NetSender.Send<ProtoProtocol.GetMoudleInfo>(msg, null);
        }
    }

    private void StartDownLoad()
    {
        string remoteUrl = "https://englishpal2019-1300493262.cos.ap-shanghai.myqcloud.com/Animation/tkyj";
        RemoteFileInfo remoteFileInfo = new RemoteFileInfo
        {
            remoteUrl = string.Format("{0}/{1}.mp4", remoteUrl, GetVideoName()),
            localUrl = string.Format("{0}/{1}.mp4", Application.persistentDataPath, GetVideoName())
        };
        Debug.Log("URL:" + remoteFileInfo.remoteUrl);
        Debug.Log("URL:" + remoteFileInfo.localUrl);

        ThreadDownLoad downLoad = GameObject.Find("UIPlazaPanel(Clone)").GetComponent<ThreadDownLoad>();
        downLoad.AddDownLoadFile(remoteFileInfo, OnDownLoad);
        downLoad.StartDownLoad();
    }
    
    private void OnDownLoad(RemoteFileInfo remoteFileInfo)
    {
        if (remoteFileInfo.isDownLoadFinish)
        {
            Debug.Log("下载完成");
            isDownLoad = false;
            downLoadSize = 0;
            totalSize = "";

            saveDownLoad = true;
        } else
        {
            downLoadSize = remoteFileInfo.downLoadSize;
            totalSize = getSize(remoteFileInfo.totalSize);
            isDownLoad = true;
        }
    }

    private string[] _term = new string[]{"","A","B"};
    private string GetVideoName()
    {
        string name = DataManager.GetInstance().roleData.curGrade + _term[DataManager.GetInstance().roleData.curTerm] 
            + "U" + DataManager.GetInstance().roleData.curUnit;
        return name;
    }

    private bool saveDownLoad = false;
    private bool isDownLoad = false;
    private float downLoadSize = 0;
    private string totalSize = "";
    MessageBoxPanel messageBoxPanel = null;
    private void Update()
    {
        if (saveDownLoad)
        {
            saveDownLoad = false;
            PlayerPrefs.SetString(GetVideoName(), "1");
            Debug.Log("写入GetVideoName():" + GetVideoName());
            DataManager.GetInstance().roleData.curMoudleId = System.Convert.ToDouble(DataManager.GetInstance().roleData.curLevel + ".1");
            WindowManager.instance.Open<UIWordGamePanel>().Init(TeachType.Animation, null, 0);
        }

        if (isDownLoad)
        {
            if (messageBoxPanel == null)
            {
                messageBoxPanel = GameTools.Instance.MsgShow("动画开始下载", "提示", null, null, false, null, null);
            }
            else
            {
                messageBoxPanel.ChangeContent("下载进度：" + getSize(downLoadSize) + "/" + totalSize);
            }
        } else
        {
            if (messageBoxPanel != null)
            {
                messageBoxPanel.Close();
                messageBoxPanel = null;
            }
        }
    }

    private string getSize(float size)
    {
        string sizeInfo = "";
        if (size >= 1024 * 1024)
        {
            sizeInfo = ((double)size / (1024 * 1024)).ToString("0.00") + "MB";
        }
        else
        {
            sizeInfo = ((double)size / 1024).ToString("0.00") + "KB";
        }
        return sizeInfo;
    }
}