using System;
using System.Collections.Generic;

public static class PoolMgr<T>
{
    private static readonly Dictionary<string, List<PoolObj>> pool = new Dictionary<string, List<PoolObj>>();

    public static T Get(string name, Func<T> alloc)
    {
        if (!pool.TryGetValue(name, out var list))
        {
            list = new List<PoolObj>();
            pool.Add(name, list);
        }

        for (var i = 0; i < list.Count; ++i)
            if (!list[i].inUse)
            {
                list[i].inUse = true;
                return list[i].obj;
            }


        var obj = alloc();
        list.Add(new PoolObj
        {
            obj = obj,
            inUse = true
        });
        return obj;
    }

    public static void Return(string name, T obj)
    {
        if (!pool.TryGetValue(name, out var list)) return;

        for (var i = 0; i < list.Count; ++i)
            if (list[i].obj.Equals(obj))
                list[i].inUse = false;
    }

    public static void Clear()
    {
        pool.Clear();
    }

    private class PoolObj
    {
        public bool inUse;
        public T obj;
    }
}