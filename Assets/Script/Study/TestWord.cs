using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestWord : MonoBehaviour
{
    public GameObject WordCard;
    public UIGrid WordGrid;
    public GameObject WordCardItem;
    private List<UIButton> WordCardItemList = new List<UIButton>();
    private List<UITexture> WordIcon = new List<UITexture>();
    private List<UILabel> WordTranslationLabel = new List<UILabel>();
    private List<UIInput> WordInput = new List<UIInput>();
    private List<UILabel> NumLabel = new List<UILabel>();
    private List<GameObject> BGLight = new List<GameObject>();
    private List<UIButton> ShowTipsListBtn = new List<UIButton>();

    public GameObject Listen;
    public UIButton ListenBtn;
    public UIButton MySpeakBtn;

    private UIWordGame _gameUIInstance;
    public SyncMoudle8Info.request _wordTestData;
    public int _wordNum = 0;
    public int _curWordIndex = 0;
    public int _testType = 1;
    public string _assetsPath;
    private int _passNum = 0;
    private Queue<int> _studyQueue;

    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _wordTestData = DataManager.GetInstance().wordTestData;
        _wordNum = _wordTestData.contentInfo1.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _testType = RandomStep() + 1;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
        //_gameUIInstance.HideStpe(); 
        //_gameUIInstance.HideCard();
        _gameUIInstance.SetStepNum(1);
        _gameUIInstance.SetCurStep(1);
        _gameUIInstance.SetProgress(_passNum, _wordNum);
    }

    /// <summary>
    /// 设置学习队列
    /// </summary>
    /// <param name="StudyQueue"></param>
    public void SetStudyQueue(Queue<int> StudyQueue)
    {
        _studyQueue = StudyQueue;
        _curWordIndex = _studyQueue.Dequeue();
    }

    /// <summary>
    /// 一个单词的开始
    /// </summary>
    public void ShowBeginTitle()
    {
        _gameUIInstance.SetProgress(_passNum, _wordNum);
        _gameUIInstance.TeacherCommand(_wordTestData.cStatements[_testType - 1], "");
        //_gameUIInstance.ShowBeginTitle(_wordTestData.statements[_step - 1]);
        //Invoke("StartTest", 2.5f);
        StartTest();
    }

    /// <summary>
    /// 开始单词测试
    /// </summary>
    public void StartTest()
    {
        ContentInit();
        Test();
    }

    private int RightIndex = 0;
    private int ClickCardID = -1;
    private int partNum;
    /// <summary>
    /// 内容初始化
    /// </summary>
    private void ContentInit()
    {
        List<table10Info> randomList = _wordTestData.randomWordList;
        partNum = 0;

        if (_testType == 1)
        {
            partNum = 1;
        }
        else if (_testType == 2)
        {
            partNum = 1;
        }
        else if (_testType == 3)
        {
            partNum = 4;
            RightIndex = Random.Range(0,3);
        }

        if (partNum < WordCardItemList.Count)
        {
            for (int i = partNum; i < WordCardItemList.Count; i++)
            {
                WordCardItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= WordCardItemList.Count)
            {
                GameObject item = Instantiate(WordCardItem);
                item.name = i.ToString();
                item.transform.parent = WordGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                WordCardItemList.Add(item.GetComponent<UIButton>());

                WordIcon.Add(WordCardItemList[i].transform.Find("WordIconBg/WordIcon").GetComponent<UITexture>());
                WordTranslationLabel.Add(WordCardItemList[i].transform.Find("BG/WordTranslation").GetComponent<UILabel>());
                WordInput.Add(WordCardItemList[i].transform.Find("WordInput").GetComponent<UIInput>());
                NumLabel.Add(WordCardItemList[i].transform.Find("NumLabel").GetComponent<UILabel>());
                BGLight.Add(WordCardItemList[i].transform.Find("BGLight").gameObject);
                ShowTipsListBtn.Add(WordCardItemList[i].transform.Find("ShowTipBtn").GetComponent<UIButton>());
            }

            BGLight[i].SetActive(false);
            ShowTipsListBtn[i].name = i.ToString();
            UIEventListener.Get(ShowTipsListBtn[i].gameObject).onClick = OnPlayShowTipsListBtn;
            UIEventListener.Get(MySpeakBtn.gameObject).onClick = PlayRecordBtn;

            if (_testType == 1)
            {
                ResourceLoader.Instance.GetTextureResources(WordIcon[i], "Texture/" + StringConst.WordPath + _assetsPath + _wordTestData.contentInfo1[_curWordIndex].icon1);
                WordTranslationLabel[i].text = _wordTestData.contentInfo1[_curWordIndex].cword;
                WordInput[i].gameObject.SetActive(false);
                NumLabel[i].gameObject.SetActive(false);
                WordCardItemList[i].enabled = false;
                Listen.SetActive(false);
                ShowTipsListBtn[i].gameObject.SetActive(true);
            }
            else if (_testType == 2)
            {
                ResourceLoader.Instance.GetTextureResources(WordIcon[i], "Texture/" + StringConst.WordPath + _assetsPath + _wordTestData.contentInfo1[_curWordIndex].icon1);
                WordTranslationLabel[i].text = _wordTestData.contentInfo1[_curWordIndex].cword;
                WordInput[i].value = "";
                WordInput[i].gameObject.SetActive(true);
                NumLabel[i].gameObject.SetActive(false);
                WordCardItemList[i].enabled = false;
                Listen.SetActive(false);
                ShowTipsListBtn[i].gameObject.SetActive(true);
            }
            else if (_testType == 3)
            {
                ClickCardID = -1;
                if (RightIndex == i)
                {
                    ResourceLoader.Instance.GetTextureResources(WordIcon[i], "Texture/" + StringConst.WordPath + _assetsPath + _wordTestData.contentInfo1[_curWordIndex].icon1);
                    WordTranslationLabel[i].text = _wordTestData.contentInfo1[_curWordIndex].cword;
                } else
                {
                    // 随机图片
                    int index = Random.Range(0, _wordTestData.randomWordList.Count);
                    if(randomList[index].word == _wordTestData.contentInfo1[_curWordIndex].word)
                    {
                        randomList.RemoveAt(index);
                        index = Random.Range(0, _wordTestData.randomWordList.Count);
                    }
                    ResourceLoader.Instance.GetTextureResources(WordIcon[i], "Texture/" + StringConst.WordPath + _assetsPath + randomList[index].icon1);
                    WordTranslationLabel[i].text = randomList[index].cword;
                    randomList.RemoveAt(index);
                }
                
                WordInput[i].gameObject.SetActive(false);
                NumLabel[i].text = (i + 1).ToString();
                NumLabel[i].gameObject.SetActive(true);
                UIEventListener.Get(ListenBtn.gameObject).onClick = OnPlayExampleBtn;
                WordCardItemList[i].enabled = true;
                WordCardItemList[i].gameObject.name = i.ToString();
                UIEventListener.Get(WordCardItemList[i].gameObject).onClick = OnClickCard;
                Listen.SetActive(true);
                WordIcon[i].transform.parent.gameObject.SetActive(true);
                ShowTipsListBtn[i].gameObject.SetActive(true);
            }

            WordCardItemList[i].gameObject.SetActive(true);
        }
        WordGrid.enabled = true;

        WordCard.SetActive(true);
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    private int ShowTipsListBtnIndex;
    /// <summary>
    /// 点击显示提示
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayShowTipsListBtn(GameObject go)
    {
        ShowTipsListBtnIndex = int.Parse(go.name);
        AudicoManager.instance.Play("effect", "Effect/reverse card");

        if (WordIcon[ShowTipsListBtnIndex].transform.parent.gameObject.activeSelf)
        {
            WordIcon[ShowTipsListBtnIndex].transform.parent.gameObject.SetActive(false);
            Invoke("OnShowTipBtnEnd", 1.5f);
        }
        else
        {
            WordIcon[ShowTipsListBtnIndex].transform.parent.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏中文提示
    /// </summary>
    private void OnShowTipBtnEnd()
    {
        for (int i = 0;i< partNum; i++)
        {
            WordIcon[i].transform.parent.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 点击卡片
    /// </summary>
    /// <param name="go"></param>
    private void OnClickCard(GameObject go)
    {
        if (_testType == 3)
        {
            AudicoManager.instance.Play("effect", "Effect/press button");
            for (int i = 0; i < 4; i++)
            {
                BGLight[i].SetActive(false);
            }
            ClickCardID = int.Parse(go.name);
            BGLight[ClickCardID].SetActive(true);

            UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = SelcetCard;
            _gameUIInstance.ShowNextButton();
        }
    }

    /// <summary>
    /// 点击播放示例音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleBtn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordTestData.contentInfo1[_curWordIndex].voice.Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 测试环节
    /// </summary>
    private void Test()
    {
        if (_testType == 1)
        {
            Invoke("SpeakTip", 0.5f);
        }
        else if (_testType == 2)
        {
            
        }
        else if (_testType == 3)
        {
            Invoke("OnPlayExampleListBtn", 0.8f);
        }
    }

    /// <summary>
    /// 输入框输入内容
    /// </summary>
    public void OnInputchanged()
    {
        if (!_gameUIInstance._nextBtn.gameObject.activeSelf && WordInput[0].value != "")
        {
            UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = SendInput;
            _gameUIInstance.ShowNextButton();
        }
    }

    /// <summary>
    /// 自动播放示例音频
    /// </summary>
    private void OnPlayExampleListBtn()
    {
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordTestData.contentInfo1[_curWordIndex].voice.Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(ListenBtn.gameObject, time);
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        UnityAction unityAction = new UnityAction(OnSpeak);
        _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Word1, _wordTestData.contentInfo1[_curWordIndex].word);
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        MySpeakBtn.gameObject.SetActive(true);
        SpeakTip();
        UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = SureSendScore;
        _gameUIInstance.ShowNextButton();
    }

    private void SureSendScore(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        MySpeakBtn.gameObject.SetActive(false);
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        _gameUIInstance.CancelSpeak();
        SendScore(_gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._isPass, 2);
    }

    /// <summary>
    /// 提交输入文本
    /// </summary>
    /// <param name="go"></param>
    private void SendInput(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        bool isPass = false;
        if (WordInput[0].value.Equals(_wordTestData.contentInfo1[_curWordIndex].word))
        {
            Debug.Log("正确");
            isPass = true;
        } else
        {
            Debug.Log("错误");
            isPass = false;
        }

        SendScore(-1, -1, -1, -1, isPass, 2);
    }

    /// <summary>
    /// 选择卡片
    /// </summary>
    private void SelcetCard(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        bool isPass = false;
        if (ClickCardID == RightIndex)
        {
            Debug.Log("正确");
            isPass = true;
        }
        else
        {
            Debug.Log("错误");
            isPass = false;
        }

        SendScore(-1, -1, -1, -1, isPass, 2);
    }

    /// <summary>
    /// 发送分数
    /// </summary>
    /// <param name="_score"></param>
    /// <param name="_fluency"></param>
    /// <param name="_unit"></param>
    /// <param name="_integrity"></param>
    /// <param name="_isPass"></param>
    /// <param name="_speakTimes"></param>
    private void SendScore(int _score, int _fluency, int _unit, int _integrity, bool _isPass, int _speakTimes)
    {
        if (_isPass)
        {
            _passNum++;
            DataManager.GetInstance().curStudyProgress.passNum = _passNum;
        }
        if (DataManager.GetInstance().curStudyProgress.completeNum < DataManager.GetInstance().curStudyProgress.totalNum)
        {
            DataManager.GetInstance().curStudyProgress.completeNum++;
        }

        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = DataManager.GetInstance().roleData.curMoudleId
        };
        scoreBaseInfo scoreBaseInfo = new scoreBaseInfo
        {
            score = _score,
            fluency = _fluency,
            nicety = _unit,
            integrity = _integrity
        };
        scoreUnit scoreUnit = new scoreUnit
        {
            isPass = _isPass,
            info = scoreBaseInfo,
            index = _curWordIndex,
            markingIndex = 1,
            markingTotal = 1,
            order = _speakTimes
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _wordNum,
            scoreInfo = scoreUnit
        };

        Debug.Log("上传：第" + _curWordIndex + "个单词的学习分数");
        NetSender.Send<ProtoProtocol.SendLearnResultInfo>(msg, OnLearnResultInfo);
    }

    /// <summary>
    /// 收到获得钻石数据
    /// </summary>
    /// <param name="msg">钻石数据</param>
    private void OnLearnResultInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：第" + _curWordIndex + "个单词的学习分数");
        var data = (SendLearnResultInfo.response)rpcRsp;
        if (data.status)
        {
            if (data.addDiamond > 0)
            {
                _gameUIInstance.AddDiamondByOther((int)data.addDiamond);
            }
            _gameUIInstance.SetProgress(_passNum, _wordNum);
            isEnd();
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    private void isEnd()
    {
        AudicoManager.instance.StopStudyAudio();
        if (_studyQueue.Count <= 0)
        {
            // 结算
            Invoke("Settlement", 1f);
        }
        else
        {
            _curWordIndex = _studyQueue.Dequeue();
            _testType = RandomStep() + 1;
            //_step = 1;
            HideAllArea();
            ShowBeginTitle();
        }
    }

    /// <summary>
    /// 随机步骤
    /// </summary>
    /// <returns></returns>
    private int RandomStep()
    {
        int weight = 0;
        for (int i = 0; i < _wordTestData.weight.Count; i++)
        {
            weight = weight + int.Parse(_wordTestData.weight[i]);
        }

        for (int i = 0; i < _wordTestData.weight.Count; i++)
        {
            int value = Random.Range(1, weight);
            if (value <= int.Parse(_wordTestData.weight[i]))
            {
                return i;
            } else
            {
                weight = weight - int.Parse(_wordTestData.weight[i]);
            }
        }

        return 0;
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        for (int i = 0; i < WordCardItemList.Count; i++)
        {
            WordIcon[i].transform.parent.gameObject.SetActive(true);
        }
        WordCard.SetActive(false);
        Listen.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
    }

    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.WordTest, _passNum, _wordNum);
    }
}