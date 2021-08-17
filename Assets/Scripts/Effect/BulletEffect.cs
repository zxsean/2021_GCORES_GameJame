using UnityEngine;

public class BulletEffect : IEffect, IGrid, IUpdatable, IMovatable, IFlyer
{
    public bool IsDestroy { get; private set; }
    public float SpeedFactor { get; set; }
    public float SpeedDecayStartTime { get; set; }
    public float SpeedDecayTime { get; set; }
    public float Speed { get; set; }
    public int Damage { get; set; }

    public int Row { get; }
    public int Col { get; }
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }
    public Renderer Renderer { get; private set; }
    public float CurPosX { get; set; }
    public float CurPosY { get; set; }
    public bool InRange(Bounds bounds)
    {
        return Renderer.bounds.Intersects(bounds);
    }

    public IEntity Target { get; set; }

    public Vector3 StartPosition
    {
        set => transform.localPosition = value;
    }

    public BulletEffect()
    {
        Reset();
    }
    
    public void Reset()
    {
        gameObject = PoolMgr<GameObject>.Get("BulletEffect", () =>
        {
            var prefab = Resources.Load("Prefabs/Effects/BulletEffect");
            var ins = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, LevelMgr.CurLevel.EffectRoot) as GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();
        gameObject.SetActive(true);
        IsDestroy = false;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        PoolMgr<GameObject>.Return("BulletEffect", gameObject);
    }
    
    public void Update()
    {
        // 碰到障碍物，销毁
        FloorMgr.GetAll<Barrier>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var barrier = list[i];
            if (barrier.InRange(Renderer.bounds))
            {
                IsDestroy = true;
                return;
            }
        }

        var eList = EntityMgr.GetAllEntity();
        for (var i = 0; i < eList.Count; ++i)
        {
            var entity = eList[i];
            if (entity == Target && 
                entity is IGrid grid && 
                grid.InRange(Renderer.bounds))
            {
                AudioMgr.PlaySound(Game.BulletHitSound);
                entity.Hp -= Damage;
                IsDestroy = true;
                return;
            }
        }
        
        // calc decay
        if (Time.realtimeSinceStartup - SpeedDecayStartTime >= SpeedDecayTime)
        {
            SpeedFactor = 1;
        }
        else
        {
            SpeedFactor = 0;
        }
        
        //追踪Target
        var pos = transform.localPosition;
        var bounds = ((IGrid)Target).Renderer.bounds;
        var dir = (bounds.center - pos).normalized;
        pos += dir * (Speed * SpeedFactor * Time.deltaTime);
        transform.localPosition = pos;
    }

    public void RevertTarget()
    {
        EntityMgr.GetAll<Boss>(out var list);
        Target = list[0];
    }
}