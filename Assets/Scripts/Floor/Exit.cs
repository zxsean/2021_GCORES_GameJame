using UnityEngine;

public class Exit : Grid, IFloor, IUpdatable
{
    public FloorType Type { get; }
    public bool IsDestroy { get; }
    
    public Exit(GameObject asset) : base(asset)
    {
        Type = ((ExitData) RawData).type;
    }
    
    public void Update()
    {
        if (InRange(EntityMgr.Player.Renderer.bounds))
        {
            // 下一关！
            LevelMgr.NextLevel();
        }
    }
}