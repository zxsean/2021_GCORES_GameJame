using UnityEngine;

public class IllusionGen : Grid, ITriggerFloor
{
    public FloorType Type { get; }
    public int TriggerId { get; private set; }
    public float Duration { get; private set; }

    public IllusionGen(GameObject asset) : base(asset)
    {
        var data = (IllusionGenData) RawData;
        TriggerId = data.triggerId;
        Type = data.type;
        Duration = data.duration;
    }

    public bool Trigger(ITriggerGrid trigger)
    {
        if (!(trigger is Player))
        {
            return false;
        }
        
        var entity = EntityMgr.GetOrCreateEntity<IllusionPlayer>();
        entity.Hp = EntityMgr.Player.Hp;
        entity.Speed = EntityMgr.Player.Speed;
        entity.Row = Row;
        entity.Col = Col;
        entity.CurPosX = CurPosX;
        entity.CurPosY = CurPosY;
        entity.Duration = Duration;
        entity.Reset();
        return true;
    }
}