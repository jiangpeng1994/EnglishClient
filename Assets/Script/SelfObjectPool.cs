/********************************************************************
	created:	2015/06/19  11:47
	file base:	SelfObjectPool
	file ext:	cs
	author:		zjw
	
	purpose:	自定义对象的对象回收池，不支持多线程
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

public class SelfObjectPool<T> where T : class
{
    List<T> m_lstObject = new List<T>();
    int m_nThresholdValue = 50;
    public SelfObjectPool(int threshold = 50)
    {
        m_nThresholdValue = threshold;
    }

    public int Threshold
    {
        get
        {
            return m_nThresholdValue;
        }
        set
        {
            m_nThresholdValue = value;
        }
    }

    //写法比较山寨，但之前的写法太多地方用了，懒得改了
    public void PreCreateObject<T1>(int num) where T1 : class,new()
    {
        if (num > m_nThresholdValue)
            num = m_nThresholdValue;
#if UNITY_EDITOR
        if (num < 0)
            Debug.LogError("有意思吗？好玩吗？！");
#endif
        for (int i = 0;i<num;++i)
        {
            T1 ret = new T1();
            m_lstObject.Add(ret as T);
        }
    }

    public T1 CreateObject<T1>() where T1 : class,new()
    {
        if (m_lstObject.Count > 0)
        {
            T1 ret = m_lstObject[m_lstObject.Count - 1] as T1;
            m_lstObject.RemoveAt(m_lstObject.Count - 1);
            return ret;
        }
        return new T1();
    }

    public void RecoverObject(T obj)
    {
        if (obj != null && m_lstObject.Count < m_nThresholdValue)
        {
            m_lstObject.Add(obj);
        }
        else 
        {
            obj = null;
        }
    }

    public void CheckThreshold()
    {
        if (m_lstObject.Count > m_nThresholdValue)
        {
            m_lstObject.RemoveRange(0, m_nThresholdValue - m_lstObject.Count);
        }
    }

    public void ClearPool()
    {
        m_lstObject.Clear();
    }

    public int Length() 
    {
        return m_lstObject.Count;
    }
}
