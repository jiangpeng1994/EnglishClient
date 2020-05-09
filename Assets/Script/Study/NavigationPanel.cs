using ProtoSprotoType;
using Sproto;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPanel : MonoBehaviour
{
    public static NavigationPanel _instance;

    public UIGrid NavigationGrid;
    public GameObject TextItem;
    private List<GameObject> TextItemList = new List<GameObject>();
    private List<UILabel> NumList = new List<UILabel>();
    private List<UILabel> ContentList = new List<UILabel>();
    private List<UITexture> IconList = new List<UITexture>();
    private List<GameObject> FinishBGList = new List<GameObject>();
    private List<GameObject> LightBGList = new List<GameObject>();

    public UIButton CloseBtn;
    public UITexture TypeIcon;
    public UILabel StudyPath;
    public UIWidget ChartAll;
    public UILabel LabelAll;
    public UIWidget ChartComplete;
    public UILabel LabelComplete;
    public UIWidget ChartPass;
    public UILabel LabelPass;
    public UILabel WaitForPassLabel;
    public UILabel TimeLabel;
    public UILabel NextBtnLabel;
    public UIButton NextBtn;

    public GameObject ReStudyPanel;
    public UIButton SureBtn;
    public UIButton NoBtn;

    private DataManager data;
    private TeachType _teachType;
    private SyncMoudle1Info.request _textData;
    private SyncMoudle2Info.request _wordData1;
    private SyncMoudle3Info.request _sentence1Data;
    private SyncMoudle4Info.request _dialogueData;
    private SyncMoudle5Info.request _wordData2;
    private SyncMoudle6Info.request _sentence2Data;
    private SyncMoudle7Info.request _wordData3;
    private SyncMoudle8Info.request _wordTestData;
    private SyncMoudle8Info.request _sentenceTestData;
    private SyncMoudle4Info.request _dialogueTestData;

    private int CompleteNum = 0;
    private int TotalNum = 0;
    private int PassNum = 0;
    private List<bool> ispass = new List<bool>();
    private long time;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void Init(TeachType teachType)
    {
        data = DataManager.GetInstance();
        _teachType = teachType;
        SetStudyPath();
        UIEventListener.Get(CloseBtn.gameObject).onClick = CloseWindow;
        ReStudyPanel.SetActive(false);

        switch (teachType)
        {
            case TeachType.Text:
                _textData = data.textData;
                time = _textData.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_kewen");
                InitDataArea(_textData.contentInfo.Count, CalculationCompleteNum(_textData.moudlePassList), CalculationPassNum(_textData.moudlePassList));
                break;
            case TeachType.Word1:
                _wordData1 = data.wordData1;
                time = _wordData1.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_danci");
                InitDataArea(_wordData1.contentInfo.Count, CalculationCompleteNum(_wordData1.moudlePassList), CalculationPassNum(_wordData1.moudlePassList));
                break;
            case TeachType.Word2:
                _wordData2 = data.wordData2;
                time = _wordData2.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_danci");
                InitDataArea(_wordData2.infoList.Count, CalculationCompleteNum(_wordData2.moudlePassList), CalculationPassNum(_wordData2.moudlePassList));
                break;
            case TeachType.Word3:
                _wordData3 = data.wordData3;
                time = _wordData3.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_danci");
                InitDataArea(_wordData3.infoList.Count, CalculationCompleteNum(_wordData3.moudlePassList), CalculationPassNum(_wordData3.moudlePassList));
                break;
            case TeachType.Sentence1:
                _sentence1Data = data.sentenceData1;
                time = _sentence1Data.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_jushi");
                InitDataArea(_sentence1Data.contentInfo.Count, CalculationCompleteNum(_sentence1Data.moudlePassList), CalculationPassNum(_sentence1Data.moudlePassList));
                break;
            case TeachType.Sentence2:
                _sentence2Data = data.sentenceData2;
                time = _sentence2Data.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_jushi");
                InitDataArea(_sentence2Data.infoList.Count, CalculationCompleteNum(_sentence2Data.moudlePassList), CalculationPassNum(_sentence2Data.moudlePassList));
                break;
            case TeachType.Dialogue:
                _dialogueData = data.dialogueData;
                time = _dialogueData.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_duihua");
                InitDataArea(_dialogueData.infoList.Count, CalculationCompleteNum(_dialogueData.moudlePassList), CalculationPassNum(_dialogueData.moudlePassList));
                break;
            case TeachType.WordTest:
                _wordTestData = data.wordTestData;
                time = _wordTestData.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_danci");
                InitDataArea(_wordTestData.contentInfo1.Count, CalculationCompleteNum(_wordTestData.moudlePassList), CalculationPassNum(_wordTestData.moudlePassList));
                break;
            case TeachType.SentenceTest:
                _sentenceTestData = data.wordTestData;
                time = _sentenceTestData.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_jushi");
                InitDataArea(_sentenceTestData.contentInfo2.Count, CalculationCompleteNum(_sentenceTestData.moudlePassList), CalculationPassNum(_sentenceTestData.moudlePassList));
                break;
            case TeachType.DialogueTest:
                _dialogueTestData = data.dialogueData;
                time = _dialogueTestData.time;
                TypeIcon.mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/title_duihua");
                InitDataArea(_dialogueTestData.infoList.Count, CalculationCompleteNum(_dialogueTestData.moudlePassList), CalculationPassNum(_dialogueTestData.moudlePassList));
                break;
        }
        GetContentData(teachType);
    }

    private void SetStudyPath()
    {
        int level = (int)System.Math.Floor(data.roleData.curMoudleId);
        string studyPath = StringConst.gradePath[data.roleData.curGrade] + "/" + StringConst.termPath[data.roleData.curTerm] + "/" + StringConst.unitPath[data.roleData.curUnit] +  "/" + StringConst.levelPath[level];
        StudyPath.text = studyPath;
    }

    private void CloseWindow(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        WindowManager.instance.Delete<NavigationUI>();
    }

    private int CalculationCompleteNum(List<moudlePassUnit> isPassList)
    {
        int CompleteNum = isPassList.Count;
        if (CompleteNum != 0)
        {
            if (isPassList[CompleteNum - 1].scoreList.Count < isPassList[CompleteNum - 1].markingTotal)
            {
                CompleteNum = CompleteNum - 1;
            }
        }

        return CompleteNum;
    }

    private bool[] isPassDataList;
    private int CalculationPassNum(List<moudlePassUnit> isPassList)
    {
        isPassDataList = new bool[isPassList.Count];
        for (int i = 0;i<isPassDataList.Length;i++)
        {
            isPassDataList[i] = false;
        }

        bool isPass = true;
        int PassNum = 0;
        for (int i = 0; i < isPassList.Count; i++)
        {
            if (isPassList[i].scoreList.Count >= isPassList[i].markingTotal)
            {
                for (int j = 0; j < isPassList[i].scoreList.Count; j++)
                {
                    if (!isPassList[i].scoreList[j])
                    {
                        isPass = false;
                        break;
                    }
                }

                if (isPass)
                {
                    if (isPassList[i].index >= isPassList.Count || isPassList[i].index < 0)
                    {
                        Debug.Log("通过的index：" + isPassList[i].index);
                    }
                    else
                    {
                        isPassDataList[isPassList[i].index] = true;
                        PassNum++;
                    }
                }

                isPass = true;
            }
        }

        return PassNum;
    }

    private int sec = 0;
    private int min = 0;
    private int hour = 0;
    /// <summary>
    /// 时间转换
    /// </summary>
    public void TimeConversion(int time)
    {
        hour = time / 3600;
        min = time / 60 - hour * 60;
        sec = time - min * 60 - hour * 3600;
    }

    private Queue<int> StudyQueue;
    private void InitDataArea(int totalNum, int completeNum, int passNum)
    {
        TotalNum = totalNum;
        CompleteNum = completeNum;
        PassNum = passNum;

        float perHeight = 140 / totalNum;

        ChartAll.height = 140;
        LabelAll.transform.localPosition = new Vector3(LabelAll.transform.localPosition.x, ChartAll.height + 20, 0);
        LabelAll.text = totalNum.ToString();

        if (completeNum == 0)
        {
            ChartComplete.transform.localScale = new Vector3(1, 0, 1);
            LabelComplete.transform.localPosition = new Vector3(LabelComplete.transform.localPosition.x, 20, 0);
        }
        else if(completeNum == totalNum)
        {
            ChartComplete.transform.localScale = new Vector3(1, 1, 1);
            ChartComplete.height = 140;
            LabelComplete.transform.localPosition = new Vector3(LabelComplete.transform.localPosition.x, ChartComplete.height + 20, 0);
        }
        else
        {
            ChartComplete.transform.localScale = new Vector3(1, 1, 1);
            ChartComplete.height = (int)(completeNum * perHeight);
            LabelComplete.transform.localPosition = new Vector3(LabelComplete.transform.localPosition.x, ChartComplete.height + 20, 0);
        }
        LabelComplete.text = completeNum.ToString();

        if (passNum == 0)
        {
            ChartPass.transform.localScale = new Vector3(1, 0, 1);
            LabelPass.transform.localPosition = new Vector3(LabelPass.transform.localPosition.x, 20, 0);
            
        }
        else if (passNum == totalNum)
        {
            ChartPass.transform.localScale = new Vector3(1, 1, 1);
            ChartPass.height = 140;
            LabelPass.transform.localPosition = new Vector3(LabelPass.transform.localPosition.x, ChartPass.height + 20, 0);
        }
        else
        {
            ChartPass.transform.localScale = new Vector3(1, 1, 1);
            ChartPass.height = (int)(passNum * perHeight);
            LabelPass.transform.localPosition = new Vector3(LabelPass.transform.localPosition.x, ChartPass.height + 20, 0);
        }
        LabelPass.text = passNum.ToString();

        int WaitForPassNum = totalNum - passNum;
        WaitForPassLabel.text = WaitForPassNum.ToString();

        switch (_teachType)
        {
            case TeachType.Text:
                TimeConversion(15* WaitForPassNum);
                break;
            case TeachType.Word1:
            case TeachType.Word2:
            case TeachType.Word3:
            case TeachType.WordTest:
                TimeConversion(40 * WaitForPassNum);
                break;
            case TeachType.Sentence1:
            case TeachType.Sentence2:
            case TeachType.SentenceTest:
                TimeConversion(60* WaitForPassNum);
                break;
            case TeachType.Dialogue:
            case TeachType.DialogueTest:
                TimeConversion(90* WaitForPassNum);
                break;
        }
        TimeLabel.text = hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00");

        StudyQueue = new Queue<int>();
        if (completeNum < totalNum)
        {
            // 完成数量 小于 总数：开始学习
            NextBtnLabel.text = "开始学习";
            UIEventListener.Get(NextBtn.gameObject).onClick = StartStudy;
            for (int i = completeNum; i<totalNum;i++)
            {
                StudyQueue.Enqueue(i);
            }
        }
        else
        {
            // 完成全部,但通过数量 小于 总数：开始纠错
            if (passNum < totalNum)
            {
                NextBtnLabel.text = "开始纠错";
                UIEventListener.Get(NextBtn.gameObject).onClick = StartStudy;
                for (int i = 0; i < totalNum; i++)
                {
                    if (!isPassDataList[i])
                    {
                        StudyQueue.Enqueue(i);
                    }
                }
            }
            else
            {
                // 通过全部：开始复习
                NextBtnLabel.text = "开始复习";
                UIEventListener.Get(NextBtn.gameObject).onClick = StartReStudy;
                for (int i = 0; i < totalNum; i++)
                {
                    StudyQueue.Enqueue(i);
                }
            }
        }
    }

    private List<string> ContentDataList = new List<string>();
    private List<bool> isFinishDataList = new List<bool>();
    private List<bool> isLightDataList = new List<bool>();
    private void GetContentData(TeachType teachType)
    {
        ContentDataList.Clear();
        isFinishDataList.Clear();
        isLightDataList.Clear();

        switch (teachType)
        {
            case TeachType.Text:
                for(int i = 0; i< _textData.contentInfo.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_textData.contentInfo[i].content);
                    } else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                    }

                    if (i == CompleteNum && CompleteNum < _textData.contentInfo.Count)
                    {
                        isLightDataList.Add(true);
                    } else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_textData.contentInfo.Count);
                break;
            case TeachType.Word1:
                for (int i = 0; i < _wordData1.contentInfo.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_wordData1.contentInfo[i].word);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //ContentDataList.Add(_wordData1.contentInfo[i].word);
                    }

                    if (i == CompleteNum && CompleteNum < _wordData1.contentInfo.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_wordData1.contentInfo.Count);
                break;
            case TeachType.Word2:
                for (int i = 0; i < _wordData2.infoList.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_wordData2.infoList[i].constantInfo.word);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //ContentDataList.Add(_wordData2.infoList[i].constantInfo.word);
                    }

                    if (i == CompleteNum && CompleteNum < _wordData2.infoList.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_wordData2.infoList.Count);
                break;
            case TeachType.Word3:
                for (int i = 0; i < _wordData3.infoList.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_wordData3.infoList[i].constantInfo.word);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //ContentDataList.Add(_wordData3.infoList[i].constantInfo.word);
                    }

                    if (i == CompleteNum && CompleteNum < _wordData3.infoList.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_wordData3.infoList.Count);
                break;
            case TeachType.Sentence1:
                for (int i = 0; i < _sentence1Data.contentInfo.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_sentence1Data.contentInfo[i].right);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //ContentDataList.Add(_sentence1Data.contentInfo[i].right);
                    }

                    if (i == CompleteNum && CompleteNum < _sentence1Data.contentInfo.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_sentence1Data.contentInfo.Count);
                break;
            case TeachType.Sentence2:
                for (int i = 0; i < _sentence2Data.infoList.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        string[] headlines = _sentence2Data.infoList[i].title.Split('句');
                        ContentDataList.Add(headlines[0] + System.Environment.NewLine + "句" + headlines[1]);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //string[] headlines = _sentence2Data.infoList[i].title.Split('句');
                        //ContentDataList.Add(headlines[0] + System.Environment.NewLine + "句" + headlines[1]);
                    }

                    if (i == CompleteNum && CompleteNum < _sentence2Data.infoList.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_sentence2Data.infoList.Count);
                break;
            case TeachType.Dialogue:
                for (int i = 0; i < _dialogueData.infoList.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        string[] headlines = _dialogueData.infoList[i].headline.Split('话');
                        ContentDataList.Add(headlines[0] + System.Environment.NewLine + "话" + headlines[1]);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //string[] headlines = _dialogueData.infoList[i].headline.Split('话');
                        //ContentDataList.Add(headlines[0] + System.Environment.NewLine + "话" + headlines[1]);
                    }

                    if (i == CompleteNum && CompleteNum < _dialogueData.infoList.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_dialogueData.infoList.Count);
                break;
            case TeachType.WordTest:
                for (int i = 0; i < _wordTestData.contentInfo1.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_wordTestData.contentInfo1[i].word);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //ContentDataList.Add(_wordData3.infoList[i].constantInfo.word);
                    }

                    if (i == CompleteNum && CompleteNum < _wordTestData.contentInfo1.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_wordTestData.contentInfo1.Count);
                break;
            case TeachType.SentenceTest:
                for (int i = 0; i < _sentenceTestData.contentInfo2.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        ContentDataList.Add(_sentenceTestData.contentInfo2[i].text);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //ContentDataList.Add(_wordData3.infoList[i].constantInfo.word);
                    }

                    if (i == CompleteNum && CompleteNum < _sentenceTestData.contentInfo2.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_sentenceTestData.contentInfo2.Count);
                break;
            case TeachType.DialogueTest:
                for (int i = 0; i < _dialogueTestData.infoList.Count; i++)
                {
                    if (i < CompleteNum)
                    {
                        isFinishDataList.Add(true);
                        string[] headlines = _dialogueTestData.infoList[i].headline.Split('话');
                        ContentDataList.Add(headlines[0] + System.Environment.NewLine + "话" + headlines[1]);
                    }
                    else
                    {
                        isFinishDataList.Add(false);
                        ContentDataList.Add("?");
                        //string[] headlines = _dialogueData.infoList[i].headline.Split('话');
                        //ContentDataList.Add(headlines[0] + System.Environment.NewLine + "话" + headlines[1]);
                    }

                    if (i == CompleteNum && CompleteNum < _dialogueTestData.infoList.Count)
                    {
                        isLightDataList.Add(true);
                    }
                    else
                    {
                        isLightDataList.Add(false);
                    }
                }
                InitContentArea(_dialogueTestData.infoList.Count);
                break;
        }
    }

    private void InitContentArea(int count)
    {
        int partNum = count;

        if (partNum < TextItemList.Count)
        {
            for (int i = partNum; i < TextItemList.Count; i++)
            {
                TextItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= TextItemList.Count)
            {
                GameObject item = Instantiate(TextItem);
                item.name = i.ToString();
                item.transform.parent = NavigationGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                TextItemList.Add(item);

                NumList.Add(TextItemList[i].transform.Find("TextBG/Num").GetComponent<UILabel>());
                ContentList.Add(TextItemList[i].transform.Find("TextBG/Content").GetComponent<UILabel>());
                IconList.Add(TextItemList[i].transform.Find("TextBG/Icon").GetComponent<UITexture>());
                FinishBGList.Add(TextItemList[i].transform.Find("TextBG/FinishBG").gameObject);
                LightBGList.Add(TextItemList[i].transform.Find("TextBG/LightBG").gameObject);
            }

            NumList[i].text = (i + 1).ToString("00");
            ContentList[i].text = ContentDataList[i];

            if (i < isPassDataList.Length)
            {
                if (isPassDataList[i])
                {
                    IconList[i].mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/fuhao2");
                }
                else
                {
                    IconList[i].mainTexture = Resources.Load<Texture>("Texture/NavigationUI" + "/fuhao1");
                }
                IconList[i].gameObject.SetActive(true);
            } else
            {
                IconList[i].gameObject.SetActive(false);
            }

            FinishBGList[i].SetActive(isFinishDataList[i]);
            LightBGList[i].SetActive(isLightDataList[i]);

            TextItemList[i].SetActive(true);
        }
        NavigationGrid.enabled = true;
    }

    /// <summary>
    /// 开始学习
    /// </summary>
    /// <param name="go"></param>
    private void StartStudy(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DataManager.GetInstance().curStudyProgress.teachType = _teachType;
        DataManager.GetInstance().curStudyProgress.totalNum = TotalNum;
        DataManager.GetInstance().curStudyProgress.completeNum = CompleteNum;
        DataManager.GetInstance().curStudyProgress.passNum = PassNum;

        WindowManager.instance.Open<UIWordGamePanel>().Init(_teachType, StudyQueue,(int)time);
        WindowManager.instance.Delete<NavigationUI>();
    }

    /// <summary>
    /// 是否确认重新学习
    /// </summary>
    /// <param name="go"></param>
    private void StartReStudy(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        UIEventListener.Get(SureBtn.gameObject).onClick = SureStartReStudy;
        UIEventListener.Get(NoBtn.gameObject).onClick = CloseReStudyPanel;
        ReStudyPanel.SetActive(true);
    }

    /// <summary>
    /// 重置学习数据，开始复习
    /// </summary>
    /// <param name="go"></param>
    private void SureStartReStudy(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        ReStudyPanel.SetActive(false);

        Debug.Log("请求：重置学习数据，开始复习");
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = DataManager.GetInstance().roleData.curMoudleId
        };
        ClearMoudleInfo.request msg = new ClearMoudleInfo.request
        {
            moudleBase = moudle,
        };
        NetSender.Send<ProtoProtocol.ClearMoudleInfo>(msg, OnClearMoudleInfo);
    }

    /// <summary>
    /// 收到重置学习数据结果
    /// </summary>
    /// <param name="msg">重置学习数据结果</param>
    private void OnClearMoudleInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：重置学习数据结果");
        var data = (ClearMoudleInfo.response)rpcRsp;
        if (data.status)
        {
            Debug.Log("重置学习数据成功");
            DataManager.GetInstance().curStudyProgress.teachType = _teachType;
            DataManager.GetInstance().curStudyProgress.totalNum = TotalNum;
            DataManager.GetInstance().curStudyProgress.completeNum = 0;
            DataManager.GetInstance().curStudyProgress.passNum = 0;

            WindowManager.instance.Open<UIWordGamePanel>().Init(_teachType, StudyQueue, 0);
            WindowManager.instance.Delete<NavigationUI>();
        } else
        {
            GameTools.Instance.TipsShow("开始复习失败，请重试");
        }
    }

    /// <summary>
    /// 取消重新学习
    /// </summary>
    /// <param name="go"></param>
    private void CloseReStudyPanel(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        ReStudyPanel.SetActive(false);
    }
}