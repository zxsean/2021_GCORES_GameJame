using UnityEngine;

public class Spike : Grid, IFloor, IUpdatable
{
    public FloorType Type { get; private set; }
    public bool IsDestroy { get; private set; }
    
    public int Damage { get; private set; }

    public Spike(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;

        var data = (SpikeData) RawData;
        Damage = data.damage;
    }

    public virtual void Update()
    {
        // 如果IEntity踩到了，那么就受伤
        EntityMgr.GetAll<IEntity>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var entity = list[i];
            if (entity is IGrid grid && InRange(grid.Renderer.bounds))
            {
                entity.Hp -= Damage;
            }
        }
    }
}