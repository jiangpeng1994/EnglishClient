//using System.Threading;
//using UnityEngine;


//[RequireComponent(typeof(AudioSource))]

/////专门录音的
//public class Record : MonoBehaviour
//{
//    public static Record wordGameInstance;

//    private AudioSource aud;

//    private string[] devices;

//    // Use this for initialization
//    private void OnDestroy()
//    {
//        wordGameInstance = null;
//    }
//    void Start()
//    {
//        if (wordGameInstance==null)
//        {
//            wordGameInstance = new Record();
//            wordGameInstance.aud = GetComponent<AudioSource>();
//            wordGameInstance.devices = Microphone.devices;
//            //Debug.LogError(devices.Length + "devices.Length");
//            if (wordGameInstance.devices.Length > 0)
//            {
//                Debug.LogError("设备有麦克风:" + wordGameInstance.devices[0]);
//            }
//            else
//            {
//                Debug.LogError("设备没有麦克风");

//            }
//        }

//        //aud = this.GetComponent<AudioSource>();
//        //获取麦克风设备，判断设备是否有麦克风

        

//    }
//    /// <summary>
//    /// 开始录音
//    /// </summary>
//    public void StartRecord()
//    {
       
//        if ((Microphone.devices.Length<1) || (Microphone.IsRecording(Microphone.devices[0])))
//        {
//            Debug.LogError("不让录音");
//            return;

//        }
//        wordGameInstance.aud.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);

//    }
//    /// <summary>
//    /// 结束录音
//    /// </summary>
//    public void EndRecord()
//    {
//        if ((Microphone.devices.Length < 1) || (!Microphone.IsRecording(Microphone.devices[0])))
//        {
//            Debug.Log("不让结束");
//            return;

//        }

//        //结束录音

//        Microphone.End(Microphone.devices[0]);

//    }
//    /// <summary>
//    /// 播放录音
//    /// </summary>
//    public void PlayRecord()
//    {

//        if ((Microphone.devices.Length < 1) || (Microphone.IsRecording(Microphone.devices[0]) || wordGameInstance.aud.clip == null))
//        {

//            return;

//        }

//        //播放录音
//        //aud = this.GetComponent<AudioSource>();
//        wordGameInstance.aud.Play();

//    }

//    /// <summary>
//    /// 返回录音时间
//    /// </summary>
//    /// <returns></returns>
//    public float RecordTime()
//    {
//        if (wordGameInstance.aud.clip!=null)
//        {
//            return wordGameInstance.aud.clip.length;
//        }
//        else
//        {
//            return 0;
//        }
       
//    }
//}