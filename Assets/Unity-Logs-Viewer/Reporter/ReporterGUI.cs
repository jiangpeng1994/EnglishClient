using UnityEngine;
using System.Collections;

public class ReporterGUI : MonoBehaviour
{
    GameObject logpanel;
	Reporter reporter;
	void Awake()
	{
		reporter = gameObject.GetComponent<Reporter>();
        logpanel = GameObject.Find("GUI/GUICamera/LogPanel");
        logpanel.transform.localScale = new Vector3(1, 1, 1);
    }

	void OnGUI()
	{
		reporter.OnGUIDraw();
	}
    private void OnDestroy()
    {
        logpanel.transform.localScale = new Vector3(0, 0, 0);
    }
}
