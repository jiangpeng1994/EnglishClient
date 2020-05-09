using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 教学类型
/// </summary>
public enum TeachType
{
    Animation = 0,
    Text = 1,
    Word1 = 2,
    Word2 = 3,
    Word3 = 4,
    Sentence1 = 5,
    Sentence2 = 6,
    Dialogue = 7,
    WordTest = 8,
    SentenceTest = 9,
    DialogueTest = 10,
    Null = 11,  
}

public partial class UIWordGamePanel : BaseWnd
{
    public UIWordGame wordGameInstance;

    /// <summary>
    /// 教学场景初始化，传入教学类型选择不同的教学场景
    /// </summary>
    /// <param name="type">教学类型</param>
    public void Init(TeachType teachType, Queue<int> StudyQueue, int time)
    {
        AudicoManager.instance.StopBGMusic();
        wordGameInstance = UIWordGame._instance;
        wordGameInstance.SetTime(time);
        wordGameInstance.SetTeachTypeIconAndDiamond("");
        wordGameInstance.StartStudyReady(teachType, StudyQueue);
    }
}