using System;
using Coffee.UIExtensions;
using UnityEngine;

public class TransitionView : MonoBehaviour
{
    public UITransitionEffect effect;

    private float StartTime { get; set; }
    private Action OnFinished { get; set; }

    public void PlayShow(Action onFinished = null)
    {
        effect.Show();
        OnFinished = onFinished;
        StartTime = Time.realtimeSinceStartup;
    }

    public void PlayHide(Action onFinished = null)
    {
        effect.Hide();
        OnFinished = onFinished;
        StartTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (OnFinished == null) return;
        if (Time.realtimeSinceStartup - StartTime < effect.duration) return;

        OnFinished?.Invoke();
        OnFinished = null;
        StartTime = 0.0f;
    }
}