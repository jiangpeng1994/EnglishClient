using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public UILabel DiamondNumLabel;
    public UIButton BackBtn;
    public UIButton TreasureChestBtn;

    // 商品UI
    public Texture[] BtnBg;
    public Color[] BtnLabelColor;
    public UIButton[] GoodsTpyeBtns;
    public UITexture[] GoodsTpyeBtnsTexture;
    public UILabel[] GoodsTpyeBtnsLabel;
    public GameObject[] GoodsTpyeBtnsPanel;

    public UIGrid RealThingGrid;
    public GameObject RealThingItem;
    private List<GameObject> RealThingItemList = new List<GameObject>();
    private List<UIButton> RealThingBtnList = new List<UIButton>();
    private List<UITexture> RealThingPicList = new List<UITexture>();
    private List<UILabel> RealThingNameList = new List<UILabel>();
    private List<UILabel> RealThingDiamondNumList = new List<UILabel>();

    public UIGrid VoucherGrid;
    public GameObject VoucherItem;
    private List<GameObject> VoucherItemList = new List<GameObject>();
    private List<UIButton> VoucherBtnList = new List<UIButton>();
    private List<UITexture> VoucherPicList = new List<UITexture>();
    private List<UILabel> VoucherNameList = new List<UILabel>();
    private List<UILabel> VoucherDiamondNumList = new List<UILabel>();

    public UIGrid MonsterCardGrid;
    public GameObject MonsterCardItem;
    private List<GameObject> MonsterCardItemList = new List<GameObject>();
    private List<UIButton> MonsterCardBtnList = new List<UIButton>();
    private List<UITexture> MonsterCardPicList = new List<UITexture>();
    private List<UILabel> MonsterCardNumList = new List<UILabel>();

    // 详情UI
    public GameObject Detail;
    public GameObject[] DetailPanel;

    public UILabel RealThingNameLabel;
    public UITexture RealThingPic;
    public UILabel RealThingDescriptionLabel;
    public UIGrid RealThingColorGrid;
    public GameObject RealThingColorItem;
    private List<GameObject> RealThingColorItemList = new List<GameObject>();
    private List<UIButton> RealThingColorBtnList = new List<UIButton>();
    private List<UITexture> RealThingColorPicList = new List<UITexture>();
    private List<GameObject> RealThingColorChooseList = new List<GameObject>();
    public UILabel RealThingBuyTimesLabel;
    public UILabel RealThingCanBuyNumLabel;
    public UILabel RealThingDiamondNumLabel;
    public UIButton RealThingBuyBtn;
    public UIButton RealThingBackBtn;

    public UILabel VoucherNameLabel;
    public UITexture VoucherPic;
    public UILabel VoucherDescriptionLabel;
    public UILabel VoucherBuyTimesLabel;
    public UILabel VoucherCanBuyNumLabel;
    public UILabel VoucherDiamondNumLabel;
    public UIButton VoucherBuyBtn;
    public UIButton VoucherBackBtn;

    public UITexture MonsterCardPic;
    public UILabel MonsterCardDiamondNumLabel;
    public UIButton MonsterCardBuyBtn;
    public UIButton MonsterCardBackBtn;

    // 确认兑换UI
    public GameObject SureBuy;
    public GameObject SureBuyPanel;
    public UILabel SureGoodNameLabel;
    public UILabel SureDiamondNumLabel;
    public UIButton SureBtn;
    public UIButton SureBackBtn;

    // 兑换成功UI
    public GameObject BuySuccessPanel;
    public UIButton BagBtn;
    public UIButton BuySuccessBackBtn;

    // 兑换失败UI
    public GameObject BuyFailPanel;
    public UILabel BuyFailSorryLabel;
    public UILabel BuyFailReasonLabel;
    public UIButton BuyFailSureBtn;

    // 商店数据
    private GetShopList.response shopInfoData;
    private List<shop1UnitInfo> RealThingData = new List<shop1UnitInfo>();
    private List<shop2UnitInfo> VoucherData = new List<shop2UnitInfo>();
    private List<shop2UnitInfo> MonsterCardData = new List<shop2UnitInfo>();
   
    void Start()
    {
        AudicoManager.instance.Play("music", "Music/shop screen");
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        DiamondNumLabel.text = DataManager.GetInstance().roleData.Diamond.ToString();
        for (int i = 0; i < GoodsTpyeBtns.Length; i++)
        {
            UIEventListener.Get(GoodsTpyeBtns[i].gameObject).onClick = GoodsTpyeBtnsOnClick;
            GoodsTpyeBtnsPanel[i].SetActive(false);
        }
        Detail.SetActive(false);
        SureBuy.SetActive(false);
        SureBuyPanel.SetActive(false);
        BuySuccessPanel.SetActive(false);
        BuyFailPanel.SetActive(false);
        InitGoodsData();
        ClickGoodsTpye(0);

        UIEventListener.Get(BackBtn.gameObject).onClick = BackBtnOnClick;
        UIEventListener.Get(TreasureChestBtn.gameObject).onClick = TreasureChestBtnOnClick;
    }

    /// <summary>
    /// 初始化商品数据
    /// </summary>
    private void InitGoodsData()
    {
        shopInfoData = DataManager.GetInstance().shopInfoData;

        RealThingData.Clear();
        VoucherData.Clear();
        MonsterCardData.Clear();
        RealThingData = shopInfoData.shopList1;
        foreach (shop2UnitInfo item in shopInfoData.shopList2)
        {
            if (item.type == 1)
            {
                VoucherData.Add(item);
            } else if (item.type == 2)
            {
                MonsterCardData.Add(item);
            }
        }

        InitRealThingData();
        InitVoucherData();
        InitMonsterCardData();
    }

    /// <summary>
    /// 初始化实物的数据
    /// </summary>
    private void InitRealThingData()
    {
        int realThingNum = RealThingData.Count;
        if (realThingNum < RealThingItemList.Count)
        {
            for (int i = realThingNum; i < RealThingItemList.Count; i++)
            {
                RealThingItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < realThingNum; i++)
        {
            if (i >= RealThingItemList.Count)
            {
                GameObject item = Instantiate(RealThingItem);
                item.name = i.ToString();
                item.transform.parent = RealThingGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                RealThingItemList.Add(item);

                RealThingBtnList.Add(RealThingItemList[i].transform.GetComponent<UIButton>());
                RealThingPicList.Add(RealThingItemList[i].transform.Find("Pic").GetComponent<UITexture>());
                RealThingNameList.Add(RealThingItemList[i].transform.Find("NameLabel").GetComponent<UILabel>());
                RealThingDiamondNumList.Add(RealThingItemList[i].transform.Find("Diamond/DiamondNumLabel").GetComponent<UILabel>());
            }

            RealThingBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(RealThingBtnList[i].gameObject).onClick = OnClickRealThingBtn;
            RealThingPicList[i].mainTexture = Resources.Load<Texture>("Texture/Goods/" + RealThingData[i].Icon1.Replace(".jpg", "").Replace(".png", ""));
            RealThingNameList[i].text = RealThingData[i].Name;
            RealThingDiamondNumList[i].text = RealThingData[i].Value.ToString();

            RealThingItemList[i].SetActive(true);
        }
        RealThingGrid.enabled = true;
    }

    /// <summary>
    /// 初始化代金券的数据
    /// </summary>
    private void InitVoucherData()
    {
        int partNum = VoucherData.Count;
        if (partNum < VoucherItemList.Count)
        {
            for (int i = partNum; i < VoucherItemList.Count; i++)
            {
                VoucherItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= VoucherItemList.Count)
            {
                GameObject item = Instantiate(VoucherItem);
                item.name = i.ToString();
                item.transform.parent = VoucherGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                VoucherItemList.Add(item);

                VoucherBtnList.Add(VoucherItemList[i].transform.GetComponent<UIButton>());
                VoucherPicList.Add(VoucherItemList[i].transform.Find("Pic").GetComponent<UITexture>());
                VoucherNameList.Add(VoucherItemList[i].transform.Find("NameLabel").GetComponent<UILabel>());
                VoucherDiamondNumList.Add(VoucherItemList[i].transform.Find("Diamond/DiamondNumLabel").GetComponent<UILabel>());
            }

            VoucherBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(VoucherBtnList[i].gameObject).onClick = OnClickVoucherBtn;
            VoucherPicList[i].mainTexture = Resources.Load<Texture>("Texture/Goods/" + VoucherData[i].Icon.Replace(".jpg", "").Replace(".png", ""));
            VoucherNameList[i].text = VoucherData[i].Name;
            VoucherDiamondNumList[i].text = VoucherData[i].Value.ToString();

            VoucherItemList[i].SetActive(true);
        }
        VoucherGrid.enabled = true;
    }

    /// <summary>
    /// 初始化怪物卡牌的数据
    /// </summary>
    private void InitMonsterCardData()
    {
        int partNum = MonsterCardData.Count;
        if (partNum < MonsterCardItemList.Count)
        {
            for (int i = partNum; i < MonsterCardItemList.Count; i++)
            {
                MonsterCardItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= MonsterCardItemList.Count)
            {
                GameObject item = Instantiate(MonsterCardItem);
                item.name = i.ToString();
                item.transform.parent = MonsterCardGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                MonsterCardItemList.Add(item);

                MonsterCardBtnList.Add(MonsterCardItemList[i].transform.GetComponent<UIButton>());
                MonsterCardPicList .Add(MonsterCardItemList[i].transform.GetComponent<UITexture>());
                MonsterCardNumList.Add(MonsterCardItemList[i].transform.Find("Diamond/DiamondNumLabel").GetComponent<UILabel>());
            }

            MonsterCardBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(MonsterCardBtnList[i].gameObject).onClick = OnClickMonsterCardBtn;
            MonsterCardPicList[i].mainTexture = Resources.Load<Texture>("Texture/MonsterCard/" + MonsterCardData[i].Icon.Replace(".jpg", "").Replace(".png", ""));
            MonsterCardNumList[i].text = MonsterCardData[i].Value.ToString();

            MonsterCardItemList[i].SetActive(true);
        }
        MonsterCardGrid.enabled = true;
    }

    /// <summary>
    /// 点击商品类型按钮
    /// </summary>
    /// <param name="go"></param>
    private void GoodsTpyeBtnsOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        int ClickNum = int.Parse(go.name);
        for (int i = 0; i < GoodsTpyeBtns.Length; i++)
        {
            GoodsTpyeBtnsTexture[i].mainTexture = BtnBg[0];
            GoodsTpyeBtnsLabel[i].color = BtnLabelColor[0];
            GoodsTpyeBtnsPanel[i].SetActive(false);
        }

        ClickGoodsTpye(ClickNum);
    }

    /// <summary>
    /// 设置商品类型按钮的选择效果
    /// </summary>
    /// <param name="ClickNum">点击商品类型按钮的编号</param>
    private void ClickGoodsTpye(int ClickNum)
    {
        GoodsTpyeBtnsTexture[ClickNum].mainTexture = BtnBg[1];
        GoodsTpyeBtnsLabel[ClickNum].color = BtnLabelColor[1];
        GoodsTpyeBtnsPanel[ClickNum].SetActive(true);
    }

    private int RealThingBuyIndex = 0;
    private int RealThingBuyColorIndex = 1;
    private int VoucherBuyIndex = 0;
    private int MonsterCardBuyIndex = 0;
    /// <summary>
    /// 点击实物
    /// </summary>
    /// <param name="go"></param>
    private void OnClickRealThingBtn(GameObject go)
    {
        RealThingBuyIndex = int.Parse(go.name);
        RealThingBuyColorIndex = 1;
        RealThingNameLabel.text = RealThingData[RealThingBuyIndex].Name;
        RealThingPic.mainTexture = Resources.Load<Texture>("Texture/Goods/" + RealThingData[RealThingBuyIndex].Icon1.Replace(".jpg", "").Replace(".png", ""));
        RealThingDescriptionLabel.text = RealThingData[RealThingBuyIndex].Depict;
        RealThingCanBuyNumLabel.text = "剩余" + RealThingData[RealThingBuyIndex].Num.ToString() + "件";
        RealThingDiamondNumLabel.text = RealThingData[RealThingBuyIndex].Value.ToString();
        UIEventListener.Get(RealThingBuyBtn.gameObject).onClick = RealThingBuyBtnOnClick;
        UIEventListener.Get(RealThingBackBtn.gameObject).onClick = CloseDetail;

        int colorNum = RealThingData[RealThingBuyIndex].LittleIcon.Count;
        if (colorNum < RealThingColorItemList.Count)
        {
            for (int i = colorNum; i < RealThingColorItemList.Count; i++)
            {
                RealThingColorItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < colorNum; i++)
        {
            if (i >= RealThingColorItemList.Count)
            {
                GameObject item = Instantiate(RealThingColorItem);
                item.name = i.ToString();
                item.transform.parent = RealThingColorGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                RealThingColorItemList.Add(item);

                RealThingColorBtnList.Add(RealThingColorItemList[i].transform.GetComponent<UIButton>());
                RealThingColorPicList.Add(RealThingColorItemList[i].transform.GetComponent<UITexture>());
                RealThingColorChooseList.Add(RealThingColorItemList[i].transform.Find("Choose").gameObject);
            }

            RealThingColorBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(RealThingColorBtnList[i].gameObject).onClick = OnClickRealThingColorBtn;
            RealThingColorPicList[i].mainTexture = Resources.Load<Texture>("Texture/Goods/" + RealThingData[RealThingBuyIndex].LittleIcon[i].Replace(".jpg", "").Replace(".png", ""));
            RealThingColorChooseList[i].gameObject.SetActive(false);
            if (i == 0)
            {
                RealThingColorChooseList[i].gameObject.SetActive(true);
            }
            RealThingColorItemList[i].SetActive(true);
        }
        RealThingColorGrid.enabled = true;
        ShowDetail(0);
    }

    /// <summary>
    /// 选择实物颜色
    /// </summary>
    /// <param name="go"></param>
    private void OnClickRealThingColorBtn(GameObject go)
    {
        int ClickNum = int.Parse(go.name);
        RealThingBuyColorIndex = ClickNum + 1;
        foreach (GameObject item in RealThingColorChooseList)
        {
            item.SetActive(false);
        }
        RealThingColorChooseList[ClickNum].SetActive(true);
    }

    /// <summary>
    /// 点击代金券
    /// </summary>
    /// <param name="go"></param>
    private void OnClickVoucherBtn(GameObject go)
    {
        VoucherBuyIndex = int.Parse(go.name);
        VoucherNameLabel.text = VoucherData[VoucherBuyIndex].Name;
        VoucherPic.mainTexture = Resources.Load<Texture>("Texture/Goods/" + VoucherData[VoucherBuyIndex].Icon.Replace(".jpg", "").Replace(".png", ""));
        VoucherDescriptionLabel.text = VoucherData[VoucherBuyIndex].Depict;
        VoucherCanBuyNumLabel.text = "剩余" + VoucherData[VoucherBuyIndex].Num.ToString() + "件";
        VoucherDiamondNumLabel.text = VoucherData[VoucherBuyIndex].Value.ToString();
        UIEventListener.Get(VoucherBuyBtn.gameObject).onClick = VoucherBuyBtnOnClick;
        UIEventListener.Get(VoucherBackBtn.gameObject).onClick = CloseDetail;
        ShowDetail(1);
    }

    /// <summary>
    /// 点击怪物卡牌
    /// </summary>
    /// <param name="go"></param>
    private void OnClickMonsterCardBtn(GameObject go)
    {
        MonsterCardBuyIndex = int.Parse(go.name);
        MonsterCardPic.mainTexture = Resources.Load<Texture>("Texture/MonsterCard/" + MonsterCardData[MonsterCardBuyIndex].Icon.Replace(".jpg", "").Replace(".png", ""));
        MonsterCardDiamondNumLabel.text = MonsterCardData[MonsterCardBuyIndex].Value.ToString();
        UIEventListener.Get(MonsterCardBuyBtn.gameObject).onClick = MonsterCardBuyBtnOnClick;
        UIEventListener.Get(MonsterCardBackBtn.gameObject).onClick = CloseDetail;
        ShowDetail(2);
    }

    /// <summary>
    /// 打开详情页面
    /// </summary>
    /// <param name="type">商品类型</param>
    private void ShowDetail(int type)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        foreach (GameObject itme in DetailPanel)
        {
            itme.SetActive(false);
        }
        DetailPanel[type].SetActive(true);
        Detail.SetActive(true);
    }

    /// <summary>
    /// 关闭详情页面
    /// </summary>
    /// <param name="go"></param>
    private void CloseDetail(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Detail.SetActive(false);
    }

    /// <summary>
    /// 兑换实物
    /// </summary>
    /// <param name="go"></param>
    private void RealThingBuyBtnOnClick(GameObject go)
    {
        OpenSureBuyGood(RealThingData[RealThingBuyIndex].Id, RealThingData[RealThingBuyIndex].type, RealThingBuyColorIndex,
            RealThingData[RealThingBuyIndex].Value, RealThingData[RealThingBuyIndex].Name);
    }

    /// <summary>
    /// 兑换代金券
    /// </summary>
    /// <param name="go"></param>
    private void VoucherBuyBtnOnClick(GameObject go)
    {
        OpenSureBuyGood(VoucherData[VoucherBuyIndex].itemId, VoucherData[VoucherBuyIndex].type, 1,
            VoucherData[VoucherBuyIndex].Value, VoucherData[VoucherBuyIndex].Name);
    }

    /// <summary>
    /// 兑换怪物卡牌
    /// </summary>
    /// <param name="go"></param>
    private void MonsterCardBuyBtnOnClick(GameObject go)
    {
        OpenSureBuyGood(MonsterCardData[MonsterCardBuyIndex].itemId, MonsterCardData[MonsterCardBuyIndex].type, 1,
            MonsterCardData[MonsterCardBuyIndex].Value, MonsterCardData[MonsterCardBuyIndex].Name);
    }

    private long BuyGoodId = 0;
    private long BuyGoodType = 0;
    private long BuyGoodIndex = 0;
    private long BuyGoodPrice = 0;
    /// <summary>
    /// 打开确认兑换界面
    /// </summary>
    /// <param name="goodId"></param>
    /// <param name="goodType"></param>
    /// <param name="goodIndex"></param>
    private void OpenSureBuyGood(long goodId, long goodType, int goodIndex, long goodDiamondNum, string goodName)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BuyGoodId = goodId;
        BuyGoodType = goodType;
        BuyGoodIndex = goodIndex;
        BuyGoodPrice = goodDiamondNum;
        SureDiamondNumLabel.text = BuyGoodPrice.ToString();
        SureGoodNameLabel.text = "兑换1个" + goodName + "吗";
        UIEventListener.Get(SureBtn.gameObject).onClick = SureBtnOnClick;
        UIEventListener.Get(SureBackBtn.gameObject).onClick = SureBackBtnOnClick;
        SureBuyPanel.SetActive(true);
        BuySuccessPanel.SetActive(false);
        BuyFailPanel.SetActive(false);
        SureBuy.SetActive(true);
    }

    /// <summary>
    /// 确认兑换
    /// </summary>
    /// <param name="go"></param>
    private void SureBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BuyGood();
    }

    /// <summary>
    /// 兑换商品
    /// </summary>
    private void BuyGood()
    {
        BuyItemByShop.request msg = new BuyItemByShop.request
        {
            itemId = BuyGoodId,
            type = BuyGoodType,
            index = BuyGoodIndex
        };
        Debug.Log("请求兑换商品");
        NetSender.Send<ProtoProtocol.BuyItemByShop>(msg, BuyGoodResult);
    }

    /// <summary>
    /// 收到兑换结果
    /// </summary>
    /// <param name="msg">兑换结果</param>
    private void BuyGoodResult(SprotoTypeBase msg)
    {
        Debug.Log("收到兑换结果");
        UIEventListener.Get(BagBtn.gameObject).onClick = TreasureChestBtnOnClick;
        UIEventListener.Get(BuySuccessBackBtn.gameObject).onClick = SureBackBtnOnClick;
        UIEventListener.Get(BuyFailSureBtn.gameObject).onClick = SureBackBtnOnClick;

        var data = (BuyItemByShop.response)msg;
        if (data.status == 0)
        {
            AudicoManager.instance.Play("effect", "Effect/get the goods");
            Debug.Log("兑换成功");
            Detail.SetActive(false);
            BuySuccessPanel.SetActive(true);
            // 更新钻石数量
            DataManager.GetInstance().roleData.Diamond = DataManager.GetInstance().roleData.Diamond - BuyGoodPrice;
            DiamondNumLabel.text = DataManager.GetInstance().roleData.Diamond.ToString();
            Debug.Log("请求更新商城数据");
            NetSender.Send<ProtoProtocol.GetShopList>(null, GetShopList);
        } else if (data.status == -1)
        {
            Debug.Log("不存在该商品，兑换失败");
            BuyFailSorryLabel.text = "很遗憾";
            BuyFailReasonLabel.text = "该商品不存在";
            BuyFailPanel.SetActive(true);
        }
        else if (data.status == -2)
        {
            Debug.Log("钻石不够，兑换失败");
            BuyFailSorryLabel.text = "对不起";
            BuyFailReasonLabel.text = "您的钻石不够了";
            BuyFailPanel.SetActive(true);
        }
        else if (data.status == -3)
        {
            Debug.Log("商品缺货，兑换失败");
            BuyFailSorryLabel.text = "很遗憾";
            BuyFailReasonLabel.text = "该商品已经被人买光了";
            BuyFailPanel.SetActive(true);
        }
        else
        {
            Debug.Log("兑换失败");
            BuyFailSorryLabel.text = "很遗憾";
            BuyFailReasonLabel.text = "兑换失败";
            BuyFailPanel.SetActive(true);
        }
        SureBuyPanel.SetActive(false);
    }

    /// <summary>
    /// 收到商店数据
    /// </summary>
    /// <param name="msg">商店数据</param>
    private void GetShopList(SprotoTypeBase msg)
    {
        Debug.Log("收到商店数据");
        var data = (GetShopList.response)msg;
        DataManager.GetInstance().shopInfoData = data;
        // 更新商店数据
        InitGoodsData();
    }

    /// <summary>
    /// 关闭兑换相关界面
    /// </summary>
    /// <param name="go"></param>
    private void SureBackBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        SureBuyPanel.SetActive(false);
        BuySuccessPanel.SetActive(false);
        BuyFailPanel.SetActive(false);
        SureBuy.SetActive(false);
    }

    /// <summary>
    /// 点击关闭商城按钮
    /// </summary>
    /// <param name="go"></param>
    private void BackBtnOnClick(GameObject go)
    {
        Debug.Log("关闭商店界面");
        AudicoManager.instance.Play("effect", "Effect/press button");
        AudicoManager.instance.Play("music", "Music/hall screen");
        Destroy(gameObject);
    }

    /// <summary>
    /// 点击我的宝箱
    /// </summary>
    /// <param name="go"></param>
    private void TreasureChestBtnOnClick(GameObject go)
    {
        Debug.Log("请求打开背包界面");
        AudicoManager.instance.Play("effect", "Effect/press button");
        NetSender.Send<ProtoProtocol.GetBagInfo>(null, GetBagInfo);
    }

    /// <summary>
    /// 收到背包数据
    /// </summary>
    /// <param name="rpcRsp">背包数据</param>
    private void GetBagInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到背包数据");
        var data = (GetBagInfo.response)rpcRsp;
        DataManager.GetInstance().bagInfoData = data;
        OpenUI("BagPanel");
    }

    /// <summary>
    /// 打开UI
    /// </summary>
    /// <param name="ui">UI名</param>
    private void OpenUI(string ui)
    {
        Transform _transform = (Instantiate(Resources.Load("UI/" + ui)) as GameObject).transform;
        _transform.parent = transform.parent;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
        Destroy(gameObject);
    }
}