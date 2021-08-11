using UnityEngine;

public class TimeStopEffect : IEffect, IUpdatable
{
    public bool IsDestroy { get; private set; }

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
    public IPlayer Target { get; set; }
    
    private GameObject gameObject { get; set; }
    private Transform transform { get; set; }
    private Renderer Renderer { get; set; }

    private float radius;
    private float startTime;

    public TimeStopEffect()
    {
        gameObject = PoolMgr<GameObject>.Get("TimeStopEffect", () =>
        {
            var prefab = Resources.Load("Prefabs/Effects/TimeStopEffect");
            var ins = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, LevelMgr.CurLevel.EffectRoot) as GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();
        
        Reset();
    }
    
    public void Update()
    {
        if (Time.realtimeSinceStartup - startTime >= Duration ||
            !Target.IsActive)
        {
            IsDestroy = true;
        }
        
        FloorMgr.GetAll<IMovatable>(out var list);
        EntityMgr.GetAll<IMovatable>(out var eList);
        list.AddRange(eList);
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

        var bounds = ((IGrid) Target).Renderer.bounds;
        transform.localPosition = bounds.center;
    }

    public void Reset()
    {
        startTime = Time.realtimeSinceStartup;
        gameObject.SetActive(true);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        PoolMgr<GameObject>.Return("TimeStopEffect", gameObject);
    }

    private bool InRange(Bounds bounds)
    {
        var radius2 = Radius * Radius;
        var center = ((IGrid)Target).Renderer.bounds.center;
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