using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIWordGame : MonoBehaviour
{
    public static UIWordGame _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        
        InvokeRepeating("ShowRay1", 0, 0.2f);
        InvokeRepeating("ShowRay2", 0, 0.1f);
        InvokeRepeating("Timer", 0, 1);
        PlayReadyEffect();
        PlayStartEffect();
        InitMenu();
    }

    void OnDestroy()
    {
        CallCancelInvoke();
        _instance = null;
    }

    void Update()
    {
        if (_isBeginTitleShow)
        {
            _beginTitle.color = new Color(_beginTitle.color.r, _beginTitle.color.g, _beginTitle.color.b, (_beginTitle.color.a - 0.02f) > 0 ? (_beginTitle.color.a - 0.02f) : 0);
        }

        if (isPlayVoiceEffect)
        {
            if (VoiceBtn != null)
            {
                shengbo.transform.position = VoiceBtn.transform.position;
                shengbo.transform.localScale = VoiceBtn.transform.localScale;
            }
        }

        tipText.transform.localPosition = new Vector3(11,-8,0);
        /*if (SureBackPanel.activeSelf)
        {
            SureBackLabel.gameObject.SetActive(false);
            SureBackLabel.gameObject.SetActive(true);
        }*/
    }
    
    /// <summary>
    /// 多少秒后执行一个方法，repeatRate不为0时可重复
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="time"></param>
    public void CallInvoke(string methodName, float time, float repeatRate = 0f)
    {
        if (repeatRate == 0)
        {
            Invoke(methodName, time);
        }
        else
        {
            InvokeRepeating(methodName, time, repeatRate);
        }

    }

    /// <summary>
    /// 取消定时器
    /// </summary>
    /// <param name="methodName">null时取消全部定时器</param>
    public void CallCancelInvoke(string methodName = null)
    {
        if (methodName == null)
        {
            CancelInvoke();
        }
        else
        {
            CancelInvoke(methodName);
        }
    }

    //========================顶部方法========================
    //进度百分比
    //public UILabel tumb_lable;
    public UILabel _teacherCommand;
    public UISprite _teachTypeIcon;
    public List<GameObject> _stepList;
    public List<GameObject> _stepLightList;
    public UILabel _progress;
    public UILabel _time;

    public UISprite _progressSlider;
    public UILabel _diamondNum;
    public GameObject _cardBtn;

    public Transform _bigRing1;
    public Transform _smallRing;
    public Transform _bigRing2;
    public Transform _ray;
    public UISprite _light;

    public UILabel fenshu;
    public UIPanel shenbo;

    /// <summary>
    /// 导师指令
    /// </summary>
    public float TeacherCommand(string text, string voice)
    {
        _teacherCommand.text = text;
        _teacherCommand.enabled = true;
        _teacherCommand.gameObject.SetActive(true);
        //float time = AudicoManager.instance.Play("music", StringConst.Mp3Teacher + voice.Replace(".mp3", ""));
        return 1;
    }

    /// <summary>
    /// 设置模块类型Icon和钻石数量
    /// </summary>
    public void SetTeachTypeIconAndDiamond(string type)
    {
        _teachTypeIcon.spriteName = type;
        _diamondNum.text = DataManager.GetInstance().roleData.Diamond.ToString();
    }

    /// <summary>
    /// 隐藏阶段
    /// </summary>
    public void HideStpe()
    {
        _stepList[0].transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置阶段数量
    /// </summary>
    public void SetStepNum(int stepNum)
    {
        for (int i = 0; i < 4; i++)
        {
            _stepList[i].SetActive(false);
        }

        for (int i = 0; i < stepNum; i++)
        {
            _stepList[i].SetActive(true);
        }
    }

    /// <summary>
    /// 设置当前阶段
    /// </summary>
    public void SetCurStep(int curStep)
    {
        for (int i = 0; i < 4; i++)
        {
            _stepLightList[i].SetActive(false);
        }

        for (int i = 0; i < curStep; i++)
        {
            _stepLightList[i].SetActive(true);
        }
    }

    /// <summary>
    /// 设置进度
    /// </summary>
    public void SetProgress(int cur, int sum, int tpye = 0)
    {
        if(tpye == 0)
        {
            _progress.text = "已通过:" + cur.ToString("00") + "/" + sum.ToString("00");
        } else
        {
            _progress.text = "已完成:" + cur.ToString("00") + "/" + sum.ToString("00");
        }
    }

    private int time = 0;
    private int sec = 0;
    private int min = 0;
    private int hour = 0;

    public void SetTime(int value)
    {
        time = value;
    }
    /// <summary>
    /// 计时
    /// </summary>
    public void Timer()
    {
        time++;
        hour = time / 3600;
        min = time / 60 - hour * 60;
        sec = time - min * 60 - hour * 3600;
        _time.text = hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00");
    }

    /// <summary>
    /// 隐藏卡片
    /// </summary>
    public void HideCard()
    {
        _cardBtn.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 喷气的特效1
    /// </summary>
    public void ShowRay1()
    {
        if (_bigRing1.localScale.y == 0.8f)
        {
            _bigRing1.localScale = new Vector3(1, 1, 1);
            _smallRing.localScale = new Vector3(1, 0.8f, 1);
            _bigRing2.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            _bigRing1.localScale = new Vector3(1, 0.8f, 1);
            _smallRing.localScale = new Vector3(1, 1, 1);
            _bigRing2.localScale = new Vector3(1, 0.8f, 1);
        }
    }

    private bool ischange = false;
    private bool ischangecolor = false;
    /// <summary>
    /// 喷气的特效2
    /// </summary>
    public void ShowRay2()
    {
        float t = 0.1f;
        if (_light.color.a >= 1f)
        {
            ischangecolor = true;
        }
        else if (_light.color.a <= 0.5f)
        {
            ischangecolor = false;
        }
        if (_ray.localScale.x > 1.6f)
        {
            ischange = true;
        }
        else if (_ray.localScale.x < 0.8f)
        {
            ischange = false;
        }
        if (ischange)
        {
            _ray.localScale = new Vector3(_ray.localScale.x - t, 1, 1);
        }
        else
        {
            _ray.localScale = new Vector3(_ray.localScale.x + t, 1, 1);
        }
        if (ischangecolor)
        {
            _light.color = new Color(_light.color.r, _light.color.g, _light.color.b, _light.color.a - t);
        }
        else
        {
            _light.color = new Color(_light.color.r, _light.color.g, _light.color.b, _light.color.a + t);
        }
    }
    //========================顶部方法========================

    //=======================模块学习入口=======================
    public GameObject MoudleUIRoot;
    private GameObject MoudleUI;

    private StudyVideo _StudyVideo;
    private StudyText _StudyText;
    private StudyWord1 _StudyWord1;
    private StudyWord2 _StudyWord2;
    private StudyWord3 _StudyWord3;
    private StudySentence1 _StudySentence1;
    private StudySentence2 _StudySentence2;
    private StudyDialogue _StudyDialogue;
    private TestWord _TestWord;
    private TestSentence _TestSentence;
    private TestDialogue _TestDialogue;

    // 教学类型Icon的图片名
    private string animationTypeIcon = "icon_donghua";
    private string textTypeIcon = "icon_kewen";
    private string wordTypeIcon = "icon_danci";
    private string sentenceTypeIcon = "icon_jushi";
    private string dialogueTypeIcon = "icon_duihua";

    public Animation StartAnimation;
    public AnimationClip WordGame_start;
    private TeachType StartTeachType;
    private Queue<int> StudyQueue;

    public UITexture SpeakBtnBG1;
    public UITexture SpeakBtnBG2;
    public UITexture SpeakBtnBG3;
    public UITexture MainBG;

    public void StartStudyReady(TeachType teachType, Queue<int> studyQueue)
    {
        string[] SpeakBtnBGs = new string[2]{ "boy","girl" };
        SpeakBtnBG1.mainTexture = Resources.Load<Texture>("Texture/Sex/SpeakBtn_" + SpeakBtnBGs[DataManager.GetInstance().roleData.Sex] + "1");
        SpeakBtnBG2.mainTexture = Resources.Load<Texture>("Texture/Sex/SpeakBtn_" + SpeakBtnBGs[DataManager.GetInstance().roleData.Sex] + "2");
        SpeakBtnBG3.mainTexture = Resources.Load<Texture>("Texture/Sex/SpeakBtn_" + SpeakBtnBGs[DataManager.GetInstance().roleData.Sex] + "2");

        StartTeachType = teachType;
        StudyQueue = studyQueue;
        if (teachType == TeachType.Animation)
        {
            RealStartStudy();
        } else
        {
            StartAnimation.Play("WordGame_start");
            Invoke("RealStartStudy", WordGame_start.length);
        }
    }

    private void RealStartStudy()
    {
        StartStudy(StartTeachType, StudyQueue);
    }

    public void StartStudy(TeachType teachType, Queue<int> studyQueue)
    {
        DataManager data = DataManager.GetInstance();
        switch (teachType)
        {
            case TeachType.Animation:
                SetTeachTypeIconAndDiamond(animationTypeIcon);
                StudyVideo();
                break;
            case TeachType.Text:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/1");
                SetTeachTypeIconAndDiamond(textTypeIcon);
                StartTextStudy(studyQueue);
                break;
            case TeachType.Word1:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/2");
                SetTeachTypeIconAndDiamond(wordTypeIcon);
                StartWord1Study(studyQueue);
                break;
            case TeachType.Sentence1:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/3");
                //wordGameInstance._progressSlider.fillAmount = progress;
                SetTeachTypeIconAndDiamond(sentenceTypeIcon);
                StartSentence1Study(studyQueue);
                break;
            case TeachType.Word2:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/2");
                SetTeachTypeIconAndDiamond(wordTypeIcon);
                StartWord2Study(studyQueue);
                break;
            case TeachType.Word3:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/2");
                SetTeachTypeIconAndDiamond(wordTypeIcon);
                StartWord3Study(studyQueue);
                break;
            case TeachType.Sentence2:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/3");
                SetTeachTypeIconAndDiamond(sentenceTypeIcon);
                StartSentence2Study((int)System.Math.Floor(data.roleData.curMoudleId), studyQueue);
                break;
            case TeachType.Dialogue:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/4");
                SetTeachTypeIconAndDiamond(dialogueTypeIcon);
                StartDialogueStudy((int)System.Math.Floor(data.roleData.curMoudleId), studyQueue);
                break;
            case TeachType.WordTest:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/2");
                SetTeachTypeIconAndDiamond(wordTypeIcon);
                TestWord();
                break;
            case TeachType.SentenceTest:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/3");
                SetTeachTypeIconAndDiamond(sentenceTypeIcon);
                TestSentence();
                break;
            case TeachType.DialogueTest:
                MainBG.mainTexture = Resources.Load<Texture>("Texture/BG/4");
                SetTeachTypeIconAndDiamond(dialogueTypeIcon);
                TestDialogue();
                break;
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void StudyVideo()
    {
        OpenMoudleUI("StudyVideo");
        _StudyVideo = MoudleUI.GetComponent<StudyVideo>();
    }

    /// <summary>
    /// 开始课文学习
    /// </summary>
    public void StartTextStudy(Queue<int> StudyQueue)
    {
        OpenMoudleUI("StudyText");
        _StudyText = MoudleUI.GetComponent<StudyText>();
        _StudyText.SetStudyQueue(StudyQueue);
        _StudyText.ShowBeginTitle();
    }

    /// <summary>
    /// 开始单词1学习
    /// </summary>
    public void StartWord1Study(Queue<int> StudyQueue)
    {
        OpenMoudleUI("StudyWord1");
        _StudyWord1 = MoudleUI.GetComponent<StudyWord1>();
        _StudyWord1.SetStudyQueue(StudyQueue);
        _StudyWord1.ShowBeginTitle();
    }

    /// <summary>
    /// 开始单词2学习
    /// </summary>
    public void StartWord2Study(Queue<int> StudyQueue)
    {
        OpenMoudleUI("StudyWord2");
        _StudyWord2 = MoudleUI.GetComponent<StudyWord2>();
        _StudyWord2.SetStudyQueue(StudyQueue);
        _StudyWord2.ShowBeginTitle();
    }

    /// <summary>
    /// 开始单词3学习
    /// </summary>
    public void StartWord3Study(Queue<int> StudyQueue)
    {
        OpenMoudleUI("StudyWord3");
        _StudyWord3 = MoudleUI.GetComponent<StudyWord3>();
        _StudyWord3.SetStudyQueue(StudyQueue);
        _StudyWord3.ShowBeginTitle();
    }

    /// <summary>
    /// 开始句式1学习
    /// </summary>
    public void StartSentence1Study(Queue<int> StudyQueue)
    {
        OpenMoudleUI("StudySentence1");
        _StudySentence1 = MoudleUI.GetComponent<StudySentence1>();
        _StudySentence1.SetStudyQueue(StudyQueue);
        _StudySentence1.ShowBeginTitle();
    }

    /// <summary>
    /// 开始句式23学习
    /// </summary>
    public void StartSentence2Study(int level, Queue<int> StudyQueue)
    {
        OpenMoudleUI("StudySentence2");
        _StudySentence2 = MoudleUI.GetComponent<StudySentence2>();
        _StudySentence2.SetStudyQueue(StudyQueue);
        _StudySentence2.ShowBeginTitle(level);
    }

    /// <summary>
    /// 开始对话123学习
    /// </summary>
    public void StartDialogueStudy(int level, Queue<int> StudyQueue)
    {
        shenbo.baseClipRegion = new Vector4(0,-49,1538,456);
        OpenMoudleUI("StudyDialogue");
        _StudyDialogue = MoudleUI.GetComponent<StudyDialogue>();
        _StudyDialogue.SetStudyQueue(StudyQueue);
        _StudyDialogue.ShowBeginTitle(level);
    }

    /// <summary>
    /// 开始单词测试
    /// </summary>
    public void TestWord()
    {
        OpenMoudleUI("TestWord");
        _TestWord = MoudleUI.GetComponent<TestWord>();
        _TestWord.SetStudyQueue(StudyQueue);
        _TestWord.ShowBeginTitle();
    }

    /// <summary>
    /// 开始句式测试
    /// </summary>
    public void TestSentence()
    {
        OpenMoudleUI("TestSentence");
        _TestSentence = MoudleUI.GetComponent<TestSentence>();
        _TestSentence.SetStudyQueue(StudyQueue);
        _TestSentence.ShowBeginTitle();
    }

    /// <summary>
    /// 开始对话测试
    /// </summary>
    public void TestDialogue()
    {
        shenbo.baseClipRegion = new Vector4(0, -49, 1538, 456);
        OpenMoudleUI("TestDialogue");
        _TestDialogue = MoudleUI.GetComponent<TestDialogue>();
        _TestDialogue.SetStudyQueue(StudyQueue);
        _TestDialogue.ShowBeginTitle(3);
    }

    /// <summary>
    /// 打开模块UI
    /// </summary>
    public void OpenMoudleUI(string moudleUIName)
    {
        MoudleUI = Instantiate(Resources.Load("UI/" + moudleUIName)) as GameObject;
        MoudleUI.transform.SetParent(MoudleUIRoot.transform);
        MoudleUI.transform.localPosition = Vector3.zero;
        MoudleUI.transform.localScale = Vector3.one;
        MoudleUI.name = moudleUIName;
    }
    //=======================模块学习入口======================

    //========================中间方法========================
    public UILabel _beginTitle;
    private Color _beginTitleColor = new Color(255, 165, 0); //橙色
    private bool _isBeginTitleShow = false;

    /// <summary>
    /// 显示开场标题
    /// </summary>
    public void ShowBeginTitle(string text, float showTime = 2.5f)
    {
        _beginTitle.text = text;
        _beginTitle.transform.localPosition = Vector3.zero;
        _beginTitle.transform.GetComponent<UILabel>().color = _beginTitleColor;
        _beginTitle.gameObject.SetActive(true);
        CallInvoke("HideBeginTitleStep1", showTime - 1.5f);
    }

    /// <summary>
    /// 消失开场标题第一步
    /// </summary>
    public void HideBeginTitleStep1()
    {
        _beginTitle.transform.DOLocalMoveY(-60, 1.5f);
        _isBeginTitleShow = true;
        CallInvoke("HideBeginTitleStep2", 1.5f);
    }

    /// <summary>
    /// 消失开场标题第二步
    /// </summary>
    public void HideBeginTitleStep2()
    {
        _beginTitle.gameObject.SetActive(false);
        _isBeginTitleShow = false;
    }
    //========================中间方法========================

    //========================底部方法========================
    public UIButton _speakBtn;
    public GameObject _speakEffect;
    public GameObject _speakEffectBG;
    public GameObject _speakContent;
    public UITexture _voiceEffect;
    public List<Texture> _voiceEffects;
    private int _voiceEffectIndex = 0;
    public UILabel _showText;
    public UIButton _nextBtn;
    public int SpeakTimes= 1;
    private UnityAction _unityAction;
    public TeachType _teachType;
    public string _content;
    public string _vocabulary;

    /// <summary>
    /// 准备说话
    /// </summary>
    public void ReadyToSpeak(int times, UnityAction unityAction, TeachType teachType, string content, string vocabulary = null)
    {
        SpeakTimes = times;
        if (unityAction != null)
        {
            _unityAction = unityAction;
        }
        _teachType = teachType;
        _content = content;
        if (vocabulary == "")
        {
            vocabulary = null;
        }
        _vocabulary = vocabulary;

        UIEventListener.Get(_speakBtn.gameObject).onClick = StartToSpeak;
        _speakBtn.gameObject.SetActive(true);
        _speakEffect.SetActive(true);
        _speakContent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 取消说话
    /// </summary>
    public void CancelSpeak()
    {
        _speakBtn.gameObject.SetActive(false);
        _speakEffect.SetActive(false);
    }

    /// <summary>
    /// 播放准备动画
    /// </summary>
    private void PlayReadyEffect()
    {
        _speakEffectBG.SetActive(!_speakEffectBG.activeSelf);
        Invoke("PlayReadyEffect", 0.3f);
    }

    /// <summary>
    /// 开始说话
    /// </summary>
    private void StartToSpeak(GameObject go)
    {
        //AudicoManager.instance.Play("effect", "Effect/press button");
        StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        StringConst.isSpeak = true;
        HideNextButton();
        _speakBtn.gameObject.SetActive(false);
        _speakEffect.SetActive(false);
        _speakContent.SetActive(true);
        _voiceEffect.gameObject.SetActive(true);
        _showText.gameObject.SetActive(false);

        // 调用科大讯飞
        string tpye = "";
        switch (_teachType)
        {
            case TeachType.Text:
            case TeachType.Dialogue:
                tpye = "read_sentence";
                break;
            case TeachType.Word1:
            case TeachType.Word2:
            case TeachType.Word3:
                tpye = "read_word";
                break;
            case TeachType.Sentence1:
            case TeachType.Sentence2:
                tpye = "read_sentence";
                break;
        }

        if (DataManager.GetInstance().roleData.IsSpeak)
        {
            SDKHandle._instance.StartSpeechEvaluator(tpye, _content, _vocabulary);
        } else
        {
            Invoke("TestVoice", 0.2f);
        }
    }

    private void TestVoice()
    {
        Debug.Log("评测内容：" + _content);
        SpeakEnd(Random.Range(60,90));
    }

    /// <summary>
    /// 播放开始动画
    /// </summary>
    private void PlayStartEffect()
    {
        _voiceEffect.mainTexture = _voiceEffects[_voiceEffectIndex];
        _voiceEffectIndex++;
        if (_voiceEffectIndex >= _voiceEffects.Count)
        {
            _voiceEffectIndex = 0;
        }

        Invoke("PlayStartEffect", 0.3f);
    }

    private int addDiamondNum = 0;
    public int _score = 0;
    public int _fluency = 0;
    public int _nicety = 0;
    public int _integrity = 0;
    public bool _isPass = false;
    /// <summary>
    /// 说话结束
    /// </summary>
    public void SpeakEnd(float score)
    {
        StringConst.isSpeak = false;
        string level = DataManager.instance.roleData.curLevel;
        if (level != "4")
        {
            if (score < 70)
            {
                GameTools.Instance.TipsShow("failed");
            } else
            {
                GameTools.Instance.TipsShow1("得分:" + score);
            }
        }
        
        _score = (int)score;
        _fluency = Random.Range(_score - 10, _score + 5);
        _nicety = Random.Range(_score - 10, _score + 5);
        _integrity = Random.Range(_score - 10, _score + 5);
        if (_fluency > 100)
        {
            _fluency = 100;
        }
        if (_nicety > 100)
        {
            _nicety = 100;
        }
        if (_integrity > 100)
        {
            _integrity = 100;
        }

        _voiceEffect.gameObject.SetActive(false);
        int num = 79;
        if (level == "1")
        {
            num = 79;
        } else if (level == "2")
        {
            num = 84;
        } else if (level == "3")
        {
            num = 89;
        }

        if (score < 70)
        {
            // 未通过
            _isPass = false;

            if (level != "4")
            {
                _showText.text = GameTools.Instance.SpeakColorText(_content, 3);
                _showText.gameObject.SetActive(true);
                AudicoManager.instance.Play("effect", "Effect/voice error");
                // todo：播放玩家录音
                if (SpeakTimes == 1)
                {
                    Invoke("PlaySpeakTryAgain", 1f);
                }
                else
                {
                    Invoke("PlayLetMeHelpYou", 1f);
                }
            } else
            {
                // 测试模式下不判断对错
                _showText.text = GameTools.Instance.SpeakColorText(_content, 5);
                _showText.gameObject.SetActive(true);
                PlaybackRecord(false);
            }
        }
        else
        {
            _isPass = true;

            if (level != "4")
            {
                AudicoManager.instance.Play("click", "Effect/get the crystal");
                if (score >= 70 && score <= num)
                {
                    // 二等通过
                    _showText.text = GameTools.Instance.SpeakColorText(_content, 2);
                }
                else
                {
                    // 一等通过
                    _showText.text = GameTools.Instance.SpeakColorText(_content, 1);
                }
                _showText.gameObject.SetActive(true);
                PlaybackRecord(true);
            } else
            {
                // 测试模式下不判断对错
                _showText.text = GameTools.Instance.SpeakColorText(_content, 5);
                _showText.gameObject.SetActive(true);
                PlaybackRecord(false);
            }
        }
        Debug.LogWarning("是否通过" + _isPass);
    }

    /// <summary>
    /// 正确回放
    /// </summary>
    private void PlaybackRecord(bool isPlay)
    {
        HideSpeakContent();
        if (isPlay)
        {
            // todo：播放玩家录音
        }
        _unityAction?.Invoke();
    }

    public Animation AddDiamondAnimation;
    public AnimationClip AddDiamond;
    public GameObject AddDiamondObj;
    /// <summary>
    /// 其他类调用的加宝石
    /// </summary>
    /// <param name="num"></param>
    public void AddDiamondByOther(int num)
    {
        AddDiamondObj.SetActive(true);
        addDiamondNum = num;
        fenshu.text = "+" + num;
        //fenshu.gameObject.SetActive(true);
        //AudicoManager.instance.Play("click", "Effect/get the crystal");
        //fenshu.transform.DOLocalMove(new Vector3(474, 320, 0), 1.5f);
        AddDiamondAnimation.Play("AddDiamond");
        Invoke("AddDiamondByOtherEnd", AddDiamond.length);
    }

    /// <summary>
    /// 其他类调用的加宝石结束
    /// </summary>
    private void AddDiamondByOtherEnd()
    {
        //fenshu.gameObject.SetActive(false);
        //fenshu.transform.localPosition = new Vector3(0, 0, 0);
        DataManager.GetInstance().roleData.Diamond = DataManager.GetInstance().roleData.Diamond + addDiamondNum;
        _diamondNum.text = DataManager.GetInstance().roleData.Diamond.ToString();
    }

    /// <summary>
    /// 播放再来读一次语音
    /// </summary>
    private void PlaySpeakTryAgain()
    {
        float time = AudicoManager.instance.Play("study", StringConst.Mp3Teacher + "Try to speak");
        Invoke("SpeakTryAgain", time);
    }

    /// <summary>
    /// 再来读一次
    /// </summary>
    private void SpeakTryAgain()
    {
        HideSpeakContent();
        ReadyToSpeak(2,null,_teachType,_content);
    }

    /// <summary>
    /// 隐藏语音识别框
    /// </summary>
    private void HideSpeakContent()
    {
        _showText.gameObject.SetActive(false);
        _speakContent.SetActive(false);
    }

    /// <summary>
    /// 播放我来帮助你
    /// </summary>
    private void PlayLetMeHelpYou()
    {
        //float time = AudicoManager.instance.Play("study", StringConst.Mp3Teacher + "Follow to read");
        //Invoke("PlayOriginalSound", time);
        PlayOriginalSound();
    }

    /// <summary>
    /// 播放原声
    /// </summary>
    private void PlayOriginalSound()
    {
        HideSpeakContent();
        _unityAction?.Invoke();
    }

    /// <summary>
    /// 显示下一步按钮
    /// </summary>
    public void ShowNextButton()
    {
        _nextBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏下一步按钮
    /// </summary>
    public void HideNextButton()
    {
        _nextBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获得下一步按钮
    /// </summary>
    /// <returns></returns>
    public GameObject GetNextButton()
    {
        return _nextBtn.gameObject;
    }

    float falseCount = 0;
    float accuracy = 0;
    float TrueCount = 0;
    int star = 0;
    TeachType TeachType = 0;
    string CardName = "";
    GetModuleFourInfo.response resultData;
    /// <summary>
    /// 显示结算
    /// </summary>
    /// <param name="teachType"></param>
    public void ShowResultPanel(TeachType teachType, float trueCount, float allCount)
    {
        CancelInvoke("Timer");
        TeachType = teachType;
        TrueCount = trueCount;
        falseCount = allCount - trueCount;
        accuracy = (trueCount / allCount) * 100;
        if (accuracy < 60)
        {
            star = 0;
        } else if (accuracy >= 60 && accuracy < 80)
        {
            star = 1;
        }
        else if (accuracy >= 80 && accuracy < 100)
        {
            star = 2;
        } else
        {
            star = 3;
        }

        Debug.Log("请求：结算数据");
        // 获得4维成绩
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = DataManager.GetInstance().roleData.curMoudleId
        };
        GetModuleFourInfo.request msg = new GetModuleFourInfo.request
        {
            moudleBase = moudle,
        };
        NetSender.Send<ProtoProtocol.GetModuleFourInfo>(msg, OnShowResultPanel);

        // 记录学习时间
        SaveCostTime();
    }

    /// <summary>
    /// 收到结算数据,请求奖励
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnShowResultPanel(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：结算数据");
        resultData = (GetModuleFourInfo.response)rpcRsp;

        if (star != 0)
        {
            Debug.Log("请求：卡片奖励");
            CardName = "";
            moudle_base moudle = new moudle_base
            {
                grade = DataManager.GetInstance().roleData.curGrade,
                term = DataManager.GetInstance().roleData.curTerm,
                unit = DataManager.GetInstance().roleData.curUnit,
                moudleId = DataManager.GetInstance().roleData.curMoudleId
            };
            GetMoudleReward.request msg = new GetMoudleReward.request
            {
                moudleBase = moudle,
                star = star
            };
            NetSender.Send<ProtoProtocol.GetMoudleReward>(msg, OnGetMoudleReward);
        } else
        {
            CardName = "";
            OpenResultPanel();
        }
    }

    public Animation GetCardAnimation;
    public AnimationClip Card;
    /// <summary>
    /// 收到卡片奖励
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnGetMoudleReward(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：卡片奖励");
        var data = (GetMoudleReward.response)rpcRsp;
        if (data.status)
        {
            CardName = data.rewardInfo.itemIcon.Replace(".jpg", "").Replace(".png", "");
        }
        else
        {
            Debug.Log("无奖励");
            CardName = "";
        }

        if (CardName == "")
        {
            GetCardAnimation.Play("Card");
            Invoke("OpenResultPanel", Card.length);
        } else
        {
            OpenResultPanel();
        }
    }

    private void OpenResultPanel()
    {
        Transform _transform = (Instantiate(Resources.Load("UI/UIResultPanel")) as GameObject).transform;
        UIResultPanel _result = _transform.GetComponent<UIResultPanel>();
        _transform.parent = GameObject.Find("GUI/GUICamera").transform;
        _transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;
        _result.ShowResult(TeachType, star, TrueCount.ToString(), falseCount.ToString(), accuracy.ToString() + "%", _time.text, resultData.info.nicety.ToString() + "%", resultData.info.integrity.ToString() + "%", resultData.info.fluency.ToString() + "%", CardName);
    }
    //========================底部方法========================

    //========================菜单方法========================
    public GameObject Menu;
    public UIButton OpenMenu;
    public UIButton CloseMenu;
    public UIButton DataBtn;
    public UIButton SoundBtn;
    public UIButton BackBtn;
    public UITexture DataSprite;
    public UITexture SoundSprite;
    public UITexture BackSprite;
    public Texture[] DataSprites;
    public Texture[] SoundSprites;
    public Texture[] BackSprites;
    public GameObject DataPanel;
    public GameObject SoundPanel;
    public GameObject BackPanel;
    public GameObject SureBackPanel;
    public UILabel SureBackLabel;
    public UIButton SureBackBtn;
    public UIButton NoBackBtn;
    //--返回--//
    public UIButton MoudleBtn;
    public UIButton HomeBtn;
    public UIButton QuitBtn;
    //--音量--//
    public UISlider MusicVoiceSlider;
    public UISlider EffectVoiceSlider;
    public UISlider StudyVoiceSlider;
    public UIButton ResetBtn;
    private int FirstChangeVoice = 3;
    //--数据--//
    public UIButton StudyDataBtn;
    public UIButton CapabilityAnalysisBtn;
    public UITexture StudyDataBtnSprite;
    public UITexture CapabilityAnalysisBtnSprite;
    public Texture[] StudyDataBtnSprites;
    public Texture[] CapabilityAnalysisBtnSprites;
    public GameObject StudyDataPanel;
    public GameObject CapabilityAnalysisPanel;
    //--学习数据--//
    public UIButton CurMoudleBtn;
    public UIButton TotalMoudleBtn;
    public UITexture CurMoudleBtnSprite;
    public UITexture TotalMoudleBtnSprite;
    public Texture[] StudyDataSprites;
    public GameObject CurMoudlePanel;
    public GameObject TotalMoudleBtnPanel;
    //--能力分析--//
    public UIButton TodayAnalysisBtn;
    public UIButton Today7AnalysisBtn;
    public UITexture TodayAnalysisBtnSprite;
    public UITexture TToday7AnalysisBtnSprite;

    /// <summary>
    /// 初始化菜单
    /// </summary>
    public void InitMenu()
    {
        UIEventListener.Get(OpenMenu.gameObject).onClick = OnOpenMenu;
        UIEventListener.Get(CloseMenu.gameObject).onClick = OnCloseMenu;
        UIEventListener.Get(DataBtn.gameObject).onClick = OnDataBtn;
        UIEventListener.Get(SoundBtn.gameObject).onClick = OnSoundBtn;
        UIEventListener.Get(BackBtn.gameObject).onClick = OnBackBtn;

        UIEventListener.Get(StudyDataBtn.gameObject).onClick = OnClickStudyDataBtn;
        UIEventListener.Get(CapabilityAnalysisBtn.gameObject).onClick = OnClickCapabilityAnalysisBtn;
        UIEventListener.Get(CurMoudleBtn.gameObject).onClick = OnClickCurMoudleBtn;
        UIEventListener.Get(TotalMoudleBtn.gameObject).onClick = OnClickTotalMoudleBtn;
        UIEventListener.Get(TodayAnalysisBtn.gameObject).onClick = OnClickTodayAnalysisBtn;
        UIEventListener.Get(Today7AnalysisBtn.gameObject).onClick = OnClickToday7AnalysisBtn;

        UIEventListener.Get(ResetBtn.gameObject).onClick = OnClickResetBtn;
        UIEventListener.Get(MoudleBtn.gameObject).onClick = OnClickMoudleBtn;
        UIEventListener.Get(HomeBtn.gameObject).onClick = OnClickHomeBtn;
        UIEventListener.Get(QuitBtn.gameObject).onClick = OnClickQuitBtn;
        UIEventListener.Get(SureBackBtn.gameObject).onClick = OnClickSureBackBtn;
        UIEventListener.Get(NoBackBtn.gameObject).onClick = OnClickNoBackBtn;
    }

    /// <summary>
    /// 打开菜单
    /// </summary>
    /// <param name="go"></param>
    private void OnOpenMenu(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        OnDataBtn(null);
        CancelInvoke("Timer");
        StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        StringConst.CanPlayStudyVioce = false;
        SureBackPanel.SetActive(false);
        Menu.SetActive(true);
    }

    /// <summary>
    /// 关闭菜单
    /// </summary>
    /// <param name="go"></param>
    private void OnCloseMenu(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        StringConst.CanPlayStudyVioce = true;
        InvokeRepeating("Timer", 0, 1);
        Menu.SetActive(false);
    }

    /// <summary>
    /// 数据页签
    /// </summary>
    /// <param name="go"></param>
    private void OnDataBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DataSprite.mainTexture = DataSprites[1];
        SoundSprite.mainTexture = SoundSprites[0];
        BackSprite.mainTexture = BackSprites[0];

        OnClickStudyDataBtn(null);
        DataPanel.SetActive(true);
        SoundPanel.SetActive(false);
        BackPanel.SetActive(false);
    }

    /// <summary>
    /// 声音页签
    /// </summary>
    /// <param name="go"></param>
    private void OnSoundBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DataSprite.mainTexture = DataSprites[0];
        SoundSprite.mainTexture = SoundSprites[1];
        BackSprite.mainTexture = BackSprites[0];

        DataPanel.SetActive(false);
        InitSoundPanel();
        SoundPanel.SetActive(true);
        BackPanel.SetActive(false);
    }

    /// <summary>
    /// 返回页签
    /// </summary>
    /// <param name="go"></param>
    private void OnBackBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DataSprite.mainTexture = DataSprites[0];
        SoundSprite.mainTexture = SoundSprites[0];
        BackSprite.mainTexture = BackSprites[1];

        DataPanel.SetActive(false);
        SoundPanel.SetActive(false);
        BackPanel.SetActive(true);
    }
    //========================数据========================
    /// <summary>
    /// 学习数据页签
    /// </summary>
    /// <param name="go"></param>
    private void OnClickStudyDataBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        StudyDataBtnSprite.mainTexture = StudyDataBtnSprites[1];
        CapabilityAnalysisBtnSprite.mainTexture = CapabilityAnalysisBtnSprites[0];

        OnClickCurMoudleBtn(null);
        StudyDataPanel.SetActive(true);
        CapabilityAnalysisPanel.SetActive(false);
    }

    /// <summary>
    /// 能力分析页签
    /// </summary>
    /// <param name="go"></param>
    private void OnClickCapabilityAnalysisBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        StudyDataBtnSprite.mainTexture = StudyDataBtnSprites[0];
        CapabilityAnalysisBtnSprite.mainTexture = CapabilityAnalysisBtnSprites[1];

        OnClickTodayAnalysisBtn(null);
        StudyDataPanel.SetActive(false);
        CapabilityAnalysisPanel.SetActive(true);
    }

    //========================学习数据========================
    public UIWidget ChartAll;
    public UILabel LabelAll;
    public UIWidget ChartComplete;
    public UILabel LabelComplete;
    public UIWidget ChartPass;
    public UILabel LabelPass;
    public UILabel WaitForPassLabel;
    public UILabel CostTimeLabel;
    public UILabel TimeLabel;

    public UILabel TotalTimeLabel;
    public UILabel TextNum;
    public UILabel WordNum;
    public UILabel SentenceNum;
    public UILabel DialogueNum;

    public UILabel RecordTimeLabel;
    public UILabel fluency;
    public UILabel nicety;
    public UILabel integrity;
    public UILabel score;
    /// <summary>
    /// 当前模块数据页签
    /// </summary>
    /// <param name="go"></param>
    private void OnClickCurMoudleBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        CurMoudleBtnSprite.mainTexture = StudyDataSprites[1];
        TotalMoudleBtnSprite.mainTexture = StudyDataSprites[0];

        CostTimeLabel.text = _time.text;

        int totalNum = DataManager.GetInstance().curStudyProgress.totalNum;
        int completeNum = DataManager.GetInstance().curStudyProgress.completeNum;
        int passNum = DataManager.GetInstance().curStudyProgress.passNum;

        float perHeight = 120 / totalNum;

        ChartAll.height = 120;
        LabelAll.transform.localPosition = new Vector3(LabelAll.transform.localPosition.x, ChartAll.height + 20, 0);
        LabelAll.text = totalNum.ToString();

        if (completeNum == 0)
        {
            ChartComplete.transform.localScale = new Vector3(1, 0, 1);
            LabelComplete.transform.localPosition = new Vector3(LabelComplete.transform.localPosition.x, 20, 0);
        }
        else if (completeNum == totalNum)
        {
            ChartComplete.transform.localScale = new Vector3(1, 1, 1);
            ChartComplete.height = 120;
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
            ChartPass.height = 120;
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

        TeachType teachType = TeachType.Text;
        switch (teachType)
        {
            case TeachType.Text:
                TimeConversion(15 * WaitForPassNum);
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
                TimeConversion(60 * WaitForPassNum);
                break;
            case TeachType.Dialogue:
            case TeachType.DialogueTest:
                TimeConversion(90 * WaitForPassNum);
                break;
        }
        TimeLabel.text = hour1.ToString("00") + ":" + min1.ToString("00") + ":" + sec1.ToString("00");

        CurMoudlePanel.SetActive(true);
        TotalMoudleBtnPanel.SetActive(false);
    }

    /// <summary>
    /// 所有模块数据页签
    /// </summary>
    /// <param name="go"></param>
    private void OnClickTotalMoudleBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        CurMoudleBtnSprite.mainTexture = StudyDataSprites[0];
        TotalMoudleBtnSprite.mainTexture = StudyDataSprites[1];
        CurMoudlePanel.SetActive(false);
        GetTotalMoudleData();
    }

    /// <summary>
    /// 请求累积通过数据
    /// </summary>
    /// <param name="day"></param>
    private void GetTotalMoudleData()
    {
        Debug.Log("请求：累积通过数据");
        NetSender.Send<ProtoProtocol.GetHistoryAccumulate>(null, OnGetTotalMoudleData);
    }

    /// <summary>
    /// 获得累积通过数据
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnGetTotalMoudleData(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：累积通过数据");
        var resultData = (GetHistoryAccumulate.response)rpcRsp;
        if (resultData.historyList.Count > 0)
        {
            TextNum.text = resultData.historyList[0].ToString();
            WordNum.text = resultData.historyList[1].ToString();
            SentenceNum.text = resultData.historyList[2].ToString();
            DialogueNum.text = resultData.historyList[3].ToString();
            if (resultData.historyList[4] == 0)
            {
                TotalTimeLabel.text = TimeConversion1(time);
            } else
            {
                TotalTimeLabel.text = TimeConversion1((int)resultData.historyList[4]);
            }
        } else
        {
            TextNum.text = "0";
            WordNum.text = "0";
            SentenceNum.text = "0";
            DialogueNum.text = "0";
            TotalTimeLabel.text = TimeConversion1(time);
        }

        TotalMoudleBtnPanel.SetActive(true);
    }

    //========================能力分析========================
    /// <summary>
    /// 今日能力分析页签
    /// </summary>
    /// <param name="go"></param>
    private void OnClickTodayAnalysisBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        TodayAnalysisBtnSprite.mainTexture = StudyDataSprites[1];
        TToday7AnalysisBtnSprite.mainTexture = StudyDataSprites[0];
        GetSorceByDay(1);
    }

    /// <summary>
    /// 7日能力分析页签
    /// </summary>
    /// <param name="go"></param>
    private void OnClickToday7AnalysisBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        TodayAnalysisBtnSprite.mainTexture = StudyDataSprites[0];
        TToday7AnalysisBtnSprite.mainTexture = StudyDataSprites[1];
        GetSorceByDay(7);
    }

    /// <summary>
    /// 请求4维成绩(时间)
    /// </summary>
    /// <param name="day"></param>
    private void GetSorceByDay(int day)
    {
        // 获得4维成绩
        Debug.Log("请求：4维成绩(时间)");
        GetTimeScoreInfo.request msg = new GetTimeScoreInfo.request
        {
            day = day,
        };
        NetSender.Send<ProtoProtocol.GetTimeScoreInfo>(msg, OnGetSorceByDay);
    }

    /// <summary>
    /// 收到4维成绩(时间)
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnGetSorceByDay(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：4维成绩(时间)");
        var resultData = (GetTimeScoreInfo.response)rpcRsp;
        fluency.text = resultData.info.fluency.ToString();
        nicety.text = resultData.info.nicety.ToString();
        integrity.text = resultData.info.integrity.ToString();
        score.text = resultData.info.score.ToString();
}

    //========================音量========================
    /// <summary>
    /// 初始化音量设置
    /// </summary>
    private void InitSoundPanel()
    {
        float musicVoice = GameDataManager.GetFloat("MusicVoice");
        if (musicVoice == -1)
        {
            musicVoice = StringConst.DefultVoice;
            GameDataManager.SetFloat("MusicVoice", musicVoice);
        }
        float effectVoice = GameDataManager.GetFloat("EffectVoice");
        if (effectVoice == -1)
        {
            effectVoice = StringConst.DefultVoice;
            GameDataManager.SetFloat("EffectVoice", effectVoice);
        }
        float studyVoice = GameDataManager.GetFloat("StudyVoice");
        if (studyVoice == -1)
        {
            studyVoice = StringConst.DefultVoice;
            GameDataManager.SetFloat("StudyVoice", studyVoice);
        }

        MusicVoiceSlider.value = musicVoice;
        EffectVoiceSlider.value = effectVoice;
        StudyVoiceSlider.value = studyVoice;

        AudicoManager.instance.ModifyBGMusicVolume(musicVoice);
        AudicoManager.instance.ModifyEffectVolume(effectVoice);
        AudicoManager.instance.ModifyStudyVolume(studyVoice);
    }

    /// <summary>
    /// 点击重置音效
    /// </summary>
    /// <param name="go"></param>
    private void OnClickResetBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        GameDataManager.SetFloat("MusicVoice", StringConst.DefultVoice);
        GameDataManager.SetFloat("EffectVoice", StringConst.DefultVoice);
        GameDataManager.SetFloat("StudyVoice", StringConst.DefultVoice);

        MusicVoiceSlider.value = StringConst.DefultVoice;
        EffectVoiceSlider.value = StringConst.DefultVoice;
        StudyVoiceSlider.value = StringConst.DefultVoice;

        AudicoManager.instance.ModifyAllVolume(StringConst.DefultVoice);
    }

    /// <summary>
    /// 滑动音乐音量条
    /// </summary>
    public void OnMusicVoiceSliderChange()
    {
        if (FirstChangeVoice <= 0)
        {
            AudicoManager.instance.Play("effect", "Effect/adjust volume");
        } else
        {
            FirstChangeVoice--;
        }
        
        GameDataManager.SetFloat("MusicVoice", MusicVoiceSlider.value);
        AudicoManager.instance.ModifyBGMusicVolume(MusicVoiceSlider.value);
    }

    /// <summary>
    /// 滑动音效音量条
    /// </summary>
    public void OnEffectVoiceSliderChange()
    {
        if (FirstChangeVoice <= 0)
        {
            AudicoManager.instance.Play("effect", "Effect/adjust volume");
        }
        else
        {
            FirstChangeVoice--;
        }
        GameDataManager.SetFloat("EffectVoice", EffectVoiceSlider.value);
        AudicoManager.instance.ModifyEffectVolume(EffectVoiceSlider.value);
    }

    /// <summary>
    /// 滑动教学音量条
    /// </summary>
    public void OnStudyVoiceSliderChange()
    {
        if (FirstChangeVoice <= 0)
        {
            AudicoManager.instance.Play("effect", "Effect/adjust volume");
        }
        else
        {
            FirstChangeVoice--;
        }
        GameDataManager.SetFloat("StudyVoice", StudyVoiceSlider.value);
        AudicoManager.instance.ModifyStudyVolume(StudyVoiceSlider.value);
    }

    //========================返回========================
    private int BackType = 0;
    /// <summary>
    /// 点击返回模块选择
    /// </summary>
    /// <param name="go"></param>
    private void OnClickMoudleBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BackType = 0;
        SureBackPanel.SetActive(true);
    }
    
    /// <summary>
    /// 点击返回主页
    /// </summary>
    /// <param name="go"></param>
    private void OnClickHomeBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BackType = 1;
        //Main.instance.RoleOffline(null);
        SureBackPanel.SetActive(true);
        //ShowResultPanel(TeachType.Word3, 22, 22);
    }

    /// <summary>
    /// 点击退出游戏
    /// </summary>
    /// <param name="go"></param>
    private void OnClickQuitBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BackType = 2;
        SureBackPanel.SetActive(true);
    }

    /// <summary>
    /// 确认退出
    /// </summary>
    /// <param name="go"></param>
    private void OnClickSureBackBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        StringConst.CanPlayStudyVioce = true;
        SaveCostTime();
        /*if (DataManager.instance.roleData.curLevel == "4")
        {
            ReSetTest();
        }*/
        if (BackType == 2)
        {
            QuitGame();
        } else if (BackType == 1)
        {
            BackHome();
        } else
        {
            BackMoudle();
        }
    }

    /// <summary>
    /// 取消退出
    /// </summary>
    /// <param name="go"></param>
    private void OnClickNoBackBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        SureBackPanel.SetActive(false);
    }

    /// <summary>
    /// 记录学习时间
    /// </summary>
    private void SaveCostTime()
    {
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = DataManager.GetInstance().roleData.curMoudleId
        };
        SaveMoudleCostTime.request msg = new SaveMoudleCostTime.request
        {
            moudleBase = moudle,
            time = time
        };
        NetSender.Send<ProtoProtocol.SaveMoudleCostTime>(msg, null);
    }

    /// <summary>
    /// 返回模块选择
    /// </summary>
    private void BackMoudle()
    {
        AudicoManager.instance.Play("music", "Music/choose screen");
        WindowManager.instance.Delete<UIWordGamePanel>();
    }

    /// <summary>
    /// 返回主页
    /// </summary>
    private void BackHome()
    {
        AudicoManager.instance.Play("music", "Music/hall screen");
        Destroy(SelectCoursePanel.instance.gameObject);
        WindowManager.instance.Delete<UIWordGamePanel>();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void QuitGame()
    {
        Application.Quit();
    }
    //========================菜单方法========================
    //========================时间方法========================
    private int sec1 = 0;
    private int min1 = 0;
    private int hour1 = 0;
    /// <summary>
    /// 时间转换
    /// </summary>
    public void TimeConversion(int time)
    {
        hour1 = time / 3600;
        min1 = time / 60 - hour1 * 60;
        sec1 = time - min1 * 60 - hour1 * 3600;
    }

    public string TimeConversion1(int time)
    {
        hour1 = time / 3600;
        min1 = time / 60 - hour1 * 60;
        sec1 = time - min1 * 60 - hour1 * 3600;
        string str = hour1.ToString("00") + "小时" + min1.ToString("00") + "分钟" + sec1.ToString("00") + "秒";
        return str;
    }

    /// <summary>
    /// 中途退出，重置测试数据
    /// </summary>
    /// <param name="go"></param>
    private void ReSetTest()
    {
        Debug.Log("请求：重置测试数据");
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
        Debug.Log("收到：重置测试数据");
        var data = (ClearMoudleInfo.response)rpcRsp;
        if (data.status)
        {
            Debug.Log("重置测试数据成功");
            DataManager.GetInstance().curStudyProgress.completeNum = 0;
            DataManager.GetInstance().curStudyProgress.passNum = 0;
        }
        else
        {
            //GameTools.Instance.TipsShow("开始复习失败，请重试");
        }
    }

    public GameObject shengbo;
    public GameObject VoiceBtn;
    public bool isPlayVoiceEffect = false;
    public void SetVoiceEffect(GameObject go, float audioClipLength)
    {
        if (StringConst.CanPlayStudyVioce && !StringConst.isSpeak)
        {
            VoiceBtn = go;
            isPlayVoiceEffect = true;
            shengbo.SetActive(false);
            shengbo.transform.position = go.transform.position;
            shengbo.transform.localScale = go.transform.localScale;
            shengbo.SetActive(true);

            CancelInvoke("StopVoiceEffect");
            Invoke("StopVoiceEffect", audioClipLength);
        } else
        {
            StopVoiceEffect();
        }
    }

    public void StopVoiceEffect()
    {
        VoiceBtn = null;
        isPlayVoiceEffect = false;
        shengbo.SetActive(false);
    }

    public void StopAddDiamond()
    {
        AddDiamondObj.SetActive(false);
        AddDiamondAnimation.Stop("AddDiamond");
    }

    public UILabel tipText;
}