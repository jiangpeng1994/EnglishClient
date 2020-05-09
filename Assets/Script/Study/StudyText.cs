using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StudyText : MonoBehaviour
{
    //互动区
    public GameObject TextInteraction;
    public UITexture TextTexture;
    public UIButton ListenerBtn;
    public UIButton PersonBtn;

    // 数据
    private UIWordGame _gameUIInstance;
    private SyncMoudle1Info.request _textData;
    /// <summary>
    /// 知识点总数
    /// </summary>
    private int _textNum = 0;
    /// <summary>
    /// 知识点通过数量
    /// </summary>
    private int _passNum = 0;
    private int _isPassNum = 0;
    private Queue<int> _studyQueue;
    /// <summary>
    /// 当前知识点编号(从0开始)
    /// </summary>
    private int _curTextIndex = 0;
    /// <summary>
    /// 步骤总数
    /// </summary>
    private int _stepNum = 0;
    /// <summary>
    /// 当前步骤编号(从1开始)
    /// </summary>
    private int _curStepIndex = 1;
    /// <summary>
    /// 资源加载路径
    /// </summary>
    private string _assetsPath;

    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _textData = DataManager.GetInstance().textData;
        _textNum = _textData.contentInfo.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _isPassNum = 0;
        _stepNum = 1;
        _curStepIndex = 1;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
        _gameUIInstance.SetProgress(_passNum, _textNum);
        _gameUIInstance.SetStepNum(_stepNum);
    }

    /// <summary>
    /// 设置学习队列
    /// </summary>
    /// <param name="StudyQueue"></param>
    public void SetStudyQueue(Queue<int> StudyQueue)
    {
        _studyQueue = StudyQueue;
        _curTextIndex = _studyQueue.Dequeue();
        //_curTextIndex = 29;
    }

    /// <summary>
    /// 课文的开始：展示命令
    /// </summary>
    public void ShowBeginTitle()
    {
        _gameUIInstance.SetStepNum(_stepNum);
        _gameUIInstance.SetCurStep(_curStepIndex);
        _gameUIInstance.SetProgress(_passNum, _textNum);
        _gameUIInstance.TeacherCommand(_textData.cStatement, "");
        _gameUIInstance.ShowBeginTitle(_textData.statement);
        //float time = AudicoManager.instance.Play("study", StringConst.Mp3Teacher + "Follow to read");
        //Invoke("ShowTextIndex", time);
        ShowTextIndex();
    }

    /// <summary>
    /// 一个句子的开始
    /// </summary>
    public void ShowTextIndex()
    {
        _gameUIInstance.ShowBeginTitle("Sentence - " + (_curTextIndex + 1).ToString());
        _gameUIInstance.SetCurStep(_curStepIndex);
        Invoke("StartStudy", 2.5f);
    }

    /// <summary>
    /// 句子学习初始化
    /// </summary>
    public void StartStudy()
    {
        TextInteraction.SetActive(true);
        //TextTexture.mainTexture = Resources.Load<Texture>("Texture/" + StringConst.TextPath + _assetsPath + _textData.contentInfo[_curTextIndex].image.Replace(".jpg", "").Replace(".png", ""));
        ResourceLoader.Instance.GetTextureResources(TextTexture, "Texture/" + StringConst.TextPath + _assetsPath + _textData.contentInfo[_curTextIndex].image);
        TextTexture.transform.DOLocalMoveX(0, 0.5f);
        Invoke("PlayVoice",0.8f);
    }

    /// <summary>
    /// 播放课文原声
    /// </summary>
    public void PlayVoice()
    {
        ListenerBtn.gameObject.SetActive(true);
        UIEventListener.Get(ListenerBtn.gameObject).onClick = PlayVoiceAgain; 
        UIEventListener.Get(PersonBtn.gameObject).onClick = PlayRecordBtn;
        Debug.Log("课文音频："+ _textData.contentInfo[_curTextIndex].voice);
        float time = AudicoManager.instance.Play("study", StringConst.TextPath + _assetsPath + _textData.contentInfo[_curTextIndex].voice.Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);

        if (_textData.contentInfo[_curTextIndex].isRead == 1)
        {
            Invoke("SpeakTip", time);
        } else
        {
           Invoke("SkipSpeak", time + 0.5f);
        }
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    public void PlayVoiceAgain(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.TextPath + _assetsPath + _textData.contentInfo[_curTextIndex].voice.Replace(".mp3", "").Replace(" ", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        UnityAction unityAction = new UnityAction(OnSpeak);
        _gameUIInstance.ReadyToSpeak(1, unityAction,TeachType.Text, _textData.contentInfo[_curTextIndex].content);
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        float time = 0f;
        if (!_gameUIInstance._isPass)
        {
            time = AudicoManager.instance.Play("study", StringConst.TextPath + _assetsPath + _textData.contentInfo[_curTextIndex].voice.Replace(".mp3", "").Replace(" ", ""));
            _gameUIInstance.SetVoiceEffect(ListenerBtn.gameObject, time);
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
    /// <param name="_nicety"></param>
    /// <param name="_integrity"></param>
    /// <param name="_isPass"></param>
    /// <param name="_order"></param>
    private void SendScore(int _score, int _fluency, int _nicety, int _integrity, bool _isPass, int _order)
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
            _gameUIInstance.SetProgress(_passNum, _textNum);
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
            nicety = _nicety,
            integrity = _integrity
        };
        scoreUnit scoreUnit = new scoreUnit
        {
            isPass = _isPass,
            info = scoreBaseInfo,
            index = _curTextIndex,
            markingIndex = _curStepIndex,
            markingTotal = 1,
            order = _order
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _textNum,
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
            GameTools.Instance.TipsShow("上传分数失败，请重新学习该知识点");
        }
    }

    /// <summary>
    /// 跳过语音
    /// </summary>
    private void SkipSpeak()
    {
        SendScore(-1, -1, -1, -1, true, 1);
    }

    /// <summary>
    /// 是否结束
    /// </summary>
    private void IsEnd()
    {
        PersonBtn.gameObject.SetActive(true);
        if (_studyQueue.Count <= 0)
        {
            // 结算
            Invoke("Settlement", 1f);
        }
        else
        {
            // 下一步
            if(_textData.contentInfo[_curTextIndex].isRead == 1)
            {
                UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextStep;
                _gameUIInstance.ShowNextButton();
            } else
            {
                NextStep(null);
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

        if (_textData.contentInfo[_curTextIndex].isRead == 1)
        {
            AudicoManager.instance.Play("effect", "Effect/press button");
        }
        _curStepIndex = 1;
        _curTextIndex = _studyQueue.Dequeue();
        _gameUIInstance.HideNextButton();
        HideAllArea();
        ShowTextIndex();
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        TextInteraction.SetActive(false);
        TextTexture.transform.localPosition = new Vector3(-1400, 0, 0);
        ListenerBtn.gameObject.SetActive(false);
        PersonBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.Text, _passNum, _textNum);
    }
}