using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 所有界面类的基类
/// </summary>
public abstract class BaseWnd
{
    protected Transform _transform;

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="wndName"></param>
    public void Open(Transform canvas, string wndName)
    {
        _transform = (GameObject.Instantiate(Resources.Load("UI/" + wndName)) as GameObject).transform;
        _transform.parent = canvas;
        //_transform.localPosition = Vector3.zero;
        _transform.localScale = Vector3.one;

        _transform.name = wndName;

        _transform.localPosition = new Vector3(-1280, 0, 0);
        _transform.DOLocalMove(new Vector3(0, 0, 0), 0.4f);
    }

    /// <summary>
    /// 显示窗口
    /// </summary>
    /// 
    public virtual void ShowUI()
    {
        if (_transform != null && _transform.gameObject != null)
            _transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// 
    public virtual void Close()
    {
        if (_transform != null && _transform.gameObject != null)
            _transform.gameObject.SetActive(false);
    }
    /// <summary>
    /// 删除窗口
    /// </summary>
    public virtual void Delete()
    {
        if(_transform != null &&  _transform.gameObject != null)
            GameObject.Destroy(_transform.gameObject);
    }

    public virtual void Update() { }
    public virtual void Init() { }
}



public class WindowManager : Singleton<WindowManager>
{
    private Transform _canvas;

    // 保存所有的打开的窗口
    private Dictionary<string, BaseWnd> _windows = new Dictionary<string, BaseWnd>();

    public UICamera uiCamera;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        MonoBehaviour.DontDestroyOnLoad(GameObject.Find("GUI"));
        _canvas = GameObject.Find("GUI/GUICamera").transform;
        uiCamera = GameObject.Find("GUI/GUICamera").GetComponent<UICamera>();
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Open<T>() where T : BaseWnd, new()
    {
        string wndName = typeof(T).Name;
        if(_windows.ContainsKey(wndName))
        {
            _windows[wndName].ShowUI();
            return _windows[wndName] as T;
        }
        else
        {
            T wnd = new T();
            wnd.Open(_canvas, wndName);
            _windows.Add(wndName, wnd);
            return wnd;
        }
    }

    /// <summary>
    /// 关闭窗口 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Close<T>() where T : BaseWnd
    {
        string wndName = typeof(T).Name;
        if (_windows.ContainsKey(wndName))
        {
            _windows[wndName].Close();
            //_windows.Remove(wndName);      //关闭只是隐藏显示  不用删除
        }
    }
    /// <summary>
    /// 删除预设
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Delete<T>() where T : BaseWnd
    {
        string wndName = typeof(T).Name;
        if (_windows.ContainsKey(wndName))
        {
            _windows[wndName].Delete();
            _windows.Remove(wndName);
        }
    }

    public T Get<T>() where T : BaseWnd
    {
        string wndName = typeof(T).Name;
        if (_windows.ContainsKey(wndName))
        {
            return _windows[wndName] as T;
        }
        else
        {
            return null;
        }
    }

    public void Update()
    {
        foreach(BaseWnd wnd in _windows.Values)
        {
            wnd.Update();
        }
    }

    public void Clear()
    {
        _windows.Clear();
    }
}
