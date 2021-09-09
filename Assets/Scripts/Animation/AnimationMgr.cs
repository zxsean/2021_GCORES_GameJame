using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationMgr
{
    private static readonly List<Anim> animations = new List<Anim>();
    private static readonly List<int> deleteIndices = new List<int>();
    private static readonly List<Action> callbacks = new List<Action>();

    public static void Play(this Animation @this, string name, Action onFinished = null)
    {
        @this.Play(name);
        var anim = new Anim
        {
            animation = @this,
            onFinished = onFinished
        };
        animations.Add(anim);
    }

    public static void Update()
    {
        for (var i = 0; i < animations.Count; ++i)
            if (!animations[i].animation.isPlaying)
            {
                deleteIndices.Add(i);
                callbacks.Add(animations[i].onFinished);
            }

        for (var i = deleteIndices.Count - 1; i >= 0; --i) animations.RemoveAt(deleteIndices[i]);

        foreach (var callback in callbacks) callback?.Invoke();

        deleteIndices.Clear();
        callbacks.Clear();
    }

    public static void Clear()
    {
        animations.Clear();
        deleteIndices.Clear();
        callbacks.Clear();
    }

    private struct Anim
    {
        public Animation animation;
        public Action onFinished;
    }
}