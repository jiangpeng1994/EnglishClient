using UnityEngine;
using Sproto;

public class TestWin : BaseWnd
{
    UIButton btnLoginTest;

    public override void Init ()
    {
        btnLoginTest = _transform.Find("Sprite").GetComponent<UIButton>();
        UIEventListener.Get(btnLoginTest.gameObject).onClick = OnClickTest;

        //ProtoMsgListener.GetInstance().Add<Protocol.sc_map_enter>(ProtoMapEnterHandler); // 监听服务端主推的协议
    }

    public override void Update () 
    {
    }

    public override void Delete()
    {
    }



    /**
     * 发送登录协议
     **/
    void OnClickTest(GameObject button)
    {
        //var msg = new ProroC2SSprotoType.logintest.request();
        //msg.account = "zouv";
        //msg.password = "mypassword";
        //NetManager.GetInstance().Send<ProroC2SProtocol.logintest>(msg, ProtoLoginResponseHandler);
    }

    /**
     * 登录协议返回
     */
    private void ProtoLoginResponseHandler(SprotoTypeBase msg)
    {
        //var csLoginResp = (SprotoType.cs_login.response)msg;
        //Debug.Log("ProtoLoginResponseHandler___" + csLoginResp.result);
        //_txtTips.text = csLoginResp.result == 1 ? "login successed" : "login failed";
    }


}
