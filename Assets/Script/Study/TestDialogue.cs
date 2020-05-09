using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TestDialogue : MonoBehaviour
{
    // 提示区
    public GameObject TipArea;
    public UITexture BG;
    public UIButton TipBtn;
    public UITexture TipBtnSprite;
    public UIWidget TipsBG;
    public UIGrid TipGrid;
    public GameObject TipItem;
    private List<GameObject> TipItemList = new List<GameObject>();
    private List<UIButton> ListenBtnList = new List<UIButton>();
    private List<UILabel> TextList = new List<UILabel>();
    private List<GameObject> VoiceList = new List<GameObject>();
    public GameObject Tip;
    public UITexture[] TipTexture;
    public UIButton TipVoice;
    public UIWidget[] TipWidget;
    public UIGrid TipsGrid;

    // 对话区
    public GameObject DialogueArea;
    public UILabel TopicLabel;
    public GameObject RoleAB;
    public GameObject SelectA;
    public GameObject SelectB;
    public UIGrid WordGrid;
    public UIScrollView WordScrollview;
    public GameObject RoleAItem;
    public GameObject RoleBItem;
    private List<GameObject> RoleItemList = new List<GameObject>();
    private List<UILabel> RoleNameList = new List<UILabel>();
    private List<UILabel> ContentList = new List<UILabel>();
    private List<UISprite> WordBGList = new List<UISprite>();
    private List<GameObject> VoicesList = new List<GameObject>();
    private List<UITexture> AvatarList = new List<UITexture>();
    private List<UIButton> ListenBtnsList = new List<UIButton>();
    private List<UIButton> MySpeakBtnList = new List<UIButton>();
    private List<GameObject> LightList = new List<GameObject>();

    // 数据
    private UIWordGame _gameUIInstance;
    public SyncMoudle4Info.request _dialogueData;
    /// <summary>
    /// 知识点总数
    /// </summary>
    public int _topicNum = 0;
    /// <summary>
    /// 知识点通过数量
    /// </summary>
    private int _passNum = 0;
    private int _isPassNum = 0;
    /// <summary>
    /// 当前知识点编号(从0开始)
    /// </summary>
    public int _curTopicIndex = 0;
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
    private Queue<int> _studyQueue;

    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _dialogueData = DataManager.GetInstance().dialogueData;
        _topicNum = _dialogueData.infoList.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _isPassNum = 0;
        _stepNum = 2;
        _curStepIndex = 1;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
        _gameUIInstance.SetProgress(_passNum, _topicNum);
        _gameUIInstance.SetStepNum(_stepNum);
    }

    /// <summary>
    /// 设置学习队列
    /// </summary>
    /// <param name="StudyQueue"></param>
    public void SetStudyQueue(Queue<int> StudyQueue)
    {
        _studyQueue = StudyQueue;
        _curTopicIndex = _studyQueue.Dequeue();
    }

    /// <summary>
    /// 该对话的难度
    /// </summary>
    public int _level = 1;
    /// <summary>
    /// 当前话题的内容
    /// </summary>
    public List<table12Info> _curDialogueList;
    /// <summary>
    /// 当前话题第几次评分
    /// </summary>
    private int _curMarkIndex = 1;
    /// <summary>
    /// 一个话题的开始
    /// </summary>
    /// <param name="level">级别</param>
    public void ShowBeginTitle(int level)
    {
        _level = level;
        _curDialogueList = _dialogueData.infoList[_curTopicIndex].contentInfo;
        _curDialogueList.OrderBy(t => t.id).ToList();
        hasSendScore.Clear();
        _curMarkIndex = 1;
        //_gameUIInstance.SetProgress(_curTopicIndex, _topicNum, 1);
        _gameUIInstance.SetCurStep(_curStepIndex);
        ShowTopicTitle();
        //_gameUIInstance.ShowBeginTitle("Dialogue - " + (_curTopicIndex + 1).ToString());
        //Invoke("ShowTopicTitle", 2.5f);
    }

    private string[] headlines;
    private string headlineVoicePath;
    private float headlineVoiceTime;
    private float headlineShowTime;
    /// <summary>
    /// 展示对话话题
    /// </summary>
    private void ShowTopicTitle()
    {
        headlines = _dialogueData.infoList[_curTopicIndex].headline.Split('话');
        headlineVoicePath = StringConst.DialoguePath + _assetsPath + _dialogueData.infoList[_curTopicIndex].headlinesMp3[0].Replace(".mp3", "");
        headlineVoiceTime = AudicoManager.instance.Play("study", headlineVoicePath);
        headlineShowTime = Mathf.Max(headlineVoiceTime, 2.5f);
        _gameUIInstance.ShowBeginTitle(headlines[0] + System.Environment.NewLine + "话" + headlines[1], headlineShowTime);
        Invoke("ShowStudyArea", headlineShowTime);
    }

    /// <summary>
    /// 一个扮演步骤的开始
    /// </summary>
    private void ShowStudyArea()
    {
        _gameUIInstance.SetCurStep(_curStepIndex);
        NeedSpeakInit();
        TipAreaInit();
        TipsInit();
        SelectRoleInit();
    }

    /// <summary>
    /// 需要说话的句子总数
    /// </summary>
    private int _speakNum = 0;
    /// <summary>
    /// 当前需要说话的句子编号
    /// </summary>
    private int _curSpeakIndex = 0;
    private List<table12Info> NeedSpeakList = new List<table12Info>();
    /// <summary>
    /// 初始化需要玩家说的句子
    /// </summary>
    private void NeedSpeakInit()
    {
        // 获得需要玩家说的句子
        NeedSpeakList.Clear();
        foreach (table12Info item in _curDialogueList)
        {
            if (item.deleUserId == _curStepIndex)
            {
                NeedSpeakList.Add(item);
            }
        }
        _speakNum = NeedSpeakList.Count;
        _curSpeakIndex = 0;
    }

    /// <summary>
    /// 提示区域展示状态
    /// </summary>
    private bool IsTipBtnShow = false;
    /// <summary>
    /// 初始化提示区
    /// </summary>
    private void TipAreaInit()
    {
        if (_curDialogueList[0].backgroundIcon != null || _curDialogueList[0].backgroundIcon != "")
        {
            BG.mainTexture = Resources.Load<Texture>("Texture/BG/" + _curDialogueList[0].backgroundIcon.Replace(".png", ""));
        }
        // 提示区复原到折叠状态
        TipBtn.transform.localPosition = new Vector3(358, 0, 0);
        IsTipBtnShow = false;
        TipBtnSprite.flip = UIBasicSprite.Flip.Nothing;
        TipArea.SetActive(true);
        //动画浮现出来提示区域
        //TipArea.transform.DOLocalMoveX(-366,2f); //-980
    }

    /// <summary>
    /// 初始化选择角色
    /// </summary>
    private void SelectRoleInit()
    {
        DialogueArea.SetActive(true);
        ShowTopic();
    }

    /// <summary>
    /// 显示话题
    /// </summary>
    private void ShowTopic()
    {
        TopicLabel.text = headlines[0];
        TopicLabel.gameObject.SetActive(true);
        TopicLabel.transform.DOLocalMoveY(253,0.6f); //-400
        Invoke("ShowRole", 0.6f);
    }

    /// <summary>
    /// 显示角色
    /// </summary>
    private void ShowRole()
    {
        RoleAB.SetActive(true);
        RoleAB.transform.DOLocalMoveY(0,0.6f); //-400
        Invoke("ShowTeacherCommand", 0.6f);
    }

    /// <summary>
    /// 显示导师指令
    /// </summary>
    private void ShowTeacherCommand()
    {
        _gameUIInstance.TeacherCommand(_dialogueData.infoList[_curTopicIndex].cStatements[_curStepIndex-1], "");
        SelectRole();
    }

    /// <summary>
    /// 选择角色
    /// </summary>
    private void SelectRole()
    {
        if (_curStepIndex == 1)
        {
            SelectA.SetActive(true);
        }
        else
        {
            SelectB.SetActive(true);
        }
        Invoke("HideSelectRole", 0.8f);
    }

    /// <summary>
    /// 隐藏选角步骤
    /// </summary>
    private void HideSelectRole()
    {
        RoleAB.transform.DOLocalMoveY(-400, 0.6f);
        Invoke("DialogueInit", 0.6f);
    }

    /// <summary>
    /// 初始化对话内容
    /// </summary>
    private void DialogueInit()
    {
        RoleItemList.Clear();
        RoleNameList.Clear();
        ContentList.Clear();
        WordBGList.Clear();
        VoicesList.Clear();
        AvatarList.Clear();
        ListenBtnsList.Clear();
        MySpeakBtnList.Clear();
        LightList.Clear();

        for (int i=0;i< _curDialogueList.Count;i++)
        {
            GameObject item;
            if (_curDialogueList[i].deleUserId == 2)
            {
                // 角色B在右边
                item = Instantiate(RoleBItem);
            } else
            {
                // 角色A和C在左边
                item = Instantiate(RoleAItem);
            }
            item.name = i.ToString();
            item.transform.parent = WordGrid.transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            RoleItemList.Add(item);

            RoleNameList.Add(RoleItemList[i].transform.Find("Role/RoleName").GetComponent<UILabel>());
            ContentList.Add(RoleItemList[i].transform.Find("WordBG/Content").GetComponent<UILabel>());
            VoicesList.Add(RoleItemList[i].transform.Find("WordBG/Voice").gameObject);
            AvatarList.Add(RoleItemList[i].transform.Find("Role/Avatar").GetComponent<UITexture>());
            ListenBtnsList.Add(RoleItemList[i].transform.Find("WordBG/Btn/ListenBtn").GetComponent<UIButton>());
            LightList.Add(RoleItemList[i].transform.Find("WordBG/Light").gameObject);
            MySpeakBtnList.Add(RoleItemList[i].transform.Find("WordBG/Btn/MySpeakBtn").GetComponent<UIButton>());

            // 设置角色名
            RoleNameList[i].text = _curDialogueList[i].headId.Replace(".png", "");
            /*if (_curDialogueList[i].deleUserId == 1)
            {
                RoleNameList[i].text = "角色A";
            } else if (_curDialogueList[i].deleUserId == 2)
            {
                RoleNameList[i].text = "角色B";
            }
            else
            {
                RoleNameList[i].text = "角色C";
            }*/

            // 设置语句
            if (_curDialogueList[i].deleUserId == _curStepIndex)
            {
                // 玩家作答语句
                ListenBtnsList[i].gameObject.SetActive(false);
                VoicesList[i].SetActive(false);
                ContentList[i].text = "___________________";
                ContentList[i].gameObject.SetActive(true);
            } else
            {
                // 系统展示语句，不同的级别，展示难度不同
                if (_level == 1)
                {
                    // 展示读音和文字
                    ListenBtnsList[i].gameObject.SetActive(true);
                    VoicesList[i].SetActive(false);
                    ContentList[i].text = _curDialogueList[i].text;
                    ContentList[i].gameObject.SetActive(true);
                }
                else if (_level == 2 || _level == 3)
                {
                    // 展示读音
                    ListenBtnsList[i].gameObject.SetActive(true);
                    VoicesList[i].SetActive(true);
                    ContentList[i].text = _curDialogueList[i].text;
                    ContentList[i].gameObject.SetActive(false);
                }
            }

            // 熄灭高亮
            LightList[i].SetActive(false);
            // 设置头像
            AvatarList[i].mainTexture = Resources.Load<Texture>("Texture/Avatar/" + _curDialogueList[i].headId.Replace(".png", ""));
            // 设置原声播放
            ListenBtnsList[i].gameObject.name = i.ToString();
            UIEventListener.Get(ListenBtnsList[i].gameObject).onClick = OnListenBtnsList;
            UIEventListener.Get(MySpeakBtnList[i].gameObject).onClick = PlayRecordBtn;

            RoleItemList[i].SetActive(true);
        }
        WordGrid.enabled = true;
        WordScrollview.ResetPosition();

        DialogueStart();
    }

    private void PlayRecordBtn(GameObject go)
    {
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.PlayRecord();
    }

    /// <summary>
    /// 播放对话中的原声
    /// </summary>
    /// <param name="go"></param>
    private void OnListenBtnsList(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.DialoguePath + _assetsPath + _curDialogueList[int.Parse(go.name)].voice.Replace(".mp3", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 当前话题进行到第几句
    /// </summary>
    private int _curDialogueIndex = 0;
    /// <summary>
    /// 当前话题一共多少句对话
    /// </summary>
    private int _dialogueNum = 0;
    /// <summary>
    /// 开始对话
    /// </summary>
    private void DialogueStart()
    {
        _curDialogueIndex = 0;
        _dialogueNum = _curDialogueList.Count;
        PlayDialogueVoice();
    }

    private List<int> hasSendScore = new List<int>();
    /// <summary>
    /// 依次播放对话语音
    /// </summary>
    private void PlayDialogueVoice()
    {
        if (_curDialogueIndex > 0)
        {
            LightList[_curDialogueIndex - 1].SetActive(false);
        }
        
        if (_curDialogueIndex < _dialogueNum)
        {
            for (int i = 0; i < TipTexture.Length; i++)
            {
                TipTexture[i].gameObject.SetActive(false);
            }

            if (_curDialogueList[_curDialogueIndex].ItemIcon.Count > 0)
            {
                for (int i = 0; i < _curDialogueList[_curDialogueIndex].ItemIcon.Count; i++)
                {
                    //Debug.Log("线索图："+ _curDialogueList[_curDialogueIndex].ItemIcon[i]);
                    ResourceLoader.Instance.GetTextureResources(TipTexture[i], "Texture/Hint/Dialogue/" + _assetsPath + _curDialogueList[_curDialogueIndex].ItemIcon[i]);
                    TipWidget[i].MakePixelPerfect();
                    TipTexture[i].transform.localScale = new Vector3(1f, 1f, 1f);
                    TipTexture[i].gameObject.SetActive(true);
                    Tip.SetActive(true);
                    TipsGrid.enabled = true;
                }
            }
            else
            {
                Tip.SetActive(false);
            }

            if (_curDialogueList[_curDialogueIndex].backgroundIcon != null || _curDialogueList[_curDialogueIndex].backgroundIcon != "")
            {
                BG.mainTexture = Resources.Load<Texture>("Texture/BG/" + _curDialogueList[_curDialogueIndex].backgroundIcon.Replace(".png", ""));
            }

            LightList[_curDialogueIndex].SetActive(true);
            if (_curDialogueList[_curDialogueIndex].deleUserId == _curStepIndex)
            {
                // 玩家发音
                SpeakTip();
            } else
            {
                // 系统发音
                float time = AudicoManager.instance.Play("study", StringConst.DialoguePath + _assetsPath + _curDialogueList[_curDialogueIndex].voice.Replace(".mp3", ""));
                _gameUIInstance.SetVoiceEffect(ListenBtnsList[_curDialogueIndex].gameObject, time);

                // 角色C
                if (_curDialogueList[_curDialogueIndex].deleUserId == 0)
                {
                    Invoke("RoleCSpeak", time);
                } else
                {
                    _curDialogueIndex++;
                    Invoke("PlayDialogueVoice", time);
                }
            }
        }
        else
        {
            PlayDialogueVoiceEnd();
        }
    }

    /// <summary>
    /// 角色C发送分数
    /// </summary>
    private void RoleCSpeak()
    {
        if (!hasSendScore.Contains(_curDialogueIndex))
        {
            hasSendScore.Add(_curDialogueIndex);
            SendScore(-1, -1, -1, -1, true, 1);
        } else
        {
            _curDialogueIndex++;
            PlayDialogueVoice();
        }
    }

    /// <summary>
    /// 提示进行语音说话
    /// </summary>
    private void SpeakTip()
    {
        TipsInit();
        UnityAction unityAction = new UnityAction(OnSpeak);
        _gameUIInstance.ReadyToSpeak(1, unityAction,TeachType.Dialogue, _curDialogueList[_curDialogueIndex].text);
    }

    public void SortRandom<T>(List<T> collection1, List<T> collection2)
    {
        for (int i = collection1.Count - 1; i > 0; i--)
        {
            int p = Random.Range(0, collection1.Count - 1);
            var temp = collection1[p];
            collection1[p] = collection1[i];
            collection1[i] = temp;

            var temp1 = collection2[p];
            collection2[p] = collection2[i];
            collection2[i] = temp1;
        }
    }

    /// <summary>
    /// 提示区初始化
    /// </summary>
    private void TipsInit()
    {
        // 初始化提示选项
        int optionNum = 0;
        if (NeedSpeakList.Count > 0)
        {
            optionNum = NeedSpeakList[_curSpeakIndex].confuse.Count;
            TipsBG.height = optionNum * 100;
        }

        if (optionNum < TipItemList.Count)
        {
            for (int i = optionNum; i < TipItemList.Count; i++)
            {
                TipItemList[i].gameObject.SetActive(false);
            }
        }

        SortRandom(NeedSpeakList[_curSpeakIndex].confuse, NeedSpeakList[_curSpeakIndex].confuseVoice);

        for (int i = 0; i < optionNum; i++)
        {
            if (i >= TipItemList.Count)
            {
                GameObject item = Instantiate(TipItem);
                item.name = i.ToString();
                item.transform.parent = TipGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                TipItemList.Add(item);

                ListenBtnList.Add(TipItemList[i].transform.Find("ListenBtn").GetComponent<UIButton>());
                TextList.Add(TipItemList[i].transform.Find("TipContent/Text").GetComponent<UILabel>());
                VoiceList.Add(TipItemList[i].transform.Find("TipContent/Voice").gameObject);
            }

            ListenBtnList[i].gameObject.name = i.ToString();
            UIEventListener.Get(ListenBtnList[i].gameObject).onClick = OnPlayTipsExampleBtn;

            // 不同的级别，选项的难度不同
            if (_level == 1)
            {
                // 展示读音和文字
                ListenBtnList[i].gameObject.SetActive(true);
                VoiceList[i].SetActive(false);
                TextList[i].text = NeedSpeakList[_curSpeakIndex].confuse[i];
                TextList[i].gameObject.SetActive(true);
            }
            else if (_level == 2)
            {
                // 展示文字
                ListenBtnList[i].gameObject.SetActive(false);
                VoiceList[i].SetActive(false);
                TextList[i].text = NeedSpeakList[_curSpeakIndex].confuse[i];
                TextList[i].gameObject.SetActive(true);
            }
            else if (_level == 3)
            {
                // 展示读音
                ListenBtnList[i].gameObject.SetActive(true);
                VoiceList[i].SetActive(true);
                TextList[i].text = NeedSpeakList[_curSpeakIndex].confuse[i];
                TextList[i].gameObject.SetActive(false);
            }

            TipItemList[i].SetActive(true);
        }
        TipGrid.enabled = true;

        UIEventListener.Get(TipBtn.gameObject).onClick = OnShowTipBtn;

        if (IsTipBtnShow)
        {
            TipBtnSprite.flip = UIBasicSprite.Flip.Nothing;
            TipBtn.transform.DOLocalMoveX(358, 0.8f);
            IsTipBtnShow = false;
        }
    }

    /// <summary>
    /// 播放提示区示例音频
    /// </summary>
    /// <param name="go"></param>
    private void OnPlayTipsExampleBtn(GameObject go)
    {
        float time = AudicoManager.instance.Play("study", StringConst.DialoguePath + _assetsPath + NeedSpeakList[_curSpeakIndex].confuseVoice[int.Parse(go.name)].Replace(".mp3", ""));
        _gameUIInstance.SetVoiceEffect(go, time);
    }

    /// <summary>
    /// 显示提示选项
    /// </summary>
    /// <param name="go"></param>
    private void OnShowTipBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (!IsTipBtnShow)
        {
            TipBtnSprite.flip = UIBasicSprite.Flip.Horizontally;
            TipBtn.transform.DOLocalMoveX(-290, 0.8f);
            IsTipBtnShow = true;
        }
        else
        {
            TipBtnSprite.flip = UIBasicSprite.Flip.Nothing;
            TipBtn.transform.DOLocalMoveX(358, 0.8f);
            IsTipBtnShow = false;
        }
    }

    /// <summary>
    /// 语音说话回调
    /// </summary>
    private void OnSpeak()
    {
        for (int i = 0; i < _dialogueNum; i++)
        {
            MySpeakBtnList[i].gameObject.SetActive(false);
        }
        MySpeakBtnList[_curDialogueIndex].gameObject.SetActive(true);
        VoicesList[_curDialogueIndex].SetActive(true);
        ContentList[_curDialogueIndex].gameObject.SetActive(false);

        SpeakTip();
        UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = SureSendScore;
        _gameUIInstance.ShowNextButton();
    }

    private void SureSendScore(GameObject go)
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        _gameUIInstance.CancelSpeak();
        SendScore(_gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._score, _gameUIInstance._isPass, 2);
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
            _isPassNum++;
        }

        if (_curMarkIndex >= _dialogueNum)
        {
            if (_isPassNum >= _dialogueNum)
            {
                _passNum++;
                DataManager.GetInstance().curStudyProgress.passNum = _passNum;
            }
            _isPassNum = 0;
            if (DataManager.GetInstance().curStudyProgress.completeNum < DataManager.GetInstance().curStudyProgress.totalNum)
            {
                DataManager.GetInstance().curStudyProgress.completeNum++;
            }
            _gameUIInstance.SetProgress(_passNum, _topicNum);
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
            index = _curTopicIndex,
            markingIndex = _curMarkIndex,
            markingTotal = _dialogueNum,
            order = _speakTimes
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _topicNum,
            scoreInfo = scoreUnit
        };

        Debug.Log("上传：第" + _curMarkIndex + "句的学习分数");
        NetSender.Send<ProtoProtocol.SendLearnResultInfo>(msg, OnLearnResultInfo);
    }

    /// <summary>
    /// 收到获得钻石数据
    /// </summary>
    /// <param name="msg">钻石数据</param>
    private void OnLearnResultInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：第" + _curMarkIndex + "句的上传结果");
        var data = (SendLearnResultInfo.response)rpcRsp;
        if (data.status)
        {
            _curMarkIndex++;
            if (data.addDiamond > 0)
            {
                _gameUIInstance.AddDiamondByOther((int)data.addDiamond);
            }
            if (_curDialogueList[_curDialogueIndex].deleUserId == _curStepIndex)
            {
                SpeakEnd();
            } else
            {
                RoleCEnd();
            }
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    private void SpeakEnd()
    {
        _curDialogueIndex++;
        _curSpeakIndex++;
        if (_curSpeakIndex >= _speakNum)
        {
            _curSpeakIndex = _speakNum - 1;
        }
        
        PlayDialogueVoice();
    }

    private void RoleCEnd()
    {
        _curDialogueIndex++;
        PlayDialogueVoice();
    }

    /// <summary>
    /// 播放对话语音结束
    /// </summary>
    private void PlayDialogueVoiceEnd()
    {
        if (_curStepIndex == 1)
        {
            // 扮演A结束，下一步扮演B
            NextRoleB();
        }
        else
        {
            // 扮演B结束
            if (_studyQueue.Count <= 0)
            {
                _gameUIInstance.HideNextButton();
                // 本模块知识学习完毕，则自动弹出报告，无需点击“下一步”
                Invoke("Settlement", 1f);
            }
            else
            {
                // 下一步进行下一个话题
                NextTopic();
            }
        }
    }

    /// <summary>
    /// 扮演下一个角色B
    /// </summary>
    private void NextRoleB()
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _curStepIndex = 2;
        _gameUIInstance.HideNextButton();
        WordGrid.transform.DestroyChildren();
        SelectA.SetActive(false);
        SelectB.SetActive(false);
        ShowStudyArea();
    }

    /// <summary>
    /// 下一个话题
    /// </summary>
    /// <param name="go"></param>
    private void NextTopic()
    {
        _gameUIInstance.StopAddDiamond();
        _gameUIInstance.StopVoiceEffect();
        AudicoManager.instance.StopStudyAudio();
        _gameUIInstance.HideNextButton();
        Tip.SetActive(false);
        _curStepIndex = 1;
        _curTopicIndex = _studyQueue.Dequeue();
        HideAllArea();
        ShowBeginTitle(_level);
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        TipArea.SetActive(false);
        DialogueArea.SetActive(false);
        TopicLabel.transform.localPosition = new Vector3(0, -400, 0);
        WordGrid.transform.DestroyChildren();
        SelectA.SetActive(false);
        SelectB.SetActive(false);
        _gameUIInstance.TeacherCommand("", "");
        Tip.SetActive(false);
    }
  
    /// <summary>
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.Dialogue, _passNum, _topicNum);
    }
}