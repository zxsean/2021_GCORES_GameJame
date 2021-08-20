using UnityEngine;

public class SpikeTrap : Grid, IFloor, IUpdatable, ITriggerFloor
{
    public FloorType Type { get; private set; }
    public bool IsDestroy { get; private set; }

    public int TriggerId { get; private set; }

    public int Damage { get; private set; }
    
    private bool IsTrigger { get; set; }
    
    private GameObject Shelter { get; set; }

    public SpikeTrap(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;
        Shelter = transform.Find("Shelter").gameObject;
        var sr = Renderer as SpriteRenderer;
        var color = sr.color;
        color.a = 0.0f;
        sr.color = color;
        
        var data = (SpikeTrapData) RawData;
        TriggerId = data.triggerId;
        Damage = data.damage;
        Shelter.SetActive(true);
    }

    public void Update()
    {
        if (!IsTrigger)
        {
            return;
        }
        
        // 如果IEntity踩到了，那么就受伤
        EntityMgr.GetAll<IEntity>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var entity = list[i];
            if (entity is IGrid grid && grid.InRange(Renderer.bounds))
            {
                entity.Hp -= Damage;
            }
        }
    }

    public bool Trigger(ITriggerGrid trigger)
    {
        if (!(trigger is IPlayer))
        {
            return false;
        }
        
        Shelter.SetActive(false);
        var sr = Renderer as SpriteRenderer;
        var color = sr.color;
        color.a = 1.0f;
        sr.color = color;
        IsTrigger = true;
        return true;
    }
}