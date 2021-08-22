using UnityEngine;

public class Monster : Grid, IEntity, IUpdatable, IMovatable
{
    public int Hp { get; set; }
    public bool IsDestroy { get; private set; }
    public float SpeedFactor { get; set; }
    public float SpeedDecayStartTime { get; set; }
    public float SpeedDecayTime { get; set; }
    public float Speed { get; private set; }
    public int Damage { get; private set; }
    public float ChaseRadius { get; private set; }
    
    public float ChaseRadius2 { get; private set; }
    
    public Vector2[] Path { get; private set; }
    private int CurPathIdx { get; set; }

    private Matrix4x4 rotateMat;
    
    private Sprite Up { get; set; }
    private Sprite Down { get; set; }
    private Sprite Left { get; set; }
    private Sprite Right { get; set; }
    
    private static MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static int BlurID = Shader.PropertyToID("_Blur");

    public Monster(GameObject asset) : base(asset)
    {
        var data = (MonsterData)RawData;
        Hp = data.hp;
        Speed = data.speed;
        Damage = data.damage;
        
        rotateMat = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 30.0f));
        ChaseRadius = data.chaseRadius;
        ChaseRadius2 = ChaseRadius * ChaseRadius;
        Path = data.path;
        
        Up = data.up;
        Down = data.down;
        Left = data.left;
        Right = data.right;
        
        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);
        Mpb.SetInt(BlurID, 0);
        Renderer.SetPropertyBlock(Mpb);
    }
    
    public void Update()
    {
        if (Hp <= 0)
        {
            // 红色
            ((SpriteRenderer)Renderer).color = Color.red;
            Renderer.GetPropertyBlock(Mpb);
            Mpb.SetInt(BlurID, 1);
            Renderer.SetPropertyBlock(Mpb);
            //gameObject.SetActive(false);
            IsDestroy = true;
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

        // chase player
        // 碰到illusion也会杀死illusion
        // idle状态下会按照path巡逻
        var chase = false;
        EntityMgr.GetAll<IPlayer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var p = list[i];
            if (p is Player)
            {
                var target = EntityMgr.Player;
                var pos = transform.localPosition;
                var bounds = target.Renderer.bounds;
                var distance = bounds.center - pos;
                var dis2 = Vector3.Dot(distance, distance);
                if (dis2 <= ChaseRadius2)
                {
                    chase = true;
                    var dir = (bounds.center - pos).normalized;
                    var offset = Speed * Time.deltaTime;
                    var targetBounds = Renderer.bounds;
                    targetBounds.center += dir * (offset * SpeedFactor * 5f);
                    // check barrier
                    FloorMgr.GetAll<Barrier>(out var bList);
                    for (var j = 0; j < bList.Count; ++j)
                    {
                        while (bList[j].InRange(targetBounds))
                        {
                            // 修正下方向
                            dir = rotateMat.MultiplyVector(dir);
                            targetBounds = Renderer.bounds;
                            targetBounds.center += dir * (offset * SpeedFactor * 5f);
                        }
                    }
                
                    pos += dir * (offset * SpeedFactor);
                    transform.localPosition = pos;
                    ChangeSprite(dir);
                    if (InRange(p.Renderer.bounds))
                    {
                        ((IEntity)p).Hp -= Damage;
                    }
                    break;
                }
            }
        }

        if (!chase)
        {
            var pos = transform.localPosition;
            var curPos = new Vector2(pos.x, pos.y);
            var nextPos = Path[CurPathIdx + 1];
            var dir = (nextPos - curPos).normalized;
            var offsetX = dir.x * Speed * 0.4f * SpeedFactor * Time.deltaTime;
            var offsetY = dir.y * Speed * 0.4f * SpeedFactor * Time.deltaTime;
            pos.x += offsetX;
            pos.y += offsetY;
            if (dir.x * (pos.x - nextPos.x) > 0.0f || 
                dir.y * (pos.y - nextPos.y) > 0.0f)
            {
                if (++CurPathIdx >= Path.Length - 1)
                {
                    CurPathIdx = 0;
                }
            }
            
            transform.localPosition = pos;
            ChangeSprite(dir);
        }
    }

    private void ChangeSprite(Vector3 dir)
    {
        dir = dir.normalized;
        var sr = Renderer as SpriteRenderer;
        const float cos45 = 0.707f;
        if (Vector3.Dot(dir, Vector3.up) > cos45)
        {
            sr.sprite = Up;
        }
        else if (Vector3.Dot(dir, Vector3.down) > cos45)
        {
            sr.sprite = Down;
        }
        else if (Vector3.Dot(dir, Vector3.left) > cos45)
        {
            sr.sprite = Left;
        }
        else if (Vector3.Dot(dir, Vector3.right) > cos45)
        {
            sr.sprite = Right;
        }
    }
}