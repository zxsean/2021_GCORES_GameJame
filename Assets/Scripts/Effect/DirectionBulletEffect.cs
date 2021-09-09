using UnityEngine;

/// <summary>
///     固定方向的子弹
/// </summary>
public class DirectionBulletEffect : IEffect, IGrid, IUpdatable, IMovatable, IFlyer
{
    public DirectionBulletEffect()
    {
        Reset();
    }

    public float Speed { get; set; }
    public int Damage { get; set; }
    public Vector3 Direction { get; set; }

    public Vector3 StartPosition
    {
        set => transform.localPosition = value;
    }

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
        gameObject.SetActive(true);
        IsDestroy = false;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        PoolMgr<GameObject>.Return("BulletEffect", gameObject);
    }


    public void RevertTarget()
    {
        Direction *= -1;
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
            if (entity is IGrid grid &&
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

        //朝固定方向发射
        var pos = transform.localPosition;
        pos += Direction * (Speed * SpeedFactor * Time.deltaTime);
        transform.localPosition = pos;
        var theta = Vector3.Dot(Direction, Vector3.right);
        theta = Mathf.Acos(theta);
        theta = Direction.y >= 0 ? theta : -theta;
        theta *= Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, theta);
    }
}