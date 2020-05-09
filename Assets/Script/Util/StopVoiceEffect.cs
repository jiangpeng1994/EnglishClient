using UnityEngine;

public class StopVoiceEffect : MonoBehaviour
{
    public void Stop(float audioClipLength)
    {
        CancelInvoke("Stop");
        Invoke("Stop", audioClipLength);
    }

    public void Stop()
    {
        gameObject.SetActive(false);
    }
}