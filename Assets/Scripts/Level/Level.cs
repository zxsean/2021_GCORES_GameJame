using UnityEngine;

/// <summary>
///     关卡
/// </summary>
public class Level
{
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

    public int Rows { get; }
    public int Cols { get; }
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public Transform FloorRoot { get; }
    public Transform EntityRoot { get; }
    public Transform EffectRoot { get; }

    private Bounds Bounds { get; }

    private Animation Anim { get; }
    private bool IsPause { get; set; }

    public void Enter(bool anim = true)
    {
        // init grid
        FloorMgr.CreateFloors(FloorRoot);
        // init entity
        EntityMgr.CreateEntities(EntityRoot);

        gameObject.SetActive(true);
        if (anim) Anim.Play("level_enter");

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
        AudioMgr.StopAllSound();
        Clear();
        if (anim)
            Anim.Play("level_exit", () => { gameObject.SetActive(false); });
        else
            gameObject.SetActive(false);
    }

    public void Clear()
    {
        FloorMgr.Clear();
        EntityMgr.Clear();
        EffectMgr.Clear();
        PoolMgr<GameObject>.Clear();
        AnimationMgr.Clear();
    }

    public bool Contains(Bounds bounds)
    {
        return Bounds.Contains(bounds.min) && Bounds.Contains(bounds.max);
    }
}