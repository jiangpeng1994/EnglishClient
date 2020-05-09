using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StudyWord2 : MonoBehaviour
{
    // 卡片区
    public GameObject WordCard;
    public UITexture WordIcon;
    public UIButton PlayExampleBtn;
    public UIButton ShowTipBtn;
    public UIButton MySpeakBtn;
    public UILabel WordTranslation;

    public GameObject WordInteraction;
    // 步骤1
    public GameObject Setp1;
    //合并区
    public GameObject CompleteItem;
    public UILabel CompleteWord;
    public UILabel CompleteSoundMark;
    public UIWidget CompleteSoundMarkBgUIWidget;
    public UIButton PlayCompleteExampleBtn;

    // 拆分区
    public UIGrid UnpackGrid;
    public GameObject UnpackItem;
    private List<GameObject> UnpackItemList = new List<GameObject>();
    private List<UILabel> WordUnpackList = new List<UILabel>();
    private List<UILabel> SoundMarkList = new List<UILabel>();
    private List<UIWidget> SoundMarkBgUIWidgetList = new List<UIWidget>();
    private List<UIButton> PlayExampleBtnList = new List<UIButton>();
    public UIWidget bgWidget1;

    // 步骤2
    public GameObject Setp2;
    // 填空区
    public UIGrid WordGrid;
    public GameObject WordItem;
    private List<GameObject> WordItemList = new List<GameObject>();
    private List<UILabel> FillWordList = new List<UILabel>();
    private List<UILabel> WordList = new List<UILabel>();
    public GameObject bg;
    public UIWidget bgWidget;

    // 选项区
    public UIGrid OptionGrid;
    public UIButton OptionBtnItem;
    private List<UIButton> OptionBtnItemList = new List<UIButton>();
    private List<UILabel> OptionWordList = new List<UILabel>();

    // 数据
    private string[] _vowel = new string[] { "a","e","i","o","u" };

    private UIWordGame _gameUIInstance;
    public SyncMoudle5Info.request _wordData2;
    /// <summary>
    /// 知识点总数
    /// </summary>
    public int _wordNum = 0;
    /// <summary>
    /// 知识点通过数量
    /// </summary>
    private int _passNum = 0;
    private int _isPassNum = 0;
    private Queue<int> _studyQueue;
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

    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _wordData2 = DataManager.GetInstance().wordData2;
        _wordNum = _wordData2.infoList.Count;
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
        //_curWordIndex = 16;
    }

    /// <summary>
    /// 一个单词的开始：展示单词编号
    /// </summary>
    public void ShowBeginTitle()
    {
        string word = _wordData2.infoList[_curWordIndex].constantInfo.word;
        int vowelNum = 0;
        for (int i = 0; i < word.Length; i++)
        {
            if (IsVowel(word[i]))
            {
                vowelNum++;
            }
        }
        
        if (vowelNum == 0)
        {
            // 没有元音,只有环节1
            _stepNum = 1;
        } else
        {
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
        _gameUIInstance.TeacherCommand(_wordData2.infoList[_curWordIndex].cStatements[_curStepIndex - 1], "");
        //_gameUIInstance.ShowBeginTitle(_wordData2.infoList[_wordIndex].statements[_step - 1]);
        //Invoke("StartStudy", 2.5f);
        StartStudy();
    }

    /// <summary>
    /// 单词学习初始化
    /// </summary>
    public void StartStudy()
    {
        WordCardInit();
        if (_curStepIndex == 1)
        {
            Setp2.SetActive(false);
            CompleteInit();
            UnpackWordInit();
            Setp1.SetActive(true);
            WordInteraction.SetActive(true);
            StartStep1();
        }
        else if (_curStepIndex == 2)
        {
            Setp1.SetActive(false);
            FillWordInit();
            OptionBtnItemInit();
            Setp2.SetActive(true);
            WordInteraction.SetActive(true);
        }
    }

    //公共步骤//
    /// <summary>
    /// 卡片区初始化
    /// </summary>
    private void WordCardInit()
    {
        if (_curStepIndex == 1)
        {
            ResourceLoader.Instance.GetTextureResources(WordIcon, "Texture/" + StringConst.WordPath + _assetsPath + _wordData2.infoList[_curWordIndex].constantInfo.icon1);
        }
        else if (_curStepIndex == 2)
        {
            ResourceLoader.Instance.GetTextureResources(WordIcon, "Texture/" + StringConst.WordPath + _assetsPath + _wordData2.infoList[_curWordIndex].constantInfo.icon2);
        }

        WordTranslation.text = _wordData2.infoList[_curWordIndex].constantInfo.cword;
        UIEventListener.Get(PlayExampleBtn.gameObject).onClick = OnPlayExampleBtn;
        UIEventListener.Get(ShowTipBtn.gameObject).onClick = OnShowTipBtn; 
        UIEventListener.Get(MySpeakBtn.gameObject).onClick = PlayRecordBtn;
        WordTranslation.gameObject.SetActive(false);
        WordTranslation.gameObject.SetActive(true);
        // WordCard1.transform.DOLocalMoveX(-366,2f); //-980
        WordCard.SetActive(true);
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    /// <summary>
    /// 播放示例音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleBtn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData2.infoList[_curWordIndex].constantInfo.voice.Replace(".mp3", "").Replace(" ", ""));
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

    //第一步骤//
    /// <summary>
    /// 合并区初始化
    /// </summary>
    private void CompleteInit()
    {
        CompleteItem.SetActive(false);
        CompleteWord.text = AddSpaceInString(_wordData2.infoList[_curWordIndex].constantInfo.word);
        CompleteSoundMark.text = _wordData2.infoList[_curWordIndex].completeVoice;
        CompleteSoundMarkBgUIWidget.width = CompleteSoundMark.width + 10;
        UIEventListener.Get(PlayCompleteExampleBtn.gameObject).onClick = OnPlayExampleBtn;
    }

    /// <summary>
    /// 拆分区初始化
    /// </summary>
    private void UnpackWordInit()
    {
        int partNum = _wordData2.infoList[_curWordIndex].soundmark.Count;

        if (partNum < UnpackItemList.Count)
        {
            for (int i = partNum; i < UnpackItemList.Count; i++)
            {
                UnpackItemList[i].gameObject.SetActive(false);
            }
        }
        int cellWidth = 0;
        int lastWidth = 0;
        int curCellWidth = 0;
        for (int i = 0; i < partNum; i++)
        {
            if (i >= UnpackItemList.Count)
            {
                GameObject item = Instantiate(UnpackItem);
                item.name = i.ToString();
                item.transform.parent = UnpackGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                UnpackItemList.Add(item);

                WordUnpackList.Add(UnpackItemList[i].transform.Find("WordUnpack").GetComponent<UILabel>());
                SoundMarkList.Add(UnpackItemList[i].transform.Find("Bubble/SoundMark").GetComponent<UILabel>());
                SoundMarkBgUIWidgetList.Add(UnpackItemList[i].transform.Find("Bubble").GetComponent<UIWidget>());
                PlayExampleBtnList.Add(UnpackItemList[i].transform.Find("PlayExampleBtn").GetComponent<UIButton>());
            }

            WordUnpackList[i].text = _wordData2.infoList[_curWordIndex].wordUnpack[i];
            SoundMarkList[i].text = _wordData2.infoList[_curWordIndex].soundmark[i];
            SoundMarkBgUIWidgetList[i].width = SoundMarkList[i].width + 10;
            PlayExampleBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(PlayExampleBtnList[i].gameObject).onClick = OnPlayExampleListBtn;

            UnpackItemList[i].SetActive(true);

            curCellWidth = WordUnpackList[i].width/2 + lastWidth;
            if (curCellWidth > cellWidth)
            {
                cellWidth = curCellWidth;
            }
            lastWidth = WordUnpackList[i].width/2;
        }

        UnpackGrid.cellWidth = cellWidth + 28;
        UnpackGrid.enabled = true;

        /*for (int i = 0; i < partNum; i++)
        {
            if(UnpackItemList[i].transform.localPosition.x + WordUnpackList[i].width / 2 > bgWidget1.width/2 ||
               UnpackItemList[i].transform.localPosition.x - WordUnpackList[i].width / 2 < -bgWidget1.width / 2)
            {
                UnpackGrid.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                bgWidget.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                bgWidget.width = 2050;
            }
        }*/
    }

    /// <summary>
    /// 播放拆分示例音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleListBtn(GameObject go)
    {
        Debug.Log("音频："+ _wordData2.infoList[_curWordIndex].unpackVoice[int.Parse(go.name)]);
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData2.infoList[_curWordIndex].unpackVoice[int.Parse(go.name)].Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 当前读到第几个音节
    /// </summary>
    private int _wordPlayIndex = 0;
    /// <summary>
    /// 当前一共多少音节
    /// </summary>
    private int _wordPlayNum = 0;
    /// <summary>
    /// 开始步骤1流程
    /// </summary>
    private void StartStep1()
    {
        _wordPlayIndex = 0;
        _wordPlayNum = _wordData2.infoList[_curWordIndex].soundmark.Count;
        if (_wordPlayNum > 1)
        {
            // 多音节
            Invoke("PlayWordVoice", 0.8f);
        }
        else
        {
            // 单音节
            SpeakTip();
        }
    }

    /// <summary>
    /// 依次播放音节语音
    /// </summary>
    private void PlayWordVoice()
    {
        if (_wordPlayIndex < _wordPlayNum)
        {
            // 系统发音
            //Debug.Log("音节音频:" + _wordData2.infoList[_curWordIndex].unpackVoice[_wordPlayIndex].Replace(".mp3", ""));
            //OnPlayExampleListBtn(PlayExampleBtnList[_wordPlayIndex].gameObject);
            float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData2.infoList[_curWordIndex].unpackVoice[_wordPlayIndex].Replace(".mp3", ""));
            _gameUIInstance.SetVoiceEffect(PlayExampleBtnList[_wordPlayIndex].gameObject, time);
            _wordPlayIndex++;
            Invoke("PlayWordVoice", time);
        }
        else
        {
            PlayWordVoiceEnd();
        }
    }

    /// <summary>
    /// 结束自动读音
    /// </summary>
    private void PlayWordVoiceEnd()
    {
        //todo:靠拢合并特效
        Invoke("SpeakTip", 1f);
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        UnityAction unityAction = new UnityAction(OnSpeak);
        _gameUIInstance.ReadyToSpeak(1, unityAction,TeachType.Word2, _wordData2.infoList[_curWordIndex].constantInfo.sendWord, _wordData2.infoList[_curWordIndex].constantInfo.customSoundmark);
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        float time = 0f;
        /*if (!_gameUIInstance._isPass)
        {
            // 系统播放原声
            time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData2.infoList[_curWordIndex].constantInfo.voice.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(PlayExampleBtn.gameObject, time);
        }*/

        Debug.Log("上传：学习分数");
        Invoke("StartSendScore", time);
    }

    private void StartSendScore()
    {
        SendScore(_gameUIInstance._score, _gameUIInstance._fluency, _gameUIInstance._nicety, _gameUIInstance._integrity, _gameUIInstance._isPass, _gameUIInstance.SpeakTimes);
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
    private void SendScore(int _score, int _fluency, int _unit, int _integrity, bool _isPass, int _order)
    {
        if (_isPass)
        {
            _isPassNum++;
        }

        if (_curStepIndex == _stepNum)
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
            markingIndex = _curStepIndex,
            markingTotal = _stepNum,
            order = _order
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _wordNum,
            scoreInfo = scoreUnit
        };
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

            if (_curStepIndex == 1)
            {
                MergeWordUnpack();
            } else
            {
                IsEnd();
            }
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    /// <summary>
    /// 拆分区合并为合并区
    /// </summary>
    private void MergeWordUnpack()
    {
        MySpeakBtn.gameObject.SetActive(true);
        for (int i = 0; i < UnpackItemList.Count; i++)
        {
            UnpackItemList[i].SetActive(false);
        }

        //todo：音节图出现特效，变成完整的单词音标，同时字母合成完整单词
        CompleteItem.SetActive(true);
        OnPlayExampleBtn(PlayCompleteExampleBtn.gameObject);
        if (_stepNum == 1)
        {
            IsEnd();
        } else
        {
            UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextStep;
            _gameUIInstance.ShowNextButton();
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

    // 第二步骤 //
    public List<int> _needFillWordNumList = new List<int>();
    public int _needFillWordIndex = 0;
    public int _needFillWord = 0;
    /// <summary>
    /// 填空区初始化
    /// </summary>
    private void FillWordInit()
    {
        string word = _wordData2.infoList[_curWordIndex].constantInfo.word;
        int wordLength = word.Length;

        if (wordLength < WordItemList.Count)
        {
            for (int i = wordLength; i < WordItemList.Count; i++)
            {
                WordItemList[i].gameObject.SetActive(false);
            }
        }

        _needFillWordNumList.Clear();
        _needFillWord = 0;
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
            }
            
            WordList[i].text = word[i].ToString();
            if (IsVowel(word[i]))
            {
                // 是元音
                FillWordList[i].text = "";
                FillWordList[i].transform.parent.gameObject.SetActive(true);
                _needFillWordNumList.Add(i);
                _needFillWord++;
            }
            else
            {
                // 不是元音
                FillWordList[i].text = "";
                FillWordList[i].transform.parent.gameObject.SetActive(false);
            }

            WordItemList[i].SetActive(true);
        }
        WordGrid.enabled = true;

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
    }

    /// <summary>
    /// 选项区初始化
    /// </summary>
    private void OptionBtnItemInit()
    {
        if (_vowel.Length < OptionBtnItemList.Count)
        {
            for (int i = _vowel.Length; i < OptionBtnItemList.Count; i++)
            {
                OptionBtnItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _vowel.Length; i++)
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

            OptionWordList[i].text = _vowel[i];
            OptionBtnItemList[i].gameObject.name = i.ToString();
            UIEventListener.Get(OptionBtnItemList[i].gameObject).onClick = OnSelectVowelBtn;

            OptionBtnItemList[i].gameObject.SetActive(true);
        }
        OptionGrid.enabled = true;
       
        canSelect = true;
    }

    public void Update()
    {
        // 解决UI偏移问题
        if (_curStepIndex == 2)
        {
            for (int i = 0; i < OptionBtnItemList.Count; i++)
            {
                OptionWordList[i].gameObject.SetActive(false);
                OptionWordList[i].gameObject.SetActive(true);
            }
        }
    }

    private bool canSelect = true;
    /// <summary>
    /// 选择元音选项
    /// </summary>
    /// <param name="go"></param>
    private void OnSelectVowelBtn(GameObject go)
    {
        if (canSelect)
        {
            int SelectNum = int.Parse(go.name);
            if (_vowel[SelectNum].Equals(WordList[_needFillWordNumList[_needFillWordIndex]].text))
            {
                // todo:正确特效(镶嵌)
                FillWordList[_needFillWordNumList[_needFillWordIndex]].text = _vowel[SelectNum];
                _needFillWordIndex++;
            }
            else
            {
                FillWordList[_needFillWordNumList[_needFillWordIndex]].text = _vowel[SelectNum];
                canSelect = false;
                // todo:错误特效(提醒)
                Invoke("CleanWrongAnswer", 0.2f);
            }

            if (_needFillWordIndex == _needFillWord)
            {
                canSelect = false;
                AudicoManager.instance.Play("click", "Effect/get the crystal");
                SendScore(0, 0, 0, 0, true, 1);
            }
        }
    }

    private void IsEnd()
    {
        //OnPlayExampleBtn(null); //拼写正确后播放单词原声
        if (_studyQueue.Count <= 0)
        {
            Invoke("Settlement", 1f);
        }
        else
        {
            UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextWord;
            _gameUIInstance.ShowNextButton();
        }
    }

    /// <summary>
    /// 清除错误答案
    /// </summary>
    private void CleanWrongAnswer()
    {
        FillWordList[_needFillWordNumList[_needFillWordIndex]].text = "";
        canSelect = true;
    }

    /// <summary>
    /// 是否是元音
    /// </summary>
    private bool IsVowel(char s)
    {
        bool ret = false;
        foreach (string item in _vowel)
        {
            if (s.ToString() == item)
            {
                ret = true;
                break;
            }
        }
        return ret;
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
        MySpeakBtn.gameObject.SetActive(false);
        WordCard.SetActive(false);
        WordInteraction.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
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
        _gameUIInstance.ShowResultPanel(TeachType.Word2, _passNum, _wordNum);
    }
}