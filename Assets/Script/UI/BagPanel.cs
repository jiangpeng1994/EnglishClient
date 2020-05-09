using ProtoSprotoType;
using Sproto;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BagPanel : MonoBehaviour
{
    public UILabel DiamondNumLabel;
    public UIButton BackBtn;

    // 物品UI
    public Texture[] BtnBg;
    public Color[] BtnLabelColor;
    public UIButton[] GoodsTpyeBtns;
    public UITexture[] GoodsTpyeBtnsTexture;
    public UILabel[] GoodsTpyeBtnsLabel;
    public GameObject[] GoodsTpyeBtnsPanel;
    public int CurType;

    public UIGrid RealThingGrid;
    public GameObject RealThingItem;
    private List<GameObject> RealThingItemList = new List<GameObject>();
    private List<UIButton> RealThingBtnList = new List<UIButton>();
    private List<UITexture> RealThingPicList = new List<UITexture>();
    private List<UILabel> RealThingNameList = new List<UILabel>();
    private List<GameObject> RealThingChooseList = new List<GameObject>();

    public UIGrid VoucherGrid;
    public GameObject VoucherItem;
    private List<GameObject> VoucherItemList = new List<GameObject>();
    private List<UIButton> VoucherBtnList = new List<UIButton>();
    private List<UITexture> VoucherPicList = new List<UITexture>();
    private List<UILabel> VoucherNameList = new List<UILabel>();
    private List<GameObject> VoucherChooseList = new List<GameObject>();

    public UIGrid MonsterCardGrid;
    public GameObject MonsterCardItem;
    private List<GameObject> MonsterCardItemList = new List<GameObject>();
    private List<UIButton> MonsterCardBtnList = new List<UIButton>();
    private List<UITexture> MonsterCardPicList = new List<UITexture>();
    private List<UILabel> MonsterCardNumList = new List<UILabel>();
    private List<GameObject> MonsterChooseList = new List<GameObject>();

    // 详情UI
    public GameObject Detail;
    public GameObject[] DetailPanel;

    public UILabel RealThingNameLabel;
    public UITexture RealThingPic;
    public UILabel RealThingDescriptionLabel;
    public UILabel RealThingDiamondNumLabel;
    public UIButton RealThingBackBtn;

    public UILabel VoucherNameLabel;
    public UITexture VoucherPic;
    public UILabel VoucherDescriptionLabel;
    public UILabel VoucherDiamondNumLabel;
    public UIButton VoucherBackBtn;

    public UITexture MonsterCardPic;
    public UILabel MonsterCardDiamondNumLabel;
    public UIButton MonsterCardBackBtn;

    /// <summary>
    /// 怪物卡牌选择按钮
    /// </summary>
    public UIScrollView MonsterCardScrollview;
    public Texture[] MonsterCardTpyeBg;
    public Color[] MonsterCardTpyeColor;
    public UIButton[] MonsterCardTpyeBtns;
    public UITexture[] MonsterCardTpyeTexture;
    public UILabel[] MonsterCardTpyeLabel;

    /// <summary>
    /// 背包删除相关
    /// </summary>
    public UIButton DeleteBtn;
    public UIButton ChooseAllBtn;
    public UIButton SureDeleteBtn;
    public GameObject Option;
    public bool isDelete;
    public List<string> RealThingChoosed = new List<string>();
    public List<string> VoucherChoosed = new List<string>();
    public List<string> MonsterChoosed = new List<string>();
    public int MonsterHaveNum;

    /// <summary>
    /// 克隆卡牌相关
    /// </summary>
    public UIButton CloneBtn;
    public UIButton CloneChooseAllBtn;
    public UIButton SureCloneBtn;
    public UIButton CloneHistoryBtn;
    public GameObject CloneOption;
    public bool isClone;
    public List<string> MonsterCloneChoosed = new List<string>();


    // 背包数据
    private GetBagInfo.response bagInfoData;
    private List<item1Info> RealThingData = new List<item1Info>();
    private List<item2Info> VoucherData = new List<item2Info>();
    private List<item3Info> CurMonsterCardData = new List<item3Info>();
    private List<item3Info> MonsterCardData = new List<item3Info>();
    private List<item3Info> MonsterCardSeaData = new List<item3Info>();
    private List<item3Info> MonsterCardFernData = new List<item3Info>();
    private List<item3Info> MonsterCardIceData = new List<item3Info>();
    private List<item3Info> MonsterCardSandData = new List<item3Info>();

    void Start()
    {
        AudicoManager.instance.Play("music", "Music/treasure screen");
        Init(0);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init(int goodsTpye)
    {
        DiamondNumLabel.text = DataManager.GetInstance().roleData.Diamond.ToString();
        for (int i = 0; i < GoodsTpyeBtns.Length; i++)
        {
            UIEventListener.Get(GoodsTpyeBtns[i].gameObject).onClick = GoodsTpyeBtnsOnClick;
            GoodsTpyeBtnsPanel[i].SetActive(false);
        }

        Detail.SetActive(false);
        isDelete = false;
        isClone = false;
        Option.SetActive(isDelete);
        CloneOption.SetActive(isClone);
        CloneAnimation.SetActive(false);
        CloneHistory.SetActive(false);
        InitGoodsData();
        ClickGoodsTpye(goodsTpye);

        UIEventListener.Get(BackBtn.gameObject).onClick = BackBtnOnClick;
        UIEventListener.Get(DeleteBtn.gameObject).onClick = DeleteBtnOnClick;
        UIEventListener.Get(ChooseAllBtn.gameObject).onClick = ChooseAllBtnOnClick;
        UIEventListener.Get(SureDeleteBtn.gameObject).onClick = SureDeleteBtnOnClick;

        UIEventListener.Get(CloneBtn.gameObject).onClick = CloneBtnOnClick;
        //UIEventListener.Get(CloneChooseAllBtn.gameObject).onClick = CloneChooseAllBtnOnClick;
        UIEventListener.Get(SureCloneBtn.gameObject).onClick = SureCloneBtnOnClick;
        UIEventListener.Get(CloneHistoryBtn.gameObject).onClick = CloneHistoryBtnOnClick;
    }

    /// <summary>
    /// 初始化商品数据
    /// </summary>
    private void InitGoodsData()
    {
        bagInfoData = DataManager.GetInstance().bagInfoData;
        RealThingData = bagInfoData.itemList1;
        VoucherData = bagInfoData.itemList2;
        MonsterCardData = bagInfoData.itemList3;

        MonsterCardSeaData.Clear();
        MonsterCardFernData.Clear();
        MonsterCardIceData.Clear();
        MonsterCardSandData.Clear();
        MonsterHaveNum = 0;
        foreach (item3Info item in MonsterCardData)
        {
            if (item.num > 0)
            {
                MonsterHaveNum++;
            }

            if (item.cardType == 1)
            {
                MonsterCardSeaData.Add(item);
            } else if (item.cardType == 2)
            {
                MonsterCardFernData.Add(item);
            }
            else if (item.cardType == 3)
            {
                MonsterCardIceData.Add(item);
            }
            else if (item.cardType == 4)
            {
                MonsterCardSandData.Add(item);
            }
        }
        RealThingChoosed.Clear();
        VoucherChoosed.Clear();
        MonsterChoosed.Clear();
        MonsterCloneChoosed.Clear();

        InitRealThingData();
        InitVoucherData();
        InitMonsterCardType();
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
                RealThingChooseList.Add(RealThingItemList[i].transform.Find("Choosed").gameObject);
            }

            RealThingBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(RealThingBtnList[i].gameObject).onClick = OnClickRealThingBtn;
            RealThingPicList[i].mainTexture = Resources.Load<Texture>("Texture/Goods/" + RealThingData[i].Icon.Replace(".jpg", "").Replace(".png", ""));
            RealThingNameList[i].text = RealThingData[i].Name;
            RealThingChooseList[i].SetActive(false);

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
                VoucherChooseList.Add(VoucherItemList[i].transform.Find("Choosed").gameObject);
            }

            VoucherBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(VoucherBtnList[i].gameObject).onClick = OnClickVoucherBtn;
            VoucherPicList[i].mainTexture = Resources.Load<Texture>("Texture/Goods/" + VoucherData[i].Icon.Replace(".jpg", "").Replace(".png", ""));
            VoucherNameList[i].text = VoucherData[i].Name;
            VoucherChooseList[i].SetActive(false);

            VoucherItemList[i].SetActive(true);
        }
        VoucherGrid.enabled = true;
    }

    /// <summary>
    /// 初始化怪物卡牌的类型
    /// </summary>
    private void InitMonsterCardType()
    {
        for (int i = 0; i < MonsterCardTpyeBtns.Length; i++)
        {
            UIEventListener.Get(MonsterCardTpyeBtns[i].gameObject).onClick = MonsterCardTpyeBtnsOnClick;
        }

        ClickMonsterCardTpye(0);
    }

    /// <summary>
    /// 点击怪物卡牌类型
    /// </summary>
    /// <param name="go"></param>
    private void MonsterCardTpyeBtnsOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        GameTools.Instance.ButtonJelly(go);
        int ClickNum = int.Parse(go.name);
        for (int i = 0; i < MonsterCardTpyeBtns.Length; i++)
        {
            MonsterCardTpyeTexture[i].mainTexture = MonsterCardTpyeBg[0];
            MonsterCardTpyeLabel[i].color = MonsterCardTpyeColor[0];
        }

        ClickMonsterCardTpye(ClickNum);
    }

    /// <summary>
    /// 设置卡牌类型按钮的选择效果
    /// </summary>
    /// <param name="ClickNum">点击卡牌类型按钮的编号</param>
    private void ClickMonsterCardTpye(int ClickNum)
    {
        MonsterCardTpyeTexture[ClickNum].mainTexture = MonsterCardTpyeBg[1];
        MonsterCardTpyeLabel[ClickNum].color = MonsterCardTpyeColor[1];
        if (ClickNum == 0)
        {
            CurMonsterCardData = MonsterCardData;
        }
        else if (ClickNum == 1)
        {
            CurMonsterCardData = MonsterCardSeaData;
        }
        else if (ClickNum == 2)
        {
            CurMonsterCardData = MonsterCardFernData;
        }
        else if (ClickNum == 3)
        {
            CurMonsterCardData = MonsterCardIceData;
        }
        else if (ClickNum == 4)
        {
            CurMonsterCardData = MonsterCardSandData;
        }

        InitMonsterCardData();
    }

    /// <summary>
    /// 初始化怪物卡牌的数据
    /// </summary>
    private void InitMonsterCardData()
    {
        int partNum = CurMonsterCardData.Count;
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
                item.transform.localScale = new Vector3(0.6f,0.6f,1);
                MonsterCardItemList.Add(item);

                MonsterCardBtnList.Add(MonsterCardItemList[i].transform.GetComponent<UIButton>());
                MonsterCardPicList .Add(MonsterCardItemList[i].transform.GetComponent<UITexture>());
                MonsterCardNumList.Add(MonsterCardItemList[i].transform.Find("NumLabel").GetComponent<UILabel>());
                MonsterChooseList.Add(MonsterCardItemList[i].transform.Find("Choosed").gameObject);
            }

            MonsterCardBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(MonsterCardBtnList[i].gameObject).onClick = OnClickMonsterCardBtn;
            MonsterCardPicList[i].mainTexture = Resources.Load<Texture>("Texture/MonsterCard/" + CurMonsterCardData[i].itemIcon.Replace(".jpg", "").Replace(".png", ""));
            if (CurMonsterCardData[i].num >= 0)
            {
                MonsterCardPicList[i].color = new Color(1, 1, 1, 1);
                MonsterCardBtnList[i].hover = new Color(1, 1, 1, 1);
                MonsterCardBtnList[i].pressed = new Color(1, 1, 1, 1);
                MonsterCardBtnList[i].defaultColor = new Color(1, 1, 1, 1);
                MonsterCardNumList[i].text = "x"+CurMonsterCardData[i].num.ToString();
                MonsterCardNumList[i].gameObject.SetActive(true);
            } else
            {
                MonsterCardPicList[i].color = new Color(0, 0, 0, 1);
                MonsterCardBtnList[i].hover = new Color(0, 0, 0, 1);
                MonsterCardBtnList[i].pressed = new Color(0, 0, 0, 1);
                MonsterCardBtnList[i].defaultColor = new Color(0, 0, 0, 1);
                MonsterCardNumList[i].text = "";
                MonsterCardNumList[i].gameObject.SetActive(false);
            }

            if (MonsterChoosed.Contains(CurMonsterCardData[i].uniqueId) || MonsterCloneChoosed.Contains(CurMonsterCardData[i].uniqueId))
            {
                MonsterChooseList[i].SetActive(true);
            } else
            {
                MonsterChooseList[i].SetActive(false);
            }

            MonsterCardItemList[i].SetActive(true);
        }
        MonsterCardGrid.enabled = true;
        MonsterCardScrollview.ResetPosition();
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
        CurType = ClickNum;
        GoodsTpyeBtnsTexture[ClickNum].mainTexture = BtnBg[1];
        GoodsTpyeBtnsLabel[ClickNum].color = BtnLabelColor[1];
        GoodsTpyeBtnsPanel[ClickNum].SetActive(true);
    }

    private int RealThingChooseIndex = 0;
    private int VoucherChooseIndex = 0;
    private int MonsterChooseIndex = 0;
    /// <summary>
    /// 点击实物
    /// </summary>
    /// <param name="go"></param>
    private void OnClickRealThingBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        RealThingChooseIndex = int.Parse(go.name);

        if (isDelete)
        {
            RealThingChooseList[RealThingChooseIndex].SetActive(!RealThingChooseList[RealThingChooseIndex].activeSelf);

            if (RealThingChooseList[RealThingChooseIndex].activeSelf)
            {
                if (!RealThingChoosed.Contains(RealThingData[RealThingChooseIndex].uniqueId))
                {
                    RealThingChoosed.Add(RealThingData[RealThingChooseIndex].uniqueId);
                }
            } else
            {
                if (RealThingChoosed.Contains(RealThingData[RealThingChooseIndex].uniqueId))
                {
                    RealThingChoosed.Remove(RealThingData[RealThingChooseIndex].uniqueId);
                }
            }
        } else
        {
            RealThingNameLabel.text = RealThingData[RealThingChooseIndex].Name;
            RealThingPic.mainTexture = Resources.Load<Texture>("Texture/Goods/" + RealThingData[RealThingChooseIndex].Icon.Replace(".jpg", "").Replace(".png", ""));
            RealThingDescriptionLabel.text = RealThingData[RealThingChooseIndex].Depict;
            RealThingDiamondNumLabel.text = RealThingData[RealThingChooseIndex].Value.ToString();
            UIEventListener.Get(RealThingBackBtn.gameObject).onClick = CloseDetail;
            ShowDetail(0);
        }
    }

    /// <summary>
    /// 点击代金券
    /// </summary>
    /// <param name="go"></param>
    private void OnClickVoucherBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        VoucherChooseIndex = int.Parse(go.name);

        if (isDelete)
        {
            VoucherChooseList[VoucherChooseIndex].SetActive(!VoucherChooseList[VoucherChooseIndex].activeSelf);

            if (VoucherChooseList[VoucherChooseIndex].activeSelf)
            {
                if (!VoucherChoosed.Contains(VoucherData[VoucherChooseIndex].uniqueId))
                {
                    VoucherChoosed.Add(VoucherData[VoucherChooseIndex].uniqueId);
                }
            }
            else
            {
                if (VoucherChoosed.Contains(VoucherData[VoucherChooseIndex].uniqueId))
                {
                    VoucherChoosed.Remove(VoucherData[VoucherChooseIndex].uniqueId);
                }
            }
        } else
        {
            VoucherNameLabel.text = VoucherData[VoucherChooseIndex].Name;
            VoucherPic.mainTexture = Resources.Load<Texture>("Texture/Goods/" + VoucherData[VoucherChooseIndex].Icon.Replace(".jpg", "").Replace(".png", ""));
            VoucherDescriptionLabel.text = VoucherData[VoucherChooseIndex].Depict;
            VoucherDiamondNumLabel.text = VoucherData[VoucherChooseIndex].Value.ToString();
            UIEventListener.Get(VoucherBackBtn.gameObject).onClick = CloseDetail;
            ShowDetail(1);
        }
    }

    /// <summary>
    /// 点击怪物卡牌
    /// </summary>
    /// <param name="go"></param>
    private void OnClickMonsterCardBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        MonsterChooseIndex = int.Parse(go.name);

        if (isDelete)
        {
            if (CurMonsterCardData[MonsterChooseIndex].num > 0)
            {
                MonsterChooseList[MonsterChooseIndex].SetActive(!MonsterChooseList[MonsterChooseIndex].activeSelf);

                if (MonsterChooseList[MonsterChooseIndex].activeSelf)
                {
                    if (!MonsterChoosed.Contains(CurMonsterCardData[MonsterChooseIndex].uniqueId))
                    {
                        MonsterChoosed.Add(CurMonsterCardData[MonsterChooseIndex].uniqueId);
                    }
                }
                else
                {
                    if (MonsterChoosed.Contains(CurMonsterCardData[MonsterChooseIndex].uniqueId))
                    {
                        MonsterChoosed.Remove(CurMonsterCardData[MonsterChooseIndex].uniqueId);
                    }
                }
            } else
            {
                GameTools.Instance.TipsShow("卡牌数量不足，无法选中");
            }
        }
        else if(isClone)
        {
            /*if (CurMonsterCardData[MonsterChooseIndex].num > 0)
            {
                MonsterChooseList[MonsterChooseIndex].SetActive(!MonsterChooseList[MonsterChooseIndex].activeSelf);

                if (MonsterChooseList[MonsterChooseIndex].activeSelf)
                {
                    if (!MonsterCloneChoosed.Contains(CurMonsterCardData[MonsterChooseIndex].uniqueId))
                    {
                        MonsterCloneChoosed.Add(CurMonsterCardData[MonsterChooseIndex].uniqueId);
                    }
                }
                else
                {
                    if (MonsterCloneChoosed.Contains(CurMonsterCardData[MonsterChooseIndex].uniqueId))
                    {
                        MonsterCloneChoosed.Remove(CurMonsterCardData[MonsterChooseIndex].uniqueId);
                    }
                }
            }
            else
            {
                GameTools.Instance.TipsShow("卡牌数量不足，无法选中");
            }*/
        }
        else
        {
            MonsterCardPic.mainTexture = Resources.Load<Texture>("Texture/MonsterCard/" + CurMonsterCardData[MonsterChooseIndex].itemIcon.Replace(".jpg", "").Replace(".png", ""));
            //MonsterCardDiamondNumLabel.text = MonsterCardData[MonsterCardBuyIndex].Value.ToString();
            UIEventListener.Get(MonsterCardBackBtn.gameObject).onClick = CloseDetail;
            ShowDetail(2);
        }
    }

    /// <summary>
    /// 打开详情页面
    /// </summary>
    /// <param name="type">商品类型</param>
    private void ShowDetail(int type)
    {
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
    /// 点击关闭背包按钮
    /// </summary>
    /// <param name="go"></param>
    private void BackBtnOnClick(GameObject go)
    {
        Debug.Log("关闭背包界面");
        AudicoManager.instance.Play("effect", "Effect/press button");
        AudicoManager.instance.Play("music", "Music/hall screen");
        Destroy(gameObject);
    }

    /// <summary>
    /// 点击删除按钮
    /// </summary>
    /// <param name="go"></param>
    private void DeleteBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (isClone)
        {
            CloneBtnOnClick(null);
        }

        isDelete = !Option.activeSelf;
        Option.SetActive(isDelete);

        if (!isDelete)
        {
            RealThingChoosed.Clear();
            VoucherChoosed.Clear();
            MonsterChoosed.Clear();
            for (int i = 0; i < RealThingItemList.Count; i++)
            {
                RealThingChooseList[i].SetActive(false);
            }
            for (int i = 0; i < VoucherItemList.Count; i++)
            {
                VoucherChooseList[i].SetActive(false);
            }
            for (int i = 0; i < MonsterCardItemList.Count; i++)
            {
                MonsterChooseList[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 点击全选按钮
    /// </summary>
    /// <param name="go"></param>
    private void ChooseAllBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");

        if (CurType == 0)
        {
            RealThingChoosed.Clear();
            for (int i = 0; i < RealThingData.Count; i++)
            {
                RealThingChooseList[i].SetActive(true);
                RealThingChoosed.Add(RealThingData[i].uniqueId);
            }
        } else if (CurType == 1)
        {
            VoucherChoosed.Clear();
            for (int i = 0; i < VoucherData.Count; i++)
            {
                VoucherChooseList[i].SetActive(true);
                VoucherChoosed.Add(VoucherData[i].uniqueId);
            }
        } else if (CurType == 2)
        {
            MonsterChoosed.Clear();
            for (int i = 0; i < CurMonsterCardData.Count; i++)
            {
                if (CurMonsterCardData[i].num > 0)
                {
                    MonsterChooseList[i].SetActive(true);
                }
            }
            for (int i = 0; i < MonsterCardData.Count; i++)
            {
                if (MonsterCardData[i].num > 0)
                {
                    MonsterChoosed.Add(MonsterCardData[i].uniqueId);
                }
            }
        }
    }

    /// <summary>
    /// 点击确认删除按钮
    /// </summary>
    /// <param name="go"></param>
    private void SureDeleteBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DeleteRealThingItem(null);
    }

    /// <summary>
    /// 删除实物
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void DeleteRealThingItem(SprotoTypeBase rpcRsp)
    {
        if (RealThingChoosed.Count > 0)
        {
            DeleteItem.request msg = new DeleteItem.request();
            msg.type = 3;
            if (RealThingChoosed.Count >= RealThingData.Count)
            {
                msg.isAll = true;
                NetSender.Send<ProtoProtocol.DeleteItem>(msg, DeleteVoucherItem);
            }
            else
            {
                msg.isAll = false;
                msg.uniqueId = RealThingChoosed[0];
                RealThingChoosed.RemoveAt(0);
                NetSender.Send<ProtoProtocol.DeleteItem>(msg, DeleteRealThingItem);
            }
        } else
        {
            DeleteVoucherItem(null);
        }
    }

    /// <summary>
    /// 删除兑换券
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void DeleteVoucherItem(SprotoTypeBase rpcRsp)
    {
        if (VoucherChoosed.Count > 0)
        {
            DeleteItem.request msg = new DeleteItem.request();
            msg.type = 1;
            if (VoucherChoosed.Count >= VoucherData.Count)
            {
                msg.isAll = true;
                NetSender.Send<ProtoProtocol.DeleteItem>(msg, DeleteMonsterItem);
            }
            else
            {
                msg.isAll = false;
                msg.uniqueId = VoucherChoosed[0];
                VoucherChoosed.RemoveAt(0);
                NetSender.Send<ProtoProtocol.DeleteItem>(msg, DeleteVoucherItem);
            }
        }
        else
        {
            DeleteMonsterItem(null);
        }
    }

    /// <summary>
    /// 删除卡牌
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void DeleteMonsterItem(SprotoTypeBase rpcRsp)
    {
        if (MonsterChoosed.Count > 0)
        {
            DeleteItem.request msg = new DeleteItem.request();
            msg.type = 2;
            /*if (MonsterChoosed.Count >= MonsterHaveNum)
            {
                msg.isAll = true;
                NetSender.Send<ProtoProtocol.DeleteItem>(msg, RefreshBag);
            }
            else
            {
                msg.isAll = false;
                msg.uniqueId = MonsterChoosed[0];
                MonsterChoosed.RemoveAt(0);
                NetSender.Send<ProtoProtocol.DeleteItem>(msg, DeleteMonsterItem);
            }*/
            msg.isAll = false;
            msg.uniqueId = MonsterChoosed[0];
            MonsterChoosed.RemoveAt(0);
            NetSender.Send<ProtoProtocol.DeleteItem>(msg, DeleteMonsterItem);
        }
        else
        {
            RefreshBag(null);
        }
    }

    /// <summary>
    /// 点击克隆按钮
    /// </summary>
    /// <param name="go"></param>
    private void CloneBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (isDelete)
        {
            DeleteBtnOnClick(null);
        }

        isClone = !CloneOption.activeSelf;
        CloneOption.SetActive(isClone);

        if (!isClone)
        {
            MonsterCloneChoosed.Clear();

            for (int i = 0; i < MonsterCardItemList.Count; i++)
            {
                MonsterChooseList[i].SetActive(false);
            }
        } else
        {
            CloneChooseAllBtnOnClick(null);
        }
    }

    /// <summary>
    /// 点击卡牌克隆全选按钮
    /// </summary>
    /// <param name="go"></param>
    private void CloneChooseAllBtnOnClick(GameObject go)
    {
        //AudicoManager.instance.Play("effect", "Effect/press button");
        MonsterCloneChoosed.Clear();
        for (int i = 0; i < CurMonsterCardData.Count; i++)
        {
            if (CurMonsterCardData[i].num > 0)
            {
                MonsterChooseList[i].SetActive(true);
            }
        }
        for (int i = 0; i < MonsterCardData.Count; i++)
        {
            if (MonsterCardData[i].num > 0)
            {
                MonsterCloneChoosed.Add(MonsterCardData[i].uniqueId);
            }
        }
    }

    /// <summary>
    /// 点击确认克隆按钮
    /// </summary>
    /// <param name="go"></param>
    private void SureCloneBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：卡牌克隆");
        NetSender.Send<ProtoProtocol.BeginClone>(null, OnBeginClone);
    }

    public GameObject CloneAnimation;
    public Animation Animation;
    public AnimationClip Clone_start;
    /// <summary>
    /// 收到克隆结果
    /// </summary>
    /// <param name="rpcRsp">背包数据</param>
    private void OnBeginClone(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：卡牌克隆结果");
        var data = (BeginClone.response)rpcRsp;
        if (data.status)
        {
            Debug.Log("收到：卡牌克隆成功");
            CloneAnimation.SetActive(true);
            Animation.Play("Clone_start");
            Invoke("OnStopAnimation", Clone_start.length);
        } else
        {
            GameTools.Instance.TipsShow("卡牌数量不足20张，无法进行克隆");
        }
    }

    /// <summary>
    /// 结束克隆动画
    /// </summary>
    private void OnStopAnimation()
    {
        RefreshBag(null);
        CloneAnimation.SetActive(false);
    }

    /// <summary>
    /// 刷新背包数据
    /// </summary>
    /// <param name="msg"></param>
    private void RefreshBag(SprotoTypeBase msg)
    {
        Debug.Log("请求：刷新背包数据");
        NetSender.Send<ProtoProtocol.GetBagInfo>(null, GetBagInfo);
    }

    /// <summary>
    /// 收到背包数据
    /// </summary>
    /// <param name="msg">背包数据</param>
    private void GetBagInfo(SprotoTypeBase msg)
    {
        Debug.Log("收到：背包数据");
        var data = (GetBagInfo.response)msg;
        DataManager.GetInstance().bagInfoData = data;
        // 更新背包数据
        Init(CurType);
    }

    /// <summary>
    /// 点击克隆历史记录按钮
    /// </summary>
    /// <param name="go"></param>
    private void CloneHistoryBtnOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：克隆历史数据");
        NetSender.Send<ProtoProtocol.GetCloneInfo>(null, OnGetCloneInfo);
    }

    public GameObject CloneHistory;
    public GameObject CloneCardList;
    public UIButton CloneHistoryCloseBtn;
    public UIGrid CloneCardGrid;
    public GameObject CloneCardItem;
    private List<GameObject> CloneCardItemList = new List<GameObject>();
    private List<UIButton> CloneCardBtnList = new List<UIButton>();
    private List<UILabel> CloneCardNumList = new List<UILabel>();
    private List<UILabel> CloneCardTimeList = new List<UILabel>();
    private bool isCloneCardList;
    private GetCloneInfo.response CloneInfo;
    /// <summary>
    /// 收到克隆历史结果
    /// </summary>
    /// <param name="rpcRsp">克隆历史</param>
    private void OnGetCloneInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：克隆历史数据");
        CloneInfo = (GetCloneInfo.response)rpcRsp;

        int partNum = CloneInfo.cloneCardList.Count;
        if (partNum < CloneCardItemList.Count)
        {
            for (int i = partNum; i < CloneCardItemList.Count; i++)
            {
                CloneCardItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= CloneCardItemList.Count)
            {
                GameObject item = Instantiate(CloneCardItem);
                item.name = i.ToString();
                item.transform.parent = CloneCardGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = new Vector3(1, 1, 1);
                CloneCardItemList.Add(item);

                CloneCardBtnList.Add(CloneCardItemList[i].transform.GetComponent<UIButton>());
                CloneCardNumList.Add(CloneCardItemList[i].transform.Find("NumLabel").GetComponent<UILabel>());
                CloneCardTimeList.Add(CloneCardItemList[i].transform.Find("TimeLabel").GetComponent<UILabel>());
            }

            CloneCardBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(CloneCardBtnList[i].gameObject).onClick = OnClickCloneCardBtn;
            CloneCardNumList[i].text = CloneInfo.cloneCardList[i].num.ToString();
            CloneCardTimeList[i].text = GetDateTime((int)CloneInfo.cloneCardList[i].timestamp);

            CloneCardItemList[i].SetActive(true);
        }
        CloneCardGrid.enabled = true;

        isCloneCardList = true;
        UIEventListener.Get(CloneHistoryCloseBtn.gameObject).onClick = OnClickCloneHistoryCloseBtn;
        CloneCardList.SetActive(true);
        CloneCard.SetActive(false);
        CloneHistory.SetActive(true);
    }

    /// <summary>
    /// 点击关闭克隆历史
    /// </summary>
    /// <param name="go"></param>
    private void OnClickCloneHistoryCloseBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (!isCloneCardList)
        {
            isCloneCardList = true;
            CloneCardList.SetActive(true);
            CloneCard.SetActive(false);
            CloneHistory.SetActive(true);
        } else
        {
            CloneCardList.SetActive(false);
            CloneCard.SetActive(false);
            CloneHistory.SetActive(false);
        }
    }

    public GameObject CloneCard;
    public UILabel CloneCardNum;
    public UILabel CloneCardTime;
    public UIButton[] CloneCardTpyeBtns;
    public UITexture[] CloneCardTpyeTexture;
    public UILabel[] CloneCardTpyeLabel;
    public UIScrollView MonsterCloneCardScrollview;
    private List<item3Info> CurMonsterCloneCardData = new List<item3Info>();
    private List<item3Info> MonsterCloneCardData = new List<item3Info>();
    private List<item3Info> MonsterCloneCardSeaData = new List<item3Info>();
    private List<item3Info> MonsterCloneCardFernData = new List<item3Info>();
    private List<item3Info> MonsterCloneCardIceData = new List<item3Info>();
    private List<item3Info> MonsterCloneCardSandData = new List<item3Info>();
    /// <summary>
    /// 点击卡组
    /// </summary>
    /// <param name="go"></param>
    private void OnClickCloneCardBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        int CloneCardChooseIndex = int.Parse(go.name);

        CloneCardNum.text = CloneInfo.cloneCardList[CloneCardChooseIndex].num.ToString();
        CloneCardTime.text = GetDateTime((int)CloneInfo.cloneCardList[CloneCardChooseIndex].timestamp);
        MonsterCloneCardData = CloneInfo.cloneCardList[CloneCardChooseIndex].info;

        MonsterCloneCardSeaData.Clear();
        MonsterCloneCardFernData.Clear();
        MonsterCloneCardIceData.Clear();
        MonsterCloneCardSandData.Clear();
        foreach (item3Info item in MonsterCloneCardData)
        {
            if (item.cardType == 1)
            {
                MonsterCloneCardSeaData.Add(item);
            }
            else if (item.cardType == 2)
            {
                MonsterCloneCardFernData.Add(item);
            }
            else if (item.cardType == 3)
            {
                MonsterCloneCardIceData.Add(item);
            }
            else if (item.cardType == 4)
            {
                MonsterCloneCardSandData.Add(item);
            }
        }
        InitCloneCardType();

        isCloneCardList = false;
        CloneCardList.SetActive(false);
        CloneCard.SetActive(true);
        CloneHistory.SetActive(true);
    }

    /// <summary>
    /// 初始化怪物卡牌克隆的类型
    /// </summary>
    private void InitCloneCardType()
    {
        for (int i = 0; i < CloneCardTpyeBtns.Length; i++)
        {
            UIEventListener.Get(CloneCardTpyeBtns[i].gameObject).onClick = CloneCardTpyeBtnsOnClick;
        }

        ClickCloneCardTpye(0);
    }

    /// <summary>
    /// 点击怪物卡牌克隆类型
    /// </summary>
    /// <param name="go"></param>
    private void CloneCardTpyeBtnsOnClick(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        GameTools.Instance.ButtonJelly(go);
        int ClickNum = int.Parse(go.name);
        ClickCloneCardTpye(ClickNum);
    }
    
    /// <summary>
    /// 设置卡牌克隆类型按钮的选择效果
    /// </summary>
    /// <param name="ClickNum">点击卡牌克隆类型按钮的编号</param>
    private void ClickCloneCardTpye(int ClickNum)
    {
        for (int i = 0; i < CloneCardTpyeBtns.Length; i++)
        {
            CloneCardTpyeTexture[i].mainTexture = MonsterCardTpyeBg[0];
            CloneCardTpyeLabel[i].color = MonsterCardTpyeColor[0];
        }

        CloneCardTpyeTexture[ClickNum].mainTexture = MonsterCardTpyeBg[1];
        CloneCardTpyeLabel[ClickNum].color = MonsterCardTpyeColor[1];
        if (ClickNum == 0)
        {
            CurMonsterCloneCardData = MonsterCloneCardData;
        }
        else if (ClickNum == 1)
        {
            CurMonsterCloneCardData = MonsterCloneCardSeaData;
        }
        else if (ClickNum == 2)
        {
            CurMonsterCloneCardData = MonsterCloneCardFernData;
        }
        else if (ClickNum == 3)
        {
            CurMonsterCloneCardData = MonsterCloneCardIceData;
        }
        else if (ClickNum == 4)
        {
            CurMonsterCloneCardData = MonsterCloneCardSandData;
        }

        InitMonsterCloneCardData();
    }

    public UIGrid MonsterCloneCardGrid;
    public GameObject MonsterCloneCardItem;
    private List<GameObject> MonsterCloneCardItemList = new List<GameObject>();
    private List<UIButton> MonsterCloneCardBtnList = new List<UIButton>();
    private List<UITexture> MonsterCloneCardPicList = new List<UITexture>();
    private List<UILabel> MonsterCloneCardNumList = new List<UILabel>();
    /// <summary>
    /// 初始化怪物克隆卡牌的数据
    /// </summary>
    private void InitMonsterCloneCardData()
    {
        int partNum = CurMonsterCloneCardData.Count;
        if (partNum < MonsterCloneCardItemList.Count)
        {
            for (int i = partNum; i < MonsterCloneCardItemList.Count; i++)
            {
                MonsterCloneCardItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= MonsterCloneCardItemList.Count)
            {
                GameObject item = Instantiate(MonsterCloneCardItem);
                item.name = i.ToString();
                item.transform.parent = MonsterCloneCardGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                MonsterCloneCardItemList.Add(item);

                MonsterCloneCardBtnList.Add(MonsterCloneCardItemList[i].transform.GetComponent<UIButton>());
                MonsterCloneCardPicList.Add(MonsterCloneCardItemList[i].transform.GetComponent<UITexture>());
                MonsterCloneCardNumList.Add(MonsterCloneCardItemList[i].transform.Find("NumLabel").GetComponent<UILabel>());
            }

            MonsterCloneCardBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(MonsterCloneCardBtnList[i].gameObject).onClick = OnClickMonsterCloneCardBtn;
            MonsterCloneCardPicList[i].mainTexture = Resources.Load<Texture>("Texture/MonsterCard/" + CurMonsterCloneCardData[i].itemIcon.Replace(".jpg", "").Replace(".png", ""));
            if (CurMonsterCloneCardData[i].num >= 0)
            {
                MonsterCloneCardPicList[i].color = new Color(1, 1, 1, 1);
                MonsterCloneCardBtnList[i].hover = new Color(1, 1, 1, 1);
                MonsterCloneCardBtnList[i].pressed = new Color(1, 1, 1, 1);
                MonsterCloneCardBtnList[i].defaultColor = new Color(1, 1, 1, 1);
                MonsterCloneCardNumList[i].text = "x" + CurMonsterCloneCardData[i].num.ToString();
                MonsterCloneCardNumList[i].gameObject.SetActive(true);
            }
            else
            {
                MonsterCloneCardPicList[i].color = new Color(0, 0, 0, 1);
                MonsterCloneCardBtnList[i].hover = new Color(0, 0, 0, 1);
                MonsterCloneCardBtnList[i].pressed = new Color(0, 0, 0, 1);
                MonsterCloneCardBtnList[i].defaultColor = new Color(0, 0, 0, 1);
                MonsterCloneCardNumList[i].text = "";
                MonsterCloneCardNumList[i].gameObject.SetActive(false);
            }

            MonsterCloneCardItemList[i].SetActive(true);
        }
        MonsterCloneCardGrid.enabled = true;
        MonsterCloneCardScrollview.ResetPosition();
    }

    /// <summary>
    /// 点击怪物克隆卡牌
    /// </summary>
    /// <param name="go"></param>
    private void OnClickMonsterCloneCardBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        int MonsterCloneChooseIndex = int.Parse(go.name);
        MonsterCardPic.mainTexture = Resources.Load<Texture>("Texture/MonsterCard/" + CurMonsterCloneCardData[MonsterCloneChooseIndex].itemIcon.Replace(".jpg", "").Replace(".png", ""));
        //MonsterCardDiamondNumLabel.text = MonsterCardData[MonsterCardBuyIndex].Value.ToString();
        UIEventListener.Get(MonsterCardBackBtn.gameObject).onClick = CloseDetail;
        ShowDetail(2);
    }

    /// <summary>  
    /// 时间戳Timestamp转换成日期  
    /// </summary>  
    /// <param name="timeStamp"></param>  
    /// <returns></returns>  
    private string GetDateTime(int timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = ((long)timeStamp * 10000000);
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime targetDt = dtStart.Add(toNow);

        string StringTime = string.Format(targetDt.ToString("yyyy-MM-dd"));
        return StringTime;
    }  
}