using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;

public class SDKHandle : MonoBehaviour
{
    public static SDKHandle _instance;
    public Text evaluationContent;
    public Text voiceInputStatus;
    public Text speechEvaluatorResult;
    public Text payResult;

    /// <summary>
    /// android层SpeechEvaluatorSDK类
    /// </summary>
    private AndroidJavaObject SpeechEvaluatorSDK;
    /// <summary>
    /// android层IAppPaySDK类
    /// </summary>
    private AndroidJavaObject IAppPaySDK;
    /// <summary>
    /// android层SMSMessageSDK类
    /// </summary>
    private AndroidJavaObject SMSMessageSDK;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this);

        //通过该API来实例化导入arr中对应的类
        SpeechEvaluatorSDK = new AndroidJavaObject("com.ssm.speechrecognizer.SpeechEvaluatorSDK");
        IAppPaySDK = new AndroidJavaObject("com.ssm.speechrecognizer.IAppPaySDK");
        SMSMessageSDK = new AndroidJavaObject("com.ssm.speechrecognizer.SMSMessageSDK");
        // 调用Android层：科大讯飞语音评测SDK的初始化
        SpeechEvaluatorSDK.Call("InitSpeechEvaluator");
        // 调用Android层：爱贝支付SDK的初始化
        IAppPaySDK.Call("InitIAppPay");
        // 调用Android层：SMSSDK的初始化
        SMSMessageSDK.Call("InitSMSMessage");
    }

    /// <summary>
    /// 点击开始语音评测按钮
    /// </summary>
    public void StartSpeechEvaluator(string category, string content, string vocabulary = null)
    {
        string Content = "";
        // 调用Android层：设置语音评测内容
        if (category.Equals("read_word"))
        {
            if (vocabulary == null)
            {
                Content = "[word]" + Environment.NewLine + content;
            } else
            {
                Content = "[word]" + Environment.NewLine + content + Environment.NewLine + "[vocabulary]" + Environment.NewLine + vocabulary;
            }
        } else if(category.Equals("read_sentence"))
        {
            Content = "[content]" + Environment.NewLine + content;
        } else if(category.Equals("read_chapter"))
        {
            Content = "[content]" + Environment.NewLine + content;
        }

        Debug.LogWarning("语音评测内容为："+ Content);
        SpeechEvaluatorSDK.Call("SetEvaluationContent", Content);
        // 调用Android层：设置语音评测参数
        /**
        * language 评测语言:en_us（英语）、zh_cn（汉语）[必填]
        * category 评测题型:read_syllable（单字，汉语专有）、read_word（词语）、read_sentence（句子）、read_chapter（篇章）[必填]
        * result_level 评测结果等级:plain、complete，默认为complete。（中文仅支持complete）[选填]
        * vad_bos 语音前端点:语音前端点:静音超时时间，即用户多长时间不说话则当做超时处理。[选填]
        * vad_eos 语音后端点:后端点静音检测时间，即用户停止说话多长时间内即认为不再输入， 自动停止录音。[选填]
        * speech_timeout 语音输入超时时间:录音超时，当录音达到时限将自动触发vad停止录音，默认-1（无超时）。[选填]
        */
        SpeechEvaluatorSDK.Call("SetSpeechEvaluatorParams", "en_us", category, "plain", "3000", "1000", "8000");
        // 调用Android层：开始语音评测
        SpeechEvaluatorSDK.Call("StartSpeechEvaluator");
    }

    /// <summary>
    /// 点击支付按钮
    /// </summary>
    public void Pay()
    {
        // 获取当前时间戳作为订单编号
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string orderid = Convert.ToInt64(ts.TotalSeconds).ToString();

        // 调用Android层：调起支付
        /**
        * waresid 商品编号:应用中的商品编号。爱贝后台的商品编号。[必填]
        * waresname 商品名称:对于消费型_应用传入价格的计费方式有效，如果不传则展示后台设置的商品名称。[选填]
        * orderid 商户订单号:商户生成的订单号，需要保证系统唯一。[必填]
        * price 支付金额:对于消费型_应用传入价格的计费方式有效，其它计费方式不需要传入本参数（单位：元）。[选填]
        * appuserid 用户在商户应用的唯一标识:建议为用户帐号。[必填]
        * cpprivateinfo 商户私有信息:支付完成后发送支付结果通知时会透传给商户。[选填]
        */
        // waresname 和 cpprivateinfo 两个参数不填时，传空字符串【“”】
        // price 不填时，传【-1】
        IAppPaySDK.Call("Pay", 6, "", orderid, 0.01, "uesr123", "");
    }

    public void StartPay()
    {
        // 调用Android层：调起支付
        string transid = "";
        IAppPaySDK.Call("StartPay", transid);
    }

    // -------- 接收来着Android层的调用 -------- // 

    /// <summary>
    /// 安卓层回调：语音评测内容
    /// </summary>
    /// <param name="message">语音评测内容</param>
    public void EvaluationContent(string message)
    {
        evaluationContent.text = message;
    }

    /// <summary>
    /// 安卓层回调：语音输入状态
    /// </summary>
    /// <param name="message">语音输入状态</param>
    public void VoiceInputStatus(string message)
    {
        
    }


    /// <summary>
    /// 安卓层回调：语音评测结果
    /// </summary>
    /// <param name="message">语音评测结果</param>
    public void SpeechEvaluatorResult(string message)
    {
        Debug.LogWarning("讯飞语音的识别结果：" + message);
        string score = message.Substring(71, 3);
        UIWordGame._instance.SpeakEnd(float.Parse(score) * 20);
    }

    /// <summary>
    /// 安卓层回调：支付成功的回调
    /// </summary>
    /// <param name="message">成功信息</param>
    public void PaySuccess(string message)
    {
        payResult.text = message;
    }

    /// <summary>
    /// 安卓层回调：支付失败的回调
    /// </summary>
    /// <param name="message">失败信息</param>
    public void PayError(string message)
    {
        payResult.text = message;
    }

    /// <summary>
    /// 安卓层回调：支付结果的回调
    /// </summary>
    /// <param name="message">支付结果</param>
    public void PayResult(string message)
    {

    }

    /// <summary>
    /// 安卓层回调：支付请求参数回调
    /// </summary>
    /// <param name="message">支付请求参数</param>
    public void PayParam(string message)
    {

    }

    /**
     * 验证码获取成功
     */
    private void GetVerificationCodeSuccess(string message)
    {
        GameTools.Instance.TipsShow("激活码已发送");
    }

    /**
     * 验证码获取失败
     */
    private void GetVerificationCodeFail(string message)
    {
        GameTools.Instance.TipsShow("激活码发送失败，请检查网络连接");
    }

    /**
     * 验证码验证成功
     */
    private void verifySuccess(string message)
    {
        payResult.text = message;
    }

    /**
     * 验证码验证失败
     */
    private void verifyFail(string message)
    {
        payResult.text = message;
    }

    //开始获取验证码
    public void GetVerificationCode(string phoneNum)
    {
        SMSMessageSDK.Call("GetVerificationCode", phoneNum);
    }

    //开始验证验证码
    public void SubmitVerificationCode()
    {
        SMSMessageSDK.Call("SubmitVerificationCode", "17623668484", "8310");
    }
}