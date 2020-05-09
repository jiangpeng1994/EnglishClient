using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour {

    public GameObject WinSprite;
    public GameObject LostSprite;
    public GameObject Star;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    public UILabel TeacherSpeake;
    public UILabel StudentSpeake;
    public UILabel Title;
    public UILabel ValueTitle;
    public UILabel ValueName;
    public UILabel TrueCount;
    public UILabel FalseCount;
    public UILabel Accuracy;
    public UILabel Time;
    public UILabel Veracity;
    public UILabel Full;
    public UILabel Fluency;
    private int _star;
    public GameObject getCard;
    public Image card;
    public UIButton GetCardBtn;
    public UILabel LevelPassLabel;
    public UIButton NextBtn;
    public UIButton BackBtn;
    public UILabel GetCardLabel;
    public UILabel NextUILabel;
    public UILabel BackUILabel;
    public UIButton CloseBtn;
    public Animation Animation;
    public AnimationClip animationClip;
    public UITexture Sex;
    private string[] Sexes = {"boy","girl"};

    private string[] TeacherSpeakeContent = new string[4] 
    { "嘿，小伙伴儿，竖起耳朵，大声表达，调整心情！这可不是你应有的样子！",
      "#看来你需要更努力才能做得更好，加油！",
      "嗯……不错，注意下细节，#，你是最棒的！",
      "太棒了，#，你是全宇宙最棒的学员！" };
    private string[] StudentSpeakeContent = new string[4]
    { "好！让你见识我真正的实力！来吧！",
      "好！请相信我的实力！",
      "好的，我会全力以赴的",
      "Yeah！谢谢！" };
    private Color TextColor = new Color(217/255f, 81/255f, 63/255f);      //课文结算字体颜色
    private Color WordColor = new Color(232/255f, 125/255f, 5/255f);   //单词结算字体颜色
    private Color SentenceColor = new Color(0 ,182/255f, 124/255f);      //句式结算字体颜色
    private Color DialogueColor = new Color(0 , 147/255f, 208/255f);     //对话结算字体颜色

    public GameObject LevelPassUI;
    public GameObject LevelPassFailUI;
    public UIButton LevelPassUICloseBtn;
    public UIButton LevelPassFailUICloseBtn;
    public GameObject[] TextStar;
    public GameObject[] WordStar;
    public GameObject[] SentenceStar;
    public GameObject[] DialogueStar;
    public GameObject UnitPassUI;
    public GameObject UnitPassFailUI;
    public UIButton UnitPassUICloseBtn;
    public UIButton UnitPassFailUICloseBtn;
    public string CardName = "";
    public int LevelPassType = -1;
    public int ButtonType = -1;

    void Start()
    {
        UIEventListener.Get(NextBtn.gameObject).onClick = OnClickStartStudy;
        UIEventListener.Get(BackBtn.gameObject).onClick = OnBackBtn;
        UIEventListener.Get(LevelPassUICloseBtn.gameObject).onClick = OnShowLevelPassUIClose;
        UIEventListener.Get(LevelPassFailUICloseBtn.gameObject).onClick = OnShowPassFailUIClose;
        UIEventListener.Get(UnitPassUICloseBtn.gameObject).onClick = OnShowUnitPassUIClose;
        UIEventListener.Get(UnitPassFailUICloseBtn.gameObject).onClick = OnShowPassFailUIClose;
        CloseBtn.gameObject.SetActive(false);
        getCard.SetActive(false);
        LevelPassUI.SetActive(false);
        LevelPassFailUI.SetActive(false);
        UnitPassUI.SetActive(false);
        UnitPassFailUI.SetActive(false);
    }

    /// <summary>
    /// 显示学习报告
    /// </summary>
    /// <param name="type">结算类型 1单词  2句式 3对话 4课文</param>
    /// <param name="star">星级</param>
    /// <param name="trueCount">正确数</param>
    /// <param name="falseCount">错误数</param>
    /// <param name="accuracy">准确率</param>
    /// <param name="useTime">用时</param>
    /// <param name="veracity">准确度</param>
    /// <param name="full">完整度</param>
    /// <param name="fluency">流畅度</param>
    public void ShowResult(TeachType teachType, int star, string trueCount, string falseCount, string accuracy, string useTime, string veracity, string full, string fluency, string cardName)
    {
        _star = star;
        CardName = cardName;
        Sex.mainTexture = Resources.Load<Texture>("Texture/Sex/" + Sexes[DataManager.GetInstance().roleData.Sex]);
        switch (teachType)
        {
            case TeachType.Text:
                Title.text = "课文学习报告";
                Title.effectColor = TextColor;
                ValueTitle.effectColor = TextColor;
                ValueName.color = TextColor;
                TrueCount.color = TextColor;
                FalseCount.color = TextColor;
                Accuracy.color = TextColor;
                Time.color = TextColor;
                Veracity.color = TextColor;
                Full.color = TextColor;
                Fluency.color = TextColor;
                break;
            case TeachType.Word1:
            case TeachType.Word2:
            case TeachType.Word3:
                Title.text = "单词学习报告";
                Title.effectColor = WordColor;
                ValueTitle.effectColor = WordColor;
                ValueName.color = WordColor;
                TrueCount.color = WordColor;
                FalseCount.color = WordColor;
                Accuracy.color = WordColor;
                Time.color = WordColor;
                Veracity.color = WordColor;
                Full.color = WordColor;
                Fluency.color = WordColor;
                break;
            case TeachType.Sentence1:
            case TeachType.Sentence2:
                Title.text = "句式学习报告";
                Title.effectColor = SentenceColor;
                ValueTitle.effectColor = SentenceColor;
                ValueName.color = SentenceColor;
                TrueCount.color = SentenceColor;
                FalseCount.color = SentenceColor;
                Accuracy.color = SentenceColor;
                Time.color = SentenceColor;
                Veracity.color = SentenceColor;
                Full.color = SentenceColor;
                Fluency.color = SentenceColor;
                break;
            case TeachType.Dialogue:
                Title.text = "对话学习报告";
                Title.effectColor = DialogueColor;
                ValueTitle.effectColor = DialogueColor;
                ValueName.color = DialogueColor;
                TrueCount.color = DialogueColor;
                FalseCount.color = DialogueColor;
                Accuracy.color = DialogueColor;
                Time.color = DialogueColor;
                Veracity.color = DialogueColor;
                Full.color = DialogueColor;
                Fluency.color = DialogueColor;
                break;
            default:
                break;
        }

        TrueCount.text = trueCount;
        FalseCount.text = falseCount;
        Accuracy.text = accuracy;
        Veracity.text = veracity;
        Full.text = full;
        Fluency.text = fluency;
        Time.text = useTime;

        TeacherSpeake.text = TeacherSpeakeContent[star].Replace("#", DataManager.GetInstance().roleData.NickName);
        StudentSpeake.text = StudentSpeakeContent[star];
        switch (star)
        {
            case 0:
                AudicoManager.instance.Play("effect", "Effect/failed report");
                Star.SetActive(false);
                WinSprite.SetActive(false);
                LostSprite.SetActive(true);
                break;
            case 1:
                AudicoManager.instance.Play("effect", "Effect/get pass");
                Star1.SetActive(true);
                Star2.SetActive(false);
                Star3.SetActive(false);
                Star.SetActive(true);
                WinSprite.SetActive(true);
                LostSprite.SetActive(false);
                break;
            case 2:
                AudicoManager.instance.Play("effect", "Effect/get pass");
                Star1.SetActive(true);
                Star2.SetActive(true);
                Star3.SetActive(false);
                Star.SetActive(true);
                WinSprite.SetActive(true);
                LostSprite.SetActive(false);
                break;
            case 3:
                AudicoManager.instance.Play("effect", "Effect/get pass");
                Star1.SetActive(true);
                Star2.SetActive(true);
                Star3.SetActive(true);
                Star.SetActive(true);
                WinSprite.SetActive(true);
                LostSprite.SetActive(false);
                break;
        }

        if (star == 3)
        {
            if (CardName == "")
            {
                // 无卡片
                LevelPassLabel.text = "下一步";
                UIEventListener.Get(GetCardBtn.gameObject).onClick = OnMoudleCloseBtn;
            }
            else
            {
                // 有卡片
                LevelPassLabel.text = "领取奖励";
                UIEventListener.Get(GetCardBtn.gameObject).onClick = OnGetCardBtn;
            }
            // 通过的下一步按钮
            ButtonType = 1;
            Invoke("ShowBtn1", 0.5f);
        }
        else
        {
            // 没通过的下一步按钮
            ButtonType = 2;
            Invoke("ShowBtn2", 0.5f);
        }

        if (star > 0)
        {
            RefreshModuleSelectUI(star);
        }
    }

    private void ShowBtn2()
    {
        GetCardBtn.gameObject.SetActive(false);
        NextBtn.gameObject.SetActive(true);
        BackBtn.gameObject.SetActive(true);
        LevelPassType = -2;
    }

    private void ShowBtn1()
    {
        GetCardBtn.gameObject.SetActive(true);
        NextBtn.gameObject.SetActive(false);
        BackBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 更改当前模块通过状态
    /// </summary>
    /// <param name="star"></param>
    private void RefreshModuleSelectUI(int star)
    {
        bool isFullStar = DataManager.GetInstance().isFullStar;
        int curMoudle = int.Parse(DataManager.GetInstance().roleData.curMoudleId.ToString().Substring(2, 1));
        int curStar = (int)DataManager.GetInstance().modulePassList[curMoudle].star;
        DataManager.GetInstance().modulePassList[curMoudle].isPass = true;
        if (star > curStar)
        {
            DataManager.GetInstance().modulePassList[curMoudle].star = star;
        }
        RefreshModuleStar();

        if (!isFullStar)
        {
            // 显示通过当前难度
            if (DataManager.GetInstance().isFullStar)
            {
                if (DataManager.GetInstance().roleData.curLevel == "4")
                {
                    LevelPassType = 2; //显示通过单元
                } else
                {
                    LevelPassType = 1; //显示通过难度
                }
            }
            else
            {
                string moudle = DataManager.GetInstance().roleData.curMoudleId.ToString().Substring(2, 1);
                if (DataManager.GetInstance().roleData.curLevel == "4")
                {
                    if (moudle.Equals("3"))
                    {
                        LevelPassType = 3; //显示未通过单元
                    }
                    else
                    {
                        LevelPassType = -2; //无显示，刷新模块
                    }
                }
                else
                {
                    if (moudle.Equals("5"))
                    {
                        LevelPassType = 0; //显示未通过难度
                    }
                    else
                    {
                        LevelPassType = -2; //无显示，刷新模块
                    }
                }
            }
        }
    }

    /// <summary>
    /// 刷新模块的星级
    /// </summary>
    private void RefreshModuleStar()
    {
        Dictionary<int, passUnit> _modulePassList = DataManager.GetInstance().modulePassList;
        int curLevel = int.Parse(DataManager.GetInstance().roleData.curLevel);

        int fullStar = 0;
        if (curLevel == 4)
        {
            fullStar = 9;
        }
        else
        {
            fullStar = 15;
        }

        int starNum = 0;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            starNum = starNum + (int)_modulePassList[i].star;
        }
        if (starNum >= fullStar)
        {
            DataManager.GetInstance().isFullStar = true;
        }
        else
        {
            DataManager.GetInstance().isFullStar = false;
        }
    }

    /// <summary>
    /// 点击获取卡牌
    /// </summary>
    /// <param name="go"></param>
    private void OnGetCardBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        //CardName = "fern-jjs";
        card.sprite = Resources.Load<Sprite>("Texture/MonsterCard/" + CardName);
        getCard.SetActive(true);
        CloseBtn.gameObject.SetActive(true);
        Invoke("PlayGetCardEffect", 2.2f);
        Invoke("SetCanClose", 3.8f);
    }

    /// <summary>
    /// 获得卡片音效
    /// </summary>
    private void PlayGetCardEffect()
    {
        AudicoManager.instance.Play("effect", "Effect/get card");
    }

    /// <summary>
    /// 设置可以关闭结算面板
    /// </summary>
    /// <param name="go"></param>
    private void SetCanClose()
    {
        isLoop = true;
        UIEventListener.Get(CloseBtn.gameObject).onClick = OnMoudleCloseBtn;
    }

    /// <summary>
    /// 点击继续学习
    /// </summary>
    private void OnClickStartStudy(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：模块" + DataManager.GetInstance().roleData.curMoudleId + "的学习数据");

        GetMoudleInfo.request msg = new GetMoudleInfo.request();
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = DataManager.GetInstance().roleData.curMoudleId
        };

        msg.moudleBase = moudle;
        NetSender.Send<ProtoProtocol.GetMoudleInfo>(msg, null);

        OnMoudleCloseBtn(gameObject);
    }

    /// <summary>
    /// 点击返回
    /// </summary>
    /// <param name="go"></param>
    private void OnBackBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        OnMoudleCloseBtn(gameObject);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    private void OnMoudleCloseBtn(GameObject go)
    {
        isLoop = false;
        if (LevelPassType == 0)
        {
            ShowLevelPassFailUI();
        } else if (LevelPassType == 1)
        {
            ShowLevelPassUI();
        } else if (LevelPassType == 2)
        {
            ShowUnitPassUI();
        } else if (LevelPassType == 3)
        {
            ShowUnitPassFailUI();
        }
        else if (LevelPassType == -2)
        {
            RefreshMoudleUI();
        }
        else
        {
            WindowManager.instance.Delete<UIWordGamePanel>();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 显示难度未通过界面
    /// </summary>
    private void ShowLevelPassFailUI()
    {
        AudicoManager.instance.Play("effect", "Effect/failed report");
        CloseBtn.gameObject.SetActive(false);
        getCard.SetActive(false);
        SetModuleStar(TextStar, (int)DataManager.GetInstance().modulePassList[2].star);
        SetModuleStar(WordStar, (int)DataManager.GetInstance().modulePassList[3].star);
        SetModuleStar(SentenceStar, (int)DataManager.GetInstance().modulePassList[4].star);
        SetModuleStar(DialogueStar, (int)DataManager.GetInstance().modulePassList[5].star);
        LevelPassFailUI.SetActive(true);
    }

    /// <summary>
    /// 显示单元未通过界面
    /// </summary>
    private void ShowUnitPassFailUI()
    {
        AudicoManager.instance.Play("effect", "Effect/failed report");
        CloseBtn.gameObject.SetActive(false);
        getCard.SetActive(false);
        UnitPassFailUI.SetActive(true);
    }

    /// <summary>
    /// 设置模块的星级
    /// </summary>
    /// <param name="starNum">星级</param>
    public void SetModuleStar(GameObject[] starLight, int starNum)
    {
        for (int i = 0; i < starLight.Length; i++)
        {
            if (i < starNum)
            {
                starLight[i].SetActive(true);
            }
            else
            {
                starLight[i].SetActive(false);
            }
        }
    }

    // --------------------------------    解锁下一个难度    -------------------------------- //
    /// <summary>
    /// 显示当前难度通过界面
    /// </summary>
    private void ShowLevelPassUI()
    {
        AudicoManager.instance.Play("effect", "Effect/get pass");
        CloseBtn.gameObject.SetActive(false);
        getCard.SetActive(false);
        LevelPassUI.SetActive(true);
    }

    /// <summary>
    /// 进入难度选择界面，关闭结算界面
    /// </summary>
    private void OnShowLevelPassUIClose(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        RefreshLevelSelectUI();
    }

    /// <summary>
    /// 该难度已经通过，解锁下一个难度
    /// </summary>
    private void RefreshLevelSelectUI()
    {
        int curUnit = (int)DataManager.GetInstance().roleData.curUnit;
        Debug.Log("请求：单元" + curUnit + "的难度数据");

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有难度");
            SelectCoursePanel.instance.OpenLevelSelectUI(curUnit, 5);
            WindowManager.instance.Delete<UIWordGamePanel>();
            Destroy(gameObject);
        }
        else
        {
            GetUnitPassInfo.request msg = new GetUnitPassInfo.request();
            moudle_base moudle = new moudle_base
            {
                grade = DataManager.GetInstance().roleData.curGrade,
                term = DataManager.GetInstance().roleData.curTerm,
                unit = curUnit,
            };
            msg.moudleBase = moudle;
            NetSender.Send<ProtoProtocol.GetUnitPassInfo>(msg, LevelInfoResponseHandler);
        }
    }

    /// <summary>
    /// 收到难度信息
    /// </summary>
    /// <param name="msg"></param>
    private void LevelInfoResponseHandler(SprotoTypeBase msg)
    {
        int curUnit = (int)DataManager.GetInstance().roleData.curUnit;
        Debug.Log("收到：单元" + curUnit + "的难度数据");

        var data = (GetUnitPassInfo.response)msg;

        Dictionary<int, unitPass> _levelPassList = new Dictionary<int, unitPass>();
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

        SelectCoursePanel.instance.OpenLevelSelectUI(curUnit, curIndex);
        WindowManager.instance.Delete<UIWordGamePanel>();
        Destroy(gameObject);
    }

    // --------------------------------    解锁下一个单元    -------------------------------- //
    /// <summary>
    /// 显示单元通过界面
    /// </summary>
    private void ShowUnitPassUI()
    {
        AudicoManager.instance.Play("effect", "Effect/get pass");
        CloseBtn.gameObject.SetActive(false);
        getCard.SetActive(false);
        UnitPassUI.SetActive(true);
    }

    /// <summary>
    /// 进入单元选择界面，关闭结算界面
    /// </summary>
    private void OnShowUnitPassUIClose(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        RefreshUnitSelectUI();
    }

    /// <summary>
    /// 该单元已经通过，解锁下一个单元
    /// </summary>
    private void RefreshUnitSelectUI()
    {
        int curGrade = (int)DataManager.GetInstance().roleData.curGrade;
        int curTerm = (int)DataManager.GetInstance().roleData.curTerm;
        Debug.Log("请求：" + curGrade + "年级" + curTerm + "学期" + "的单元数据");

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有单元");
            SelectCoursePanel.instance.OpenUnitSelectUI(curGrade, curTerm, 10);
            WindowManager.instance.Delete<UIWordGamePanel>();
            Destroy(gameObject);
        }
        else
        {
            GetTermPassInfo.request msg = new GetTermPassInfo.request();
            moudle_base moudle = new moudle_base
            {
                grade = curGrade,
                term = curTerm,
            };
            msg.moudleBase = moudle;
            NetSender.Send<ProtoProtocol.GetTermPassInfo>(msg, UnitInfoResponseHandler);
        }
    }

    /// <summary>
    /// 收到单元信息
    /// </summary>
    /// <param name="msg"></param>
    private void UnitInfoResponseHandler(SprotoTypeBase msg)
    {
        int curGrade = (int)DataManager.GetInstance().roleData.curGrade;
        int curTerm = (int)DataManager.GetInstance().roleData.curTerm;
        Debug.Log("收到：" + curGrade + "年级" + curTerm + "学期" + "的单元数据");

        var data = (GetTermPassInfo.response)msg;
        DataManager.GetInstance().roleData.totalUnitNum = data.unitPassList.Count;
        Debug.Log("单元数量" + DataManager.GetInstance().roleData.totalUnitNum);

        Dictionary<int, unitPass> _unitPassList = new Dictionary<int, unitPass>();
        foreach (unitPass item in data.unitPassList)
        {
            int index = (int)item.index;
            _unitPassList.Add(index, item);
        }

        int curIndex = _unitPassList.Count + 1; ;
        for (int i = 1; i <= _unitPassList.Count; i++)
        {
            Debug.Log("单元" + _unitPassList[i].index + ":" + _unitPassList[i].isPass.ToString());
            if (_unitPassList[i].isPass == false)
            {
                curIndex = i;
                break;
            }
        }

        DataManager.GetInstance().unitPassList = _unitPassList;

        SelectCoursePanel.instance.OpenUnitSelectUI(curGrade, curTerm, curIndex);
        WindowManager.instance.Delete<UIWordGamePanel>();
        Destroy(gameObject);
    }

    // --------------------------------    解锁下一个模块    -------------------------------- //
    /// <summary>
    /// 进入模块选择界面，关闭结算界面
    /// </summary>
    private void OnShowPassFailUIClose(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        RefreshMoudleUI();
    }

    /// <summary>
    /// 刷新模块选择状态
    /// </summary>
    /// <param name="btn"></param>
    public void RefreshMoudleUI()
    {
        int curLevel = int.Parse(DataManager.GetInstance().roleData.curLevel);
        Debug.Log("请求：难度" + curLevel + "的模块数据");

        GetLevelPassInfo.request msg = new GetLevelPassInfo.request();
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = curLevel,
        };
        msg.moudleBase = moudle;
        NetSender.Send<ProtoProtocol.GetLevelPassInfo>(msg, MoudleInfoInfoResponseHandler);
    }

    /// <summary>
    ///  收到模块信息
    /// </summary>
    /// <param name="msg"></param>
    public void MoudleInfoInfoResponseHandler(SprotoTypeBase msg)
    {
        int curLevel = int.Parse(DataManager.GetInstance().roleData.curLevel);
        bool isLastUnit = DataManager.GetInstance().isLastUnit;
        Debug.Log("收到：难度" + curLevel + "的模块数据");

        var data = (GetLevelPassInfo.response)msg;

        Dictionary<int, passUnit> _modulePassList = new Dictionary<int, passUnit>();
        foreach (passUnit item in data.passList)
        {
            int index = int.Parse(item.moudleId.ToString().Substring(2, 1));

            // 最后1个单元没有句式模块，默认3星通过
            if (isLastUnit && curLevel != 4 && index == 4)
            {
                item.isPass = true;
            }

            if (isLastUnit && curLevel == 4 && index == 2)
            {
                item.isPass = true;
            }

            // isPass为true就设置为3星
            if (item.isPass)
            {
                item.star = 3;
            }
            _modulePassList.Add(index, item);
        }

        int curIndex = _modulePassList.Count + 1;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            Debug.Log("模块" + i + ":" + "是否通过" + _modulePassList[i].isPass.ToString());
            Debug.Log("模块" + i + ":" + "星星" + _modulePassList[i].star.ToString());
            if (_modulePassList[i].isPass == false && _modulePassList[i].star <= 0)
            {
                curIndex = i;
                break;
            }
        }

        int fullStar = 0;
        if (curLevel == 4)
        {
            fullStar = 9;
        }
        else
        {
            fullStar = 15;
        }

        int starNum = 0;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            starNum = starNum + (int)_modulePassList[i].star;
        }
        if (starNum >= fullStar)
        {
            DataManager.GetInstance().isFullStar = true;
        }
        else
        {
            DataManager.GetInstance().isFullStar = false;
        }

        DataManager.GetInstance().modulePassList = _modulePassList;
        DataManager.GetInstance().isLastUnit = isLastUnit;

        // 1-3模块：1-6   4模块：3-6
        if (curLevel == 4)
        {
            curIndex = curIndex + 2;
        }

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有模块");
            SelectCoursePanel.instance.OpenModuleSelectUI(curLevel, isLastUnit, 6, _modulePassList);
        }
        else
        {
            SelectCoursePanel.instance.OpenModuleSelectUI(curLevel, isLastUnit, curIndex, _modulePassList);
        }
        WindowManager.instance.Delete<UIWordGamePanel>();
        Destroy(gameObject);
    }

    private bool isLoop = false;
    private void Update()
    {
        if (isLoop)
        {
            Animation.Play("getwait");
        }
        /*if (ButtonType == 1)
        {
            GetCardBtn.gameObject.SetActive(true);
            GetCardLabel.gameObject.SetActive(true);
            NextBtn.gameObject.SetActive(false);
            BackBtn.gameObject.SetActive(false);
        }
        else if (ButtonType == 2)
        {
            GetCardBtn.gameObject.SetActive(false);
            NextBtn.gameObject.SetActive(true);
            BackBtn.gameObject.SetActive(true);
            NextUILabel.gameObject.SetActive(true);
            BackUILabel.gameObject.SetActive(true);
        }*/
    }
}