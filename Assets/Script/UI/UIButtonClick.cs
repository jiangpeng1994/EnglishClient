//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example script of how a button can be scaled visibly when it gets pressed.
/// </summary>

public class UIButtonClick : MonoBehaviour
{
    public Transform tweenTarget;
    Vector3 pressed = new Vector3(0.95f, 0.95f, 0.95f);
    float duration = 0.01f;

    Vector3 mScale;
    bool mStarted = false;

    void Start()
    {
        if (!mStarted)
        {
            mStarted = true;
            if (tweenTarget == null) tweenTarget = transform;
            mScale = tweenTarget.localScale;
        }
    }
    
    void OnDisable()
    {
        if (mStarted && tweenTarget != null)
        {
            TweenScale tc = tweenTarget.GetComponent<TweenScale>();

            if (tc != null)
            {
                tc.value = mScale;
                tc.enabled = false;
            }
        }
    }

    void OnPress(bool isPressed)
    {
        if (enabled)
        {
            if (!mStarted) Start();
            TweenScale.Begin(tweenTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) : mScale).method = UITweener.Method.EaseInOut;
        }
    }
}
