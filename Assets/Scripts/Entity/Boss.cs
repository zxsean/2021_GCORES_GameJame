using UnityEngine;

public class Boss : Grid, IEntity, IUpdatable
{
    private int hp;

    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            IsHurt = true;
        }
    }
    public bool IsDestroy { get; private set; }
    
    private int Interval { get; set; }
    
    private float LastTime { get; set; }
    private float BulletSpeed { get; set; }
    private int BulletDamage { get; set; }
    private float BulletDuration { get; set; }
    private float Radius { get; set; }
    private float Speed { get; set; }
    private float BulletRevertSpeed { get; set; }
    private float BulletAcceleration { get; set; }
    private int Winding { get; set; }
    private float Length { get; set; }
    private bool IsHurt { get; set; }
    
    private Sprite Up { get; set; }
    private Sprite Down { get; set; }
    private Sprite Left { get; set; }
    private Sprite Right { get; set; }
    
    private static MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static int BlurID = Shader.PropertyToID("_Blur");


    public Boss(GameObject asset) : base(asset)
    {
        var data = (BossData) RawData;
        Hp = data.hp;
        Interval = data.interval;
        BulletDamage = data.bulletDamage;
        BulletSpeed = data.bulletSpeed;
        BulletRevertSpeed = data.bulletRevertSpeed;
        BulletDuration = data.bulletDuration;
        BulletAcceleration = data.bulletAcceleration;
        Radius = transform.localPosition.magnitude;
        Speed = data.speed;
        Winding = (int) data.winding;
        Length = 2 * Mathf.PI * Radius;
        
        Up = data.up;
        Down = data.down;
        Left = data.left;
        Right = data.right;
        
        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);
        Mpb.SetFloat(BlurID, 0);
        Renderer.SetPropertyBlock(Mpb);
    }
    
    public void Update()
    {
        var sr = Renderer as SpriteRenderer;
        var color = sr.color;
        if (IsHurt)
        {
            // 闪一下红色
            color = Color.red;
            sr.color = color;
            IsHurt = false;
        }
        else
        {
            sr.color = Color.white;
        }
        
        if (Hp <= 0)
        {
            Mpb.Clear();
            Renderer.GetPropertyBlock(Mpb);
            Mpb.SetFloat(BlurID, 0.5f);
            Renderer.SetPropertyBlock(Mpb);
            // 通关！
            IsDestroy = true;
            LevelMgr.CurLevel.Pause();
            CameraMgr.Move(transform.localPosition, () =>
            {
                LevelMgr.NextLevel();
            });
        }

        // move
        // 计算一个旋转矩阵 用来旋转位置
        var rand = Time.deltaTime * Speed / Length;
        rand *= Winding;
        var pos = transform.localPosition;
        pos.x = pos.x * Mathf.Cos(rand) - pos.y * Mathf.Sin(rand);
        pos.y = pos.x * Mathf.Sin(rand) + pos.y * Mathf.Cos(rand);
        transform.localPosition = pos;
        ChangeSprite(-pos);
        
        // 玩家碰到死
        EntityMgr.GetAll<IPlayer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var p = list[i];
            if (p is IGrid grid && InRange(grid.Renderer.bounds))
            {
                ((IEntity)p).Hp -= BulletDamage;
            }
        }

        // shoot bullet in interval
        if (Time.realtimeSinceStartup - LastTime >= Interval)
        {
            var effect = EffectMgr.CreateEffect<BulletEffect>();
            effect.Damage = BulletDamage;
            effect.Speed = BulletSpeed;
            effect.RevertSpeed = BulletRevertSpeed;
            effect.Acceleration = BulletAcceleration;
            effect.Duration = BulletDuration;
            effect.Target = EntityMgr.Player;
            effect.StartPosition = Renderer.bounds.center;
            LastTime = Time.realtimeSinceStartup;
            AudioMgr.PlaySound(Game.BulletShootSound);
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