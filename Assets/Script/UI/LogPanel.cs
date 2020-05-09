using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPanel : MonoBehaviour
{
    GameObject reporters;
    public GameObject clbtn;
    public GameObject logbtn;
    public GameObject warnbtn;
    public GameObject errbtn;
    public GameObject exibtn;
    // Use this for initialization
    void Start()
    {
        reporters = GameObject.Find("Reporter");
        UIEventListener.Get(clbtn).onClick = Onclickbtn;
        UIEventListener.Get(logbtn).onClick = Onclickbtn;
        UIEventListener.Get(warnbtn).onClick = Onclickbtn;
        UIEventListener.Get(errbtn).onClick = Onclickbtn;
        UIEventListener.Get(exibtn).onClick = Onclickbtn;
    }

    private void Onclickbtn(GameObject go)
    {
        switch (go.name)
        {
            case "clbtn":
                reporters.GetComponent<Reporter>().clear();
                break;
            case "logbtn":
                bool b = reporters.GetComponent<Reporter>().showLog;
                reporters.GetComponent<Reporter>().showLog = !b;
                reporters.GetComponent<Reporter>().calculateCurrentLog();
                break;
            case "warnbtn":

                bool bb = reporters.GetComponent<Reporter>().showWarning;
                reporters.GetComponent<Reporter>().showWarning = !bb;
                reporters.GetComponent<Reporter>().calculateCurrentLog();

                break;
            case "errbtn":
                bool bbc = reporters.GetComponent<Reporter>().showError;
                reporters.GetComponent<Reporter>().showError = !bbc;
                reporters.GetComponent<Reporter>().calculateCurrentLog();
                break;
            case "exibtn":
                reporters.GetComponent<Reporter>().show = false;
                ReporterGUI gui = reporters.GetComponent<ReporterGUI>();
                DestroyImmediate(gui);
                break;
            default:
                break;
        }
    }
}
