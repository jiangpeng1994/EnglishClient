using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StudySentence2 : MonoBehaviour
{
    // 场景区
    public GameObject ScenesArea;
    public UITexture BG;
    public GameObject RoleA;
    public UITexture WhoA;
    public UIWidget WordBGA;
    public UILabel NumA;
    public UILabel ContentA;
    public UIButton ListenBtnA;
    public UIButton MySpeakBtnA;
    public GameObject VoiceA;
    public GameObject LightA;
    public GameObject TipA;
    public UITexture TipTextureA;
    public UIButton TipVoiceA;
    public UIWidget TipWidgetA;

    public GameObject RoleB;
    public UITexture WhoB;
    public UIWidget WordBGB;
    public UILabel NumB;
    public UILabel ContentB;
    public UIButton ListenBtnB;
    public UIButton MySpeakBtnB;
    public GameObject VoiceB;
    public GameObject LightB;
    public GameObject TipB;
    public UITexture TipTextureB;
    public UIButton TipVoiceB;
    public UIWidget TipWidgetB;

    // 笔记区
    public GameObject NotesArea;
    public UIGrid Note1Grid;
    public UIGrid Note2Grid;
    public GameObject NoteItem;
    
    // 数据
    private UIWordGame _gameUIInstance;
    public SyncMoudle6Info.request _sentence2Data;
    public int _sentenceNum = 0;
    public int _curSentenceIndex = 0;
    public int _level = 1;
    public int _curStepIndex = 1;
    public string _assetsPath;
    private Queue<int> _studyQueue;
    private int _passNum = 0;
    private int _isPassNum = 0;
    private int _stepNum = 0;

    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _sentence2Data = DataManager.GetInstance().sentenceData2;
        _sentenceNum = _sentence2Data.infoList.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _isPassNum = 0;
        _stepNum = 2;
        _curStepIndex = 1;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
        _gameUIInstance.SetProgress(_passNum, _sentenceNum);
        _gameUIInstance.SetStepNum(_stepNum);
    }

    /// <summary>
    /// 设置学习队列
    /// </summary>
    /// <param name="StudyQueue"></param>
    public void SetStudyQueue(Queue<int> StudyQueue)
    {
        _studyQueue = StudyQueue;
        _curSentenceIndex = _studyQueue.Dequeue();
    }

    private string[] headlines;
    private string headlineVoicePath;
    private float headlineVoiceTime;
    private float headlineShowTime;
    /// <summary>
    /// 一个句式的开始：展示句式编号
    /// </summary>
    /// <param name="level">级别</param>
    public void ShowBeginTitle(int level)
    {
        _level = level;
        _gameUIInstance.SetStepNum(_stepNum);
        _gameUIInstance.SetCurStep(_curStepIndex);
        _gameUIInstance.SetProgress(_passNum, _sentenceNum);
        headlines = _sentence2Data.infoList[_curSentenceIndex].title.Split('句');
        Debug.Log(_sentence2Data.infoList[_curSentenceIndex].title);
        Debug.Log(headlines[0]);
        Debug.Log(headlines[1]);
        //headlineVoicePath = StringConst.SentencePath + _voicePath + _sentence1Data.infoList[_sentenceIndex].headlinesMp3[0].Replace(".mp3", "");
        //headlineVoiceTime = AudicoManager.instance.Play("study", headlineVoicePath);
        //headlineShowTime = Mathf.Max(headlineVoiceTime, 2.5f);
        _gameUIInstance.ShowBeginTitle(headlines[0] + System.Environment.NewLine + "句" + headlines[1], 4.5f);
        NotesAreaInit();
        Invoke("ShowTeacherCommand", 4.5f);
    }

    /// <summary>
    /// 一个阶段的开始：展示阶段指令
    /// </summary>
    private void ShowTeacherCommand()
    {
        _gameUIInstance.SetCurStep(_curStepIndex);
        //headlineVoicePath = StringConst.SentencePath + _voicePath + _sentence1Data.infoList[_sentenceIndex].sVoice[_step-1].Replace(".mp3", "");
        //headlineVoiceTime = AudicoManager.instance.Play("study", headlineVoicePath);
        //headlineShowTime = Mathf.Max(headlineVoiceTime, 2.5f);
        _gameUIInstance.TeacherCommand(_sentence2Data.infoList[_curSentenceIndex].cStatements[_curStepIndex - 1], "");
        //_gameUIInstance.ShowBeginTitle(_sentence2Data.infoList[_sentenceIndex].statements[_step - 1]);
        StartStudy();
        //Invoke("StartStudy", 2.5f);
    }

    /// <summary>
    /// 句式学习初始化
    /// </summary>
    private void StartStudy()
    {
        ScenesAreaInit();
        NotesArea.SetActive(true);
        Invoke("AppearanceRoleA", 0.5f);
    }

    /// <summary>
    /// 场景区初始化
    /// </summary>
    private void ScenesAreaInit()
    {
        ListenBtnA.gameObject.name = "0";
        ListenBtnB.gameObject.name = "1";
        ListenBtnA.gameObject.SetActive(true);
        ListenBtnB.gameObject.SetActive(true);

        // 根据难度不同，显示不同提示
        if (_level == 2)
        {
            ContentA.gameObject.SetActive(true);
            ContentB.gameObject.SetActive(true);
            VoiceA.SetActive(false);
            VoiceB.SetActive(false);
        }
        else if (_level == 3)
        {
            ContentA.gameObject.SetActive(false);
            ContentB.gameObject.SetActive(false);
            VoiceA.SetActive(true);
            VoiceB.SetActive(true);
        }
        UIEventListener.Get(MySpeakBtnA.gameObject).onClick = PlayRecordBtn;
        UIEventListener.Get(MySpeakBtnB.gameObject).onClick = PlayRecordBtn;

        if (_curStepIndex == 1)
        {
            Debug.Log("背景图："+ _sentence2Data.infoList[_curSentenceIndex].scene1);
            if (_sentence2Data.infoList[_curSentenceIndex].scene1 != null || _sentence2Data.infoList[_curSentenceIndex].scene1 != "")
            {
                BG.mainTexture = Resources.Load<Texture>("Texture/BG/" + _sentence2Data.infoList[_curSentenceIndex].scene1.Replace(".png", ""));
            }
            WhoA.mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _sentence2Data.infoList[_curSentenceIndex].scene1sb[0].Replace(".png", ""));
            WhoB.mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _sentence2Data.infoList[_curSentenceIndex].scene1sb[1].Replace(".png", ""));
            ContentA.text = _sentence2Data.infoList[_curSentenceIndex].scene1text[0];
            ContentB.text = _sentence2Data.infoList[_curSentenceIndex].scene1text[1];
            UIEventListener.Get(ListenBtnA.gameObject).onClick = OnPlayExample1Btn;
            UIEventListener.Get(ListenBtnB.gameObject).onClick = OnPlayExample1Btn;
            if (_sentence2Data.infoList[_curSentenceIndex].scene1st.Count > 0)
            {
                //Debug.Log("线索图：" + _sentence2Data.infoList[_curSentenceIndex].scene1st[0]);
                ResourceLoader.Instance.GetTextureResources(TipTextureA, "Texture/Hint/Sentence/" + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene1st[0]);
                TipWidgetA.MakePixelPerfect();
                TipTextureA.transform.localScale = new Vector3(0.56f, 0.56f, 0.56f);
                TipA.SetActive(true);
            }
            else
            {
                TipA.SetActive(false);
            }

            UIEventListener.Get(TipVoiceA.gameObject).onClick = OnPlayExampleTipBtn1;
            TipB.SetActive(false);
        } else
        {
            //Debug.Log("背景图：" + _sentence2Data.infoList[_curSentenceIndex].scene2);
            if (_sentence2Data.infoList[_curSentenceIndex].scene2 != null || _sentence2Data.infoList[_curSentenceIndex].scene2 != "")
            {
                BG.mainTexture = Resources.Load<Texture>("Texture/BG/" + _sentence2Data.infoList[_curSentenceIndex].scene2.Replace(".png", ""));
            }
            WhoA.mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _sentence2Data.infoList[_curSentenceIndex].scene2sb[0].Replace(".png", ""));
            WhoB.mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _sentence2Data.infoList[_curSentenceIndex].scene2sb[1].Replace(".png", ""));
            ContentA.text = _sentence2Data.infoList[_curSentenceIndex].scene2text[0];
            ContentB.text = _sentence2Data.infoList[_curSentenceIndex].scene2text[1];
            UIEventListener.Get(ListenBtnA.gameObject).onClick = OnPlayExample2Btn;
            UIEventListener.Get(ListenBtnB.gameObject).onClick = OnPlayExample2Btn;

            if(_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 1)
            {
                ContentA.text = "___________________________";
                ContentA.gameObject.SetActive(true);
                VoiceA.SetActive(false);
                ListenBtnA.gameObject.SetActive(false);
                //Debug.Log("线索图数量：" + _sentence2Data.infoList[_curSentenceIndex].scene2st.Count);
                if (_sentence2Data.infoList[_curSentenceIndex].scene2st.Count > 0)
                {
                    //Debug.Log("线索图：" + _sentence2Data.infoList[_curSentenceIndex].scene2st[0]);
                    ResourceLoader.Instance.GetTextureResources(TipTextureA, "Texture/Hint/Sentence/" + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2st[0]);
                    TipWidgetA.MakePixelPerfect();
                    TipTextureA.transform.localScale = new Vector3(0.56f, 0.56f, 0.56f);
                    TipA.SetActive(true);
                } else
                {
                    TipA.SetActive(false);
                }
                
                UIEventListener.Get(TipVoiceA.gameObject).onClick = OnPlayExampleTipBtn;
                TipB.SetActive(false);
            } else
            {
                ContentB.text = "___________________________";
                ContentB.gameObject.SetActive(true);
                VoiceB.SetActive(false);
                ListenBtnB.gameObject.SetActive(false);
                if (_sentence2Data.infoList[_curSentenceIndex].scene2st.Count > 0)
                {
                    //Debug.Log("线索图：" + _sentence2Data.infoList[_curSentenceIndex].scene2st[0]);
                    ResourceLoader.Instance.GetTextureResources(TipTextureB, "Texture/Hint/Sentence/" + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2st[0]);
                    TipWidgetB.MakePixelPerfect();
                    TipTextureB.transform.localScale = new Vector3(0.56f, 0.56f, 0.56f);
                    TipB.SetActive(true);
                }
                else
                {
                    TipB.SetActive(false);
                }

                UIEventListener.Get(TipVoiceB.gameObject).onClick = OnPlayExampleTipBtn;
                TipA.SetActive(false);
            }
        }

        RoleA.SetActive(false);
        RoleB.SetActive(false);
        WordBGA.gameObject.SetActive(false);
        WordBGB.gameObject.SetActive(false);
        MySpeakBtnA.gameObject.SetActive(false);
        MySpeakBtnB.gameObject.SetActive(false);
        
        ScenesArea.SetActive(true);
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    /// <summary>
    /// 播放提示音频1
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleTipBtn1(GameObject go)
    {
        if (_sentence2Data.infoList[_curSentenceIndex].cluesVoice1!= null && _sentence2Data.infoList[_curSentenceIndex].cluesVoice1.Count > 0)
        {
            float time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].cluesVoice1[0].Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(go, time);
        }
    }

    /// <summary>
    /// 播放提示音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleTipBtn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].cluesVoice2[0].Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    private List<GameObject> Note1ItemList = new List<GameObject>();
    private List<UILabel> Num1List = new List<UILabel>();
    private List<UITexture> Who1List = new List<UITexture>();
    private List<UIButton> ListenBtn1List = new List<UIButton>();
    private List<UILabel> Text1List = new List<UILabel>();
    private List<GameObject> Voice1List = new List<GameObject>();

    private List<GameObject> Note2ItemList = new List<GameObject>();
    private List<UILabel> Num2List = new List<UILabel>();
    private List<UITexture> Who2List = new List<UITexture>();
    private List<UIButton> ListenBtn2List = new List<UIButton>();
    private List<UILabel> Text2List = new List<UILabel>();
    private List<GameObject> Voice2List = new List<GameObject>();
    /// <summary>
    /// 笔记区初始化
    /// </summary>
    private void NotesAreaInit()
    {
        int noteNum = 2;

        if (noteNum < Note1ItemList.Count)
        {
            for (int i = noteNum; i < Note1ItemList.Count; i++)
            {
                Note1ItemList[i].gameObject.SetActive(false);
            }
        }

        if (noteNum < Note2ItemList.Count)
        {
            for (int i = noteNum; i < Note2ItemList.Count; i++)
            {
                Note2ItemList[i].gameObject.SetActive(false);
            }
        }

        // 笔记1
        for (int i = 0; i < noteNum; i++)
        {
            if (i >= Note1ItemList.Count)
            {
                GameObject item = Instantiate(NoteItem);
                item.name = i.ToString();
                item.transform.parent = Note1Grid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                Note1ItemList.Add(item);

                Num1List.Add(Note1ItemList[i].transform.Find("Num").GetComponent<UILabel>());
                Who1List.Add(Note1ItemList[i].transform.Find("Role/Avatar").GetComponent<UITexture>());
                ListenBtn1List.Add(Note1ItemList[i].transform.Find("Content/ListenBtn").GetComponent<UIButton>());
                Text1List.Add(Note1ItemList[i].transform.Find("Content/ContentText/Text").GetComponent<UILabel>());
                Voice1List.Add(Note1ItemList[i].transform.Find("Content/ContentText/Voice").gameObject);
            }

            // 设置编号
            Num1List[i].text = (i + 1).ToString();
            // 设置头像
            Who1List[i].mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _sentence2Data.infoList[_curSentenceIndex].scene1sb[i].Replace(".png", ""));
            // 设置文字
            Text1List[i].text = _sentence2Data.infoList[_curSentenceIndex].scene1textCo[i];
            if (Text1List[i].text.Contains("[-]"))
            {
                Text1List[i].color = new Color(1, 1, 1);
            }
            else
            {
                Text1List[i].color = new Color(0, 0, 0);
            }

            // 设置原声
            ListenBtn1List[i].gameObject.name = i.ToString();
            UIEventListener.Get(ListenBtn1List[i].gameObject).onClick = OnPlayExample1Btn;
            ListenBtn1List[i].gameObject.SetActive(true);

            // 不同的级别，选项的难度不同
            if (_level == 2)
            {
                // 展示读音和文字
                Voice1List[i].SetActive(false);
                Text1List[i].gameObject.SetActive(true);
            }
            else if (_level == 3)
            {
                // 展示读音
                Voice1List[i].SetActive(true);
                Text1List[i].gameObject.SetActive(false);
            }

            Note1ItemList[i].SetActive(true);
        }
        Note1Grid.enabled = true;

        // 笔记2
        for (int i = 0; i < noteNum; i++)
        {
            if (i >= Note2ItemList.Count)
            {
                GameObject item = Instantiate(NoteItem);
                item.name = i.ToString();
                item.transform.parent = Note2Grid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                Note2ItemList.Add(item);

                Num2List.Add(Note2ItemList[i].transform.Find("Num").GetComponent<UILabel>());
                Who2List.Add(Note2ItemList[i].transform.Find("Role/Avatar").GetComponent<UITexture>());
                ListenBtn2List.Add(Note2ItemList[i].transform.Find("Content/ListenBtn").GetComponent<UIButton>());
                Text2List.Add(Note2ItemList[i].transform.Find("Content/ContentText/Text").GetComponent<UILabel>());
                Voice2List.Add(Note2ItemList[i].transform.Find("Content/ContentText/Voice").gameObject);
            }

            // 设置编号
            Num2List[i].text = (i + 1).ToString();
            // 设置头像
            Who2List[i].mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _sentence2Data.infoList[_curSentenceIndex].scene2sb[i].Replace(".png", ""));
            // 设置文字
            Text2List[i].text = _sentence2Data.infoList[_curSentenceIndex].scene2textCo[i];
            if(Text2List[i].text.Contains("[-]"))
            {
                Text2List[i].color = new Color(1,1,1);
            } else
            {
                Text2List[i].color = new Color(0, 0, 0);
            }

            // 设置原声
            ListenBtn2List[i].gameObject.name = i.ToString();
            UIEventListener.Get(ListenBtn2List[i].gameObject).onClick = OnPlayExample2Btn;
            ListenBtn2List[i].gameObject.SetActive(true);

            // 不同的级别，选项的难度不同
            if (_level == 2)
            {
                // 展示读音和文字
                Voice2List[i].SetActive(false);
                Text2List[i].gameObject.SetActive(true);
            }
            else if (_level == 3)
            {
                // 展示读音
                Voice2List[i].SetActive(true);
                Text2List[i].gameObject.SetActive(false);
            }

            Note2ItemList[i].SetActive(true);
        }
        Note2Grid.enabled = true;

        Note1Grid.gameObject.SetActive(false);
        Note2Grid.gameObject.SetActive(false);
    }

    /// <summary>
    /// 笔记1的语音
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExample1Btn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene1voice[int.Parse(go.name)].Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 笔记2的语音
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExample2Btn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2voice[int.Parse(go.name)].Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 角色A出现
    /// </summary>
    private void AppearanceRoleA()
    {
        RoleA.SetActive(true);
        AppearanceRoleB();
    }

    /// <summary>
    /// 角色B出现
    /// </summary>
    private void AppearanceRoleB()
    {
        RoleB.SetActive(true);
        RoleASpeak();
    }

    /// <summary>
    /// 角色A说话
    /// </summary>
    private void RoleASpeak()
    {
        LightA.SetActive(true);
        WordBGA.gameObject.SetActive(true);
        float time = 0;
        if (_curStepIndex == 1)
        {
            time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene1voice[0].Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenBtnA.gameObject, time);
        } else if(_curStepIndex == 2)
        {
            if (_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 1)
            {
                time = 1f;
            } else
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2voice[0].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(ListenBtnA.gameObject, time);
            }
        }
            
        Invoke("RoleBSpeak", time);
    }

    /// <summary>
    /// 角色B说话
    /// </summary>
    private void RoleBSpeak()
    {
        LightA.SetActive(false);
        LightB.SetActive(true);
        WordBGB.gameObject.SetActive(true);
        float time = 0;
        if (_curStepIndex == 1)
        {
            time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene1voice[1].Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenBtnB.gameObject, time);
        }
        else if (_curStepIndex == 2)
        {
            if (_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 2 || _sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == -1)
            {
                time = 1f;
            }
            else
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2voice[1].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(ListenBtnB.gameObject, time);
            }
        }
        Invoke("IsSpeak", time + 0.5f);
    }

    /// <summary>
    /// 是否需要玩家作答
    /// </summary>
    private void IsSpeak()
    {
        if (_curStepIndex == 1)
        {
            SendScore(-1, -1, -1, -1, true, 1);
        }
        else
        {
            SpeakTip();
        }
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        UnityAction unityAction = new UnityAction(OnSpeak);
        if (_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 1)
        {
            _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Sentence2, _sentence2Data.infoList[_curSentenceIndex].scene2text[0]);
        }
        else
        {
            _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Sentence2, _sentence2Data.infoList[_curSentenceIndex].scene2text[1]);
        }
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        float time = 1f;
        if (!_gameUIInstance._isPass)
        {
            if (_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 1)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2voice[0].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(ListenBtn2List[0].gameObject, time);
            } else
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence2Data.infoList[_curSentenceIndex].scene2voice[1].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(ListenBtn2List[1].gameObject, time);
            }
        }

        if (_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 1)
        {
            ContentA.text = _sentence2Data.infoList[_curSentenceIndex].scene2text[0];
        }
        else
        {
            ContentB.text = _sentence2Data.infoList[_curSentenceIndex].scene2text[1];
        }

        SendScore(_gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._isPass, _gameUIInstance.SpeakTimes);
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
            _gameUIInstance.SetProgress(_passNum, _sentenceNum);
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
            index = _curSentenceIndex,
            markingIndex = _curStepIndex,
            markingTotal = 2,
            order = _order
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _sentenceNum,
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
            HideRole();
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    /// <summary>
    /// 隐藏角色
    /// </summary>
    private void HideRole()
    {
        if (_curStepIndex == 2)
        {
            if (_sentence2Data.infoList[_curSentenceIndex].LearnTextIndex == 1)
            {
                ListenBtnA.gameObject.SetActive(true);
                MySpeakBtnA.gameObject.SetActive(true);
            }
            else
            {
                ListenBtnB.gameObject.SetActive(true);
                MySpeakBtnB.gameObject.SetActive(true);
            }
        } else
        {
            LightB.SetActive(false);
            RoleA.SetActive(false);
            RoleB.SetActive(false);
        }

        ShowNote();
    }

    /// <summary>
    /// 显示笔记
    /// </summary>
    private void ShowNote()
    {
        if (_curStepIndex == 1)
        {
            Note1Grid.gameObject.SetActive(true);
        }
        else if (_curStepIndex == 2)
        {
            Note2Grid.gameObject.SetActive(true);
        }
        // 下一步
        UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextStep;
        _gameUIInstance.ShowNextButton();
    }

    /// <summary>
    /// 下一个步骤
    /// </summary>
    private void NextStep(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        
        if (_curStepIndex == 1)
        {
            _curStepIndex = 2;
            _gameUIInstance.HideNextButton();
            HideAllArea();
            ShowTeacherCommand();
        }
        else if (_curStepIndex == 2)
        {
            if (_studyQueue.Count <= 0)
            {
                // 本模块知识学习完毕，则自动弹出报告，无需点击“下一步”
                Invoke("Settlement", 1f);
            }
            else
            {
                _curStepIndex = 1;
                _curSentenceIndex = _studyQueue.Dequeue();
                HideAllArea();
                ShowBeginTitle(_level);
            }
        }
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        MySpeakBtnA.gameObject.SetActive(false);
        MySpeakBtnB.gameObject.SetActive(false);
        LightB.SetActive(false);
        RoleA.SetActive(false);
        RoleB.SetActive(false);
        ScenesArea.SetActive(false);
        NotesArea.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
    }

    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.Sentence2, _passNum, _sentenceNum);
    }
}