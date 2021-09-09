using UnityEngine;

public class IllusionGen : Grid, ITriggerFloor
{
    public IllusionGen(GameObject asset) : base(asset)
    {
        var data = (IllusionGenData) RawData;
        TriggerId = data.triggerId;
        Type = data.type;
        Duration = data.duration;
    }

    public float Duration { get; }
    public FloorType Type { get; }
    public int TriggerId { get; }

    public bool Trigger(ITriggerGrid trigger)
    {
        if (!(trigger is Player)) return false;

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