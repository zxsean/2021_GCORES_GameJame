using UnityEngine;

/// <summary>
/// 关卡
/// </summary>
public class Level
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }
    public Transform FloorRoot { get; private set; }
    public Transform EntityRoot { get; private set; }
    
    private Bounds Bounds { get; set; }

    public Level(GameObject asset)
    {
        var data = asset.GetComponent<LevelData>();
        // init level
        Rows = data.rows;
        Cols = data.cols;
        gameObject = asset;
        transform = asset.transform;
        FloorRoot = transform.Find("Floor");
        EntityRoot = transform.Find("Entity");
        
        Bounds = new Bounds(Vector3.zero, new Vector3(Cols, Rows, 1.0f));
    }

    public void Enter()
    {
        // init grid
        FloorMgr.CreateFloors(FloorRoot);
        // init entity
        EntityMgr.CreateEntities(EntityRoot);
        
        gameObject.SetActive(true);
    }
    
    public void Update()
    {
        FloorMgr.Update();
        EntityMgr.Update();
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        FloorMgr.Clear();
        EntityMgr.Clear();
    }

    public bool Contains(Bounds bounds)
    {
        return Bounds.Contains(bounds.min) && Bounds.Contains(bounds.max);
    }
}