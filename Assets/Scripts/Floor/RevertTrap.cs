using System.Collections.Generic;
using UnityEngine;

public class RevertTrap : Grid, IFloor, IUpdatable, ITriggerFloor
{
    private readonly Dictionary<IFlyer, bool> inDic = new Dictionary<IFlyer, bool>();


    public RevertTrap(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;

        var data = (RevertTrapData) RawData;
        TriggerId = data.triggerId;
        Duration = data.duration;

        Down = transform.Find("Down").gameObject;
        Up = transform.Find("Up").gameObject;
        DownRenderer = Down.GetComponent<Renderer>();
        UpRenderer = Up.GetComponent<Renderer>();

        Anim = gameObject.GetComponent<Animation>();
        SwitchState(false);
    }

    private bool IsTrigger { get; set; }
    private GameObject Down { get; }
    private Renderer DownRenderer { get; }
    private GameObject Up { get; }
    private Renderer UpRenderer { get; }
    private float Duration { get; }
    private float StartTime { get; set; }
    private Animation Anim { get; }
    public FloorType Type { get; }
    public int TriggerId { get; }

    public bool Trigger(ITriggerGrid trigger)
    {
        if (IsTrigger && Time.realtimeSinceStartup - StartTime < Duration) return false;

        if (!(trigger is IPlayer)) return false;

        StartTime = Time.realtimeSinceStartup;
        IsTrigger = true;
        SwitchState();
        return true;
    }

    public bool IsDestroy { get; private set; }

    public void Update()
    {
        if (!IsTrigger) return;

        // Duration到了就恢复原状
        if (Time.realtimeSinceStartup - StartTime >= Duration)
        {
            IsTrigger = false;
            SwitchState();
            return;
        }

        // 如果Flyer踩到了，那么就反转方向
        EffectMgr.GetAll<IFlyer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var flyer = list[i];
            if (flyer is IGrid grid && InRange(grid.Renderer.bounds))
            {
                if (!inDic.ContainsKey(flyer))
                {
                    flyer.RevertTarget();
                    inDic.Add(flyer, true);
                }
            }
            else
            {
                inDic.Remove(flyer);
            }
        }
    }


    private void SwitchState(bool anim = true)
    {
        if (anim) Anim.Play(IsTrigger ? "trap_fadein" : "trap_fadeout");
        Renderer = IsTrigger ? DownRenderer : UpRenderer;
    }
}