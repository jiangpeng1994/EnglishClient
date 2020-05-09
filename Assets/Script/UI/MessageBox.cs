using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour {
    public static MessageBox _instance;
    public GameObject _btn_back;    //返回按钮  
    public UILabel my_text;           //消息盒子信息
    public GameObject quxiao;       //取消按钮
    public GameObject queding;      //确定按钮
    public UILabel title;        //标题
	// Use this for initialization
	void Awake () {
        if (_instance==null)
        {
            _instance = this;
        }
    }
}
