using UnityEngine;

public class IllusionPlayer : IGrid, IEntity, IUpdatable, IEffectTarget, IPlayer
{
    private static readonly MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static readonly int BulerID = Shader.PropertyToID("_Blur");

    public IllusionPlayer()
    {
        gameObject = PoolMgr<GameObject>.Get("IllusionPlayer", () =>
        {
            var prefab = Resources.Load("Prefabs/Entities/IllusionPlayer");
            var ins =
                Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, LevelMgr.CurLevel.EntityRoot) as
                    GameObject;
            ins.SetActive(true);
            return ins;
        });
        transform = gameObject.transform;
        Renderer = gameObject.GetComponent<Renderer>();
        IsActive = true;
        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);

        var data = gameObject.GetComponent<IllusionPlayerData>();
        Up = data.up;
        Down = data.down;
        Left = data.left;
        Right = data.right;
    }

    public float Speed { get; set; }

    public float Duration { get; set; }
    private float startTime { get; set; }

    private Sprite Up { get; }
    private Sprite Down { get; }
    private Sprite Left { get; }
    private Sprite Right { get; }

    public int Hp { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public Renderer Renderer { get; }
    public float CurPosX { get; set; }
    public float CurPosY { get; set; }

    public bool InRange(Bounds bounds)
    {
        var selfBounds = Renderer.bounds;
        selfBounds.extents *= 0.5f;
        return selfBounds.Intersects(bounds);
    }

    public bool IsActive { get; private set; }
    public bool IsDestroy { get; private set; }

    public void Update()
    {
        if (Time.realtimeSinceStartup - startTime >= Duration ||
            Hp <= 0)
        {
            gameObject.SetActive(false);
            IsActive = false;
            IsDestroy = true;
            PoolMgr<GameObject>.Return("IllusionPlayer", gameObject);
            return;
        }

        var offsetX = 0.0f;
        var offsetY = 0.0f;

        // 方向
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            offsetY += Speed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Up;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            offsetY -= Speed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Down;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            offsetX -= Speed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Left;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            offsetX += Speed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Right;
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
            if (list[i].InRange(bounds))
            {
                offsetX = 0;
                offsetY = 0;
                break;
            }

        CurPosX += offsetX;
        CurPosY += offsetY;
        var pos = transform.localPosition;
        pos.x = CurPosX;
        pos.y = CurPosY;
        transform.localPosition = pos;

        // 先用顶点颜色来模拟淡出效果
        Renderer.GetPropertyBlock(Mpb);
        var blur = Mathf.Lerp(0, 1, (Time.realtimeSinceStartup - startTime) / Duration);
        Mpb.SetFloat(BulerID, blur);
        Renderer.SetPropertyBlock(Mpb);
    }

    public void Reset()
    {
        IsActive = true;
        var pos = transform.localPosition;
        pos.x = CurPosX;
        pos.y = CurPosY;
        transform.localPosition = pos;
        gameObject.SetActive(true);
        startTime = Time.realtimeSinceStartup;
    }
}