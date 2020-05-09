using DG.Tweening;
using UnityEngine;

public class UITips : MonoBehaviour
{

    public GameObject TipsBg;
    public UILabel _tips;

    private bool flickerBool;
    private bool tranforBool;
    private int _type = 1;

    private void Start()
    {
        flickerBool = false;
        tranforBool = true;
        Invoke("Disappear", 1.5f);
        Invoke("Deleted", 5f);
        if (_type != 1)
        {
            TipsBg.transform.DOLocalMoveY(-186, 0.8f);
        }
    }
    public void Init(string tips, int type = 1)
    {
        _tips.text = tips;
        _type = type;
        if (type == 1)
        {
            TipsBg.transform.localPosition = new Vector3(0, -330, 0);
        } else
        {
            TipsBg.transform.localPosition = new Vector3(565, -262, 0);
        }
    }
    public void Disappear()
    {
        flickerBool = true;
        tranforBool = false;

    }
    public void Deleted()
    {
        Destroy(gameObject);

    }
    // Update is called once per frame
    void Update()
    {
        if (flickerBool)
        {
            TipsBg.transform.GetComponent<UISprite>().color = new Color(TipsBg.transform.GetComponent<UISprite>().color.r, TipsBg.transform.GetComponent<UISprite>().color.g, TipsBg.transform.GetComponent<UISprite>().color.b, (TipsBg.transform.GetComponent<UISprite>().color.a - 0.02f) > 0 ? (TipsBg.transform.GetComponent<UISprite>().color.a - 0.02f) : 0);
            _tips.color = new Color(_tips.color.r, _tips.color.g, _tips.color.b, (_tips.color.a - 0.02f) > 0 ? (_tips.color.a - 0.02f) : 0);
        }
        if (tranforBool)
        {
            if (_type == 1)
            {
                TipsBg.transform.localPosition = new Vector3(0, (TipsBg.transform.localPosition.y + 480 / (60 * 1f)) > 150 ? 150 : (TipsBg.transform.localPosition.y + 480 / (60 * 1f)), 0);
            }
        }
    }
}
