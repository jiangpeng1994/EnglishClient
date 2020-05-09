using Sproto;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class UIPlazaPanel : MonoBehaviour {

    public static UIPlazaPanel instance;

    public GameObject Loading;
    public GameObject Plaza;

    public UIButton StudyBtn;
    public UIButton m_Read;
    public UIButton m_Navigation;
    public UIButton ShopBtn;
    public UIButton BagBtn;
    public UIButton InfoBtn;
    public UITexture m_HeadOnTexture;
    public UITexture m_HeadOffTexture;
    public Texture[] SexHeadOnTexture;
    public Texture[] SexHeadOffTexture;
    public UISlider LoadingSlider;

    public GameObject Notice;
    public UIButton CloseNoticeBtn;
    public UILabel Title;
    public UILabel Content;
    public UIButton GetBtn;
    public GameObject Reward;
    public UILabel Num;
    private int RewardNum;

    //动画
    public VideoPlayer videoPlayer;
    public UITexture TextTexture;

    /// <summary>
    /// android层IAppPaySDK类
    /// </summary>
    private AndroidJavaObject IAppPaySDK;

    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        AudicoManager.instance.StopMusicAudio();
        Notice.SetActive(false);
        LoadingSlider.value = 0;
        Plaza.SetActive(false);
        Loading.SetActive(true);
        InvokeRepeating("LoadingScene", 0, 0.01f);

        videoPlayer.Play();

        m_HeadOnTexture.mainTexture = SexHeadOnTexture[DataManager.GetInstance().roleData.Sex];
        m_HeadOnTexture.gameObject.SetActive(false);
        m_HeadOffTexture.mainTexture = SexHeadOffTexture[DataManager.GetInstance().roleData.Sex];
        m_HeadOffTexture.gameObject.SetActive(true);
        UIEventListener.Get(StudyBtn.gameObject).onClick = OnClickStudyBtn;
        UIEventListener.Get(InfoBtn.gameObject).onClick = OnClickInfoBtn;
        UIEventListener.Get(ShopBtn.gameObject).onClick = OnClickShopBtn;
        UIEventListener.Get(BagBtn.gameObject).onClick = OnClickBagBtn;

        UIEventListener.Get(CloseNoticeBtn.gameObject).onClick = OnCloseNoteBtn;
        UIEventListener.Get(GetBtn.gameObject).onClick = OnGetBtn;

        Debug.Log("请求：公告数据");
        NetSender.Send<ProtoProtocol.GetNoticeInfo>(null, OnGetNoticeInfo);
        //UIEventListener.Get(m_Navigation.gameObject).onClick = OnClickPay;

        //IAppPaySDK = new AndroidJavaObject("com.ssm.speechrecognizer.IAppPaySDK");
        // 调用Android层：爱贝支付SDK的初始化
        //IAppPaySDK.Call("InitIAppPay");
    }

    /// <summary>
    /// 收到公告消息
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnGetNoticeInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：公告数据");
        var data = (ProtoSprotoType.GetNoticeInfo.response)rpcRsp;
        if (data.status)
        {
            Title.text = data.title;
            Content.text = data.content;
            if (data.Received || data.itemNum <= 0)
            {
                GetBtn.gameObject.SetActive(false);
                Reward.gameObject.SetActive(false);
            } else
            {
                GetBtn.gameObject.SetActive(true);
                Num.text = "X " + data.itemNum.ToString();
                RewardNum = (int)data.itemNum;
                Reward.gameObject.SetActive(true);
            }

            Notice.SetActive(true);
        }
    }

    private void LoadingScene()
    {
        LoadingSlider.value += 0.005f;
        if (LoadingSlider.value >= 1)
        {
            CancelInvoke("LoadingScene");
            EnterPlaza();
        }
    }

    private void EnterPlaza()
    {
        LoadingSlider.value = 1;
        Loading.SetActive(false);
        Plaza.SetActive(true);
        AudicoManager.instance.Play("music", "Music/hall screen");
    }

    void Update()
    {
        if (videoPlayer.texture == null)
        {
            return;
        }

        TextTexture.mainTexture = videoPlayer.texture;
    }

    /// <summary>
    /// 打开学习界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickStudyBtn(GameObject go)
    {
        OpenUI("SelectCoursePanel");
    }

    /// <summary>
    /// 打开商店界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickShopBtn(GameObject go)
    {
        Debug.Log("请求：商店数据");
        NetSender.Send<ProtoProtocol.GetShopList>(null, GetShopList);
    }

    /// <summary>
    /// 打开背包界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickBagBtn(GameObject go)
    {
        Debug.Log("请求：背包数据");
        NetSender.Send<ProtoProtocol.GetBagInfo>(null, GetBagInfo);
    }

    /// <summary>
    /// 收到商店数据
    /// </summary>
    /// <param name="msg">商店数据</param>
    private void GetShopList(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：商店数据");
        var data = (ProtoSprotoType.GetShopList.response)rpcRsp;
        DataManager.GetInstance().shopInfoData = data;
        OpenUI("ShopPanel");
    }

    /// <summary>
    /// 收到背包数据
    /// </summary>
    /// <param name="rpcRsp">背包数据</param>
    private void GetBagInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：背包数据");
        var data = (ProtoSprotoType.GetBagInfo.response)rpcRsp;
        DataManager.GetInstance().bagInfoData = data;
        OpenUI("BagPanel");
    }

    /// <summary>
    /// 打开个人信息界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickInfoBtn(GameObject go)
    {
        m_HeadOffTexture.gameObject.SetActive(false);
        m_HeadOnTexture.gameObject.SetActive(true);
        Invoke("OpenUserInfoPanel", 0.4f);
    }

    /// <summary>
    /// 打开个人信息UI
    /// </summary>
    private void OpenUserInfoPanel()
    {
        OpenUI("UserInfoPanel");
        Invoke("HeadOff", 0.2f);
    }

    /// <summary>
    /// 个人信息转头
    /// </summary>
    private void HeadOff()
    {
        m_HeadOffTexture.gameObject.SetActive(true);
        m_HeadOnTexture.gameObject.SetActive(false);
    }

    /// <summary>
    /// 打开UI
    /// </summary>
    /// <param name="ui">UI名</param>
    private void OpenUI(string ui)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Transform _transform = (Instantiate(Resources.Load("UI/" + ui)) as GameObject).transform;
        _transform.parent = transform.parent;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 关闭公告界面
    /// </summary>
    /// <param name="go"></param>
    private void OnCloseNoteBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Notice.SetActive(false);
    }

    /// <summary>
    /// 获得奖励
    /// </summary>
    /// <param name="go"></param>
    private void OnGetBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (DataManager.instance.roleData.IsTraveler)
        {
            GameTools.Instance.TipsShow("先要注册账号，才能领取奖励。");
        } else
        {
            Debug.Log("请求：获得奖励");
            NetSender.Send<ProtoProtocol.ReceiveNoticeReward>(null, OnReceiveNoticeReward);
        }
    }

    /// <summary>
    /// 收到获得奖励结果
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnReceiveNoticeReward(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：获得奖励结果");
        var data = (ProtoSprotoType.ReceiveNoticeReward.response)rpcRsp;
        if (data.status)
        {
            GetBtn.gameObject.SetActive(false);
            Reward.gameObject.SetActive(false);
            DataManager.instance.roleData.Diamond += RewardNum;
            GameTools.Instance.TipsShow("领取奖励成功");
        } else
        {
            GameTools.Instance.TipsShow("领取奖励失败");
        }
    }

    /// <summary>
    /// 刷新性别
    /// </summary>
    public void RefreshSex()
    {
        m_HeadOnTexture.mainTexture = SexHeadOnTexture[DataManager.GetInstance().roleData.Sex];
        m_HeadOffTexture.mainTexture = SexHeadOffTexture[DataManager.GetInstance().roleData.Sex];
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void OnClickPay(GameObject go)
    {
        //StartCoroutine(SendGet("http://47.110.254.9:8001/getPayOrderid?account=" + DataManager.GetInstance().roleData.ID + "&waresid=" + Random.Range(1, 2).ToString()));
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
            IAppPaySDK.Call("StartPay", getData.text);
        }
    }
}