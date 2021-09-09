using UnityEngine;

/// <summary>
///     追踪子弹
/// </summary>
public class BulletEffect : IEffect, IGrid, IUpdatable, IMovatable, IFlyer
{
    public BulletEffect()
    {
        Reset();
    }

    public float Speed { get; set; }
    public float RevertSpeed { get; set; }
    public float Acceleration { get; set; }
    public int Damage { get; set; }
    public float Duration { get; set; }
    private float StartTime { get; set; }

    public IEntity Target { get; set; }

    public Vector3 StartPosition
    {
        set => transform.localPosition = value;
    }

    private Animation Anim { get; set; }

    public void Reset()
    {
        gameObject = PoolMgr<GameObject>.Get("BulletEffect", () =>
        {
            var prefab = Resources.Load("Prefabs/Effects/BulletEffect");
            var ins =
                Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, LevelMgr.CurLevel.EffectRoot) as
                    GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();
        Anim = gameObject.GetComponent<Animation>();
        gameObject.SetActive(true);
        IsDestroy = false;
        StartTime = Time.realtimeSinceStartup;
    }

    public void Destroy()
    {
        Anim.Play("bullet_fadeout", () =>
        {
            var sr = (SpriteRenderer) Renderer;
            var color = sr.color;
            color.a = 1.0f;
            sr.color = color;
            //播放消失动画
            gameObject.SetActive(false);
            PoolMgr<GameObject>.Return("BulletEffect", gameObject);
        });
    }

    public void RevertTarget()
    {
        EntityMgr.GetAll<Boss>(out var list);
        Target = list[0];
        Speed = RevertSpeed;
    }

    public int Row { get; }
    public int Col { get; }
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }
    public Renderer Renderer { get; private set; }
    public float CurPosX { get; set; }
    public float CurPosY { get; set; }

    public bool InRange(Bounds bounds)
    {
        var selfBounds = Renderer.bounds;
        selfBounds.extents *= 0.8f;
        return selfBounds.Intersects(bounds);
    }

    public float SpeedFactor { get; set; }
    public float SpeedDecayStartTime { get; set; }
    public float SpeedDecayTime { get; set; }
    public bool IsDestroy { get; private set; }

    public void Update()
    {
        // 最后1s渐变消失
        var passTime = Time.realtimeSinceStartup - StartTime;
        // 时间到了，销毁
        if (passTime >= Duration)
        {
            IsDestroy = true;
            return;
        }

        // 碰到障碍物，销毁
        FloorMgr.GetAll<Barrier>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var barrier = list[i];
            if (InRange(barrier.Renderer.bounds))
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
                InRange(grid.Renderer.bounds))
            {
                AudioMgr.PlaySound(Game.BulletHitSound, transform);
                entity.Hp -= Damage;
                IsDestroy = true;
                return;
            }
        }

        // calc decay
        if (Time.realtimeSinceStartup - SpeedDecayStartTime >= SpeedDecayTime)
            SpeedFactor = 1;
        else
            SpeedFactor = 0;

        //追踪Target
        var pos = transform.localPosition;
        var bounds = ((IGrid) Target).Renderer.bounds;
        var dir = (bounds.center - pos).normalized;
        var t = Time.deltaTime;
        Speed += Acceleration * t;
        pos += dir * (Speed * t * SpeedFactor);
        transform.localPosition = pos;
        var theta = Vector3.Dot(dir, Vector3.right);
        theta = Mathf.Acos(theta);
        theta = dir.y >= 0 ? theta : -theta;
        theta *= Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, theta);
    }
}