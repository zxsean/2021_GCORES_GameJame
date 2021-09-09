using UnityEngine;

public class Exit : Grid, IFloor, IUpdatable
{
    public Exit(GameObject asset) : base(asset)
    {
        Type = ((ExitData) RawData).type;
    }

    public FloorType Type { get; }
    public bool IsDestroy { get; }

    public void Update()
    {
        if (InRange(EntityMgr.Player.Renderer.bounds))
            // 下一关！
            LevelMgr.NextLevel();
    }
}