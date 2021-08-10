
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Floor管理器
/// </summary>
public static class FloorMgr
{
    private static List<IFloor> floors = new List<IFloor>();

    public static void CreateFloors(Transform assets)
    {
        for (var i = 0; i < assets.childCount; ++i)
        {
            CreateFloor(assets.GetChild(i));
        }
    }
    
    public static void CreateFloor(Transform asset)
    {
        var data = asset.GetComponent<FloorData>();
        IFloor floor = null;
        switch (data.type)
        {
            case FloorType.Barrier:
                floor = new Barrier(asset.gameObject);
                break;
            case FloorType.MovedBarrier:
                floor = new MovedBarrier(asset.gameObject);
                break;
            case FloorType.Spike:
                break;
            case FloorType.MovedSpike:
                break;
            case FloorType.Flyer:
                break;
        }
        floors.Add(floor);
    }
    
    public static void Update()
    {
        for (var i = 0; i < floors.Count; ++i)
        {
            var floor = floors[i];
            if (floor is IUpdatable updatable)
            {
                updatable.Update();
            }
        }
        
        // 清理掉被标记为删除的Floor
        for (var i = floors.Count - 1; i >= 0; --i)
        {
            if (floors[i] is IUpdatable updatable && updatable.IsDestroy)
            {
                floors.RemoveAt(i);
            }
        }
    }

    public static void Clear()
    {
        floors.Clear();
    }

    public static void GetAll<T>(out List<T> list) where T : IFloor
    {
        list = new List<T>();
        for (var i = 0; i < floors.Count; ++i)
        {
            if (floors[i] is T)
            {
                list.Add((T)floors[i]);
            }
        }
    }
}