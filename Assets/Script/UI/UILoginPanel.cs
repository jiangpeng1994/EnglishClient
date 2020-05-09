using Sproto;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class UILoginPanel : MonoBehaviour
{
    public static UILoginPanel instance;
    public UIButton m_RememberPWD;
    public UIButton m_YouKe;
    public UIButton m_Enter;
    public UIButton m_Register;
    public UIButton m_ForGet;

    public UIInput m_InputAccount;
    public UIInput m_InputPWD;

    public GameObject m_Tourist;   //游客说明界面
    public GameObject m_Maincenter;   //登陆界面
    public GameObject m_ReGetPwDcenter;   //更换密码界面
    public GameObject m_NewActcenter;   //注册界面

    public GameObject m_SprRemember;   //对勾图片

    public UIButton m_YouKeNext;   //游客界面下一步
    public UIButton m_YouKeBack;   //游客界面返回登陆按钮

    public UIChangePWD UIChangePWD;
    public UIRegisterAccount UIRegisterAccount;

    private bool m_IsRememberPWD;    //是否记住密码

    const string RemAccountKey = "Account";
    const string RemPWDKey = "PassWord";
    const string IsRememberKey = "IsRemember";

    public static string Host = "47.110.254.9";
    public static int Port = 9777;
    private long session = 0;
    private string token = "";

    public UILoginPanel()
    {
        NetReceiver.RemoveAll();
        NetReceiver.AddHandler<ProtoProtocol.client_user_info>(UserInfo);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle1Info>(Moudle1Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle2Info>(Moudle2Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle3Info>(Moudle3Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle4Info>(Moudle4Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle5Info>(Moudle5Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle6Info>(Moudle6Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle7Info>(Moudle7Info);
        NetReceiver.AddHandler<ProtoProtocol.SyncMoudle8Info>(Moudle8Info);
        NetReceiver.AddHandler<ProtoProtocol.sync_role_offline>(Main.instance.RoleOffline);
    }

    void Start()
    {
        AudicoManager.instance.Play("music", "Music/logon screen");
        if (instance == null)
        {
            instance = this;
        }

        UIEventListener.Get(m_RememberPWD.gameObject).onClick = OnClickRememberPWD;
        UIEventListener.Get(m_Enter.gameObject).onClick = OnClickEnter;
        UIEventListener.Get(m_YouKe.gameObject).onClick = OnClickYouKe;
        UIEventListener.Get(m_Register.gameObject).onClick = OnClickRegister;
        UIEventListener.Get(m_ForGet.gameObject).onClick = OnClickForGet;

        UIEventListener.Get(m_YouKeNext.gameObject).onClick = OnClickYouKeNext;
        UIEventListener.Get(m_YouKeBack.gameObject).onClick = OnClickYouKeBack;

        m_Maincenter.SetActive(true);
        m_Tourist.SetActive(false);
        m_ReGetPwDcenter.SetActive(false);
        m_NewActcenter.SetActive(false);

        // 记住密码功能
        m_SprRemember.SetActive(GameDataManager.GetBool(IsRememberKey));
        m_IsRememberPWD = m_SprRemember.activeSelf;
        string act = GameDataManager.GetString(RemAccountKey);
        if (act != null && act != "")
        {
            m_InputAccount.value = act;
        }
        string pwd = GameDataManager.GetString(RemPWDKey);
        if (pwd != null && pwd != "")
        {
            m_InputPWD.value = pwd;
        }
    }

    /// <summary>
    /// 点击记住密码
    /// </summary>
    /// <param name="button"></param>
    private void OnClickRememberPWD(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_SprRemember.SetActive(!m_SprRemember.activeSelf);
        m_IsRememberPWD = m_SprRemember.activeSelf;
        GameDataManager.SetBool(IsRememberKey, m_SprRemember.activeSelf);
    }

    /// <summary>
    /// 点击账号登陆
    /// </summary>
    /// <param name="button"></param>
    private void OnClickEnter(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求账号登陆");
        if (GlobalActionManager.CheckPhoneIsAble(m_InputAccount.value) == false)
        {
            Debug.Log("请输入正确的手机号码");
            GameTools.Instance.TipsShow("请输入正确的手机号码");
            return;
        }
        if (m_InputPWD == null || m_InputPWD.value == "" || Regex.IsMatch(m_InputPWD.value, @"[\u4e00-\u9fa5]"))
        {
            Debug.Log("请输入正确的密码");
            GameTools.Instance.TipsShow("请输入正确的密码");
            return;
        }

        m_Enter.transform.GetComponent<BoxCollider>().enabled = false;
        NetCore.Connect(Host, Port, null);
        var msg = new ProtoSprotoType.logintest.request
        {
            account = m_InputAccount.value,
            password = m_InputPWD.value
        };
        NetSender.Send<ProtoProtocol.logintest>(msg, (info) =>
        {
            m_Enter.transform.GetComponent<BoxCollider>().enabled = true;
            var a = info as ProtoSprotoType.logintest.response;
            if (a.session == -1)
            {
                Debug.Log("密码有误");
                GameTools.Instance.TipsShow("密码有误");
                return;
            }
           
            ConnectServer(a.session, a.token, a.ip, a.port);
        });
    }

    /// <summary>
    /// 账号验证通过，请求进入游戏
    /// </summary>
    /// <param name="session"></param>
    /// <param name="token"></param>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    private void ConnectServer(long session, string token, string ip, long port)
    {
        Debug.Log("账号验证通过，请求进入游戏大厅");
        var info = new ProtoSprotoType.login.request
        {
            session = session,
            token = token
        };
        Host = ip;
        Port = (int)port;
        NetCore.Connect(Host, Port, null);
        NetSender.Send<ProtoProtocol.login>(info);
    }

    /// <summary>
    /// 点击游客登陆
    /// </summary>
    /// <param name="button"></param>
    private void OnClickYouKe(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_Tourist.SetActive(true);
        m_Maincenter.SetActive(false);
    }

    /// <summary>
    /// 进入游客
    /// </summary>
    /// <param name="button"></param>
    private void OnClickYouKeNext(GameObject button)
    {
        Debug.Log("请求游客登陆");
        AudicoManager.instance.Play("effect", "Effect/press button");
        NetCore.Connect(Host, Port, null);
        NetSender.Send<ProtoProtocol.travelerLogin>(null, ProtoTravelerResponseHandler);
    }

    /// <summary>
    /// 退出游客
    /// </summary>
    /// <param name="button"></param>
    private void OnClickYouKeBack(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_Tourist.SetActive(false);
        m_Maincenter.SetActive(true);
    }

    /// <summary>
    /// 游客验证通过
    /// </summary>
    /// <param name="msg"></param>
    private void ProtoTravelerResponseHandler(SprotoTypeBase msg)
    {
        var a = msg as ProtoSprotoType.travelerLogin.response;
        ConnectServer(a.session, a.token, a.ip, a.port);
    }

    /// <summary>
    /// 点击注册
    /// </summary>
    /// <param name="button"></param>
    private void OnClickRegister(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        UIRegisterAccount.Init();
        m_Maincenter.SetActive(false);
        m_NewActcenter.SetActive(true);
    }

    /// <summary>
    /// 点击找回密码
    /// </summary>
    /// <param name="button"></param>
    private void OnClickForGet(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        UIChangePWD.Init();
        m_Maincenter.SetActive(false);
        m_ReGetPwDcenter.SetActive(true);
    }

    IEnumerator SendGet(string _url)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            Debug.Log(getData.error);
        }
        else
        {
            if (getData.text == "0")
            {
                //成功
            }
            else
            {
                //失败
            }
        }
    }

    private void EnterGame(Int64 Sex, string NickName)
    {
        Debug.Log("进入游戏大厅");
        DataManager.GetInstance().roleData.Phone = m_InputAccount.value;
        DataManager.GetInstance().roleData.IsVIP = false;
        DataManager.GetInstance().roleData.IsSpeak = true;
        //记住密码
        if (m_IsRememberPWD)
        {
            GameDataManager.SetString(RemAccountKey, m_InputAccount.value);
            GameDataManager.SetString(RemPWDKey, m_InputPWD.value);
        }

        ////打开游戏大厅
        Transform _transform = (Instantiate(Resources.Load("UI/UIPlazaPanel")) as GameObject).transform;
        UIPlazaPanel _plaza = _transform.GetComponent<UIPlazaPanel>();
        _transform.parent = transform.parent;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
        gameObject.SetActive(false);
        m_Tourist.SetActive(false);
        m_Maincenter.SetActive(true);
    }

    /// <summary>
    /// 课文123
    /// </summary>
    /// <param name="msg">课文123数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle1Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块2的课文数据");
        var data = (ProtoSprotoType.SyncMoudle1Info.request)msg;

        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().textData = data;
        /*for(int i = 0; i < data.moudlePassList.Count; i++)
        {
            Debug.Log("第" + i + "个");
            for (int j = 0; j< data.moudlePassList[i].scoreList.Count;j++)
            {
                Debug.Log("第" + j + "小个:" + data.moudlePassList[i].scoreList[j]);
            }
        }*/

        WindowManager.instance.Open<NavigationUI>().Init(TeachType.Text);
        return null;
    }

    /// <summary>
    /// 单词1
    /// </summary>
    /// <param name="msg">单词1数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle2Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块3的单词数据");
        var data = (ProtoSprotoType.SyncMoudle2Info.request)msg;
        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().wordData1 = data;

        WindowManager.instance.Open<NavigationUI>().Init(TeachType.Word1);
        return null;
    }

    /// <summary>
    /// 句式1
    /// </summary>
    /// <param name="msg">句式1数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle3Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块4的句式数据");
        var data = (ProtoSprotoType.SyncMoudle3Info.request)msg;
        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().sentenceData1 = data;

        WindowManager.instance.Open<NavigationUI>().Init(TeachType.Sentence1);
        return null;
    }

    /// <summary>
    /// 对话123
    /// </summary>
    /// <param name="msg">对话123数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle4Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块5的对话数据");
        var data = (ProtoSprotoType.SyncMoudle4Info.request)msg;

        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().dialogueData = data;

        int type = (int)Math.Floor(data.moudleBase.moudleId);
        if (type == 4)
        {
            //DataManager.GetInstance().curStudyProgress.totalNum = data.infoList.Count;
            //DataManager.GetInstance().curStudyProgress.completeNum = 0;
            //DataManager.GetInstance().curStudyProgress.passNum = 0;
            //WindowManager.instance.Open<UIWordGamePanel>().Init(TeachType.DialogueTest,null,0);
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.DialogueTest);
        } else
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.Dialogue);
        }
        
        return null;
    }

    /// <summary>
    /// 单词2
    /// </summary>
    /// <param name="msg">level2单词数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle5Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块3的单词数据");
        var data = (ProtoSprotoType.SyncMoudle5Info.request)msg;

        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().wordData2 = data;

        WindowManager.instance.Open<NavigationUI>().Init(TeachType.Word2);
        return null;
    }

    /// <summary>
    /// 句式23
    /// </summary>
    /// <param name="msg">句式23数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle6Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块4的句式数据");
        var data = (ProtoSprotoType.SyncMoudle6Info.request)msg;

        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().sentenceData2 = data;

        WindowManager.instance.Open<NavigationUI>().Init(TeachType.Sentence2);
        return null;
    }

    /// <summary>
    /// 单词3
    /// </summary>
    /// <param name="msg">单词3数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle7Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块3的单词数据");
        var data = (ProtoSprotoType.SyncMoudle7Info.request)msg;

        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().wordData3 = data;

        WindowManager.instance.Open<NavigationUI>().Init(TeachType.Word3);
        return null;
    }

    /// <summary>
    /// 单词测试
    /// </summary>
    /// <param name="msg">单词测试数据</param>
    /// <returns></returns>
    private SprotoTypeBase Moudle8Info(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块3/4的单词/句式数据");
        var data = (ProtoSprotoType.SyncMoudle8Info.request)msg;

        DataManager.GetInstance().roleData.curGrade = data.moudleBase.grade;
        DataManager.GetInstance().roleData.curTerm = data.moudleBase.term;
        DataManager.GetInstance().roleData.curUnit = data.moudleBase.unit;
        DataManager.GetInstance().roleData.curMoudleId = data.moudleBase.moudleId;
        DataManager.GetInstance().wordTestData = data;

        //DataManager.GetInstance().curStudyProgress.completeNum = 0;
        //DataManager.GetInstance().curStudyProgress.passNum = 0;

        if (data.moudleBase.moudleId == 4.1)
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.WordTest);
            //DataManager.GetInstance().curStudyProgress.totalNum = data.contentInfo1.Count;
            //WindowManager.instance.Open<UIWordGamePanel>().Init(TeachType.WordTest,null,0);
        } else
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.SentenceTest);
            //DataManager.GetInstance().curStudyProgress.totalNum = data.contentInfo2.Count;
            //WindowManager.instance.Open<UIWordGamePanel>().Init(TeachType.SentenceTest,null,0);
        }
        
        return null;
    }

    /// <summary>
    /// 获取玩家信息数据
    /// </summary>
    /// <param name="msg">玩家信息数据</param>
    /// <returns></returns>
    private SprotoTypeBase UserInfo(SprotoTypeBase msg)
    {
        Main.isStart = true;
        Debug.Log("收到：玩家信息数据");
        var data = (ProtoSprotoType.client_user_info.request)msg;

        DataManager.GetInstance().roleData.ID = data.ID.ToString();
        DataManager.GetInstance().roleData.NickName = data.NickName;
        DataManager.GetInstance().roleData.IsTraveler = data.IsTraveler;
        DataManager.GetInstance().roleData.Sex = data.Sex;
        DataManager.GetInstance().roleData.Diamond = data.Diamond;

        EnterGame(data.Sex, data.NickName);
        return null;
    }

    /// <summary>
    /// 获取模块得分信息
    /// </summary>
    /// <param name="msg"></param>
    private void GetResultInfoDelegWord(SprotoTypeBase msg)
    {
        Debug.Log("收到：模块得分信息");
        var info = (ProtoSprotoType.GetResultInfo.response)msg;
        //info.order
        //info.
        DataManager.GetInstance().roleData.curProgress = info.order;
        DataManager.GetInstance().roleData.Scores = info.score;

        string temp = (info.moudleBase.moudleId - 1).ToString();
        //Debug.Log("temp:" + temp);
        //float temp = (float)info.moudleBase.curMoudleId - 1;
        //Debug.Log("temp:" + temp);
        //Debug.LogError(temp == 0.2f);
        if (temp == "0.2")
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.Text);
        }
        else if (temp == "0.3")
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.Word1);
        }
        else if (temp == "0.5")
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.Dialogue);
        }
        else if (temp == "0.4")
        {
            WindowManager.instance.Open<NavigationUI>().Init(TeachType.Sentence1);
        }

        Invoke("HidePlaza", 0.5f);
    }
    /// <summary>
    /// 给学习界面一个动画时间
    /// </summary>
    public void HidePlaza()
    {
        gameObject.SetActive(false);
        if (UIPlazaPanel.instance != null)
        {
            UIPlazaPanel.instance.gameObject.SetActive(false);
        }
        if (SelectCoursePanel.instance != null)
        {
            Destroy(SelectCoursePanel.instance.gameObject);
        }
    }

    private void OnDestroy()
    {
        NetReceiver.RemoveAll();
        instance = null;
    }
}