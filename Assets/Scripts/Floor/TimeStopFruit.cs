using System.Collections.Generic;
using UnityEngine;

public class TimeStopFruit : Grid, IFloor, IUpdatable
{
    public FloorType Type { get; private set; }
    public bool IsDestroy { get; private set; }
    
    private float Radius { get; set; }
    private float Duration { get; set; }
    private float DecayTime { get; set; }
    
    public TimeStopFruit(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;

        var data = (TimeStopFruitData) RawData;
        Radius = data.radius;
        Duration = data.duration;
        DecayTime = data.decayTime;
    }
    
    public void Update()
    {
        // 如果Player踩到了，那么就产生时停特效
        EntityMgr.GetAll<IPlayer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var player = list[i];
            if (player is IGrid grid && grid.InRange(Renderer.bounds))
            {
                var effect = EffectMgr.CreateTargetEffect<IEffectTarget, TimeStopEffect>((IEffectTarget)player);
                effect.Duration = Duration;
                effect.Radius = Radius;
                effect.DecayTime = DecayTime;
                effect.Target = player;
            }
        }
    }
}