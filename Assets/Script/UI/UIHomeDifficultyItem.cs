using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeDifficultyItem : MonoBehaviour {

    public SelectCoursePanel m_Home;
    public UISprite m_SuoUISprite;
    public BoxCollider m_SuoBoxCollider;
    public GameObject m_JianTou;
    public GameObject m_Pass;
    public UISprite m_BG;

    /// <summary>
    /// 选择的难度
    /// </summary>
    private int _levelIndex;
    /// <summary>
    /// 是否是最后一个单元
    /// </summary>
    private bool _isLastUnit = false;
    /// <summary>
    /// 该难度的模块通过列表
    /// </summary>
    private Dictionary<int, passUnit> _modulePassList;

    /// <summary>
    /// 初始化难度Item
    /// </summary>
    /// <param name="difficultyInfo"></param>
    public void Init(int index, int curNum)
    {
        _levelIndex = index;
        if (DataManager.GetInstance().roleData.curUnit < DataManager.GetInstance().roleData.totalUnitNum)
        {
            _isLastUnit = false;
        } else
        {
            _isLastUnit = true;
        }

        // VIP模式
        if (DataManager.GetInstance().roleData.IsVIP)
        {
            _isLastUnit = false;
        }

        if (index < curNum)
        {
            // 通过
            m_SuoUISprite.color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
            m_SuoBoxCollider.enabled = true;
            m_JianTou.SetActive(false);
            m_Pass.SetActive(true);
        }
        else if (index == curNum)
        {
            // 当前
            m_SuoUISprite.color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
            m_SuoBoxCollider.enabled = true;
            m_JianTou.SetActive(true);
            m_Pass.SetActive(false);
        }
        else
        {
            // 上锁
            m_SuoUISprite.color = new Color(108f / 255, 108f / 255, 108f / 255, 255 / 255);
            m_SuoBoxCollider.enabled = false;
            m_JianTou.SetActive(false);
            m_Pass.SetActive(false);
        }

        UIEventListener.Get(gameObject).onClick = OnClickSelect;
        gameObject.GetComponent<UIButton>().defaultColor = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
    }

    /// <summary>
    /// 选择难度
    /// </summary>
    /// <param name="btn"></param>
    public void OnClickSelect(GameObject btn)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：难度" + _levelIndex + "的模块数据");

        GetLevelPassInfo.request msg = new GetLevelPassInfo.request();
        moudle_base moudle = new moudle_base
        {
            grade = DataManager.GetInstance().roleData.curGrade,
            term = DataManager.GetInstance().roleData.curTerm,
            unit = DataManager.GetInstance().roleData.curUnit,
            moudleId = _levelIndex,
        };
        msg.moudleBase = moudle;
        NetSender.Send<ProtoProtocol.GetLevelPassInfo>(msg, DifficultyInfoInfoResponseHandler);
    }

    /// <summary>
    ///  收到模块信息
    /// </summary>
    /// <param name="msg"></param>
    public void DifficultyInfoInfoResponseHandler(SprotoTypeBase msg)
    {
        Debug.Log("收到：难度" + _levelIndex + "的模块数据");
        var data = (GetLevelPassInfo.response)msg;

        _modulePassList = new Dictionary<int, passUnit>();
        foreach (passUnit item in data.passList)
        {
            int index = int.Parse(item.moudleId.ToString().Substring(2, 1));

            // 最后1个单元没有句式模块，默认3星通过
            if (_isLastUnit && _levelIndex != 4 && index == 4)
            {
                item.isPass = true;
            }

            if (_isLastUnit && _levelIndex == 4 && index == 2)
            {
                item.isPass = true;
            }

            // isPass为true就设置为3星
            if (item.isPass)
            {
                item.star = 3;
            }
            
            _modulePassList.Add(index, item);
        }

        int curIndex = _modulePassList.Count + 1;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            Debug.Log("模块"+i + ":" + "是否通过" + _modulePassList[i].isPass.ToString());
            Debug.Log("模块" + i + ":" + "星星" + _modulePassList[i].star.ToString());
            if (_modulePassList[i].isPass == false && _modulePassList[i].star <= 0)
            {
                curIndex = i;
                break;
            }
        }

        int fullStar = 0;
        if (_levelIndex == 4)
        {
            fullStar = 9;
        } else
        {
            fullStar = 15;
        }

        int starNum = 0;
        for (int i = 1; i <= _modulePassList.Count; i++)
        {
            starNum = starNum + (int)_modulePassList[i].star;
        }
        if (starNum >= fullStar)
        {
            DataManager.GetInstance().isFullStar = true;
        } else
        {
            DataManager.GetInstance().isFullStar = false;
        }

        DataManager.GetInstance().modulePassList = _modulePassList;
        DataManager.GetInstance().isLastUnit = _isLastUnit;

        // 1-3模块：1-6   4模块：3-6
        if (_levelIndex == 4)
        {
            curIndex = curIndex + 2;
        }

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有模块");
            m_Home.OpenModuleSelectUI(_levelIndex, _isLastUnit, 6, _modulePassList);
        } else
        {
            m_Home.OpenModuleSelectUI(_levelIndex, _isLastUnit, curIndex, _modulePassList);
        }
    }
}