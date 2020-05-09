using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudicoManager : MonoBehaviour
{
    public static AudicoManager instance; 
    private AudioSource _musicAudio;
    private AudioSource _effectAudio;
    private AudioSource _clickAudio;
    private AudioSource _studyAudio;
    public const string music = "music";
    public const string effect = "effect";
    public const string click = "click";
    public const string study = "study";
    private string _AudioRootURL;

    public void Awake()
    {
        instance = this;
        _AudioRootURL = Application.persistentDataPath + "/Resources/Audio/";
        _musicAudio = transform.Find("MusicAudio").GetComponent<AudioSource>();
        _effectAudio = transform.Find("EffectAudio").GetComponent<AudioSource>();
        _clickAudio = transform.Find("ClickAudio").GetComponent<AudioSource>();
        _studyAudio = transform.Find("StudyAudio").GetComponent<AudioSource>();
        ReadLocalConfig();
    }

    //读取本地配置
    public void ReadLocalConfig()
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

        ModifyBGMusicVolume(musicVoice);
        ModifyEffectVolume(effectVoice);
        ModifyStudyVolume(studyVoice);
    }

    private AudioClip audioClip;
    private float audioClipLength;
    private Object audioClipObj;
    WWW audio;
    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="audioClipType">音频类型</param>
    /// <param name="audioClipName">音频名字</param>
    public float Play(string audioClipType, string audioClipName)
    {
        audioClip = null;
        audioClipLength = 0;
        audioClipObj = Resources.Load("Audio/" + audioClipName);
        //Debug.Log("音频路径：" + _AudioRootURL);
        if (audioClipObj != null)
        {
            audioClip = Instantiate(audioClipObj) as AudioClip;
            audioClipLength = audioClip.length;

            if (audioClipType == music && audioClip != null)
            {
                PlayBGMusic(audioClip);
            }
            else if (audioClipType == effect && audioClip != null)
            {
                _effectAudio.Stop();
                _effectAudio.PlayOneShot(audioClip);
            }
            else if (audioClipType == click && audioClip != null)
            {
                _clickAudio.Stop();
                _clickAudio.PlayOneShot(audioClip);
            }
            else if (audioClipType == study && audioClip != null && StringConst.CanPlayStudyVioce && !StringConst.isSpeak)
            {
                _studyAudio.Stop();
                _studyAudio.PlayOneShot(audioClip);
            }
        } else
        {
            string path = "file://" + _AudioRootURL + audioClipName + ".mp3";
            Debug.Log("音频地址：" + path);
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                ((DownloadHandlerAudioClip)uwr.downloadHandler).compressed = false;
                ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;
                uwr.SendWebRequest();

                while (!uwr.isDone)
                {
                }

                audioClip =  DownloadHandlerAudioClip.GetContent(uwr);
                audioClipLength = audioClip.length;
                if (audioClipType == study && audioClip != null && StringConst.CanPlayStudyVioce && !StringConst.isSpeak)
                {
                    _studyAudio.Stop();
                    _studyAudio.PlayOneShot(audioClip);
                }
            }
        }
        
        return audioClipLength;
    }

    private string _RecordURL = "file://" + @"/storage/emulated/0/EnglishRecord.wav";
    /// <summary>
    /// 播放用户的录音
    /// </summary>
    public void PlayRecord()
    {
        Debug.Log("播放本地语音");
        StartCoroutine("StartPlayRecord");
    }

    private IEnumerator StartPlayRecord()
    {
        WWW record = new WWW(_RecordURL);
        yield return record;

        audioClip = record.GetAudioClip() as AudioClip;
        if (audioClip != null)
        {
            _studyAudio.Stop();
            _studyAudio.PlayOneShot(audioClip);
        }
    }

    public void StopStudyAudio()
    {
        if (_studyAudio != null)
        {
            _studyAudio.Stop();
        }

        if (_clickAudio != null)
        {
            _clickAudio.Stop();
        }
    }

    public void StopMusicAudio()
    {
        if (_musicAudio != null)
        {
            _musicAudio.Stop();
        }
    }

    /// <summary>
    /// 获取音频长度
    /// </summary>
    /// <param name="audio">音频名字</param>
    public float GetAudioLength(string audioClipName)
    {
        AudioClip audio = null;
        if (Resources.Load("Audio/" + audioClipName)!=null)
        {
            audio = Instantiate(Resources.Load("Audio/" + audioClipName)) as AudioClip;
        }
        
        if (audio != null)
        {
            return audio.length;
        }

        return 0;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="bgAudioClip">背景音乐</param>
    public void PlayBGMusic(AudioClip bgAudioClip)
    {
        if (_musicAudio != null && bgAudioClip != null)
        {
            _musicAudio.Stop();
            _musicAudio.clip = bgAudioClip;
            _musicAudio.loop = true;
            _musicAudio.Play();
        }
    }

    /// <summary>
    /// 关闭背景音乐
    /// </summary>
    public void StopBGMusic()
    {
        if (_musicAudio != null)
        {
            _musicAudio.Stop();
        }
    }

    /// <summary>
    /// 修改背景音乐音量
    /// </summary>
    /// <param name="volume"></param>
    public void ModifyBGMusicVolume(float volume)
    {
        if (_musicAudio != null)
        {
            _musicAudio.volume = volume;
        }
    }

    /// <summary>
    /// 修改特效音量
    /// </summary>
    /// <param name="volume"></param>
    public void ModifyEffectVolume(float volume)
    {
        if (_effectAudio != null)
        {
            _effectAudio.volume = volume;
        }
    }

    /// <summary>
    /// 修改学习音量
    /// </summary>
    /// <param name="volume"></param>
    public void ModifyStudyVolume(float volume)
    {
        if (_studyAudio != null)
        {
            _studyAudio.volume = volume;
        }
    }

    /// <summary>
    /// 修改所有音量
    /// </summary>
    /// <param name="volume"></param>
    public void ModifyAllVolume(float volume)
    {
        if (_musicAudio != null)
        {
            _musicAudio.volume = volume;
        }
        if (_effectAudio != null)
        {
            _effectAudio.volume = volume;
        }
        if (_studyAudio != null)
        {
            _studyAudio.volume = volume;
        }
    }

    //播放UI按钮声音
    public void PlayButtonAudio()
    {
        Play(effect, "btnAudio");
    }

    /// <summary>
    /// 暂停音频
    /// </summary>
    public void SetAll()
    {
        _effectAudio.mute = !_effectAudio.mute;
        _musicAudio.mute = !_musicAudio.mute;
        _studyAudio.mute = !_studyAudio.mute;
        _clickAudio.mute = !_clickAudio.mute;
    }
}