using UnityEngine;

public class WaitUI : MonoBehaviour
{
    public GameObject mask;
    public UITexture ui;
    public float speed = 12;

    void Start()
    {
        mask.gameObject.SetActive(false);
    }

    private int WaitTime = 100; //两秒
    private int ShowTime = 150; //三秒

    void FixedUpdate()
    {
        if (mask.gameObject.activeSelf)
        {
            ShowTime = ShowTime - 1;
            if (ShowTime <= 0)
            {
                NetSender.RemoveAllHandler();
                GameTools.Instance.TipsShow("网络状态不稳定，请稍后重试");
            } else
            {
                ui.transform.Rotate(Vector3.back * speed, Space.World);
            }
        } else
        {
            ShowTime = 150;
        }

        if (NetSender.showWaitUI)
        {
            WaitTime = WaitTime - 1;
            if (WaitTime <= 0)
            {
                //time = 100;
            } else
            {
                return;
            }
        } else
        {
            WaitTime = 100;
        }

        if (mask.gameObject.activeSelf != NetSender.showWaitUI)
        {
            mask.gameObject.SetActive(NetSender.showWaitUI);
        }
    }
}