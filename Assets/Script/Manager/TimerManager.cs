using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;


/// <summary>
/// 时间准确的计时器，使用Time.realtimeSinceStartup来计时
/// </summary>
public class TimerManager
{
    private static Dictionary<string, TimerUnAffectItem> dictList = new Dictionary<string, TimerUnAffectItem>();

    /// <summary>
    /// 注册计时
    /// </summary>
    /// <param name="timerKey">计时器索引，用来取消注册</param>
    /// <param name="totalNum">总时间</param>
    /// <param name="delayTime">间隔时间</param>
    /// <param name="callback">间隔时间执行回调，会传一个所剩时间（float）</param>
    /// <param name="endCallback">结束回调</param>
	public static void Register(string timerKey, float totalTime, float delayTime, Action<float> callback, Action endCallback)
    {
        TimerUnAffectItem timerItem = null;

        //删除已有
        if (dictList.ContainsKey(timerKey))
        {
            UnRegister(timerKey);
            dictList.Remove(timerKey);
        }

        GameObject objectItem = new GameObject();
        objectItem.name = timerKey;

        timerItem = objectItem.AddComponent<TimerUnAffectItem>();
        dictList.Add(timerKey, timerItem);

        if (timerItem != null)
        {
            timerItem.Run(totalTime, delayTime, callback, endCallback);
        }
    }

    /// <summary>
    /// 取消注册计时
    /// </summary>
    /// <param name="timerKey">Timer key.</param>
    public static void UnRegister(string timerKey)
    {
        if (!dictList.ContainsKey(timerKey)) return;

        TimerUnAffectItem timerItem = dictList[timerKey];
        if (timerItem != null)
        {
            timerItem.Stop();
            GameObject.Destroy(timerItem.gameObject);
        }
    }
    /// <summary>
    /// 清除所有计时
    /// </summary>
    public static void ClearAllTimer()
    {
        if (dictList.Keys.Count == 0)
        {
            return;
        }
        List<string> keys = new List<string>(dictList.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            TimerUnAffectItem timerItem = dictList[keys[i]];
            if (timerItem != null)
            {
                timerItem.Stop();
                GameObject.Destroy(timerItem.gameObject);
            }
        }
    }
}

class TimerUnAffectItem : MonoBehaviour
{
    private float startTime;
    private float totalTime;
    private float delayTime;
    private Action<float> callback;
    private Action endCallback;

    // private int currentIndex;

    public void Run(float totalTime, float delayTime, Action<float> callback, Action endCallback)
    {
        this.Stop();

        //    this.currentIndex = totalNum;
        this.startTime = Time.realtimeSinceStartup;
        this.totalTime = totalTime;
        this.delayTime = delayTime;
        this.callback = callback;
        this.endCallback = endCallback;

        this.StartCoroutine("EnumeratorAction");
    }

    public void Stop()
    {
        this.StopCoroutine("EnumeratorAction");
    }

    private IEnumerator EnumeratorAction()
    {
        yield return new WaitForSeconds(this.delayTime);
        float curTotalTime = Time.realtimeSinceStartup - startTime;
        if (curTotalTime < totalTime)
        {
            if (this.callback != null) this.callback(totalTime - curTotalTime);
        }
        else
        {
            if (this.endCallback != null) this.endCallback();
            yield break;
        }

        this.StartCoroutine("EnumeratorAction");
    }
}