using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class IFiyManager : MonoBehaviour
{

    public static IFiyManager _instance;
    private AndroidJavaClass jc;
    private AndroidJavaObject jo;
    private string showResult = ""; //当前语音结果返回对象，为""时才能用；结束时记得重置
    private TeachType _tftPanel;    //是哪个脚本要用这个讯飞
    private Transform _current_Trf;  //语音对象全局只能有一个地方调用，当一个语音对象存在的时候（即不为null），不能调用此方法，回调结束记得释放 
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this);
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //CallJava("startSpeechListener");
    }
    /// <summary>
    /// 调用继承UnityPlayer的java类方法
    /// </summary>
    /// <param name="method"></param>
    private void CallJava(string method)
    {
        //jo.Call();
        jo.Call(method);
    }
    /// <summary>
    /// 这里是各种状态返回
    /// </summary>
    /// <param name="recognizerResult"></param>
    public void Result(string recognizerResult)
    {
        showResult += recognizerResult;
        Debug.LogError(recognizerResult);
    }
    /// <summary>
    /// 安卓sdk那边表示这次结果返回结束此时我们应该把showResult返回，我是把值传给调用接口传入的TransForm下的uilable
    /// </summary>
    public void Stop()
    {
        //_current_Trf.GetComponent<UILabel>().text = showResult;
        //WindowManager.instance.Get<MessageBoxPanel>().InitClassItem(showResult, wordGameInstance.gameObject, "Onquxiao", "Onqueding", "语音返回", true, "返回", "确定");
        showResult = "";
        _current_Trf = null;
    }
    /// <summary>
    /// 开始录音 外部调用接口
    /// </summary>
    /// 返回值 0表示有其他地方引用 1表示可以使用录音
    public int StartSpeak(Transform currentTrf, TeachType tft)
    {
        _tftPanel = tft;

        if (_current_Trf == null)
        {
            _current_Trf = currentTrf;
            CallJava("startSpeechListener");
            return 1;
        }
        else
        {
            return 0;
        }


    }
    /// <summary>
    /// 取消听写 外部调用接口
    /// </summary>
    /// 
    public void CancelSpeechListener()
    {
        if (_current_Trf != null)
        {
            _current_Trf = null;
            showResult = "";
            CallJava("CancelSpeechListener");
        }

    }
    /// <summary>
    /// 停止听写 外部调用接口
    /// </summary>
    /// 
    public void SpeechListener()
    {
        if (_current_Trf != null)
        {
            _current_Trf = null;
            showResult = "";
            CallJava("SpeechListener");
        }

    }
    /// <summary>
    /// 获取验证码
    /// </summary>
    /// 
    public void GetCode(string phoneNum)
    {
        jo.Call("getCode", phoneNum);
    }
    /// <summary>
    /// 收到讯飞听写的内容
    /// </summary>
    /// <param name="words"></param>
    public void Words(string words)
    {
        //string words = DataManager.instance._jushi_infos.contentInfo[UIWordGame.wordGameInstance._jushi_cur_num].rightText2;//测试
        //string words = WindowManager.instance.Get<UIWordGamePanel>().wordGameInstance._cur_word;//测试
        switch (_tftPanel)
        {
            case TeachType.Word1://单词发送给哪个脚本
                if (WindowManager.instance.Get<UIWordGamePanel>() != null)
                {
                    words = words.Replace(" ", "").Replace("。", "").Replace(".", "").ToLower();
                    Debug.LogError("***"+words + "***单词");
                }

                break;
            case TeachType.Animation://动画发送给哪个脚本
                break;
            case TeachType.Dialogue://对话发送给哪个脚本
                if (WindowManager.instance.Get<UIWordGamePanel>() != null)
                {

                    List<string> wordAll = new List<string>();
                    wordAll = GameTools.Instance.RemoveMark(words);
                    Debug.LogError("***（" + words + "）***对话");
                }
                break;
            case TeachType.Sentence1://句式发送给哪个脚本
                if (WindowManager.instance.Get<UIWordGamePanel>() != null)
                {
                    List<string> juShi = new List<string>();
                    juShi = GameTools.Instance.RemoveMark(words);
                    Debug.LogError("***" + words + "***句子");
                }
                break;
            case TeachType.Text://课文发送给哪个脚本
                if (WindowManager.instance.Get<UIWordGamePanel>() != null)
                {

                    List<string> wordAll = new List<string>();
                    wordAll = GameTools.Instance.RemoveMark(words);
                    Debug.LogError("***（" + words + "）***课文");
                }
                break;
            case TeachType.Null:
                break;
            default:
                break;
        }

        _current_Trf = null;
        _tftPanel = TeachType.Null;

    }

    /// <summary>
    /// 播放上次录音
    /// </summary>
    public void PlaySound()
    {
        CallJava("playSound");
    }


    public void STATR()
    {
        Debug.LogError("开始播放");
        StartCoroutine("playMovice");
    }
    private IEnumerator playMovice()
    {
        //Handheld.PlayFullScreenMovie("test.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        Handheld.PlayFullScreenMovie("test.mp4", Color.black, FullScreenMovieControlMode.Hidden);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.LogError("播放完视频了");
        GameTools.Instance.TipsShow("播放完视频了");
    }
}
