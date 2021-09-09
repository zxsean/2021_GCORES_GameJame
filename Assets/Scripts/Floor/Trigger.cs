using System.Collections.Generic;
using UnityEngine;

public class Trigger : Grid, IFloor, IUpdatable
{
    private readonly Dictionary<ITriggerGrid, bool> inDic = new Dictionary<ITriggerGrid, bool>();

    public Trigger(GameObject asset) : base(asset)
    {
        var data = (TriggerData) RawData;
        Type = data.type;
        TargetId = data.targetId;
        Down = transform.Find("Down").gameObject;
        Up = transform.Find("Up").gameObject;
        DownRenderer = Down.GetComponent<Renderer>();
        UpRenderer = Up.GetComponent<Renderer>();
        SwitchState();
    }

    public int TargetId { get; }

    private GameObject Down { get; }
    private Renderer DownRenderer { get; }
    private GameObject Up { get; }
    private Renderer UpRenderer { get; }
    public FloorType Type { get; }
    public bool IsDestroy { get; }

    public void Update()
    {
        // 如果IPlayer踩到了，那么就触发
        EntityMgr.GetAll<ITriggerGrid>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var trigger = list[i];
            if (InRange(trigger.Renderer.bounds))
            {
                if (!inDic.ContainsKey(trigger))
                {
                    var target = FloorMgr.GetTrigger(TargetId);
                    if (target.Trigger(trigger)) inDic.Add(trigger, true);
                }
            }
            else
            {
                inDic.Remove(trigger);
            }
        }

        SwitchState();
    }

    private void SwitchState()
    {
        var inTrigger = inDic.Count != 0;
        Down.SetActive(inTrigger);
        Up.SetActive(!inTrigger);
        Renderer = inTrigger ? DownRenderer : UpRenderer;
    }
}