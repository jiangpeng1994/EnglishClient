using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeUntilItem : MonoBehaviour {

    public SelectCoursePanel m_Home;
    public UITexture m_SuoUISprite;
    public GameObject m_Suo;
    public BoxCollider m_SuoBoxCollider;
    public GameObject m_JianTou;
    public GameObject m_Pass;
    private Dictionary<int, unitPass> _levelPassList = new Dictionary<int, unitPass>();

    private int m_Unit;
    private int SuoType;

    /// <summary>
    /// 初始化单元Item
    /// </summary>
    /// <param name="index">这是第几个单元</param>
    /// <param name="curNum">当前需要进行的单元</param>
    public void Init(int index, int curNum, int totalUnitNum)
    {
        m_Unit = index;
        if (index <= totalUnitNum)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (index < curNum)
        {
            // 通过
            SuoType = 0;
            m_Suo.SetActive(false);
            m_SuoUISprite.color = new Color(255 / 255, 255 / 255, 255 / 255, 255/255);
            m_SuoBoxCollider.enabled = true;
            m_JianTou.SetActive(false);
            m_Pass.SetActive(true);
        } else if (index == curNum)
        {
            // 当前
            SuoType = 1;
            m_Suo.SetActive(false);
            m_SuoUISprite.color = new Color(255 / 255, 255 / 255, 255 / 255, 255/255);
            m_SuoBoxCollider.enabled = true;
            m_JianTou.SetActive(true);
            m_Pass.SetActive(false);
        } else
        {
            // 上锁
            SuoType = -1;
            m_Suo.SetActive(true);
            m_SuoUISprite.color = new Color(108f/255, 108f/255, 108f/255, 255 / 255);
            m_SuoBoxCollider.enabled = false;
            m_JianTou.SetActive(false);
            m_Pass.SetActive(false);
        }
        
        UIEventListener.Get(gameObject).onClick = OnClickSelect;
        //gameObject.GetComponent<UIButton>().defaultColor = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
    }

    /// <summary>
    /// 检测是否下载了该单元的资源。
    /// </summary>
    public bool CheckIsDownload()
    {
        string taskId = DataManager.GetInstance().roleData.curGrade.ToString()
            + "." + DataManager.GetInstance().roleData.curTerm.ToString()
            + "." + m_Unit;
        // 解压完成了，才算真的完成了。
        string key = "IsUnZip-" + taskId;

        bool isUnZip = GameDataManager.GetBool(key);
        //isUnZip = false;
        if (!isUnZip)
        {
            // 没有下载，需要提醒是否下载。
            GameTools.Instance.MsgShow("该单元的资源暂未下载，是否现在下载", "提示",
            null,
            () => {
                Download(taskId);
            }, true, null, "下载");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 下载该单元的资源。
    /// </summary>
    public void Download(string taskId)
    {
        Transform _transform = (Instantiate(Resources.Load("UI/DownLoadUI")) as GameObject).transform;
        _transform.parent = transform.parent;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
        DownloadUI downloadUI = _transform.GetComponent<DownloadUI>();

        string key = "IsDownload-" + taskId;
        bool isDownload = GameDataManager.GetBool(key);
        //isDownload = false;
        DownLoadInfo downLoadInfo = DataManager.GetInstance().downLoadInfoList[taskId];
        downloadUI.AddDownloadTask(taskId, downLoadInfo.url, Application.persistentDataPath + "/Resources.zip", downLoadInfo.size, isDownload, Application.persistentDataPath + "/", false);
        downloadUI.Download();
    }

    /// <summary>
    /// 选择单元
    /// </summary>
    /// <param name="btn"></param>
    private void OnClickSelect(GameObject btn)
    {
        if (!CheckIsDownload())
        {
            return;
        }

        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：单元" + m_Unit + "的难度数据");

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有难度");
            m_Home.OpenLevelSelectUI(m_Unit, 5);
        }
        else
        {
            GetUnitPassInfo.request msg = new GetUnitPassInfo.request();
            moudle_base moudle = new moudle_base
            {
                grade = DataManager.GetInstance().roleData.curGrade,
                term = DataManager.GetInstance().roleData.curTerm,
                unit = m_Unit,
            };
            msg.moudleBase = moudle;
            NetSender.Send<ProtoProtocol.GetUnitPassInfo>(msg, UnitInfoResponseHandler);
        }
    }

    /// <summary>
    /// 收到难度信息
    /// </summary>
    /// <param name="msg"></param>
    private void UnitInfoResponseHandler(SprotoTypeBase msg)
    {
        Debug.Log("收到：单元" + m_Unit + "的难度数据");
        var data = (GetUnitPassInfo.response)msg;

        _levelPassList = new Dictionary<int, unitPass>();
        foreach (unitPass item in data.moudlePassList)
        {
            int index = (int)item.index;
            _levelPassList.Add(index, item);
        }

        int curIndex = _levelPassList.Count + 1;
        for (int i = 1; i <= _levelPassList.Count; i++)
        {
            if (_levelPassList[i].isPass == false)
            {
                curIndex = i;
                break;
            }
        }

        DataManager.GetInstance().levelPassList = _levelPassList;

        m_Home.OpenLevelSelectUI(m_Unit, curIndex);
    }
}