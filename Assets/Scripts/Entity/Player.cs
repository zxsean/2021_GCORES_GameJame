using UnityEngine;

public class Player : IEntity
{
    public bool IsDestroy { get; private set; }

    public float CurPosX
    {
        get => curPosX;
        set => curPosX = value;
    }

    public float CurPosY { 
        get => curPosY;
        set => curPosY = value;
    }
    
    public int Row { get; private set; }
    public int Col { get; private set; }
    public float Speed { get; private set; }
    public GameObject gameObject { get; set; }
    public Transform transform { get; set; }

    private float curPosX;
    private float curPosY;
    public Bounds Bounds => new Bounds(new Vector3(curPosX, curPosY), Vector3.one);
    private float CurSpeed { get; set; }

    public Player(PlayerData data)
    {
        Row = data.row;
        Col = data.col;
        Speed = data.speed;
        CurSpeed = Speed;
        LevelMgr.GetPosByRowAndCol(data.levelId, Row, Col, out curPosX, out curPosY);
        
        var prefab = Resources.Load(data.prefab);
        gameObject = Object.Instantiate(
            prefab, 
            new Vector3(curPosX, curPosY), 
            Quaternion.identity, 
            LevelMgr.CurLevel.PlayerRoot) as GameObject;
        transform = gameObject.transform;
    }

    public void Update()
    {
        // 处理状态
        ProcessStates();
        
        // 处理输入
        ProcessInputs();
    }

    private void ProcessStates()
    {
        
    }

    private void ProcessInputs()
    {
        var offsetX = 0.0f;
        var offsetY = 0.0f;
        
        // 方向
        if (Input.GetKey(KeyCode.W))
        {
            offsetY += CurSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            offsetY -= CurSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            offsetX -= CurSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            offsetX += CurSpeed * Time.deltaTime;
        }

        var bounds = Bounds;
        bounds.center = new Vector3(curPosX + offsetX, curPosY + offsetY);
        var grid = GridMgr.GetIntersectedGridByBounds(bounds);
        if (grid != null && grid.CanPass)
        {
            curPosX += offsetX;
            curPosY += offsetY;
        }
    }
}