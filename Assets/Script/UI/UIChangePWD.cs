using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class UIChangePWD : MonoBehaviour {

    public UIButton m_Next;
    public UIButton m_Back;
    public UISprite m_First;
    public UISprite m_Second;
    public UISprite m_Third;
    public GameObject m_PanelFirst;
    public GameObject m_PanelSecond;
    public GameObject m_PanelThird;
    public GameObject m_ChangePWD;
    public GameObject m_Maincenter;
    public GameObject m_PanelContent;
    public GameObject m_OverPanel;
    public UIInput m_InputPhoneNum;
    public UIInput m_InputPhoneCode;
    public UIInput m_InputPWD1;
    public UIInput m_InputPWD2;
    public UISprite m_JinDuTiao;
    public UIButton QuitBtn;
    private int m_type;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        m_PanelFirst.SetActive(true);
        m_PanelSecond.SetActive(false);
        m_PanelThird.SetActive(false);
        m_type = 1;
        ChangeStep();
        m_First.gameObject.SetActive(true);
        m_Second.gameObject.SetActive(true);
        m_Third.gameObject.SetActive(true);
        m_PanelContent.SetActive(true);
        m_OverPanel.SetActive(false);
        m_InputPhoneNum.value = "";
        m_InputPhoneCode.value = "";
        m_InputPWD1.value = "";
        m_InputPWD2.value = "";
        m_JinDuTiao.fillAmount = 1;
    }

    void Start()
    {
        UIEventListener.Get(m_Next.gameObject).onClick = OnClickNext;
        UIEventListener.Get(m_Back.gameObject).onClick = OnClickBack;
        UIEventListener.Get(QuitBtn.gameObject).onClick = OnClickReturn;
    }

    /// <summary>
    /// 找回密码点击下一步
    /// </summary>
    /// <param name="button"></param>
    private void OnClickNext(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (m_type==1)
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

            // SDK获取手机激活码
            SDKHandle._instance.GetVerificationCode(m_InputPhoneNum.value);

            m_type = m_type + 1;
            ChangeStep();
        }
        else if(m_type == 2)
        {
            if (m_InputPhoneCode == null || m_InputPhoneCode.value == "" || !Regex.IsMatch(m_InputPhoneCode.value, "[0-9]"))
            {
                GameTools.Instance.TipsShow("激活码输入有误");
                return;
            }

            m_type = m_type + 1;
            ChangeStep();
        }
        else if(m_type == 3)
        {
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

            StartCoroutine(RequestChangePassward("http://47.110.254.9:8001/changePassward?account=" + m_InputPhoneNum.value + "&password=" + m_InputPWD1.value + "&authCode=" + m_InputPhoneCode.value));
        }
    }

    /// <summary>
    /// 返回登陆
    /// </summary>
    /// <param name="button"></param>
    private void OnClickBack(GameObject button)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (m_type == 1)
        {
            m_ChangePWD.SetActive(false);
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
                m_First.spriteName = "title_bg2";
                m_Second.spriteName = "title_bg3";
                m_Third.spriteName = "title_bg3";
                m_PanelFirst.SetActive(true);
                m_PanelSecond.SetActive(false);
                m_PanelThird.SetActive(false);
                break;
            case 2:
                m_First.spriteName = "title_bg1";
                m_Second.spriteName = "title_bg4";
                m_Third.spriteName = "title_bg3";
                m_PanelFirst.SetActive(false);
                m_PanelSecond.SetActive(true);
                m_PanelThird.SetActive(false);
                break;
            case 3:
                m_First.spriteName = "title_bg1";
                m_Second.spriteName = "title_bg3";
                m_Third.spriteName = "title_bg4";
                m_PanelFirst.SetActive(false);
                m_PanelSecond.SetActive(false);
                m_PanelThird.SetActive(true);
                break;
            case 4:
                AudicoManager.instance.Play("effect", "Effect/get card");
                m_First.gameObject.SetActive(false);
                m_Second.gameObject.SetActive(false);
                m_Third.gameObject.SetActive(false);
                m_PanelContent.SetActive(false);
                m_OverPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 请求更改密码
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator RequestChangePassward(string url)
    {
        WWW getData = new WWW(url);
        yield return getData;

        if (getData.error != null)
        {
            GameTools.Instance.TipsShow("找回密码失败!错误码:" + getData.error);
            m_ChangePWD.SetActive(false);
            m_Maincenter.SetActive(true);
        }
        else
        {
            if (getData.text == "0")
            {
                // 找回密码成功
                AudicoManager.instance.Play("effect", "Effect/get card");
                m_type = 4;
                ChangeStep();
            }
            else if (getData.text == "468")
            {
                GameTools.Instance.TipsShow("验证码错误，请重新输入验证码");
                m_type = 2;
                ChangeStep();
            }
            else
            {
                GameTools.Instance.TipsShow("找回密码失败!错误码:" + getData.text);
                m_ChangePWD.SetActive(false);
                m_Maincenter.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 密码强度计算
    /// </summary>
    void Update () {
        if (m_InputPWD1 != null && m_PanelThird.activeSelf&& m_InputPWD1.value != "")
        {
            bool [] yesNum = new bool[4];
            yesNum[0] = Regex.IsMatch(m_InputPWD1.value, "[0-9]");
            yesNum[1] = Regex.IsMatch(m_InputPWD1.value, "[A-Z]");
            yesNum[2] = Regex.IsMatch(m_InputPWD1.value, "[a-z]");
            int count = 0;
            for (int i = 0; i < yesNum.Length; i++)
            {
                if (yesNum[i]==true)
                {
                    count = count + 1;
                }
            }
            if (count < 2)
            {
                m_JinDuTiao.fillAmount = 0.33f;
            }
            else if (count == 2)
            {
                m_JinDuTiao.fillAmount = 0.66f;
            }
            else
            {
                m_JinDuTiao.fillAmount = 1;
            }
        }
    }

    /// <summary>
    /// 找回密码成功，返回登陆界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickReturn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        m_ChangePWD.SetActive(false);
        m_Maincenter.SetActive(true);
    }
}