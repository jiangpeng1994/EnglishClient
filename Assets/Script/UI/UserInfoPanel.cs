using ProtoSprotoType;
using Sproto;
using System;
using UnityEngine;

public class UserInfoPanel : MonoBehaviour
{
    public UIButton DataBtn;
    public UIButton SoundBtn;
    public UIButton BackBtn;
    public UITexture DataBtnTexture;
    public UITexture SoundBtnTexture;
    public Texture[] DataBtnTextures;
    public Texture[] SoundBtnTextures;

    public GameObject DataPanel;
    public UITexture HeadTexture;
    public Texture[] HeadTextures;
    public UILabel Name;
    public UILabel Sex;
    public UILabel Phone;
    public UILabel Gold;
    public UIButton ChangeUesrButton;

    public GameObject SoundPanel;
    public UISlider MusicVoiceSlider;
    public UISlider EffectVoiceSlider;
    public UISlider StudyVoiceSlider;
    public UIButton ResetBtn;
    public UIButton ChangeBtn;

    private int FirstChangeVoice = 3;

    public GameObject ChangePanel;
    public UIButton CloseChangePanelBtn;
    public UIButton SureChangeBtn;
    public UIButton ChooseBoyBtn;
    public UIButton ChooseGirlBtn;
    public UIInput ChangeNameInput;
    public UITexture BtnBoy;
    public UITexture BtnGirl;
    public Texture[] BtnSexs;
    public int ChooseSex;

    void Start()
    {
        UIEventListener.Get(DataBtn.gameObject).onClick = OnClickDataBtn;
        UIEventListener.Get(SoundBtn.gameObject).onClick = OnClickSoundBtn;
        UIEventListener.Get(BackBtn.gameObject).onClick = OnClickBackBtn;
        UIEventListener.Get(ChangeUesrButton.gameObject).onClick = OnClickChangeUesrBtn;
        UIEventListener.Get(ResetBtn.gameObject).onClick = OnClickResetBtn;
        UIEventListener.Get(ChangeBtn.gameObject).onClick = OnClickChangeBtn;
        UIEventListener.Get(CloseChangePanelBtn.gameObject).onClick = OnClickCloseChangePanelBtn;
        UIEventListener.Get(SureChangeBtn.gameObject).onClick = OnClickSureChangeBtn;
        UIEventListener.Get(ChooseBoyBtn.gameObject).onClick = OnClickChooseBoyBtn;
        UIEventListener.Get(ChooseGirlBtn.gameObject).onClick = OnClickChooseGirlBtn;
        OnClickDataBtn(null);
        ChangePanel.SetActive(false);
    }

    /// <summary>
    /// 点击个人信息
    /// </summary>
    /// <param name="go"></param>
    private void OnClickDataBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DataBtnTexture.mainTexture = DataBtnTextures[1];
        SoundBtnTexture.mainTexture = SoundBtnTextures[0];
        SoundPanel.SetActive(false);
        InitDataPanel();
        DataPanel.SetActive(true);
    }

    /// <summary>
    /// 初始化个人信息面板
    /// </summary>
    private void InitDataPanel()
    {
        if (DataManager.GetInstance().roleData.NickName == "traveler")
        {
            Name.text = "游客";
            Phone.text = "无";
        } else
        {
            Name.text = DataManager.GetInstance().roleData.NickName;
            Phone.text = DataManager.GetInstance().roleData.Phone;
        }
        
        if (DataManager.GetInstance().roleData.Sex == 0)
        {
            HeadTexture.mainTexture = HeadTextures[0];
            Sex.text = "男";
        } else
        {
            HeadTexture.mainTexture = HeadTextures[1];
            Sex.text = "女";
        }

        Gold.text = DataManager.GetInstance().roleData.Diamond.ToString();
    }

    /// <summary>
    /// 点击音量设置
    /// </summary>
    /// <param name="go"></param>
    private void OnClickSoundBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        DataBtnTexture.mainTexture = DataBtnTextures[0];
        SoundBtnTexture.mainTexture = SoundBtnTextures[1];
        DataPanel.SetActive(false);
        InitSoundPanel();
        SoundPanel.SetActive(true);
    }

    /// <summary>
    /// 初始化音量设置
    /// </summary>
    private void InitSoundPanel()
    {
        float musicVoice = GameDataManager.GetFloat("MusicVoice");
        if (musicVoice == -1)
        {
            musicVoice = StringConst.DefultVoice;
            GameDataManager.SetFloat("MusicVoice", musicVoice);
        }
        float effectVoice = GameDataManager.GetFloat("EffectVoice");
        if (effectVoice == -1)
        {
            effectVoice = StringConst.DefultVoice;
            GameDataManager.SetFloat("EffectVoice", effectVoice);
        }
        float studyVoice = GameDataManager.GetFloat("StudyVoice");
        if (studyVoice == -1)
        {
            studyVoice = StringConst.DefultVoice;
            GameDataManager.SetFloat("StudyVoice", studyVoice);
        }

        MusicVoiceSlider.value = musicVoice;
        EffectVoiceSlider.value = effectVoice;
        StudyVoiceSlider.value = studyVoice;

        AudicoManager.instance.ModifyBGMusicVolume(musicVoice);
        AudicoManager.instance.ModifyEffectVolume(effectVoice);
        AudicoManager.instance.ModifyStudyVolume(studyVoice);
    }

    /// <summary>
    /// 点击返回
    /// </summary>
    /// <param name="go"></param>
    private void OnClickBackBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        Destroy(gameObject);
    }

    /// <summary>
    /// 点击切换用户
    /// </summary>
    /// <param name="go"></param>
    private void OnClickChangeUesrBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        AudicoManager.instance.Play("music", "Music/logon screen");
        Destroy(UIPlazaPanel.instance.gameObject);
        UILoginPanel.instance.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    /// <summary>
    /// 点击重置音效
    /// </summary>
    /// <param name="go"></param>
    private void OnClickResetBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        GameDataManager.SetFloat("MusicVoice", StringConst.DefultVoice);
        GameDataManager.SetFloat("EffectVoice", StringConst.DefultVoice);
        GameDataManager.SetFloat("StudyVoice", StringConst.DefultVoice);

        MusicVoiceSlider.value = StringConst.DefultVoice;
        EffectVoiceSlider.value = StringConst.DefultVoice;
        StudyVoiceSlider.value = StringConst.DefultVoice;

        AudicoManager.instance.ModifyAllVolume(StringConst.DefultVoice);
    }

    /// <summary>
    /// 滑动音乐音量条
    /// </summary>
    public void OnMusicVoiceSliderChange()
    {
        if (FirstChangeVoice <= 0)
        {
            AudicoManager.instance.Play("effect", "Effect/adjust volume");
        }
        else
        {
            FirstChangeVoice--;
        }
        GameDataManager.SetFloat("MusicVoice", MusicVoiceSlider.value);
        AudicoManager.instance.ModifyBGMusicVolume(MusicVoiceSlider.value);
    }

    /// <summary>
    /// 滑动音效音量条
    /// </summary>
    public void OnEffectVoiceSliderChange()
    {
        if (FirstChangeVoice <= 0)
        {
            AudicoManager.instance.Play("effect", "Effect/adjust volume");
        }
        else
        {
            FirstChangeVoice--;
        }
        GameDataManager.SetFloat("EffectVoice", EffectVoiceSlider.value);
        AudicoManager.instance.ModifyEffectVolume(EffectVoiceSlider.value);
    }

    /// <summary>
    /// 滑动教学音量条
    /// </summary>
    public void OnStudyVoiceSliderChange()
    {
        if (FirstChangeVoice <= 0)
        {
            AudicoManager.instance.Play("effect", "Effect/adjust volume");
        }
        else
        {
            FirstChangeVoice--;
        }
        GameDataManager.SetFloat("StudyVoice", StudyVoiceSlider.value);
        AudicoManager.instance.ModifyStudyVolume(StudyVoiceSlider.value);
    }

    /// <summary>
    /// 点击修改注册
    /// </summary>
    /// <param name="go"></param>
    private void OnClickChangeBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (DataManager.GetInstance().roleData.NickName == "traveler")
        {
            ChangeNameInput.value = "游客";
        }
        else
        {
            ChangeNameInput.value = DataManager.GetInstance().roleData.NickName;
        }

        if (DataManager.GetInstance().roleData.Sex == 0)
        {
            BtnBoy.mainTexture = BtnSexs[1];
            BtnGirl.mainTexture = BtnSexs[0];
            ChooseSex = 0;
            //Sex.text = "男";
        }
        else
        {
            BtnBoy.mainTexture = BtnSexs[0];
            BtnGirl.mainTexture = BtnSexs[1];
            ChooseSex = 1;
            //Sex.text = "女";
        }
        ChangePanel.SetActive(true);
    }

    /// <summary>
    /// 关闭修改注册
    /// </summary>
    /// <param name="go"></param>
    private void OnClickCloseChangePanelBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        ChangePanel.SetActive(false);
    }

    /// <summary>
    /// 确认修改
    /// </summary>
    /// <param name="go"></param>
    private void OnClickSureChangeBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        if (ChangeNameInput.value == "")
        {
            GameTools.Instance.TipsShow("昵称不能为空");
            return;
        }

        ChangeRoleInfo.request msg = new ChangeRoleInfo.request
        {
            name = ChangeNameInput.value,
            sex = ChooseSex
        };
        Debug.Log("请求：修改个人信息");
        NetSender.Send<ProtoProtocol.ChangeRoleInfo>(msg, OnChangeRoleInfo);
    }

    /// <summary>
    /// 收到请求结果
    /// </summary>
    /// <param name="rpcRsp"></param>
    private void OnChangeRoleInfo(SprotoTypeBase rpcRsp)
    {
        Debug.Log("请求：修改个人信息结果");
        var data = (ChangeRoleInfo.response)rpcRsp;
        if (data.status == 0)
        {
            DataManager.GetInstance().roleData.NickName = data.name;
            DataManager.GetInstance().roleData.Sex = data.sex;
            Name.text = DataManager.GetInstance().roleData.NickName;
            if (DataManager.GetInstance().roleData.Sex == 0)
            {
                HeadTexture.mainTexture = HeadTextures[0];
                Sex.text = "男";
            }
            else
            {
                HeadTexture.mainTexture = HeadTextures[1];
                Sex.text = "女";
            }
            ChangePanel.SetActive(false);
            UIPlazaPanel.instance.RefreshSex();
            GameTools.Instance.TipsShow("修改成功");
        } else if (data.status == -1)
        {
            GameTools.Instance.TipsShow("修改失败，游客无法进行修改");
        }
        else if (data.status == -4)
        {
            GameTools.Instance.TipsShow("修改失败，名字长度不符合要求");
        } else
        {
            GameTools.Instance.TipsShow("修改失败，请稍后再重试");
        }
    }

    /// <summary>
    /// 选择男孩
    /// </summary>
    /// <param name="go"></param>
    private void OnClickChooseBoyBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BtnBoy.mainTexture = BtnSexs[1];
        BtnGirl.mainTexture = BtnSexs[0];
        ChooseSex = 0;
    }

    /// <summary>
    /// 选择女孩
    /// </summary>
    /// <param name="go"></param>
    private void OnClickChooseGirlBtn(GameObject go)
    {
        AudicoManager.instance.Play("effect", "Effect/press button");
        BtnBoy.mainTexture = BtnSexs[0];
        BtnGirl.mainTexture = BtnSexs[1];
        ChooseSex = 1;
    }
}