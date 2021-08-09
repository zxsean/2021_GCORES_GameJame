
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid管理器
/// </summary>
public static class GridMgr
{
    private static List<Grid> grids = new List<Grid>();

    public static void CreateGrids(GridData[] data)
    {
        for (var i = 0; i < data.Length; ++i)
        {
            CreateGrid(data[i]);
        }
    }
    
    public static void CreateGrid(GridData data)
    {
        Grid grid = null;
        switch ((GridType)data.type)
        {
            case GridType.Empty:
                grid = new EmptyGrid(data);
                break;
            case GridType.Barrier:
                grid = new BarrierGrid(data);
                break;
            case GridType.MovedBarrier:
                grid = new MovedBarrierGrid(data);
                break;
            case GridType.Spike:
                break;
            case GridType.MovedSpike:
                break;
            case GridType.Flyer:
                break;
        }
        grids.Add(grid);
    }

    public static void Clear()
    {
        grids.Clear();
    }

    public static Grid GetIntersectedGridByBounds(Bounds bounds)
    {
        foreach (var grid in grids)
        {
            if (grid.InRange(bounds))
            {
                return grid;
            }
        }
        
        return null;
    }
}