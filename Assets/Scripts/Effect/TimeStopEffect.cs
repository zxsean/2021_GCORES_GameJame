using UnityEngine;

public class TimeStopEffect : IEffect, IUpdatable
{
    private static readonly MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static readonly int BulerID = Shader.PropertyToID("_Blur");

    private float radius;
    private float startTime;
    private IPlayer target;


    public TimeStopEffect()
    {
        gameObject = PoolMgr<GameObject>.Get("TimeStopEffect", () =>
        {
            var prefab = Resources.Load("Prefabs/Effects/TimeStopEffect");
            var ins =
                Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, LevelMgr.CurLevel.EffectRoot) as
                    GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();

        Reset();
    }

    public float Radius
    {
        get => radius;
        set
        {
            radius = value;
            var scale = transform.localScale;
            scale.x = radius * 2;
            scale.y = radius * 2;
            transform.localScale = scale;
        }
    }

    public float Duration { get; set; }
    public float DecayTime { get; set; }

    public IPlayer Target
    {
        get => target;
        set
        {
            target = value;
            var bounds = target.Renderer.bounds;
            transform.localPosition = bounds.center;
        }
    }

    private GameObject gameObject { get; }
    private Transform transform { get; }
    private Renderer Renderer { get; }

    public void Reset()
    {
        startTime = Time.realtimeSinceStartup;

        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);
        gameObject.SetActive(true);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        PoolMgr<GameObject>.Return("TimeStopEffect", gameObject);
    }

    public bool IsDestroy { get; private set; }

    public void Update()
    {
        if (Time.realtimeSinceStartup - startTime >= Duration ||
            !Target.IsActive)
            IsDestroy = true;

        FloorMgr.GetAll<IMovatable>(out var list);
        EntityMgr.GetAll<IMovatable>(out var eList);
        EffectMgr.GetAll<IMovatable>(out var efList);
        list.AddRange(eList);
        list.AddRange(efList);
        for (var i = 0; i < list.Count; ++i)
        {
            var moved = list[i];
            if (moved is IGrid grid && InRange(grid.Renderer.bounds))
            {
                moved.SpeedFactor = 0;
                moved.SpeedDecayStartTime = Time.realtimeSinceStartup;
            }
            else
            {
                moved.SpeedFactor = 1;
            }

            moved.SpeedDecayTime = DecayTime;
        }

        var bounds = Target.Renderer.bounds;
        transform.localPosition = bounds.center;

        // 先用顶点颜色来模拟淡出效果
        // 先用顶点颜色来模拟淡出效果
        var blur = Mathf.Lerp(0, 1, (Time.realtimeSinceStartup - startTime) / Duration);
        Mpb.SetFloat(BulerID, blur);
        Renderer.SetPropertyBlock(Mpb);
    }

    private bool InRange(Bounds bounds)
    {
        var radius2 = Radius * Radius;
        var center = Target.Renderer.bounds.center;
        return Mathf.Pow(bounds.min.x - center.x, 2) +
               Mathf.Pow(bounds.min.y - center.y, 2) <= radius2 ||
               Mathf.Pow(bounds.min.x + bounds.size.x - center.x, 2) +
               Mathf.Pow(bounds.min.y - center.y, 2) <= radius2 ||
               Mathf.Pow(bounds.min.x - center.x, 2) +
               Mathf.Pow(bounds.min.y + bounds.size.y - center.y, 2) <= radius2 ||
               Mathf.Pow(bounds.max.x - center.x, 2) +
               Mathf.Pow(bounds.max.y - center.y, 2) <= radius2;
    }
}