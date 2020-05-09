using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StudyWord3 : MonoBehaviour
{
    // 卡片区
    public GameObject WordCard;
    public UITexture WordIcon;
    public UIButton PlayExampleBtn;
    public UIButton ShowTipBtn;
    public UILabel WordTranslation;

    public GameObject WordInteraction;
    // 步骤1
    public GameObject Setp1;
    // 填空区
    public UIGrid WordGrid;
    public GameObject WordItem;
    private List<GameObject> WordItemList = new List<GameObject>();
    private List<UILabel> FillWordList = new List<UILabel>();
    private List<UILabel> WordList = new List<UILabel>();
    private List<GameObject> LockList = new List<GameObject>();

    // 选项区
    public UIGrid OptionGrid;
    public UIButton OptionBtnItem;
    private List<UIButton> OptionBtnItemList = new List<UIButton>();
    private List<UILabel> OptionWordList = new List<UILabel>();
    public GameObject bg;
    public UIWidget bgWidget;

    // 步骤2
    public GameObject Setp2;

    public UIGrid WordGroup1;
    public GameObject Word1Item;
    private List<GameObject> WordItem1List = new List<GameObject>();
    private List<UITexture> Word1Textrue = new List<UITexture>();
    private List<UILabel> Word1List = new List<UILabel>();
    private List<UIButton> Voice1List = new List<UIButton>();

    public UIGrid WordGroup2;
    private List<GameObject> WordItem2List = new List<GameObject>();
    private List<UITexture> Word2Textrue = new List<UITexture>();
    private List<UILabel> Word2List = new List<UILabel>();
    private List<UIButton> Voice2List = new List<UIButton>();
    private List<UIButton> MyVoice2List = new List<UIButton>();
    private List<GameObject> Word2InputLight = new List<GameObject>();

    // 数据
    private UIWordGame _gameUIInstance;
    public SyncMoudle7Info.request _wordData3;
    /// <summary>
    /// 知识点总数
    /// </summary>
    public int _wordNum = 0;
    /// <summary>
    /// 知识点通过数量
    /// </summary>
    private int _passNum = 0;
    private Queue<int> _studyQueue;
    private int _isPassNum = 0;
    /// <summary>
    /// 当前知识点编号(从0开始)
    /// </summary>
    public int _curWordIndex = 0;
    /// <summary>
    /// 步骤总数
    /// </summary>
    private int _stepNum = 0;
    /// <summary>
    /// 当前步骤编号(从1开始)
    /// </summary>
    public int _curStepIndex = 1;
    /// <summary>
    /// 资源加载路径
    /// </summary>
    public string _assetsPath;
    public bool _hasTowStep = true;

    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _wordData3 = DataManager.GetInstance().wordData3;
        _wordNum = _wordData3.infoList.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _isPassNum = 0;
        _curStepIndex = 1;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
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
        //_curWordIndex = 5;
    }

    /// <summary>
    /// 一个单词的开始：展示单词编号
    /// </summary>
    public void ShowBeginTitle()
    {
        if (_wordData3.infoList[_curWordIndex].expandWord.Count == 0)
        {
            _hasTowStep = false;
            _stepNum = 1;
        }
        else
        {
            _hasTowStep = true;
            _stepNum = 2;
        }

        _gameUIInstance.SetStepNum(_stepNum);
        _gameUIInstance.SetCurStep(_curStepIndex);
        _gameUIInstance.SetProgress(_passNum, _wordNum);
        _gameUIInstance.ShowBeginTitle("Word - " + (_curWordIndex + 1).ToString());
        Invoke("ShowTeacherCommand", 2.5f);
    }

    /// <summary>
    /// 一个阶段的开始：展示阶段指令
    /// </summary>
    public void ShowTeacherCommand()
    {
        _gameUIInstance.SetCurStep(_curStepIndex);
        _gameUIInstance.TeacherCommand(_wordData3.infoList[_curWordIndex].cStatements[_curStepIndex - 1], "");
        //_gameUIInstance.ShowBeginTitle(_wordData3.infoList[_wordIndex].statements[_step - 1]);
        //Invoke("StartStudy", 2.5f);
        StartStudy();
    }

    /// <summary>
    /// 单词学习初始化
    /// </summary>
    public void StartStudy()
    {
        if (_curStepIndex == 1)
        {
            Setp2.SetActive(false);
            WordCardInit();
            FillWordInit();
            OptionBtnItemInit();
            Setp1.SetActive(true);
        }
        else if (_curStepIndex == 2)
        {
            Setp1.SetActive(false);
            WordGroup1Init();
            WordGroup2Init();
            Setp2.SetActive(true);
        }
    }

    //公共步骤//
    /// <summary>
    /// 卡片区初始化
    /// </summary>
    private void WordCardInit()
    {
        ResourceLoader.Instance.GetTextureResources(WordIcon, "Texture/" + StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].constantInfo.icon1);
        WordTranslation.text = _wordData3.infoList[_curWordIndex].constantInfo.cword;
        UIEventListener.Get(PlayExampleBtn.gameObject).onClick = OnPlayExampleBtn;
        UIEventListener.Get(ShowTipBtn.gameObject).onClick = OnShowTipBtn;
        WordTranslation.gameObject.SetActive(false);
        WordTranslation.gameObject.SetActive(true);
        //WordCard1.transform.DOLocalMoveX(-366,2f); //-980
        WordCard.SetActive(true);
    }

    /// <summary>
    /// 播放示例音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleBtn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].constantInfo.voice.Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 显示中文提示
    /// </summary>
    /// <param name="go"></param>
    private void OnShowTipBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/reverse card");
        if (WordIcon.transform.parent.gameObject.activeSelf)
        {
            WordIcon.transform.parent.gameObject.SetActive(false);
            Invoke("OnShowTipBtnEnd", 1.5f);
        }
        else
        {
            WordIcon.transform.parent.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏中文提示
    /// </summary>
    private void OnShowTipBtnEnd()
    {
        WordIcon.transform.parent.gameObject.SetActive(true);
    }

    // 第一步骤 //
    public int _needFillWordIndex = 0;
    public int _needFillWord = 0;
    /// <summary>
    /// 填空区初始化
    /// </summary>
    private void FillWordInit()
    {
        string word = _wordData3.infoList[_curWordIndex].constantInfo.word;
        int wordLength = word.Length;

        if (wordLength < WordItemList.Count)
        {
            for (int i = wordLength; i < WordItemList.Count; i++)
            {
                WordItemList[i].gameObject.SetActive(false);
            }
        }

        _needFillWord = wordLength;
        _needFillWordIndex = 0;

        for (int i = 0; i < wordLength; i++)
        {
            if (i >= WordItemList.Count)
            {
                GameObject item = Instantiate(WordItem);
                item.name = i.ToString();
                item.transform.parent = WordGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                WordItemList.Add(item);

                FillWordList.Add(WordItemList[i].transform.Find("InputBg/FillWord").GetComponent<UILabel>());
                WordList.Add(WordItemList[i].transform.Find("Word").GetComponent<UILabel>());
                LockList.Add(WordItemList[i].transform.Find("InputBg/Lock").gameObject);
            }
            
            WordList[i].text = word[i].ToString();
            FillWordList[i].text = "";
            FillWordList[i].transform.parent.gameObject.SetActive(true);
            LockList[i].SetActive(true);

            if (i==0)
            {
                LockList[i].SetActive(false);
            }

            WordItemList[i].SetActive(true);
        }
        WordGrid.enabled = true;
    }

    public List<char> _wordCharList = new List<char>();
    /// <summary>
    /// 选项区初始化
    /// </summary>
    private void OptionBtnItemInit()
    {
        string word = _wordData3.infoList[_curWordIndex].constantInfo.word;
        int wordLength = word.Length;

        _wordCharList.Clear();
        for (int i = 0; i < wordLength; i++)
        {
            _wordCharList.Add(word[i]);
        }
        _wordCharList = SortRandom(_wordCharList);

        if (wordLength < OptionBtnItemList.Count)
        {
            for (int i = wordLength; i < OptionBtnItemList.Count; i++)
            {
                OptionBtnItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < wordLength; i++)
        {
            if (i >= OptionBtnItemList.Count)
            {
                GameObject item = Instantiate(OptionBtnItem.gameObject);
                item.name = i.ToString();
                item.transform.parent = OptionGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                OptionBtnItemList.Add(item.GetComponent<UIButton>());

                OptionWordList.Add(OptionBtnItemList[i].gameObject.transform.Find("OptionWord").GetComponent<UILabel>());
            }

            OptionWordList[i].text = _wordCharList[i].ToString();
            OptionBtnItemList[i].gameObject.name = i.ToString();
            UIEventListener.Get(OptionBtnItemList[i].gameObject).onClick = OnSelectWordCharBtn;

            OptionBtnItemList[i].gameObject.SetActive(true);
        }

        /*OptionGrid.maxPerLine = 10;
        if (wordLength >= 16)
        {
            OptionGrid.maxPerLine = 7;
        }*/

        float scale = 1;
        if (wordLength <= 8)
        {
            scale = 1;
            bgWidget.width = 740;
        }
        else if (wordLength <= 10)
        {
            scale = 0.8f;
            bgWidget.width = 960;
        }
        else if (wordLength <= 12)
        {
            scale = 0.7f;
            bgWidget.width = 1100;
        }
        else if (wordLength <= 14)
        {
            scale = 0.6f;
            bgWidget.width = 1280;
        }
        else if (wordLength <= 18)
        {
            scale = 0.5f;
            bgWidget.width = 1600;
        }
        else if (wordLength <= 20)
        {
            scale = 0.45f;
            bgWidget.width = 1780;
        }
        else
        {
            scale = 0.4f;
            bgWidget.width = 2050;
        }
        WordGrid.transform.localScale = new Vector3(scale, scale, 1);
        bg.transform.localScale = new Vector3(scale, scale, 1);

        OptionGrid.enabled = true;
    }

    private int SelectNum;
    private bool canSelect = true;
    /// <summary>
    /// 选择单词字符选项
    /// </summary>
    /// <param name="go"></param>
    private void OnSelectWordCharBtn(GameObject go)
    {
        if (canSelect)
        {
            OptionBtnItemList[int.Parse(go.name)].gameObject.SetActive(false);

            SelectNum = int.Parse(go.name);
            FillWordList[_needFillWordIndex].text = _wordCharList[SelectNum].ToString();
            
            if (_wordCharList[SelectNum].ToString().Equals(WordList[_needFillWordIndex].text))
            {
                // todo:正确特效,镶嵌
                _needFillWordIndex++;
            }
            else
            {
                canSelect = false;
                // todo:错误特效，提醒
                Invoke("CleanWrongAnswer", 1f);
            }

            if (_needFillWordIndex == _needFillWord)
            {
                AudicoManager.instance.Play("click", "Effect/get the crystal");
                //_gameUIInstance.AddDiamondByOther(5);
                OnPlayExampleBtn(PlayExampleBtn.gameObject);
                if (_hasTowStep)
                {
                    // 下一个步骤
                    SendScore(0, 0, 0, 0, true, 1, (_wordData3.infoList[_curWordIndex].expandWord.Count / 2) + 1,1);
                } else
                {
                    SendScore(0, 0, 0, 0, true, 1,1,1);
                }
            } else
            {
                LockList[_needFillWordIndex].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 发送分数
    /// </summary>
    /// <param name="_score"></param>
    /// <param name="_fluency"></param>
    /// <param name="_unit"></param>
    /// <param name="_integrity"></param>
    /// <param name="_isPass"></param>
    /// <param name="_order"></param>
    private void SendScore(int _score, int _fluency, int _unit, int _integrity, bool _isPass, int _order,int _stepNum, int _step)
    {
        if (_isPass)
        {
            _isPassNum++;
        }

        if (_step == _stepNum)
        {
            if (_isPassNum >= _stepNum)
            {
                _passNum++;
                DataManager.GetInstance().curStudyProgress.passNum = _passNum;
            }
            _isPassNum = 0;
            if (DataManager.GetInstance().curStudyProgress.completeNum < DataManager.GetInstance().curStudyProgress.totalNum)
            {
                DataManager.GetInstance().curStudyProgress.completeNum++;
            }
            _gameUIInstance.SetProgress(_passNum, _wordNum);
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
            markingIndex = _step,
            markingTotal = _stepNum,
            order = _order
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _wordNum,
            scoreInfo = scoreUnit
        };

        Debug.Log("上传：学习分数");
        NetSender.Send<ProtoProtocol.SendLearnResultInfo>(msg, OnLearnResultInfo);
    }

    /// <summary>
    /// 收到获得钻石数据
    /// </summary>
    /// <param name="msg">钻石数据</param>
    private void OnLearnResultInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：获取钻石结果");
        var data = (SendLearnResultInfo.response)rpcRsp;
        if (data.status)
        {
            if (data.addDiamond > 0)
            {
                _gameUIInstance.AddDiamondByOther((int)data.addDiamond);
            }
            IsEnd();
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    /// <summary>
    /// 清除错误答案
    /// </summary>
    private void CleanWrongAnswer()
    {
        FillWordList[_needFillWordIndex].text = "";
        OptionBtnItemList[SelectNum].gameObject.SetActive(true);
        canSelect = true;
    }

    /// <summary>
    /// 示例组初始化
    /// </summary>
    private void WordGroup1Init()
    {
        int partNum = _wordData3.infoList[_curWordIndex].expandWord.Count / 2;
        if (partNum < WordItem1List.Count)
        {
            for (int i = partNum; i < WordItem1List.Count; i++)
            {
                WordItem1List[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= WordItem1List.Count)
            {
                GameObject item = Instantiate(Word1Item);
                item.name = i.ToString();
                item.transform.parent = WordGroup1.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                WordItem1List.Add(item);

                Word1Textrue.Add(WordItem1List[i].transform.Find("WordIconBg/WordIcon").GetComponent<UITexture>());
                Word1List.Add(WordItem1List[i].transform.Find("WordLabel").GetComponent<UILabel>());
                Voice1List.Add(WordItem1List[i].transform.Find("PlayVoiceBtn").GetComponent<UIButton>());
            }

            ResourceLoader.Instance.GetTextureResources(Word1Textrue[i], "Texture/" + StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].expandIcon[i]);
            Word1List[i].text = _wordData3.infoList[_curWordIndex].expandWord[i];
            Word1List[i].gameObject.SetActive(true);
            Voice1List[i].gameObject.name = i.ToString();
            UIEventListener.Get(Voice1List[i].gameObject).onClick = OnPlayStep2Btn;
            Voice1List[i].gameObject.SetActive(true);

            WordItem1List[i].gameObject.SetActive(true);
        }
        WordGroup1.enabled = true;
    }

    /// <summary>
    /// 学习组初始化
    /// </summary>
    private void WordGroup2Init()
    {
        int partNum = _wordData3.infoList[_curWordIndex].expandWord.Count / 2;
        if (partNum < WordItem2List.Count)
        {
            for (int i = partNum; i < WordItem2List.Count; i++)
            {
                WordItem2List[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= WordItem2List.Count)
            {
                GameObject item = Instantiate(Word1Item);
                item.name = i.ToString();
                item.transform.parent = WordGroup2.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                WordItem2List.Add(item);

                Word2Textrue.Add(WordItem2List[i].transform.Find("WordIconBg/WordIcon").GetComponent<UITexture>());
                Word2List.Add(WordItem2List[i].transform.Find("WordInput/WordInputLabel").GetComponent<UILabel>());
                Word2InputLight.Add(WordItem2List[i].transform.Find("WordInput/WordLight").gameObject);
                Voice2List.Add(WordItem2List[i].transform.Find("PlayVoiceBtn1").GetComponent<UIButton>());
                MyVoice2List.Add(WordItem2List[i].transform.Find("MyVoiceBtn").GetComponent<UIButton>());
            }

            ResourceLoader.Instance.GetTextureResources(Word2Textrue[i], "Texture/" + StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].expandIcon[i+ partNum]);
            Word2List[i].transform.parent.gameObject.SetActive(true);
            Word2List[i].gameObject.SetActive(false);
            Voice2List[i].gameObject.SetActive(false);
            MyVoice2List[i].gameObject.SetActive(false);
            Word2InputLight[i].gameObject.SetActive(false);
            WordItem2List[i].gameObject.SetActive(true);
            Voice2List[i].gameObject.name = i.ToString();
            UIEventListener.Get(Voice2List[i].gameObject).onClick = OnPlayStep2Btn;
            UIEventListener.Get(MyVoice2List[i].gameObject).onClick = PlayRecordBtn;
        }
        WordGroup2.enabled = true;

        StartPlayWordVoice(partNum);
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    /// <summary>
    /// 播放Step2音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayStep2Btn(GameObject go)
    {
        Debug.Log("音频："+ _wordData3.infoList[_curWordIndex].expandVoice[int.Parse(go.name)]);
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].expandVoice[int.Parse(go.name)].Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    private int _wordPlayIndex;
    private int _wordPlayNum;
    /// <summary>
    /// 初始化播放句式选项
    /// </summary>
    private void StartPlayWordVoice(int partNum)
    {
        _wordPlayIndex = 0;
        _wordPlayNum = partNum;
        Invoke("PlayWordVoice", 0.8f);
    }

    /// <summary>
    /// 依次播放单词音频
    /// </summary>
    private void PlayWordVoice()
    {
        // 系统发音
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].expandVoice[_wordPlayIndex].Replace(".mp3", "").Replace(" ", "")); ;
        _gameUIInstance.SetVoiceEffect(Voice1List[_wordPlayIndex].gameObject, time);
        _wordPlayIndex++;

        if (_wordPlayIndex < _wordPlayNum)
        {
            Invoke("PlayWordVoice", time);
        }
        else
        {
            Invoke("SpeakTipInit", time);
        }
    }

    private int SpeakTipIndex;
    private int SpeakTipNum;
    /// <summary>
    /// 语音提示初始化
    /// </summary>
    private void SpeakTipInit()
    {
        SpeakTipNum = _wordData3.infoList[_curWordIndex].expandWord.Count / 2;
        SpeakTipIndex = 0;
        SpeakTip();
    }
    
    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        if (SpeakTipIndex>0)
        {
            Word2InputLight[SpeakTipIndex - 1].gameObject.SetActive(false);
        }
        Word2InputLight[SpeakTipIndex].gameObject.SetActive(true);
        UnityAction unityAction = new UnityAction(OnSpeak);
        if (_wordData3.infoList[_curWordIndex].expandWord[SpeakTipIndex].Contains(" "))
        {
            _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Sentence1, _wordData3.infoList[_curWordIndex].expandWord[SpeakTipIndex]);
        } else
        {
            _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Word3, _wordData3.infoList[_curWordIndex].expandWord[SpeakTipIndex]);
        }
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        float time = 0f;
        
        if (!_gameUIInstance._isPass)
        {
            Word2List[SpeakTipIndex].text = GameTools.Instance.SpeakColorText(_wordData3.infoList[_curWordIndex].expandWord[SpeakTipIndex], 3);
            // 系统播放原声
            time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData3.infoList[_curWordIndex].expandVoice[SpeakTipIndex].Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(Voice2List[SpeakTipIndex].gameObject, time);
        } else
        {
            Word2List[SpeakTipIndex].text = GameTools.Instance.SpeakColorText(_wordData3.infoList[_curWordIndex].expandWord[SpeakTipIndex], 1);
        }
        Word2List[SpeakTipIndex].color = new Color(1, 1, 1);
        Word2List[SpeakTipIndex].gameObject.SetActive(true);

        SendScore(_gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._isPass, _gameUIInstance.SpeakTimes, (_wordData3.infoList[_curWordIndex].expandWord.Count / 2) + 1, SpeakTipIndex+2);
    }

    public void IsEnd()
    {
        if (_hasTowStep)
        {
            if (_curStepIndex == 1)
            {
                UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextStep;
                _gameUIInstance.ShowNextButton();
            } else
            {
                Voice2List[SpeakTipIndex].gameObject.SetActive(true);
                MyVoice2List[SpeakTipIndex].gameObject.SetActive(true);
                if (SpeakTipIndex - 1 >= 0)
                {
                    MyVoice2List[SpeakTipIndex-1].gameObject.SetActive(false);
                }

                SpeakTipIndex++;
                if (SpeakTipIndex < SpeakTipNum)
                {
                    SpeakTip();
                }
                else
                {
                    if (_studyQueue.Count <= 0)
                    {
                        // 结算
                        Invoke("Settlement", 1f);
                    }
                    else
                    {
                        // 下一个单词
                        UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextWord;
                        _gameUIInstance.ShowNextButton();
                    }
                }
            }
        } else
        {
            if (_studyQueue.Count <= 0)
            {
                // 结算
                Invoke("Settlement", 1f);
            }
            else
            {
                // 下一个单词
                UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextWord;
                _gameUIInstance.ShowNextButton();
            }
        }
    }

    /// <summary>
    /// 下一个步骤
    /// </summary>
    public void NextStep(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _curStepIndex = 2;
        _gameUIInstance.HideNextButton();
        HideAllArea();
        ShowTeacherCommand();
    }

    /// <summary>
    /// 下一个单词
    /// </summary>
    public void NextWord(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _curStepIndex = 1;
        _curWordIndex = _studyQueue.Dequeue();
        _gameUIInstance.HideNextButton();
        HideAllArea();
        ShowBeginTitle();
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        Setp1.SetActive(false);
        Setp2.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
    }

    /// <summary>
    /// 字母打乱顺序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public List<T> SortRandom<T>(List<T> collection)
    {
        for (int i = collection.Count - 1; i > 0; i--)
        {
            int p = Random.Range(0, collection.Count - 1);
            var temp = collection[p];
            collection[p] = collection[i];
            collection[i] = temp;
        }
        return collection;
    }

    /// <summary>
    /// 在字符串中每个字符之间添加一个空格
    /// </summary>
    private string AddSpaceInString(string s)
    {
        string ret = "";
        string[] strings = s.Split();
        for (int i = 0; i < strings.Length; i++)
        {
            if (i != strings.Length - 1)
            {
                ret = ret + strings[i] + " ";
            }
            else
            {
                ret = ret + strings[i];
            }
        }
        return ret;
    }

    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.Word3, _passNum, _wordNum);
    }
}