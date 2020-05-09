using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class UIRegisterAccount : MonoBehaviour {

    public UILabel m_JiDu;
    public UIButton m_Next;
    public UIButton m_Back;
    public UIButton m_Return;
    public GameObject m_PanelFirst;
    public GameObject m_PanelSecond;
    public GameObject m_PanelThird;
    public UIButton m_Nan;
    public GameObject m_NanTip;
    public UIButton m_Nv;
    public GameObject m_NvTip;
    public UIInput m_InputName;
    public UIButton m_RandomName;
    public UIInput m_InputPhoneNum;
    public UIInput m_InputPWD1;
    public UIInput m_InputPWD2;
    public UIButton m_Book;
    public UIButton m_Class;
    public UIButton m_Part;
    public UILabel m_BookText;
    public UILabel m_ClassText;
    public UILabel m_PartText;
    public GameObject m_BookList;
    public GameObject m_ClassList;
    public GameObject m_PartList;
    public GameObject m_BookItem;
    public GameObject m_ClassItem;
    public GameObject m_PartItem;
    public UIGrid m_BookGrid;
    public UIGrid m_ClassGrid;
    public UIGrid m_PartGrid;
    public UIButton m_JiHuo;
    public UIInput m_InputPhoneCode;
    public UILabel m_Des;
    public GameObject m_RegisterAccount;
    public GameObject m_Maincenter;
    public GameObject m_PanelContent;
    public GameObject m_OverPanel;
    public UIButton m_RegisterBgBtn;

    private int m_Sex;   //性別  0男  1女
    private int m_type;
    private int m_BookType;
    private int m_ClassType;
    private int m_PartType;
    private bool m_IsCreateItem = false;
    private string[] Names = {"风格豆腐干","俺是个","胡椒粉" };
    private string[] BookTypes = { "PEP人教版小学英语" };
    private string[] ClassTypes = { "三年级上", "三年级下", "四年级上", "四年级下", "五年级上", "五年级下", "六年级上", "六年级下" };
    private string[] PartTypes = { "单元1", "单元2", "单元3", "单元4", "单元5", "单元6", "单元7" };

    /// <summary>
    /// 注册初始化
    /// </summary>
    public void Init()
    {
        m_type = 1;
        m_InputName.value = Names[Random.Range(0, Names.Length)];
        m_PanelFirst.SetActive(true);
        m_PanelSecond.SetActive(false);
        m_PanelThird.SetActive(false);
        m_JiDu.text = "(1/3)";
        m_NanTip.SetActive(true);
        m_NvTip.SetActive(false);
        m_Sex = 0;
        m_PanelContent.SetActive(true);
        m_OverPanel.SetActive(false);
        m_InputPhoneNum.value = "";
        m_InputPWD1.value = "";
        m_InputPWD2.value = "";
        m_InputPhoneCode.value = "";
        m_BookText.text = BookTypes[0];
        m_ClassText.text = ClassTypes[0];
        m_PartText.text = PartTypes[0];
        m_BookType = 0;
        m_ClassType = 0;
        m_PartType = 0;
        m_BookList.SetActive(false);
        m_ClassList.SetActive(false);
        m_PartList.SetActive(false);
        m_BookItem.SetActive(false);
        m_ClassItem.SetActive(false);
        m_PartItem.SetActive(false);
        CreateItem();
    }

    void Start () {
        UIEventListener.Get(m_Back.gameObject).onClick = OnClickBack;
        UIEventListener.Get(m_RandomName.gameObject).onClick = OnClickRandomName;
        UIEventListener.Get(m_Nan.gameObject).onClick = OnClickMan;
        UIEventListener.Get(m_Nv.gameObject).onClick = OnClickWoman;
        UIEventListener.Get(m_Next.gameObject).onClick = OnClickNext;

        UIEventListener.Get(m_Book.gameObject).onClick = OnClickSelectBook;
        UIEventListener.Get(m_Class.gameObject).onClick = OnClickSelectClass;
        UIEventListener.Get(m_Part.gameObject).onClick = OnClickSelectPart;
        UIEventListener.Get(m_JiHuo.gameObject).onClick = OnGetVerificationCode;
        UIEventListener.Get(m_Return.gameObject).onClick = OnClickReturn;
        UIEventListener.Get(m_RegisterBgBtn.gameObject).onClick = OnClickRegisterBgBtn;
    }

    /// <summary>
    /// 创建教材选择内容
    /// </summary>
    private void CreateItem()
    {
        if (m_IsCreateItem==true)
        {
            return;
        }
        for (int i = 0; i < BookTypes.Length; i++)
        {
            GameObject go = GameObject.Instantiate(m_BookItem, m_BookGrid.transform);
            go.transform.localScale = Vector3.one;
            go.name = i.ToString();
            UIEventListener.Get(go).onClick = OnClickBookContent;
            UILabel text = go.transform.Find("Label").GetComponent<UILabel>();
            text.text = BookTypes[i];
            go.SetActive(true);
        }
        for (int i = 0; i < ClassTypes.Length; i++)
        {
            GameObject go = GameObject.Instantiate(m_ClassItem, m_ClassGrid.transform);
            go.transform.localScale = Vector3.one;
            go.name = i.ToString();
            UIEventListener.Get(go).onClick = OnClickClassContent;
            UILabel text = go.transform.Find("Label").GetComponent<UILabel>();
            text.text = ClassTypes[i];
            go.SetActive(true);
        }
        for (int i = 0; i < PartTypes.Length; i++)
        {
            GameObject go = GameObject.Instantiate(m_PartItem, m_PartGrid.transform);
            go.transform.localScale = Vector3.one;
            go.name = i.ToString();
            UIEventListener.Get(go).onClick = OnClickPartContent;
            UILabel text = go.transform.Find("Label").GetComponent<UILabel>();
            text.text = PartTypes[i];
            go.SetActive(true);
        }
        m_IsCreateItem = true;
    }

    /// <summary>
    /// 注册点击下一步
    /// </summary>
    /// <param name="button"></param>
    private void OnClickNext(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (m_type == 1)
        {
            if (m_InputName == null || m_InputName.value == "")
            {
                GameTools.Instance.TipsShow("请输入昵称");
                return;
            }

            m_type = m_type + 1;
            ChangeStep();
        }
        else if (m_type == 2)
        {
            if (m_InputPhoneNum.value == "")
            {
                GameTools.Instance.TipsShow("请输入手机号码");
                return;
            }
            if (GlobalActionManager.CheckPhoneIsAble(m_InputPhoneNum.value) == false)
            {
                GameTools.Instance.TipsShow("请输入正确的手机号码");
                return;
            }
            if (m_InputPWD1.value == "")
            {
                GameTools.Instance.TipsShow("请输入密码");
                return;
            }
            if (m_InputPWD2.value == "")
            {
                GameTools.Instance.TipsShow("请输入确认密码");
                return;
            }
            if (Regex.IsMatch(m_InputPWD1.value, @"[\u4e00-\u9fa5]"))
            {
                GameTools.Instance.TipsShow("密码不能包含中文");
                return;
            }
            if (Regex.IsMatch(m_InputPWD1.value, @"^\d+$"))
            {
                GameTools.Instance.TipsShow("密码不能为纯数字");
                return;
            }
            if (m_InputPWD1.value.Length < 8)
            {
                GameTools.Instance.TipsShow("密码不能小于8位");
                return;
            }
            if (m_InputPWD1.value != m_InputPWD2.value)
            {
                GameTools.Instance.TipsShow("两次输入的密码不一致");
                return;
            }

            m_type = m_type + 1;
            ChangeStep();
        }
        else if (m_type == 3)
        {
            if (m_InputPhoneCode == null || m_InputPhoneCode.value == "" || !Regex.IsMatch(m_InputPhoneCode.value, "[0-9]"))
            {
                GameTools.Instance.TipsShow("激活码输入有误");
                return;
            }

            // 注册账号，服务器验证激活码
            StartCoroutine(RequestRegistration("http://47.110.254.9:8001/createAccount?account=" + m_InputPhoneNum.value + "&password=" + m_InputPWD1.value + "&nickname=" + m_InputName.value + "&authCode=" + m_InputPhoneCode.value + "&sex=" + m_Sex.ToString()));
        }
    }

    /// <summary>
    /// 注册点击返回
    /// </summary>
    /// <param name="button"></param>
    private void OnClickBack(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (m_type == 1)
        {
            m_RegisterAccount.SetActive(false);
            m_Maincenter.SetActive(true);
        }
        else if (m_type == 2)
        {
            m_type = m_type - 1;
            ChangeStep();
        }
        else if (m_type == 3)
        {
            m_type = m_type - 1;
            ChangeStep();
        }
    }

    /// <summary>
    /// 步骤变化
    /// </summary>
    private void ChangeStep()
    {
        switch (m_type)
        {
            case 1:
                m_PanelFirst.SetActive(true);
                m_PanelSecond.SetActive(false);
                m_PanelThird.SetActive(false);
                m_JiDu.text = "(1/3)";
                break;
            case 2:
                m_PanelFirst.SetActive(false);
                m_PanelSecond.SetActive(true);
                m_PanelThird.SetActive(false);
                m_JiDu.text = "(2/3)";
                break;
            case 3:
                m_PanelFirst.SetActive(false);
                m_PanelSecond.SetActive(false);
                m_PanelThird.SetActive(true);
                m_JiDu.text = "(3/3)";
                m_Des.text = "您注册的账号手机号为" + m_InputPhoneNum.value + "，请点击验证";
                break;
            case 4:
                AudicoManager.instance.Play("effect", "Effect/get card");
                m_PanelContent.SetActive(false);
                m_OverPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 随机名字
    /// </summary>
    /// <param name="button"></param>
    private void OnClickRandomName(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/reverse card");
        m_InputName.value = Names[Random.Range(0, Names.Length)];
    }

    /// <summary>
    /// 选择性别男
    /// </summary>
    /// <param name="button"></param>
    private void OnClickMan(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (m_Sex == 0)
        {
            return;
        }
        m_NanTip.SetActive(true);
        m_NvTip.SetActive(false);
        m_Sex = 0;
    }

    /// <summary>
    /// 选择性别女
    /// </summary>
    /// <param name="button"></param>
    private void OnClickWoman(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (m_Sex == 1)
        {
            return;
        }
        m_NanTip.SetActive(false);
        m_NvTip.SetActive(true);
        m_Sex = 1;
    }

    /// <summary>
    /// 打开选择教材列表
    /// </summary>
    /// <param name="button"></param>
    private void OnClickSelectBook(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_BookList.SetActive(!m_BookList.activeSelf);
        m_ClassList.SetActive(false);
        m_PartList.SetActive(false);
    }

    /// <summary>
    /// 打开选择年级列表
    /// </summary>
    /// <param name="button"></param>
    private void OnClickSelectClass(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_BookList.SetActive(false);
        m_ClassList.SetActive(!m_ClassList.activeSelf);
        m_PartList.SetActive(false);
    }

    /// <summary>
    /// 打开选择单元列表
    /// </summary>
    /// <param name="button"></param>
    private void OnClickSelectPart(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_BookList.SetActive(false);
        m_ClassList.SetActive(false);
        m_PartList.SetActive(!m_PartList.activeSelf);
    }

    /// <summary>
    /// 选择教材
    /// </summary>
    /// <param name="button"></param>
    private void OnClickBookContent(GameObject button)
    {
        m_BookType = int.Parse(button.name);
        m_BookText.text = BookTypes[m_BookType];
        m_BookList.SetActive(false);
    }

    /// <summary>
    /// 选择年级
    /// </summary>
    /// <param name="button"></param>
    private void OnClickClassContent(GameObject button)
    {
        m_ClassType = int.Parse(button.name);
        m_ClassText.text = ClassTypes[m_ClassType];
        m_ClassList.SetActive(false);
    }

    /// <summary>
    /// 选择单元
    /// </summary>
    /// <param name="button"></param>
    private void OnClickPartContent(GameObject button)
    {
        m_PartType = int.Parse(button.name);
        m_PartText.text = PartTypes[m_PartType];
        m_PartList.SetActive(false);
    }

    /// <summary>
    /// 点击注册背景
    /// </summary>
    /// <param name="go"></param>
    private void OnClickRegisterBgBtn(GameObject go)
    {
        m_BookList.SetActive(false);
        m_ClassList.SetActive(false);
        m_PartList.SetActive(false);
    }

    /// <summary>
    /// 点击获取激活码
    /// </summary>
    /// <param name="button"></param>
    private void OnGetVerificationCode(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        // SDK获取手机激活码
        SDKHandle._instance.GetVerificationCode(m_InputPhoneNum.value);
    }

    /// <summary>
    /// 请求服务器进行注册
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator RequestRegistration(string url)
    {
        WWW getData = new WWW(url);
        yield return getData;

        if (getData.error != null)
        {
            GameTools.Instance.TipsShow("注册失败!错误码:"+ getData.error);
            m_RegisterAccount.SetActive(false);
            m_Maincenter.SetActive(true);
        }
        else
        {
            if (getData.text == "0")
            {
                // 注册成功
                AudicoManager.instance.Play("effect", "Effect/get card");
                m_type = 4;
                ChangeStep();
            }
            else if(getData.text == "-3")
            {
                // 账号已存在
                GameTools.Instance.TipsShow("该账号已被注册！请返回登陆！");
                m_RegisterAccount.SetActive(false);
                m_Maincenter.SetActive(true);
            }
            else
            {
                GameTools.Instance.TipsShow("注册失败!请返回重新注册!错误码:"+ getData.text);
                m_RegisterAccount.SetActive(false);
                m_Maincenter.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 注册成功，返回登陆界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickReturn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_RegisterAccount.SetActive(false);
        m_Maincenter.SetActive(true);
    }
}