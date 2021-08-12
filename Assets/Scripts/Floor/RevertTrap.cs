using UnityEngine;

public class RevertTrap : Grid, IFloor, IUpdatable, ITriggerFloor
{
    public FloorType Type { get; private set; }
    public bool IsDestroy { get; private set; }
    private bool IsTrigger { get; set; }
    public int TriggerId { get; private set; }
    private GameObject Shelter { get; set; }
    
    public RevertTrap(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;

        var data = (RevertTrapData) RawData;
        TriggerId = data.triggerId;
        Shelter = transform.Find("Shelter").gameObject;
        Shelter.SetActive(true);
    }

    public void Update()
    {
        if (!IsTrigger)
        {
            return;
        }
        
        // 如果Flyer踩到了，那么就反转方向
        EffectMgr.GetAll<IFlyer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var flyer = list[i];
            if (flyer is IGrid grid && grid.InRange(Renderer.bounds))
            {
                flyer.RevertTarget();
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
        IsTrigger = true;
        return true;
    }

}