using System.Collections.Generic;

public static class EffectMgr
{
    private static readonly List<IEffect> effects = new List<IEffect>();
    private static readonly Dictionary<IEffectTarget, IEffect> targetEffects = new Dictionary<IEffectTarget, IEffect>();

    public static T CreateEffect<T>() where T : IEffect, new()
    {
        var effect = new T();
        effects.Add(effect);
        return effect;
    }

    public static E CreateTargetEffect<T, E>(T target)
        where T : IEffectTarget
        where E : IEffect, new()
    {
        if (!targetEffects.TryGetValue(target, out var effect))
        {
            effect = new E();
            targetEffects.Add(target, effect);
            return (E) effect;
        }

        effect.Reset();
        return (E) effect;
    }

    public static void Update()
    {
        for (var i = 0; i < effects.Count; ++i)
            if (effects[i] is IUpdatable updatable)
                updatable.Update();

        foreach (var kvp in targetEffects)
            if (kvp.Value is IUpdatable updatable)
                updatable.Update();

        for (var i = effects.Count - 1; i >= 0; --i)
            if (effects[i] is IUpdatable updatable && updatable.IsDestroy)
            {
                effects[i].Destroy();
                effects.RemoveAt(i);
            }

        var delList = new List<IEffectTarget>();
        foreach (var kvp in targetEffects)
            if (kvp.Value is IUpdatable updatable && updatable.IsDestroy)
            {
                kvp.Value.Destroy();
                delList.Add(kvp.Key);
            }

        for (var i = 0; i < delList.Count; ++i) targetEffects.Remove(delList[i]);
    }

    public static void Clear()
    {
        for (var i = 0; i < effects.Count; ++i) effects[i].Destroy();

        foreach (var kvp in targetEffects) kvp.Value.Destroy();

        effects.Clear();
        targetEffects.Clear();
    }

    public static void GetAll<T>(out List<T> list)
    {
        list = new List<T>();
        for (var i = 0; i < effects.Count; ++i)
            if (effects[i] is T)
                list.Add((T) effects[i]);
    }
}