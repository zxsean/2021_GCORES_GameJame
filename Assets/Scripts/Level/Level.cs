using UnityEngine;

/// <summary>
/// 关卡
/// </summary>
public class Level : IUpdatable
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public GameObject gameObject { get; set; }
    public Transform transform { get; set; }
    public Transform GridRoot { get; set; }
    public Transform PlayerRoot { get; set; }
    public Transform MonsterRoot { get; set; }
    
    public Level(LevelData data)
    {
        // init level
        Rows = data.rows;
        Cols = data.cols;
        var prefab = Resources.Load(data.prefab);
        gameObject = Object.Instantiate(
            prefab, 
            Vector3.zero, 
            Quaternion.identity, 
            Game.Root) as GameObject;
        transform = gameObject.transform;
        GridRoot = transform.Find("Grid");
        PlayerRoot = transform.Find("Player");
        MonsterRoot = transform.Find("Monster");
    }

    public void Enter()
    {
        gameObject.SetActive(true);
    }
    
    public void Update()
    {
        EntityMgr.Update();
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        GridMgr.Clear();
    }
}