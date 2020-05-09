using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxPanel : BaseWnd
{
    private MessageBox _instance;

    private GameObject _currentObj; //当前调用的对象，注意这个obj对象在回调的时候不能摧毁，否则回调方法找不到会报错
    private string _current_quxiao_callback; //当前取消的回调
    private string _current_queding_callback; //当前确定的回调

    private Vector3 _quxiao_vec;                //记录一下取消按钮在左边的位置
    private Vector3 _queding_vec;               //记录一下确定按钮在右边的位置
    /// <summary>
    ///  唤醒box
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="currentObj">调用该方法的对象</param>
    /// <param name="quxiao_callback">取消的回调</param>
    /// <param name="queding_callback">确定的回调</param>
    /// <param name="title">标题</param>
    /// <param name="backIsNull">返回按钮是否存在</param>
    /// <param name="quxiaoBtnName">取消按钮的名字</param>
    /// <param name="quedingBtnName">确定按钮的名字</param>
    public MessageBoxPanel Init(string content                  /*内容*/,
                     string title = null,            /*标题*/
                     Action quxiao_callback = null,        /*取消的回调*/
                     Action queding_callback = null,        /*确定的回调*/
                     bool backIsNull = true,         /*返回按钮是否存在*/
                     string quxiaoBtnName = null,      /*取消按钮的名字*/
                     string quedingBtnName = null    /*确定按钮的名字*/)
    {
        _instance = MessageBox._instance;


        //_currentObj = currentObj;
        //_current_quxiao_callback = quxiao_callback;
        //_current_queding_callback = queding_callback;


        _instance.my_text.text = content;

        if (title == null)
        {
            _instance.title.text = "提示";
        }
        else
        {
            _instance.title.text = title;
        }
        if (quxiaoBtnName != null)
        {
            _instance.quxiao.transform.Find("Label").GetComponent<UILabel>().text = quxiaoBtnName;
        }
        if (quedingBtnName != null)
        {
            _instance.queding.transform.Find("Label").GetComponent<UILabel>().text = quedingBtnName;
        }
        if (backIsNull)
        {

            _instance._btn_back.SetActive(true);
            UIEventListener.Get(_instance._btn_back).onClick = OnClickBtn;
        }
        else
        {

            _instance._btn_back.SetActive(false);

        }


        if (quxiao_callback != null)
        {
            _instance.quxiao.SetActive(true);
            UIEventListener.Get(_instance.quxiao).onClick = (GameObject obj) =>
            {
                AudicoManager.instance.Play("effect", "Effect/press button");
                quxiao_callback.Invoke();
                OnHide();
            };
        }
        else
        {
            _instance.quxiao.SetActive(false);

        }



        if (queding_callback != null)
        {
            _instance.queding.SetActive(true);
            UIEventListener.Get(_instance.queding).onClick = (GameObject obj) =>
            {
                AudicoManager.instance.Play("effect", "Effect/press button");
                queding_callback.Invoke();
                OnHide();
            };
        }
        else
        {
            _instance.queding.SetActive(false);
        }

        if (queding_callback != null && quxiao_callback != null)
        {
            _instance.quxiao.transform.localPosition = new Vector3(-174,-142,0);
            _instance.queding.transform.localPosition = new Vector3(183, -142, 0);
        } else
        {
            _instance.quxiao.transform.localPosition = new Vector3(0, -142, 0);
            _instance.queding.transform.localPosition = new Vector3(0, -142, 0);
        }

        _transform.localScale = new Vector3(1, 1, 1);
        _transform.localPosition = new Vector3(0, 0, 0);

        return this;
    }

    public void ChangeContent(string content)
    {
        _instance.my_text.text = content;
    }
    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    /// <param name="obj"></param>
    public void OnQuxiaoClickBtn(GameObject obj)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        _currentObj.SendMessage(_current_quxiao_callback);
        OnHide();
    }
    /// <summary>
    /// 确定按钮绑定事件
    /// </summary>
    /// <param name="obj"></param>
    public void OnQuedingClickBtn(GameObject obj)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        _currentObj.SendMessage(_current_queding_callback);
        OnHide();
    }
    public void OnClickBtn(GameObject obj)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        OnHide();
    }
    /// <summary>
    /// 界面隐藏
    /// </summary>
    private void OnHide()
    {
        _currentObj = null;
        _current_quxiao_callback = null;
        _current_queding_callback = null;
        _instance.my_text.text = "";
        _transform.localScale = new Vector3(0, 0, 0);
        _transform.localPosition = new Vector3(10000, 10000, 0);
    }
}
