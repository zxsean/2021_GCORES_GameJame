using UnityEngine;

public class SpikeTrap : Grid, IFloor, IUpdatable, ITriggerFloor
{
    public FloorType Type { get; private set; }
    public bool IsDestroy { get; private set; }

    public int TriggerId { get; private set; }

    public int Damage { get; private set; }
    
    private bool IsTrigger { get; set; }
    
    private GameObject Down { get; set; }
    private Renderer DownRenderer { get; set; }
    private GameObject Up { get; set; }
    private Renderer UpRenderer { get; set; }
    private float Duration { get; set; }
    private float StartTime { get; set; }
    private Animation Anim { get; set; }

    public SpikeTrap(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;

        var data = (SpikeTrapData) RawData;
        TriggerId = data.triggerId;
        Damage = data.damage;
        Duration = data.duration;

        Down = transform.Find("Down").gameObject;
        Up = transform.Find("Up").gameObject;
        DownRenderer = Down.GetComponent<Renderer>();
        UpRenderer = Up.GetComponent<Renderer>();
        
        Anim = gameObject.GetComponent<Animation>();
        SwitchState(false);
    }

    public void Update()
    {
        if (!IsTrigger)
        {
            return;
        }
        
        // Duration到了就恢复原状
        if (Time.realtimeSinceStartup - StartTime >= Duration)
        {
            IsTrigger = false;
            SwitchState();
            return;
        }
        
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

    public bool Trigger(ITriggerGrid trigger)
    {
        if (IsTrigger && Time.realtimeSinceStartup - StartTime < Duration) return false;

        if (!(trigger is IPlayer))
        {
            return false;
        }
        
        StartTime = Time.realtimeSinceStartup;
        IsTrigger = true;
        SwitchState();
        return true;
    }
    
    private void SwitchState(bool anim = true)
    {
        if (anim)
        {
            Anim.Play(IsTrigger ? "trap_fadein" : "trap_fadeout");
        }

        Renderer = IsTrigger ? DownRenderer : UpRenderer;
    }
}