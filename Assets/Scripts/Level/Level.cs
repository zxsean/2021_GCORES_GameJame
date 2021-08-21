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
    public Transform EffectRoot { get; private set; }

    private Bounds Bounds { get; set; }
    
    private Animation Anim { get; set; }
    private bool IsPause { get; set; }

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
        EffectRoot = transform.Find("Effect");
        Anim = transform.GetComponent<Animation>();
        
        Bounds = new Bounds(Vector3.zero, new Vector3(data.width, data.height, 1.0f));
    }

    public void Enter(bool anim = true)
    {
        // init grid
        FloorMgr.CreateFloors(FloorRoot);
        // init entity
        EntityMgr.CreateEntities(EntityRoot);
        
        gameObject.SetActive(true);
        if (anim)
        {
            Anim.Play("level_enter");
        }
        
        CameraMgr.Follow(EntityMgr.Player.transform);
    }
    
    
    public void Update()
    {
        if (IsPause) return;
        EntityMgr.Update();
        EffectMgr.Update();
        FloorMgr.Update();
    }

    public void Pause()
    {
        IsPause = true;
    }

    public void Resume()
    {
        IsPause = false;
    }

    public void Exit(bool anim = true)
    {
        CameraMgr.UnFollow();
        Clear();
        if (anim)
        {
            Anim.Play("level_exit", () =>
            {
                gameObject.SetActive(false);
            });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        FloorMgr.Clear();
        EntityMgr.Clear();
        EffectMgr.Clear();
        PoolMgr<GameObject>.Clear();
    }

    public bool Contains(Bounds bounds)
    {
        return Bounds.Contains(bounds.min) && Bounds.Contains(bounds.max);
    }
}