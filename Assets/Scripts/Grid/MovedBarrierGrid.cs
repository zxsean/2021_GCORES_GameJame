using UnityEngine;

public class MovedBarrierGrid : BarrierGrid
{
    public Vector2[] Path { get; set; }
    public float Speed { get; set; }
    
    private int CurPathIdx { get; set; }
    private bool Inverse { get; set; }

    public MovedBarrierGrid(GridData data) : base(data)
    {
        var barrierData = data as MovedBarrierGridData;
        Path = barrierData.path;
        Speed = barrierData.speed;
        CurPathIdx = 0;
        Inverse = false;
    }

    public override void Update()
    {
        // move
        var curPos = new Vector2(curPosX, curPosY);
        var idx = Inverse ? -1 : 1;
        var nextPos = Path[CurPathIdx + idx];
        var dir = (nextPos - curPos).normalized;
        curPosX += dir.x * Speed * Time.deltaTime;
        curPosY += dir.y * Speed * Time.deltaTime;
        if (Vector2.Distance(new Vector2(curPosX, curPosY), nextPos) <= 1E-6f)
        {
            if (CurPathIdx >= Path.Length - 1)
            {
                Inverse = true;
            }
            else if(CurPathIdx <= 0)
            {
                Inverse = false;
            }
            idx = Inverse ? -1 : 1;
            CurPathIdx += idx;
        }

        // 如果碰到了Entity，则强制位移Entity
        var entities = EntityMgr.GetAllEntity();
        foreach (var entity in entities)
        {
            if (InRange(entity.Bounds))
            {
                entity.CurPosX += dir.x * Speed * Time.deltaTime;
                entity.CurPosY += dir.y * Speed * Time.deltaTime;
            }
        }
        
        transform.localPosition = new Vector3(curPosX, curPosY);
    }
}