using UnityEngine;

public class IllusionPlayer : IGrid, IEntity, IUpdatable, IEffectTarget, IPlayer
{
    public int Row { get; set; }
    public int Col { get; set; }
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public Renderer Renderer { get; }
    public float CurPosX { get; set; }
    public float CurPosY { get; set; }
    public bool IsDestroy { get; private set; }
    public bool IsActive { get; private set; }

    public float Speed { get; set; }
    
    public int Hp { get; set; }
    
    public float Duration { get; set; }
    private float startTime { get; set; }

    public IllusionPlayer()
    {
        gameObject = PoolMgr<GameObject>.Get("IllusionPlayer", () =>
        {
            var prefab = Resources.Load("Prefabs/Entities/IllusionPlayer");
            var ins = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, LevelMgr.CurLevel.EntityRoot) as GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();
        IsActive = true;
    }

    public void Update()
    {
        
        if (Time.realtimeSinceStartup - startTime >= Duration || 
            Hp <= 0)
        {
            gameObject.SetActive(false);
            IsActive = false;
            return;
        }
        
        var offsetX = 0.0f;
        var offsetY = 0.0f;
        
        // 方向
        if (Input.GetKey(KeyCode.W))
        {
            offsetY += Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            offsetY -= Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            offsetX -= Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            offsetX += Speed * Time.deltaTime;
        }
        
        var bounds = Renderer.bounds;
        bounds.center = new Vector3(bounds.center.x + offsetX, bounds.center.y + offsetY);
        
        // check level contains
        if (!LevelMgr.CurLevel.Contains(bounds))
        {
            offsetX = 0;
            offsetY = 0;
        }
        
        // check barrier
        FloorMgr.GetAll<Barrier>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i].InRange(bounds))
            {
                offsetX = 0;
                offsetY = 0;
                break;
            }
        }

        CurPosX += offsetX;
        CurPosY += offsetY;
        var pos = transform.localPosition;
        pos.x = CurPosX;
        pos.y = CurPosY;
        transform.localPosition = pos;
        
        // 先用顶点颜色来模拟淡出效果
        var spRenderer = Renderer as SpriteRenderer;
        var color = spRenderer.color;
        color.a = Mathf.Lerp(1, 0, (Time.realtimeSinceStartup - startTime) / Duration);
        spRenderer.color = color;
    }

    public void Reset()
    {
        IsActive = true;
        gameObject.SetActive(true);
        startTime = Time.realtimeSinceStartup;
    }
    
    public bool InRange(Bounds bounds)
    {
        return Renderer.bounds.Intersects(bounds);
    }
}