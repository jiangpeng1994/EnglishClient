using System;
using System.Collections.Generic;
using UnityEngine;
public class GameTools
{
    private static GameTools instance;


    public static GameTools Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameTools();
            }
            return instance;
        }
    }

    /// <summary>
    /// 当前时间转换为毫秒级时间戳 13位
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public long ConvertDateTimeToInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位     
        return t;
    }

    /// <summary>
    /// 当前是否有音乐在播放
    /// </summary>
    /// <returns></returns>
    public bool MusicStopOrHoldOn()
    {
        return false;
        //return WindowManager.instance.Get<AudicoManager>().MusicAudio.isPlaying;
    }
    /// <summary>
    /// 上次录音的音频记录可以播放，java实现的
    /// </summary>
    public void RecordPlay()
    {
        IFiyManager._instance.PlaySound();
    }
    /// <summary>
    /// 讯飞录音开始
    /// </summary>
    /// <param name="currentTrf">传入一个当前使用讯飞的对象</param>
    /// <param name="tft"></param>
    public void RecordStart(Transform currentTrf, TeachType tft)
    {
        IFiyManager._instance.StartSpeak(currentTrf, tft);
    }
    /// <summary>
    /// 讯飞录音取消
    /// </summary>
    public void RecordCancelSpeechListener()
    {
        IFiyManager._instance.CancelSpeechListener();
    }
    /// <summary>
    /// 播放MP4 仅支持移动平台使用
    /// </summary>
    public void MP4Play(string path)
    {
        Handheld.PlayFullScreenMovie(path, Color.black, FullScreenMovieControlMode.Hidden);
    }
    /// <summary>
    /// 上漂的小提示
    /// </summary>
    /// <param name="tips">提示的字体</param>
    public void TipsShow(string tips)
    {
        GameObject obj = (GameObject.Instantiate(Resources.Load("UI/TipsPanel")) as GameObject);
        obj.transform.GetComponent<UITips>().Init(tips);
        obj.transform.parent = GameObject.Find("GUI/GUICamera").transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

    }

    /// <summary>
    /// 上漂的小提示
    /// </summary>
    /// <param name="tips">提示的字体</param>
    public void TipsShow1(string tips)
    {
        GameObject obj = (GameObject.Instantiate(Resources.Load("UI/TipsPanel1")) as GameObject);
        obj.transform.GetComponent<UITips>().Init(tips,2);
        obj.transform.parent = GameObject.Find("GUI/GUICamera").transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

    }
    /// <summary>
    /// 消息盒子，可以带方法
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="quxiao_callback">取消的回调</param>
    /// <param name="queding_callback">确定的回调</param>
    /// <param name="title">标题</param>
    /// <param name="backIsNull">返回按钮是否存在</param>
    /// <param name="quxiaoBtnName">取消按钮的名字</param>
    /// <param name="quedingBtnName">确定按钮的名字</param>
    //public void MsgShow(
    //                 string content                  /*内容*/,
    //                 string title = null,            /*标题*/
    //                 GameObject currentObj = null,   /*调用该方法的对象，对应的回调方法放在该对象身上*/
    //                 string quxiao_callback = null,  /*取消的回调*/
    //                 string queding_callback = null, /*确定的回调*/

    //                 bool backIsNull = true,         /*返回按钮是否存在*/
    //                 string quxiaoBtnName = null,      /*取消按钮的名字*/
    //                 string quedingBtnName = null      /*确定按钮的名字*/
    //    )
    //{
    //    WindowManager.instance.Open<MessageBoxPanel>().InitClassItem(content, title, currentObj, quxiao_callback, queding_callback, backIsNull, quxiaoBtnName, quedingBtnName);
    //}
    public MessageBoxPanel MsgShow(
                     string content                  /*内容*/,
                     string title = null,            /*标题*/
                     Action quxiao_callback = null,  /*取消的回调*/
                     Action queding_callback = null, /*确定的回调*/
                     bool backIsNull = true,         /*返回按钮是否存在*/
                     string quxiaoBtnName = null,      /*取消按钮的名字*/
                     string quedingBtnName = null      /*确定按钮的名字*/
        )
    {
        return WindowManager.instance.Open<MessageBoxPanel>().Init(content, title, quxiao_callback, queding_callback, backIsNull, quxiaoBtnName, quedingBtnName);
    }
    //public void TestFunc(Action cc)
    //{
    //    //cc;
    //    cc.Invoke();
    //}

    /// <summary>
    /// 按钮的果冻效果
    /// </summary>
    /// <param name="go">按钮的gameobject</param>
    public void ButtonJelly(GameObject go)
    {
        if (go.GetComponent<JellyAnimition>() == null)
        {
            go.AddComponent<JellyAnimition>();

        }
        go.GetComponent<JellyAnimition>().trans = go.transform;
        go.GetComponent<JellyAnimition>().OnClick();
    }
    /// <summary>
    /// 句子换成一串字符list
    /// </summary>
    /// <param name="str"></param>
    public List<string> RemoveMark(string str)
    {
        str = str.ToLower().Replace("-", "").Replace(".", " ").Replace(",", " ").Replace("!", " ").Replace(";", " ").Replace("?", " ");
        string[] all = str.Split(' ');
        List<string> ret = new List<string>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != "" && all[i] != " ")
            {
                ret.Add(all[i]);
            }

        }
        return ret;
    }
    /// <summary>
    /// 颜文字
    /// </summary>
    /// <param name="str"></param>
    public string ColorText(string str,bool bl)//1文字，2对错
    {
        if (!bl)
        {
            return "[D92A2A]" + str + " [-]";
        }
        else
        {
            return "[2AD93C]" + str + " [-]";
        }

    }

    /// <summary>
    /// 颜文字
    /// </summary>
    public string SpeakColorText(string str, int type)
    {
        if (type == 1)
        {
            return "[2D9A69]" + str + " [-]";
        }
        else if(type == 2)
        {
            return "[DB8442]" + str + " [-]";
        } else if (type == 3)
        {
            return "[FF241A]" + str + " [-]";
        } else if (type == 4)
        {
            return "[FF2300]" + str + " [-]";
        } else if (type == 5)
        {
            return "[000000]" + str + " [-]";
        }
        return str;
    }
}