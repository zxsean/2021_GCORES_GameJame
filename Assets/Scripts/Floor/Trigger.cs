using System.Collections.Generic;
using UnityEngine;

public class Trigger : Grid, IFloor, IUpdatable
{
    public FloorType Type { get; }
    public bool IsDestroy { get; }
    
    public int TargetId { get; private set; }
    
    private Dictionary<ITriggerGrid, bool> inDic = new Dictionary<ITriggerGrid, bool>();

    public Trigger(GameObject asset) : base(asset)
    {
        var data = (TriggerData) RawData;
        Type = data.type;
        TargetId = data.targetId;
    }
    
    public void Update()
    {
        // 如果IPlayer踩到了，那么就触发
        EntityMgr.GetAll<ITriggerGrid>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var trigger = list[i];
            if (trigger.InRange(Renderer.bounds))
            {
                if (!inDic.ContainsKey(trigger))
                {
                    var target = FloorMgr.GetTrigger(TargetId);
                    if (target.Trigger(trigger))
                    {
                        inDic.Add(trigger, true);
                    }
                }
            }
            else
            {
                inDic.Remove(trigger);
            }
        }
    }
}