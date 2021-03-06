using UnityEngine;

public class Spike : Grid, IFloor, IUpdatable
{
    public Spike(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;

        var data = (SpikeData) RawData;
        Damage = data.damage;
    }

    public int Damage { get; }
    public FloorType Type { get; }
    public bool IsDestroy { get; private set; }

    public virtual void Update()
    {
        // 如果IEntity踩到了，那么就受伤
        EntityMgr.GetAll<IEntity>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var entity = list[i];
            if (entity is IGrid grid && InRange(grid.Renderer.bounds)) entity.Hp -= Damage;
        }
    }
}