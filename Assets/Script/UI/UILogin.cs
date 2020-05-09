using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class UILogin : MonoBehaviour {

    public static UILogin _instance;

    //在这里拖拽，但是点击事件写在Panel脚本里，这里可以写协程，用协程
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        Invoke("OnDestroy", 1f);
    }
    private void OnDestroy()
    {
        _instance = null;
    }
}
