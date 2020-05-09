using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JellyAnimition : MonoBehaviour
{
    public Transform trans;

    public void OnAnimition(bool play)
    {
        if (play)
        {
            trans.DOShakeScale(0.3f, 0.15f, 2, 20);//.SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            trans.DOKill();
            trans.localScale = Vector3.one;
            //Debug.Log(trans.localScale);
        }
    }


    public void OnClick()
    {
        //trans. = false;
        //OnAnimition(false);go
        //go.transform.GetComponent<BoxCollider2D>().enabled = false;
        trans.DOShakeScale(0.3f, 0.3f, 8, 80).OnComplete(() =>
        {
            //go.transform.GetComponent<BoxCollider2D>().enabled = true;
            //btn.interactable = true;
            OnAnimition(true);
            Invoke("Destroy", 0.3f);
        });

    }
    private void OnDestroy()
    {
        OnAnimition(false);
    }
    public void Destroy()
    {
        OnAnimition(false);
    }
}