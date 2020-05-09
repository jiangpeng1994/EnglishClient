using ProtoSprotoType;
using Sproto;
using System.Collections.Generic;
using UnityEngine;

public class ClassItem : MonoBehaviour
{
    public UISprite _classNum;
    public UILabel _classLabel;
    public SelectCoursePanel _selectCoursePanel;

    private int grade;
    private int term;
    private const string str1 = "年级（上）";
    private const string str2 = "年级（下）";
    private Dictionary<int, unitPass> _unitPassList = new Dictionary<int, unitPass>();

    /// <summary>
    /// 初始化年级Item
    /// </summary>
    /// <param name="grade">年级</param>
    /// <param name="term">学期</param>
    public void InitClassItem(int grade, int term)
    {
        this.grade = grade;
        this.term = term;

        if (term == 1)
        {
            // 上学期
            _classLabel.text = str1;
        }
        else if (term == 2)
        {
            // 下学期
            _classLabel.text = str2;
        }
        _classNum.spriteName = grade.ToString();

        UIEventListener.Get(gameObject).onClick = SelectClassOnClick;
    }

    /// <summary>
    /// 选择年级
    /// </summary>
    /// <param name="btn"></param>
    private void SelectClassOnClick(GameObject btn)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Debug.Log("请求：" + grade + _classLabel.text + "的单元数据");

        if (DataManager.GetInstance().roleData.IsVIP)
        {
            Debug.Log("VIP:开启所有单元");
            _selectCoursePanel.OpenUnitSelectUI(grade, term, 10);
        } else
        {
            GetTermPassInfo.request msg = new GetTermPassInfo.request();
            moudle_base moudle = new moudle_base
            {
                grade = grade,
                term = term,
            };
            msg.moudleBase = moudle;
            NetSender.Send<ProtoProtocol.GetTermPassInfo>(msg, GradeInfoResponseHandler);
        }
    }

    /// <summary>
    /// 收到单元信息
    /// </summary>
    /// <param name="msg"></param>
    private void GradeInfoResponseHandler(SprotoTypeBase msg)
    {
        Debug.Log("收到：" + grade + _classLabel.text + "的单元数据");
        var data = (GetTermPassInfo.response)msg;
        DataManager.GetInstance().roleData.totalUnitNum = data.unitPassList.Count;
        Debug.Log("单元数量" + DataManager.GetInstance().roleData.totalUnitNum);

        _unitPassList = new Dictionary<int, unitPass>();
        foreach (unitPass item in data.unitPassList)
        {
            int index = (int)item.index;
            _unitPassList.Add(index, item);
        }

        int curIndex = _unitPassList.Count + 1; ;
        for (int i = 1; i <= _unitPassList.Count; i++)
        {
            Debug.Log("单元" + _unitPassList[i].index + ":" + _unitPassList[i].isPass.ToString());
            if (_unitPassList[i].isPass == false)
            {
                curIndex = i;
                break;
            }
        }

        DataManager.GetInstance().unitPassList = _unitPassList;

        _selectCoursePanel.OpenUnitSelectUI(grade, term, curIndex);
    }
}