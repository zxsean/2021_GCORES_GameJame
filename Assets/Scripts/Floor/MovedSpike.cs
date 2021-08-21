using UnityEngine;

public class MovedSpike : Spike, IUpdatable, IMovatable, ITriggerFloor
{
    public float SpeedFactor { get; set; }
    public float SpeedDecayStartTime { get; set; }
    public float SpeedDecayTime { get; set; }
    
    public Vector2[] Path { get; private set; }
    public float Speed { get; private set; }
    
    private int CurPathIdx { get; set; }
    
    public int TriggerId { get; private set; }
    private bool IsTrigger { get; set; }
    
    private bool IsRevert { get; set; }

    public MovedSpike(GameObject asset) : base(asset)
    {
        var data = (MovedSpikeData)RawData;
        Path = new Vector2[data.path.Length];
        for (var i = 0; i < Path.Length; ++i)
        {
            var path = data.path[i];
            //LevelMgr.GetPosByRowAndCol(dataPos.x, dataPos.y, out var path);
            Path[i] = path;
        }
        Speed = data.speed;
        CurPathIdx = 0;
        TriggerId = data.triggerId;
        IsRevert = data.isRevert;
        IsTrigger = IsRevert;
    }

    public override void Update()
    {
        if (TriggerId > 0 && !IsTrigger)
        {
            return;
        }
        // calc decay
        if (Time.realtimeSinceStartup - SpeedDecayStartTime >= SpeedDecayTime)
        {
            SpeedFactor = 1;
        }
        else
        {
            SpeedFactor = 0;
        }
        
        // move
        var nextPos = Path[CurPathIdx + 1];
        var dir = (nextPos - Path[CurPathIdx]).normalized;
        var offsetX = dir.x * Speed * SpeedFactor * Time.deltaTime;
        var offsetY = dir.y * Speed * SpeedFactor * Time.deltaTime;
        CurPosX += offsetX;
        CurPosY += offsetY;
        if (dir.x * (CurPosX - nextPos.x) > 0.0f || 
            dir.y * (CurPosY - nextPos.y) > 0.0f)
        {
            if (++CurPathIdx >= Path.Length - 1)
            {
                CurPathIdx = 0;
            }
        }
        
        base.Update();
        
        var pos = transform.localPosition;
        pos.x = CurPosX;
        pos.y = CurPosY;
        transform.localPosition = pos;
    }
    
    public bool Trigger(ITriggerGrid trigger)
    {
        if (!(trigger is IPlayer))
        {
            return false;
        }

        IsTrigger = !IsRevert;
        return true;
    }
}