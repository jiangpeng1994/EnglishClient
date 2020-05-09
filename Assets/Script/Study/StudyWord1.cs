using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StudyWord1 : MonoBehaviour
{
    // 步骤1
    public GameObject WordArea;
    // 卡片区
    public GameObject WordCard;
    public UITexture WordIcon;
    public UIButton PlayExampleBtn;
    public UIButton ShowTipBtn;
    public UIButton MyVoiceBtn;
    public UILabel WordTranslation;
    public GameObject Right;
    public UITable uiTable;

    // 数据
    private UIWordGame _gameUIInstance;
    private SyncMoudle2Info.request _wordData1;
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
        _wordData1 = DataManager.GetInstance().wordData1;
        _wordNum = _wordData1.contentInfo.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _isPassNum = 0;
        _stepNum = 2;
        _curStepIndex = 1;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
        _gameUIInstance.SetProgress(_passNum, _wordNum);
        _gameUIInstance.SetStepNum(_stepNum);
    }

    /// <summary>
    /// 设置学习队列
    /// </summary>
    /// <param name="StudyQueue"></param>
    public void SetStudyQueue(Queue<int> StudyQueue)
    {
        _studyQueue = StudyQueue;
        _curWordIndex = _studyQueue.Dequeue();
        //_curWordIndex = 2;
    }

    /// <summary>
    /// 一个单词的开始：展示单词编号
    /// </summary>
    public void ShowBeginTitle()
    {
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
        _gameUIInstance.TeacherCommand(_wordData1.cStatements[_curStepIndex - 1], "");
        //_gameUIInstance.ShowBeginTitle(_wordData1.statements[_step - 1]);
        //Invoke("StartStudy", 2.5f);
        StartStudy();
    }

    /// <summary>
    /// 单词学习初始化
    /// </summary>
    public void StartStudy()
    {
        WordCardInit();
        WordArea.SetActive(true);
        Invoke("PlayVoice", 1);
    }

    /// <summary>
    /// 卡片区初始化
    /// </summary>
    private void WordCardInit()
    {
        if (_curStepIndex == 1)
        {
            WordCard.transform.localPosition = new Vector3(0, -50, 0);
            Right.SetActive(false);
            ResourceLoader.Instance.GetTextureResources(WordIcon, "Texture/" + StringConst.WordPath + _assetsPath + _wordData1.contentInfo[_curWordIndex].icon1);
        }
        else
        {
            WordCard.transform.localPosition = new Vector3(-400, -50, 0);
            Right.SetActive(true);
            ResourceLoader.Instance.GetTextureResources(WordIcon, "Texture/" + StringConst.WordPath + _assetsPath + _wordData1.contentInfo[_curWordIndex].icon2);

            int sum = 0;
            GameObject left = Instantiate(Resources.Load<GameObject>("UI/Left"), Right.transform);
            char[] words = _wordData1.contentInfo[_curWordIndex].word.ToCharArray();
            foreach (char word in words)
            {
                GameObject go = new GameObject(word.ToString());
                go.transform.SetParent(Right.transform);
                UITexture tex = go.AddComponent<UITexture>();
                UIWidget widget = go.GetComponent<UIWidget>();

                if (word >= 'a' && word <= 'z')
                {
                    tex.mainTexture = Resources.Load<Texture>("Texture/EnglishWord/" + word);
                } else if (word >= 'A' && word <= 'Z')
                {
                    tex.mainTexture = Resources.Load<Texture>("Texture/EnglishWord/" + word + "1");
                } else
                {
                    if(word == '.')
                    {
                        tex.mainTexture = Resources.Load<Texture>("Texture/EnglishWord/1");
                    } else if (word == '-')
                    {
                        tex.mainTexture = Resources.Load<Texture>("Texture/EnglishWord/2");
                    } else if (word == ' ')
                    {
                        tex.mainTexture = Resources.Load<Texture>("Texture/EnglishWord/3");
                    } else if (word == '\'')
                    {
                        tex.mainTexture = Resources.Load<Texture>("Texture/EnglishWord/4");
                    }
                }
                
                widget.MakePixelPerfect();
                sum = sum + widget.width;
            }

            int width = (832 - sum + words.Length) / 2;
            GameObject right = Instantiate(Resources.Load<GameObject>("UI/Right"), Right.transform);
            right.GetComponent<UIWidget>().width = width;
            left.GetComponent<UIWidget>().width = width;

            uiTable.repositionNow = true;
            uiTable.enabled = true;
        }
        
        WordTranslation.text = _wordData1.contentInfo[_curWordIndex].cword;
        UIEventListener.Get(PlayExampleBtn.gameObject).onClick = OnPlayExampleBtn;
        UIEventListener.Get(MyVoiceBtn.gameObject).onClick = PlayRecordBtn;
        UIEventListener.Get(ShowTipBtn.gameObject).onClick = OnShowTipBtn;
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
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData1.contentInfo[_curWordIndex].voice.Replace(".mp3", "").Replace(" ", ""));
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

    /// <summary>
    /// 播放系统原声
    /// </summary>
    public void PlayVoice()
    {
        Debug.Log("音频："+StringConst.WordPath + _assetsPath + _wordData1.contentInfo[_curWordIndex].voice.Replace(".mp3", "").Replace(" ", ""));
        float time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData1.contentInfo[_curWordIndex].voice.Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(PlayExampleBtn.gameObject, time);
        Invoke("SpeakTip", time);
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        UnityAction unityAction = new UnityAction(OnSpeak);
        _gameUIInstance.ReadyToSpeak(1, unityAction,TeachType.Word1, _wordData1.contentInfo[_curWordIndex].sendWord, _wordData1.contentInfo[_curWordIndex].customSoundmark);
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        float time = 0f;
        if (!_gameUIInstance._isPass)
        {
            // 系统播放原声
            time = AudicoManager.instance.Play("study", StringConst.WordPath + _assetsPath + _wordData1.contentInfo[_curWordIndex].voice.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(PlayExampleBtn.gameObject, time);
        }

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
            markingTotal = 2,
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
            ShowNextbtn();
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    /// <summary>
    /// 显示下一步
    /// </summary>
    public void ShowNextbtn()
    {
        MyVoiceBtn.gameObject.SetActive(true);
        UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextStep;
        _gameUIInstance.ShowNextButton();
    }

    /// <summary>
    /// 下一个步骤
    /// </summary>
    public void NextStep(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        HideAllArea();

        if (_curStepIndex == 1)
        {
            // 下一个阶段
            _curStepIndex = 2;
            ShowTeacherCommand();
        } else if (_curStepIndex == 2)
        {
            if (_studyQueue.Count <= 0)
            {
                // 本模块知识学习完毕，则自动弹出报告，无需点击“下一步”
                Invoke("Settlement", 1f);
            } else
            {
                // 下一个单词
                _curStepIndex = 1;
                _curWordIndex = _studyQueue.Dequeue();
                ShowBeginTitle();
            }
        }
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        MyVoiceBtn.gameObject.SetActive(false);
        WordArea.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
        UITexture[] texs = Right.GetComponentsInChildren<UITexture>();
        int length = texs.Length;
        for (int i = 0; i < length; i++)
        {
            DestroyImmediate(texs[i].gameObject);
        }
    }

    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.Word1, _passNum, _wordNum);
    }
}