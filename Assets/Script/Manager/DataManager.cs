using ProtoSprotoType;
using System;
using System.Collections.Generic;

public class DataManager
{
    public static DataManager instance;
    /// <summary>
    /// 角色数据
    /// </summary>
    public RoleAttribute roleData;
    /// <summary>
    /// 当前学习进度数据
    /// </summary>
    public CurStudyProgress curStudyProgress;
    /// <summary>
    /// 课文数据
    /// </summary>
    public SyncMoudle1Info.request textData;
    /// <summary>
    /// 单词1数据
    /// </summary>
    public SyncMoudle2Info.request wordData1;
    /// <summary>
    /// 句式1数据
    /// </summary>
    public SyncMoudle3Info.request sentenceData1;
    /// <summary>
    /// 对话123数据
    /// </summary>
    public SyncMoudle4Info.request dialogueData;
    /// <summary>
    /// 单词2数据
    /// </summary>
    public SyncMoudle5Info.request wordData2;
    /// <summary>
    /// 句式23数据
    /// </summary>
    public SyncMoudle6Info.request sentenceData2;
    /// <summary>
    /// 单词3数据
    /// </summary>
    public SyncMoudle7Info.request wordData3;
    /// <summary>
    /// 单词测试数据
    /// </summary>
    public SyncMoudle8Info.request wordTestData;
    /// <summary>
    /// 商店数据
    /// </summary>
    public GetShopList.response shopInfoData;
    /// <summary>
    /// 背包数据
    /// </summary>
    public GetBagInfo.response bagInfoData;

    /// <summary>
    /// 单元解锁数据
    /// </summary>
    public Dictionary<int, unitPass> unitPassList;
    /// <summary>
    /// 难度解锁数据
    /// </summary>
    public Dictionary<int, unitPass> levelPassList;
    /// <summary>
    /// 模块解锁数据
    /// </summary>
    public Dictionary<int, passUnit> modulePassList;
    /// <summary>
    /// 该难度的所有模块是否都是满星（15）
    /// </summary>
    public bool isFullStar;
    /// <summary>
    /// 是否是最后一个单元
    /// </summary>
    public bool isLastUnit = false;
    /// <summary>
    /// 下载资源信息列表
    /// </summary>
    public Dictionary<string, DownLoadInfo> downLoadInfoList;

    public static DataManager GetInstance()
    {
        if (instance==null)
        {
            instance = new DataManager
            {
                roleData = new RoleAttribute(),
                curStudyProgress = new CurStudyProgress(),
                downLoadInfoList = new Dictionary<string, DownLoadInfo>()
            };
        }

        return instance;
    }
}

/// <summary>
/// 角色属性
/// </summary>
public class RoleAttribute
{
    public Int64 curGrade;      //当前选择的年级
    public Int64 curTerm;       //当前选择的学期
    public Int64 curUnit;       //当前选择的单元
    public Int64 totalUnitNum;       //当前选择的单元
    public string curLevel;     //当前选择的等级
    public double curMoudleId;     //当前选择的模块

    public string ID;
    public string NickName;       // 玩家昵称
    public bool IsTraveler;
    public Int64 Sex;                   // 玩家性别
    public Int64 Diamond;          // 玩家钻石
    public string Phone;             // 玩家电话
    public bool IsVIP;                 // 是否是高级账号
    public bool IsSpeak;                 // 是否启用语音识别

    public Int64 curProgress;        //当前学习模块的学习单词序号
    public List<long> Scores;      //当前学习模块的目前所得分数

    public string cur_diamond;    //当前选择模块的钻石数量
    public string cur_card;       //当前选择模块的卡片（忘记干嘛用了）
}

/// <summary>
/// 当前学习进度
/// </summary>
public class CurStudyProgress
{
    public TeachType teachType;  //当前学习的类型
    public int totalNum;               // 当前学习总共的数量
    public int completeNum;       // 当前学习完成的数量
    public int passNum;               // 当前学习通过的数量
}

/// <summary>
/// 下载资源信息
/// </summary>
public class DownLoadInfo
{
    public String url;                     // 下载链接
    public long size;                     // 下载大小

    public DownLoadInfo(string url, long size)
    {
        this.url = url;
        this.size = size;
    }
}