using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StudySentence1 : MonoBehaviour
{
    // 提示区
    public GameObject TipsArea;
    public UILabel TipLabel;

    // 步骤1选择区
    public GameObject SentenceSelectArea;
    public UIGrid SentenceGrid;
    public UIButton SentenceItem;

    // 学习区
    public GameObject StudySentenceArea;
    public UILabel SentenceLabel;
    public UIButton ListenerBtn;
    public UIButton PersonBtn;
    public UITexture TipTexture;
    public UIButton TipListenBtn;
    public UIWidget TipWidget;
    public GameObject Tip;

    // 数据
    private UIWordGame _gameUIInstance;
    public SyncMoudle3Info.request _sentence1Data;
    /// <summary>
    /// 知识点总数
    /// </summary>
    public int _sentenceNum = 0;
    /// <summary>
    /// 知识点通过数量
    /// </summary>
    private int _passNum = 0;
    private int _isPassNum = 0;
    private Queue<int> _studyQueue;
    /// <summary>
    /// 当前知识点编号(从0开始)
    /// </summary>
    public int _curSentenceIndex = 0;
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
        _sentence1Data = DataManager.GetInstance().sentenceData1;
        _sentenceNum = _sentence1Data.contentInfo.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _isPassNum = 0;
        _stepNum = 4;
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

    /// <summary>
    /// 一个句式的开始：展示句式编号
    /// </summary>
    public void ShowBeginTitle()
    {
        _gameUIInstance.SetStepNum(_stepNum);
        _gameUIInstance.SetCurStep(_curStepIndex);
        _gameUIInstance.SetProgress(_passNum, _sentenceNum);
        _gameUIInstance.ShowBeginTitle(_sentence1Data.cStatements[_curSentenceIndex], 2.5f);
        Invoke("ShowTeacherCommand", 2.5f);
    }

    /// <summary>
    /// 一个阶段的开始：展示阶段指令
    /// </summary>
    private void ShowTeacherCommand()
    {
        _gameUIInstance.SetCurStep(_curStepIndex);
        _gameUIInstance.TeacherCommand(_sentence1Data.statements[_curStepIndex - 1], "");
        //_gameUIInstance.ShowBeginTitle(_sentence1Data.steps[_step - 1]);
        //Invoke("StartStudy", 2.5f);
        StartStudy();
    }

    /// <summary>
    /// 句式学习初始化
    /// </summary>
    private void StartStudy()
    {
        if (_curStepIndex == 1)
        {
            StudySentenceArea.SetActive(false);
            SentenceSelectAreaInit();
        } else
        {
            TipsAreaInit();
            StudySentenceAreaInit();
        }
    }

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

    private List<GameObject> SentenceItemList = new List<GameObject>();
    private List<UIButton> ListenerBtnList = new List<UIButton>();
    private List<UILabel> SentenceLabelList = new List<UILabel>();
    /// <summary>
    /// 选择区初始化
    /// </summary>
    private void SentenceSelectAreaInit()
    {
        int partNum = _sentence1Data.contentInfo[_curSentenceIndex].option.Count;
        _sentence1Data.contentInfo[_curSentenceIndex].option = SortRandom(_sentence1Data.contentInfo[_curSentenceIndex].option);
        if (partNum < SentenceItemList.Count)
        {
            for (int i = partNum; i < SentenceItemList.Count; i++)
            {
                SentenceItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partNum; i++)
        {
            if (i >= SentenceItemList.Count)
            {
                GameObject item = Instantiate(SentenceItem.gameObject);
                item.name = i.ToString();
                item.transform.parent = SentenceGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                SentenceItemList.Add(item);

                ListenerBtnList.Add(SentenceItemList[i].transform.Find("ListenerBtn").GetComponent<UIButton>());
                SentenceLabelList.Add(SentenceItemList[i].transform.Find("SentenceLabel").GetComponent<UILabel>());
            }

            ListenerBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(ListenerBtnList[i].gameObject).onClick = OnPlayExampleBtn;
            ListenerBtnList[i].gameObject.SetActive(false);
            SentenceLabelList[i].text = _sentence1Data.contentInfo[_curSentenceIndex].option[i];

            SentenceItemList[i].transform.localScale = new Vector3(1,1,1);
            SentenceItemList[i].gameObject.name = i.ToString();
            UIEventListener.Get(SentenceItemList[i].gameObject).onClick = OnSentenceSelectBtn;

            SentenceItemList[i].SetActive(true);
        }
        SentenceGrid.enabled = true;

        for (int i = 0; i < partNum; i++)
        {
            SentenceItemList[i].SetActive(false);
        }
        SentenceSelectArea.SetActive(true);
        StartPlaySentenceVoice(partNum);
    }

    private int _answerTimes;
    private int _sentencePlayIndex;
    private int _sentencePlayNum;
    /// <summary>
    /// 初始化播放句式选项
    /// </summary>
    private void StartPlaySentenceVoice(int partNum)
    {
        _answerTimes = 0;
        _sentencePlayIndex = 0;
        _sentencePlayNum = partNum;
        PlaySentenceVoice();
    }

    /// <summary>
    /// 依次播放句式选项
    /// </summary>
    private void PlaySentenceVoice()
    {
        SentenceItemList[_sentencePlayIndex].SetActive(true);
        // 系统发音
        float time = 0f;
        _sentencePlayIndex++;

        if (_sentencePlayIndex < _sentencePlayNum)
        {
            Invoke("PlaySentenceVoice", time);
        }
        else
        {
            Invoke("TipsAreaInit", time);
        }
    }

    /// <summary>
    /// 提示区初始化
    /// </summary>
    private void TipsAreaInit()
    {
        TipLabel.text = _sentence1Data.contentInfo[_curSentenceIndex].tips;
        TipsArea.SetActive(true);
    }

    /// <summary>
    /// 选择句式按钮
    /// </summary>
    private void OnSentenceSelectBtn(GameObject go)
    {
        string select = SentenceLabelList[int.Parse(go.name)].text;
        //Debug.Log("选择的句子：" + select);
        //Debug.Log("正确的句子：" + _sentence1Data.contentInfo[_curSentenceIndex].right);
        if (select.Equals(_sentence1Data.contentInfo[_curSentenceIndex].right))
        {
            AudicoManager.instance.Play("effect", "Effect/get the crystal");
            for (int i = 0; i < _sentence1Data.contentInfo[_curSentenceIndex].option.Count; i++)
            {
                if (i != int.Parse(go.name))
                {
                    SentenceItemList[i].SetActive(false);
                }
            }
            SentenceItemList[int.Parse(go.name)].transform.DOLocalMoveY(0,1f);
            SentenceItemList[int.Parse(go.name)].transform.DOScale(new Vector3(1.2f,1.2f,1.2f),1f);

            SendScore(-1, -1, -1, -1, true, _answerTimes);
        } else
        {
            AudicoManager.instance.Play("effect", "Effect/voice error");
            SentenceItemList[int.Parse(go.name)].SetActive(false);
            _answerTimes++;
        }
    }

    /// <summary>
    /// 学习区域初始化
    /// </summary>
    private void StudySentenceAreaInit()
    {
        ListenerBtn.gameObject.name = _curStepIndex.ToString();
        UIEventListener.Get(ListenerBtn.gameObject).onClick = OnPlayExampleBtn;
        UIEventListener.Get(PersonBtn.gameObject).onClick = PlayRecordBtn;
        
        TipListenBtn.gameObject.name = _curStepIndex.ToString();
        UIEventListener.Get(TipListenBtn.gameObject).onClick = OnPlayCluesBtn;

        if (_curStepIndex == 2)
        {
            SentenceLabel.text = _sentence1Data.contentInfo[_curSentenceIndex].rightText1Co;
            if (_sentence1Data.contentInfo[_curSentenceIndex].rightIcon1 != "" && _sentence1Data.contentInfo[_curSentenceIndex].rightIcon1 != null)
            {
                ResourceLoader.Instance.GetTextureResources(TipTexture, "Texture/Hint/Sentence/" + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightIcon1);
                TipWidget.MakePixelPerfect();
                Tip.SetActive(true);
            } else
            {
                Tip.SetActive(false);
            }
        }
        else if (_curStepIndex == 3)
        {
            SentenceLabel.text = "";
            //SentenceLabel.text = "[5581ff]" + _sentence1Data.contentInfo[_curSentenceIndex].right.Replace("...", "[000000] ____[-]") + "[-]";
            if (_sentence1Data.contentInfo[_curSentenceIndex].rightIcon2 != "" && _sentence1Data.contentInfo[_curSentenceIndex].rightIcon2 != null)
            {
                ResourceLoader.Instance.GetTextureResources(TipTexture, "Texture/Hint/Sentence/" + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightIcon2);
                TipWidget.MakePixelPerfect();
                Tip.SetActive(true);
            }
            else
            {
                Tip.SetActive(false);
            }
        }
        else if (_curStepIndex == 4)
        {
            SentenceLabel.text = "";
            //SentenceLabel.text = "[5581ff]" + _sentence1Data.contentInfo[_curSentenceIndex].right.Replace("...", "[000000] ____[-]") + "[-]";
            if (_sentence1Data.contentInfo[_curSentenceIndex].rightIcon3 != "" && _sentence1Data.contentInfo[_curSentenceIndex].rightIcon3 != null)
            {
                ResourceLoader.Instance.GetTextureResources(TipTexture, "Texture/Hint/Sentence/" + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightIcon3);
                TipWidget.MakePixelPerfect();
                Tip.SetActive(true);
            }
            else
            {
                Tip.SetActive(false);
            }
        }
        
        StudySentenceArea.SetActive(true);
        AotoPlayClues();
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    /// <summary>
    /// 自动播放提示音
    /// </summary>
    private void AotoPlayClues()
    {
        float time = 0;
        if (_curStepIndex == 2)
        {
            if (_sentence1Data.contentInfo[_curSentenceIndex].cluesVoice1.Count > 0)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentenceHintPath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].cluesVoice1[0].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(TipListenBtn.gameObject, time);
            }
        }
        /*else if (_curStepIndex == 3)
        {
            if (_sentence1Data.contentInfo[_curSentenceIndex].cluesVoice2.Count > 0)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].cluesVoice2[0].Replace(".mp3", "").Replace(" ", ""));
            }
        }
        else if (_curStepIndex == 4)
        {
            if (_sentence1Data.contentInfo[_curSentenceIndex].cluesVoice3.Count > 0)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].cluesVoice3[0].Replace(".mp3", "").Replace(" ", ""));
            }
        }*/
        Invoke("PlayStudySentenceVoice", time+0.2f);
    }

    /// <summary>
    /// 播放句子原声
    /// </summary>
    /// <param name="num"></param>
    private void PlayStudySentenceVoice()
    {
        SentenceLabel.gameObject.SetActive(true);

        if (_curStepIndex == 2)
        {
            ListenerBtn.gameObject.SetActive(true);
            float time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightVoice1.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
            Invoke("SendStep2Score", time);
        }
        else if (_curStepIndex == 3)
        {
            ListenerBtn.gameObject.SetActive(false);
            SpeakTip();
        }
        else if (_curStepIndex == 4)
        {
            ListenerBtn.gameObject.SetActive(false);
            SpeakTip();
        }
    }

    /// <summary>
    /// 发送步骤2的分数
    /// </summary>
    private void SendStep2Score()
    {
        SendScore(-1,-1,-1,-1,true,1);
    }

    /// <summary>
    /// 显示下一步
    /// </summary>
    private void ShowNextStep()
    {
        if (_curStepIndex == 3 || _curStepIndex == 4)
        {
            ListenerBtn.gameObject.SetActive(true);
            PersonBtn.gameObject.SetActive(true);
        }
        
        UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = OnNextStep;
        _gameUIInstance.ShowNextButton();
    }

    /// <summary>
    /// 点击下一步
    /// </summary>
    /// <param name="go"></param>
    private void OnNextStep(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();

        if (_curStepIndex < 4)
        {
            _curStepIndex++;
            _gameUIInstance.HideNextButton();
            HideAllArea();
            ShowTeacherCommand();
        }
        else
        {
            if (_studyQueue.Count <= 0)
            {
                _gameUIInstance.HideNextButton();
                // 本模块知识学习完毕，则自动弹出报告，无需点击“下一步”
                Invoke("Settlement", 1f);
            }
            else
            {
                _curStepIndex = 1;
                _curSentenceIndex = _studyQueue.Dequeue();
                _gameUIInstance.HideNextButton();
                HideAllArea();
                ShowBeginTitle();
            }
        }
    }

    /// <summary>
    /// 句式的语音
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayExampleBtn(GameObject go)
    {
        float time = 0;
        int num = int.Parse(go.name);
        if (num == 2)
        {
            time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightVoice1.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
        } else if(num == 3)
        {
            time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightVoice2.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
        } else if (num == 4)
        {
            time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightVoice3.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
        }
    }

    /// <summary>
    /// 提示句式的语音
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayCluesBtn(GameObject go)
    {
        float time = 0;
        int num = int.Parse(go.name);
        if (num == 2)
        {
            if (_sentence1Data.contentInfo[_curSentenceIndex].cluesVoice1.Count > 0)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentenceHintPath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].cluesVoice1[0].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(TipListenBtn.gameObject, time);
            }
        }
        else if (num == 3)
        {
            if (_sentence1Data.contentInfo[_curSentenceIndex].cluesVoice2.Count > 0)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentenceHintPath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].cluesVoice2[0].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(TipListenBtn.gameObject, time);
            }
        }
        else if (num == 4)
        {
            if (_sentence1Data.contentInfo[_curSentenceIndex].cluesVoice3.Count > 0)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentenceHintPath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].cluesVoice3[0].Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(TipListenBtn.gameObject, time);
            }
        }
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        UnityAction unityAction = new UnityAction(OnSpeak);
        if (_curStepIndex == 3)
        {
            _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Sentence1, _sentence1Data.contentInfo[_curSentenceIndex].rightText2);
        }
        else if (_curStepIndex == 4)
        {
            _gameUIInstance.ReadyToSpeak(1, unityAction, TeachType.Sentence1, _sentence1Data.contentInfo[_curSentenceIndex].rightText3);
        }
    }

    /// <summary>
    /// 语音说话回调,发送分数
    /// </summary>
    private void OnSpeak()
    {
        if (_curStepIndex == 3)
        {
            SentenceLabel.text = _sentence1Data.contentInfo[_curSentenceIndex].rightText2Co;
        }
        else if (_curStepIndex == 4)
        {
            SentenceLabel.text = _sentence1Data.contentInfo[_curSentenceIndex].rightText3Co;
        }

        float time = 1f;
        if (!_gameUIInstance._isPass)
        {
            // 系统播放原声
            if (_curStepIndex == 3)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightVoice2.Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
            }
            else if (_curStepIndex == 4)
            {
                time = AudicoManager.instance.Play("study", StringConst.SentencePath + _assetsPath + _sentence1Data.contentInfo[_curSentenceIndex].rightVoice3.Replace(".mp3", "").Replace(" ", ""));
                _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
            }
        }

        Debug.Log("上传：学习分数");
        SendScore(_gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._isPass,_gameUIInstance.SpeakTimes);
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
    private void SendScore(int _score,int _fluency,int _unit,int _integrity,bool _isPass,int _order)
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
            markingTotal = _stepNum,
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

            ShowNextStep();
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        TipsArea.SetActive(false);
        SentenceSelectArea.SetActive(false);
        StudySentenceArea.SetActive(false);
        ListenerBtn.gameObject.SetActive(false);
        PersonBtn.gameObject.SetActive(false);
        SentenceLabel.gameObject.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
    }

    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.Sentence1, _passNum, _sentenceNum);
    }
}