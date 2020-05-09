using ProtoSprotoType;
using System.Collections.Generic;
using UnityEngine;

public class SelectCoursePanel : MonoBehaviour {

    public static SelectCoursePanel instance;
    // 顶部元素
    public UIButton _backBtn;
    public UIButton _helpBtn;
    public UILabel _classLabel;
    public UILabel _unitLabel;
    public UILabel _levelLabel;

    // 年级选择界面
    public GameObject _classSelectUI;
    public UIButton _classLeftBtn;
    public UIButton _classRightBtn;
    public GameObject _classItem;
    public UIGrid _classGrid;
    public UIScrollBar _classScrollBar;
    public UIPanel _scrollPanel;
    public UIScrollView _scrollView;

    // 单元选择界面
    public GameObject _unitSelectUI;
    public UIHomeUntilItem m_Unit1;
    public UIHomeUntilItem m_Unit2;
    public UIHomeUntilItem m_Unit3;
    public UIHomeUntilItem m_Unit4;
    public UIHomeUntilItem m_Unit5;
    public UIHomeUntilItem m_Unit6;
    public UIHomeUntilItem m_Unit7;

    // 等级选择界面
    public GameObject _levelSelectUI;
    public UIHomeDifficultyItem m_Level1;
    public UIHomeDifficultyItem m_Level2;
    public UIHomeDifficultyItem m_Level3;
    public UIHomeDifficultyItem m_Level4;

    // 模块选择界面
    public GameObject _moduleSelectUI;
    public UIHomeModuleItem[] m_Modules;
    public UIHomeModuleItem m_Module1;
    public UIHomeModuleItem m_Module2;
    public UIHomeModuleItem m_Module3;
    public UIHomeModuleItem m_Module4;
    public UIHomeModuleItem m_Module5;
    public UIHomeModuleItem m_Module6;

    public UIGrid _moduleGrid;

    /// <summary>
    /// 当前界面类型：1 年级 2 单元 3 等级 4 模块
    /// </summary>
    private int curUIType;
    public Animation Animation;
    public AnimationClip Class_start;
    public AnimationClip Unit_start;
    public AnimationClip Level_start;
    public AnimationClip Module_start;

    void Start()
    {
        AudicoManager.instance.Play("music", "Music/choose screen");
        if (instance == null)
        {
            instance = this;
        }

        InitSelectCourse();
        SetClickEvent();
    }

    /// <summary>
    /// 初始化选课界面
    /// </summary>
    public void InitSelectCourse()
    {
        HideSceneElements();
        OpenClassSelectUI();
    }

    /// <summary>
    /// 隐藏场景元素
    /// </summary>
    private void HideSceneElements()
    {
        _classItem.SetActive(false);
        _classLabel.gameObject.SetActive(false);
        _unitLabel.gameObject.SetActive(false);
        _levelLabel.gameObject.SetActive(false);
    }

    // --------------------------   年级学期选择   --------------------------------- //
    /// <summary>
    /// 打开年级选择界面
    /// </summary>
    private void OpenClassSelectUI()
    {
        // 8个学期，从三年级上册开始到六年级下册
        for (int i = 0; i < 8; i++)
        {
            GameObject item = Instantiate(_classItem);
            item.name = i.ToString();
            item.transform.parent = _classGrid.transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            ClassItem classItem = item.transform.GetComponent<ClassItem>();
            if (i % 2 == 0)
            {
                 // 上学期
                classItem.InitClassItem(3 + i / 2, 1);
            }
            else
            {
                 // 下学期
                classItem.InitClassItem(3 + i / 2, 2);
            }
            item.SetActive(true);
        }
        _classGrid.enabled = true;

        ShowSelectCourseUI(1);
    }

    // ----------------------------   单元选择   ----------------------------------- //
    /// <summary>
    /// 打开单元选择界面
    /// </summary>
    /// <param name="grade">选择的年级</param>
    /// <param name="term">选择的学期</param>
    public void OpenUnitSelectUI(int grade, int term, int curNum)
    {
        DataManager.GetInstance().roleData.curGrade = grade;
        DataManager.GetInstance().roleData.curTerm = term;
   
        _classLabel.text = StringConst.gradePath[grade] + StringConst.termPath[term];
        _classLabel.gameObject.SetActive(true);

        ShowSelectCourseUI(2);
        RefreshUnitSelectUI(curNum);
    }

    /// <summary>
    /// 刷新单元解锁状态
    /// </summary>
    /// <param name="curNum"></param>
    public void RefreshUnitSelectUI(int curNum)
    {
        int totalUnitNum = (int)DataManager.GetInstance().roleData.totalUnitNum;
        m_Unit1.Init(1, curNum, totalUnitNum);
        m_Unit2.Init(2, curNum, totalUnitNum);
        m_Unit3.Init(3, curNum, totalUnitNum);
        m_Unit4.Init(4, curNum, totalUnitNum);
        m_Unit5.Init(5, curNum, totalUnitNum);
        m_Unit6.Init(6, curNum, totalUnitNum);
        m_Unit7.Init(7, curNum, totalUnitNum);
    }

    // ----------------------------   难度选择   ----------------------------------- //
    /// <summary>
    /// 打开难度选择界面
    /// </summary>
    /// <param name="unit">选择的单元</param>
    public void OpenLevelSelectUI(int unit, int curIndex)
    {
        DataManager.GetInstance().roleData.curUnit = unit;

        _unitLabel.text = StringConst.unitPath[unit];
        _unitLabel.gameObject.SetActive(true);

        RefreshLevelSelectUI(curIndex);
        ShowSelectCourseUI(3);
    }

    /// <summary>
    /// 刷新难度解锁状态
    /// </summary>
    /// <param name="curIndex"></param>
    private void RefreshLevelSelectUI(int curIndex)
    {
        m_Level1.Init(1, curIndex);
        m_Level2.Init(2, curIndex);
        m_Level3.Init(3, curIndex);
        m_Level4.Init(4, curIndex);
    }

    // ----------------------------   模块选择   ----------------------------------- //
    /// <summary>
    /// 打开模块选择界面
    /// </summary>
    public void OpenModuleSelectUI(int curLevel, bool isLastUnit, int curModule, Dictionary<int, passUnit> modulePassList)
    {
        DataManager.GetInstance().roleData.curLevel = curLevel.ToString();

        _levelLabel.text = StringConst.levelPath[curLevel];
        _levelLabel.gameObject.SetActive(true);

        RefreshModuleSelectUI(curLevel, isLastUnit, curModule, modulePassList);
        ShowSelectCourseUI(4);
    }

    /// <summary>
    /// 刷新模块解锁状态
    /// </summary>
    private void RefreshModuleSelectUI(int curLevel, bool isLastUnit, int curModule, Dictionary<int, passUnit> modulePassList)
    {
        if (curLevel == 4)
        {
            if (isLastUnit)
            {
                m_Modules[3].Init(4, false, curModule, 3);
            }
            else
            {
                m_Modules[3].Init(4, true, curModule, (int)modulePassList[2].star);
            }

            m_Modules[0].Init(1, false, curModule, 3);
            m_Modules[1].Init(2, false, curModule, 3);
            m_Modules[2].Init(3, true, curModule, (int)modulePassList[1].star);
            m_Modules[4].Init(5, true, curModule, (int)modulePassList[3].star);
            m_Modules[5].Init(6, false, curModule, 3);
        }
        else
        {
            if (isLastUnit)
            {
                m_Modules[3].Init(4, false, curModule, 3);
            }
            else
            {
                m_Modules[3].Init(4, true, curModule, (int)modulePassList[4].star);
            }

            m_Modules[0].Init(1, true, curModule, (int)modulePassList[1].star);
            m_Modules[1].Init(2, true, curModule, (int)modulePassList[2].star);
            m_Modules[2].Init(3, true, curModule, (int)modulePassList[3].star);
            m_Modules[4].Init(5, true, curModule, (int)modulePassList[5].star);
            m_Modules[5].Init(6, false, curModule, 3);
        }

        _moduleGrid.enabled = true;
    }

    /// <summary>
    /// 设置点击事件
    /// </summary>
    public void SetClickEvent()
    {
        UIEventListener.Get(_backBtn.gameObject).onClick = BackOnClick;
        UIEventListener.Get(_helpBtn.gameObject).onClick = HelpOnClick;
        UIEventListener.Get(_classLeftBtn.gameObject).onClick = ClassLeftOnClick;
        UIEventListener.Get(_classRightBtn.gameObject).onClick = ClassRightOnClick;
        _classLeftBtn.defaultColor = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
        _classRightBtn.defaultColor = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
    }

    /// <summary>
    /// 帮助按钮
    /// </summary>
    /// <param name="btn"></param>
    void HelpOnClick(GameObject btn)
    {

    }

    /// <summary>
    /// 年级右翻页按钮
    /// </summary>
    /// <param name="btn"></param>
    void ClassRightOnClick(GameObject btn)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        _classScrollBar.value += 0.6f;
    }

    /// <summary>
    /// 年级左翻页按钮
    /// </summary>
    /// <param name="btn"></param>
    void ClassLeftOnClick(GameObject btn)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        _classScrollBar.value -= 0.6f;
    }

    /// <summary>
    /// 点击返回按钮
    /// </summary>
    /// <param name="btn"></param>
    private void BackOnClick(GameObject btn)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
       if (curUIType == 1)
        {
            Debug.Log("关闭选课界面");
            AudicoManager.instance.Play("music", "Music/hall screen");
            Destroy(gameObject);
        } else
        {
            curUIType -= 1;
            ShowSelectCourseUI(curUIType);
        }
    }

    /// <summary>
    /// 显示选课界面
    /// </summary>
    /// <param name="uiType">1年级 2单元 3难度 4模块</param>
    public void ShowSelectCourseUI(int uiType)
    {
        switch (uiType)
        {
            case 1:
                Animation.Play("Class_start");
                _scrollPanel.clipOffset = new Vector2(0, 0);
                _scrollView.enabled = false;
                Invoke("ResetScrollPanelPosition", Class_start.length);
                //_classSelectUI.SetActive(true);
                //_unitSelectUI.SetActive(false);
                //_levelSelectUI.SetActive(false);
                //_moduleSelectUI.SetActive(false);
                //_classLabel.gameObject.SetActive(false);
                //_unitLabel.gameObject.SetActive(false);
                //_levelLabel.gameObject.SetActive(false);
                curUIType = 1;
                break;
            case 2:
                //lastv2 = _scrollPanel.clipOffset;
                Animation.Play("Unit_start");
                //_classSelectUI.SetActive(false);
                //_unitSelectUI.SetActive(true);
                //_levelSelectUI.SetActive(false);
                //_moduleSelectUI.SetActive(false);
                //_classLabel.gameObject.SetActive(true);
                //_unitLabel.gameObject.SetActive(false);
                //_levelLabel.gameObject.SetActive(false);
                curUIType = 2;
                break;
            case 3:
                Animation.Play("Level_start");
                //_classSelectUI.SetActive(false);
                //_unitSelectUI.SetActive(false);
                //_levelSelectUI.SetActive(true);
                //_moduleSelectUI.SetActive(false);
                //_classLabel.gameObject.SetActive(true);
                //_unitLabel.gameObject.SetActive(true);
                //_levelLabel.gameObject.SetActive(false);
                curUIType = 3;
                break;
            case 4:
                Animation.Play("Module_start");
                //_classSelectUI.SetActive(false);
                //_unitSelectUI.SetActive(false);
                //_levelSelectUI.SetActive(false);
                //_moduleSelectUI.SetActive(true);
                //_classLabel.gameObject.SetActive(true);
                //_unitLabel.gameObject.SetActive(true);
                //_levelLabel.gameObject.SetActive(true);
                curUIType = 4;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 修复年级选择动画导致的显示问题
    /// </summary>
    public void ResetScrollPanelPosition()
    {
        _classLeftBtn.gameObject.SetActive(false);
        _classRightBtn.gameObject.SetActive(false);
        _classLeftBtn.gameObject.SetActive(true);
        _classRightBtn.gameObject.SetActive(true);
        _scrollPanel.clipOffset = new Vector2(0,0);
        _scrollView.enabled = true;
    }
    void OnDestroy()
    {
        instance = null;
    }
}