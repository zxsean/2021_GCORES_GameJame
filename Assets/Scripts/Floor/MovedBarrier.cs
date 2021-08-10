using UnityEngine;

public class MovedBarrier: Barrier, IUpdatable
{
    public Vector2[] Path { get; set; }
    public float Speed { get; set; }
    public bool IsDestroy { get; private set; }
    
    private int CurPathIdx { get; set; }

    public MovedBarrier(GameObject asset) : base(asset)
    {
        var barrierData = (MovedBarrierData)RawData;
        Path = new Vector2[barrierData.path.Length];
        for (var i = 0; i < Path.Length; ++i)
        {
            var dataPos = barrierData.path[i];
            LevelMgr.GetPosByRowAndCol(dataPos.x, dataPos.y, out var path);
            Path[i] = path;
        }
        Speed = barrierData.speed;
        CurPathIdx = 0;
    }

    public void Update()
    {
        // move
        var nextPos = Path[CurPathIdx + 1];
        var dir = (nextPos - Path[CurPathIdx]).normalized;
        var offsetX = dir.x * Speed * Time.deltaTime;
        var offsetY = dir.y * Speed * Time.deltaTime;
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

        // 如果碰到了Entity，则强制位移Entity
        var entities = EntityMgr.GetAllEntity();
        for (var i = 0; i < entities.Count; ++i)
        {
            if (entities[i] is Grid grid && grid.InRange(Renderer.bounds))
            {
                //多推出一点距离保证不会被粘住
                grid.CurPosX += offsetX + dir.x * 0.1f;
                grid.CurPosY += offsetY + dir.y * 0.1f;
            }
        }

        var pos = transform.localPosition;
        pos.x = CurPosX;
        pos.y = CurPosY;
        transform.localPosition = pos;
    }
}