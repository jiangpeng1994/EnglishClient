using ProtoSprotoType;
using Sproto;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class StudyVideo : MonoBehaviour
{
    //互动区
    public GameObject Mask;
    public GameObject Video;
    public UIButton BackBtn;
    public UIButton PlayBtn;
    public UIButton PauseBtn;
    public UIButton SkipBtn;
    public UIButton RePlayBtn;
    public VideoPlayer videoPlayer;
    public UITexture TextTexture;
    private bool isSkip;
    public string key;
    private bool isStart;

    private UIWordGame _gameUIInstance;
    
    private bool isDown;
    private bool isLoad;

    void Awake()
    {
        Debug.Log("[视频学习]-进入");
        _gameUIInstance = UIWordGame._instance;
        _gameUIInstance.SetStepNum(1);
        _gameUIInstance.SetCurStep(1);
        _gameUIInstance.TeacherCommand("", "");

        isDown = false;
        isLoad = false;
        isStart = false;
        isSkip = false;
        key = DataManager.GetInstance().roleData.ID + ":" + DataManager.GetInstance().roleData.curGrade.ToString() + DataManager.GetInstance().roleData.curTerm.ToString() + DataManager.GetInstance().roleData.curUnit.ToString();
        Video.SetActive(true);
        Mask.gameObject.SetActive(true);
        PlayBtn.gameObject.SetActive(true);
        SkipBtn.gameObject.SetActive(true);
        RePlayBtn.gameObject.SetActive(false);
        PauseBtn.gameObject.SetActive(false);

        UIEventListener.Get(BackBtn.gameObject).onClick = Quit;
        UIEventListener.Get(PlayBtn.gameObject).onClick = OnClickPlayBtn;
        UIEventListener.Get(SkipBtn.gameObject).onClick = OnClickSkipBtn;
        UIEventListener.Get(RePlayBtn.gameObject).onClick = OnClickRePlayBtn;
        UIEventListener.Get(PauseBtn.gameObject).onClick = OnClickPauseBtn;
    }

    private void Start()
    {
        //_urlPath = _urlPath + "/" + _videoName;
        //_localPath = Application.persistentDataPath + "/" + _videoName;
        //Debug.Log("path:" + _localPath);
        //DirectoryInfo directoryInfo = new DirectoryInfo(_localPath);
        // 判断一下本地是否有了该视频 ,如果没有就下载
        //if (!File.Exists(_localPath))
        //{
         //   StartCoroutine(DownLoadVideo());
        //}
    }

    /// <summary>
    /// 下载视频到内存
    /// </summary>
    /// <returns></returns>
    /*private IEnumerator DownLoadVideo()
    {
        //WWW www = new WWW("");
        //yield return www;

        //if (www.isDone)
        //{
        //    isDown = true;
         //   SaveVideo(www.bytes);
        //}
    }*/

    /// <summary>
    /// 视频保存到本地
    /// </summary>
    /// <param name="bytes"></param>
    private void SaveVideo(byte[] bytes)
    {
        //FileInfo fileInfo = new FileInfo("");
        //Stream stream = fileInfo.Create();
        //stream.Write(bytes, 0, bytes.Length);
        //Debug.Log("视频下载完成");
        //stream.Close();
        //stream.Dispose();
    }

    /// <summary>
    /// 点击播放
    /// </summary>
    /// <param name="go"></param>
    private void OnClickPlayBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("[视频学习]-点击播放");
        //if (!File.Exists(""))
        //{
         //   GameTools.Instance.TipsShow("视频下载中，请稍后重试");
        //} else
        //{
            if (isLoad)
            {
                PlayVideo();
            } else
            {
                StartCoroutine(LoadVideo());
            }
        //}
    }

    private string[] _term = new string[] { "", "A", "B" };
    private string GetVideoName()
    {
        string name = DataManager.GetInstance().roleData.curGrade + _term[DataManager.GetInstance().roleData.curTerm]
            + "U" + DataManager.GetInstance().roleData.curUnit;
        return name;
    }

    /// <summary>
    /// 从本地加载视频
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadVideo()
    {
        string _assetsPath = DataManager.GetInstance().roleData.curGrade + "." + DataManager.GetInstance().roleData.curTerm + "." +
            DataManager.GetInstance().roleData.curUnit;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = string.Format("{0}/{1}/{2}/{3}/{4}.mp4", Application.persistentDataPath, "Resources", "Mp4", _assetsPath, GetVideoName());
        videoPlayer.Prepare();
        videoPlayer.playOnAwake = false;
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        PlayVideo();
        isLoad = true;
    }

    /// <summary>
    /// 播放视频
    /// </summary>
    private void PlayVideo()
    {
        PauseBtn.gameObject.SetActive(true);
        PlayBtn.gameObject.SetActive(false);
        videoPlayer.Play();
        isStart = true;
    }

    /// <summary>
    /// 点击跳过
    /// </summary>
    /// <param name="go"></param>
    private void OnClickSkipBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("[视频学习]-点击跳过");
        if (int.Parse(DataManager.GetInstance().roleData.curLevel) > 1)
        {
            isSkip = true;
            SendScore(-1, -1, -1, -1, true, 1);
        } else
        {
            string canSkip = GameDataManager.GetString(key);
            if (canSkip != null && canSkip != "" && canSkip == "1")
            {
                isSkip = true;
                SendScore(-1, -1, -1, -1, true, 1);
            } else
            {
                GameTools.Instance.TipsShow("第1次学习该单元视频，无法跳过");
            }
        }
    }

    /// <summary>
    /// 点击重播
    /// </summary>
    /// <param name="go"></param>
    private void OnClickRePlayBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("[视频学习]-点击播放");
        PauseBtn.gameObject.SetActive(true);
        PlayBtn.gameObject.SetActive(false);
        RePlayBtn.gameObject.SetActive(false);
        videoPlayer.Play();
        isStart = true;
    }

    /// <summary>
    /// 点击暂停
    /// </summary>
    /// <param name="go"></param>
    private void OnClickPauseBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("[视频学习]-点击暂停");
        PlayBtn.gameObject.SetActive(true);
        PauseBtn.gameObject.SetActive(false);
        videoPlayer.Pause();
        isStart = false;
    }

    void Update()
    {
        if (videoPlayer.texture == null || !isStart)
        {
            return;
        }

        if (!videoPlayer.isPlaying)
        {
            isStart = false;
            OnStop();
        } else
        {
            TextTexture.mainTexture = videoPlayer.texture;
        }
    }

    /// <summary>
    /// 视频播放结束
    /// </summary>
    private void OnStop()
    {
        SendScore(-1, -1, -1, -1, true, 1);
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
            index = 1,
            markingIndex = 1,
            markingTotal = 1,
            order = _order
        };
        SendLearnResultInfo.request msg = new SendLearnResultInfo.request
        {
            moudleBase = moudle,
            moudleTotal = 1,
            scoreInfo = scoreUnit
        };

        Debug.Log("[视频学习]-上传：播放完毕");
        NetSender.Send<ProtoProtocol.SendLearnResultInfo>(msg, OnLearnResultInfo);
    }

    /// <summary>
    /// 收到获得钻石数据
    /// </summary>
    /// <param name="msg">钻石数据</param>
    private void OnLearnResultInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("[视频学习]-收到：播放完毕");
        var data = (SendLearnResultInfo.response)rpcRsp;
        Debug.Log("[视频学习]-收到：播放完毕状态：" + data.status);

        if (data.status)
        {
            GameDataManager.SetString(key,"1");
            if (data.addDiamond > 0) 
            {
                _gameUIInstance.AddDiamondByOther((int)data.addDiamond);
            }

            if (isSkip)
            {
                Quit(null);
            } else
            {
                RePlayBtn.gameObject.SetActive(true);
                PauseBtn.gameObject.SetActive(false);
                PlayBtn.gameObject.SetActive(false);
                videoPlayer.Stop();
            }
        }
        else
        {
            GameTools.Instance.TipsShow("上传分数失败，请检查网络状态");
        }
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="go"></param>
    private void Quit(GameObject go)
    {
        Debug.Log("[视频学习]-退出");
        RefreshMoudleUI();
    }

    /// <summary>
    /// 刷新模块选择状态
    /// </summary>
    /// <param name="btn"></param>
    public void RefreshMoudleUI()
    {
        int curLevel = int.Parse(DataManager.GetInstance().roleData.curLevel);
        Debug.Log("请求：难度" + curLevel + "的模块数据");

        GetLevelPassInfo.request msg = new GetLevelPassInfo.request();
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = curLevel,
        };
        msg.moudleBase = moudle;
        NetSender.Send<ProtoProtocol.GetLevelPassInfo>(msg, MoudleInfoInfoResponseHandler);
    }

    /// <summary>
    ///  收到模块信息
    /// </summary>
    /// <param name="msg"></param>
    public void MoudleInfoInfoResponseHandler(SprotoTypeBase msg)
    {
        int curLevel = int.Parse(DataManager.GetInstance().roleData.curLevel);
        bool isLastUnit = DataManager.GetInstance().isLastUnit;
        Debug.Log("收到：难度" + curLevel + "的模块数据");

        var data = (GetLevelPassInfo.response)msg;

        Dictionary<int, passUnit> _modulePassList = new Dictionary<int, passUnit>();
        foreach (passUnit item in data.passList)
        {
            int index = int.Parse(item.moudleId.ToString().Substring(2, 1));

            // 最后1个单元没有句式模块，默认3星通过
            if (isLastUnit && curLevel != 4 && index == 4)
            {
                item.isPass = true;
            }

            if (isLastUnit && curLevel == 4 && index == 2)
            {
                item.isPass = true;
            }

            // isPass为true就设置为3星
            if (item.isPass)
            {
                item.star = 3;
            }
            _modulePassList.Add(index, item);
        }

        int curIndex = _modulePassList.Count + 1;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            if (_modulePassList[i].isPass == false && _modulePassList[i].star <= 0)
            {
                curIndex = i;
                break;
            }
        }

        int fullStar = 0;
        if (curLevel == 4)
        {
            fullStar = 9;
        }
        else
        {
            fullStar = 15;
        }

        int starNum = 0;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            starNum = starNum + (int)_modulePassList[i].star;
        }
        if (starNum >= fullStar)
        {
            DataManager.GetInstance().isFullStar = true;
        }
        else
        {
            DataManager.GetInstance().isFullStar = false;
        }

        DataManager.GetInstance().modulePassList = _modulePassList;
        DataManager.GetInstance().isLastUnit = isLastUnit;

        // 1-3模块：1-6   4模块：3-6
        if (curLevel == 4)
        {
            curIndex = curIndex + 2;
        }

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有模块");
            SelectCoursePanel.instance.OpenModuleSelectUI(curLevel, isLastUnit, 6, _modulePassList);
        }
        else
        {
            SelectCoursePanel.instance.OpenModuleSelectUI(curLevel, isLastUnit, curIndex, _modulePassList);
        }

        AudicoManager.instance.Play("effect", "Effect/press button");
        WindowManager.instance.Delete<UIWordGamePanel>();
    }
}