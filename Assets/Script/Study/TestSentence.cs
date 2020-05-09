using DG.Tweening;
using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;

public class TestSentence : MonoBehaviour
{
    // 步骤1
    public GameObject Setp1;
    public GameObject BG;
    public UIWidget bgWidget;
    public List<Texture> BGList;
    // 填空区
    public UIGrid WordGrid;
    public UIButton WordItem;
    private List<UIButton> WordItemList = new List<UIButton>();
    private List<UILabel> WordList = new List<UILabel>();
    private List<UILabel> WordAnswerList = new List<UILabel>();

    // 选项区
    public UIGrid OptionGrid;
    public UIButton OptionBtnItem;
    private List<UIButton> OptionBtnItemList = new List<UIButton>();
    private List<UILabel> OptionWordList = new List<UILabel>();
    private List<UITexture> OptionBGList = new List<UITexture>();

    // 数据
    private UIWordGame _gameUIInstance;
    public SyncMoudle8Info.request _sentenceTestData;
    /// <summary>
    /// 知识点总数
    /// </summary>
    public int _sentenceNum = 0;
    /// <summary>
    /// 知识点通过数量
    /// </summary>
    private int _passNum = 0;
    private Queue<int> _studyQueue;
    public int _curSentenceIndex = 0;
    public string _assetsPath;
    
    void Awake()
    {
        _gameUIInstance = UIWordGame._instance;
        _sentenceTestData = DataManager.GetInstance().wordTestData;
        _sentenceNum = _sentenceTestData.contentInfo2.Count;
        _passNum = DataManager.GetInstance().curStudyProgress.passNum;
        _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit + "/";
        //_gameUIInstance.HideStpe(); 
        //_gameUIInstance.HideCard();
        _gameUIInstance.SetStepNum(1);
        _gameUIInstance.SetCurStep(1);
        _gameUIInstance.SetProgress(_passNum, _sentenceNum);
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
    /// 测试开始
    /// </summary>
    public void ShowBeginTitle()
    {
        _gameUIInstance.TeacherCommand(_sentenceTestData.cStatements[3], "");
        //_gameUIInstance.ShowBeginTitle(_sentenceTestData.statements[2]);
        //Invoke("ShowTeacherCommand", 2.5f);
        ShowTeacherCommand();
    }

    /// <summary>
    /// 一个句子的开始
    /// </summary>
    public void ShowTeacherCommand()
    {
        _gameUIInstance.TeacherCommand(_sentenceTestData.cStatements[3], "");
        //_gameUIInstance.ShowBeginTitle("Sentence - " + (_sentenceIndex + 1).ToString());
        _gameUIInstance.SetProgress(_passNum, _sentenceNum);
        StartStudy();
    }

    /// <summary>
    /// 单词学习初始化
    /// </summary>
    public void StartStudy()
    {
        FillWordInit();
        OptionBtnItemInit();
        Setp1.SetActive(true);
    }

    // 第一步骤 //
    public int _needFillWordIndex = 0;
    public int _needFillWord = 0;
    /// <summary>
    /// 填空区初始化
    /// </summary>
    private void FillWordInit()
    {
        int wordNum = _sentenceTestData.contentInfo2[_curSentenceIndex].chaosText.Count;
        bgWidget.width = 210 + (wordNum - 1) * 115;
        BG.SetActive(true);

        if (wordNum < WordItemList.Count)
        {
            for (int i = wordNum; i < WordItemList.Count; i++)
            {
                WordItemList[i].gameObject.SetActive(false);
            }
        }

        _needFillWord = wordNum;
        _needFillWordIndex = 0;

        for (int i = 0; i < wordNum; i++)
        {
            if (i >= WordItemList.Count)
            {
                GameObject item = Instantiate(WordItem.gameObject);
                item.name = i.ToString();
                item.transform.parent = WordGrid.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                WordItemList.Add(item.GetComponent<UIButton>());
                
                WordList.Add(WordItemList[i].transform.Find("Word").GetComponent<UILabel>());
                WordAnswerList.Add(WordItemList[i].transform.Find("WordAnswer").GetComponent<UILabel>());
            }

            WordList[i].text = "";
            WordAnswerList[i].text = _sentenceTestData.contentInfo2[_curSentenceIndex].chaosText[i];
            WordItemList[i].gameObject.name = i.ToString();
            UIEventListener.Get(WordItemList[i].gameObject).onClick = OnSelectWordBtn;

            WordItemList[i].gameObject.SetActive(true);
        }
        WordGrid.enabled = true;
    }


    public List<string> _wordStringList;
    /// <summary>
    /// 选项区初始化
    /// </summary>
    private void OptionBtnItemInit()
    {
        int wordNum = _sentenceTestData.contentInfo2[_curSentenceIndex].chaosText.Count;
        _wordStringList = _sentenceTestData.contentInfo2[_curSentenceIndex].chaosText;
        _wordStringList = SortRandom(_wordStringList);

        if (wordNum < OptionBtnItemList.Count)
        {
            for (int i = wordNum; i < OptionBtnItemList.Count; i++)
            {
                OptionBtnItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < wordNum; i++)
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
                OptionBGList.Add(OptionBtnItemList[i].gameObject.transform.Find("OptionBG").GetComponent<UITexture>());
            }

            OptionWordList[i].text = _wordStringList[i];
            OptionBGList[i].mainTexture = BGList[i % BGList.Count];
            OptionBtnItemList[i].gameObject.name = i.ToString();
            UIEventListener.Get(OptionBtnItemList[i].gameObject).onClick = OnSelectWordCharBtn;

            OptionBtnItemList[i].gameObject.SetActive(true);
        }
        OptionGrid.enabled = true;
    }

    private int SelectNum;
    /// <summary>
    /// 选择单词字符选项
    /// </summary>
    /// <param name="go"></param>
    private void OnSelectWordCharBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/poke bubble");
        OptionBtnItemList[int.Parse(go.name)].gameObject.SetActive(false);

        SelectNum = int.Parse(go.name);
        for (int i = 0; i < _wordStringList.Count; i++)
        {
            if (WordList[i].text.Equals(""))
            {
                WordList[i].text = _wordStringList[SelectNum].ToString();
                break;
            }
        }
        _needFillWordIndex++;
            
        if (_needFillWordIndex == _needFillWord)
        {
            UIEventListener.Get(_gameUIInstance.GetNextButton()).onClick = NextWord;
            _gameUIInstance.ShowNextButton();
        } 
    }

    /// <summary>
    /// 消除作答
    /// </summary>
    private void OnSelectWordBtn(GameObject go)
    {
        int SelectNum = int.Parse(go.name);
        string SelectWord = WordList[SelectNum].text;
        if (!SelectWord.Equals(""))
        {
            WordList[SelectNum].text = "";
            for (int i = 0; i < _wordStringList.Count; i++)
            {
                if (_wordStringList[i].Equals(SelectWord))
                {
                    OptionBtnItemList[i].gameObject.SetActive(true);
                }
            }
        }

        _needFillWordIndex--;
        _gameUIInstance.HideNextButton();
    }

    /// <summary>
    /// 下一个单词
    /// </summary>
    public void NextWord(GameObject go)
    {
        _gameUIInstance.HideNextButton();

        bool isPass = false;
        int wordNum = _sentenceTestData.contentInfo2[_curSentenceIndex].chaosText.Count;
        string text = "";
        for (int i=0; i< wordNum; i++)
        {
            text = text + WordList[i].text;
        }
        if (text == _sentenceTestData.contentInfo2[_curSentenceIndex].text.Replace(" ", ""))
        {
            isPass = true;
        } else
        {
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
            index = _curSentenceIndex,
            markingIndex = 1,
            markingTotal = 1,
            order = _speakTimes
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = _sentenceNum,
            scoreInfo = scoreUnit
        };

        Debug.Log("上传：第" + _curSentenceIndex + "句的学习分数");
        NetSender.Send<ProtoProtocol.SendLearnResultInfo>(msg, OnLearnResultInfo);
    }

    /// <summary>
    /// 收到获得钻石数据
    /// </summary>
    /// <param name="msg">钻石数据</param>
    private void OnLearnResultInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("收到：第" + _curSentenceIndex + "句的上传结果");
        var data = (SendLearnResultInfo.response)rpcRsp;
        if (data.status)
        {
            if (data.addDiamond > 0)
            {
                _gameUIInstance.AddDiamondByOther((int)data.addDiamond);
            }
            _gameUIInstance.SetProgress(_passNum, _sentenceNum);
            isEnd();
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请重新学习");
        }
    }

    private void isEnd()
    {
        if (_studyQueue.Count <= 0)
        {
            // 结算
            Invoke("Settlement", 1f);
        }
        else
        {
            _curSentenceIndex = _studyQueue.Dequeue();
            HideAllArea();
            ShowTeacherCommand();
        }
    }

    /// <summary>
    /// 隐藏所有区域
    /// </summary>
    private void HideAllArea()
    {
        Setp1.SetActive(false);
        BG.SetActive(false);
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
    /// 结算
    /// </summary>
    private void Settlement()
    {
        _gameUIInstance.ShowResultPanel(TeachType.SentenceTest, _passNum, _sentenceNum);
    }
}