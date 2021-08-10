using UnityEngine;

public class TimeStopEffect : IEffect, IUpdatable
{
    public bool IsDestroy { get; private set; }
    
    public float Radius { get; set; }
    public float Duration { get; set; }
    public float DecayTime { get; set; }
    public Player Target { get; set; }
    
    private GameObject gameObject { get; set; }
    private Transform transform { get; set; }
    private Renderer Renderer { get; set; }
    
    private float startTime;

    public TimeStopEffect()
    {
        gameObject = PoolMgr<GameObject>.Get("TimeStopEffect", () =>
        {
            var prefab = Resources.Load("TimeStopEffect");
            var ins = Object.Instantiate(prefab) as GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();
        
        Reset();
    }
    
    public void Update()
    {
        if (Time.realtimeSinceStartup - startTime >= Duration)
        {
            IsDestroy = true;
        }
        
        FloorMgr.GetAll<IMovedFloor>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var moved = list[i];
            if (moved is Grid grid && InRange(grid.Renderer.bounds))
            {
                moved.SpeedFactor = 0;
            }
            else
            {
                moved.SpeedFactor = 1;
            }
            moved.SpeedDecayTime = DecayTime;
            moved.SpeedDecayStartTime = Time.realtimeSinceStartup;
        }
        
        var pos = transform.localPosition;
        pos.x = Target.CurPosX;
        pos.y = Target.CurPosY;
        transform.localPosition = pos;
    }

    public void Reset()
    {
        startTime = Time.realtimeSinceStartup;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        PoolMgr<GameObject>.Return("TimeStopEffect", gameObject);
    }

    private bool InRange(Bounds bounds)
    {
        var radius2 = Radius * Radius;
        return Mathf.Pow(bounds.min.x - Target.CurPosX, 2) +
               Mathf.Pow(bounds.min.y - Target.CurPosY, 2) <= radius2 ||
               Mathf.Pow(bounds.min.x + bounds.size.x - Target.CurPosX, 2) +
               Mathf.Pow(bounds.min.y - Target.CurPosY, 2) <= radius2 ||
               Mathf.Pow(bounds.min.x - Target.CurPosX, 2) +
               Mathf.Pow(bounds.min.y + bounds.size.y - Target.CurPosY, 2) <= radius2 ||
               Mathf.Pow(bounds.max.x - Target.CurPosX, 2) +
               Mathf.Pow(bounds.max.y - Target.CurPosY, 2) <= radius2;
    }
}