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
    private float Radius { get; set; }
    private float Speed { get; set; }
    private int Winding { get; set; }
    private float Length { get; set; }
    private bool IsHurt { get; set; }

    public Boss(GameObject asset) : base(asset)
    {
        var data = (BossData) RawData;
        Hp = data.hp;
        Interval = data.interval;
        BulletDamage = data.bulletDamage;
        BulletSpeed = data.bulletSpeed;
        Radius = transform.localPosition.magnitude;
        Speed = data.speed;
        Winding = (int) data.winding;
        Length = 2 * Mathf.PI * Radius;
    }
    
    public void Update()
    {
        if (Hp <= 0)
        {
            // 通关！
            IsDestroy = true;
            LevelMgr.NextLevel();
        }

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

        // move
        // 计算一个旋转矩阵 用来旋转位置
        var rand = Time.deltaTime * Speed / Length;
        rand *= Winding;
        var pos = transform.localPosition;
        pos.x = pos.x * Mathf.Cos(rand) - pos.y * Mathf.Sin(rand);
        pos.y = pos.x * Mathf.Sin(rand) + pos.y * Mathf.Cos(rand);
        transform.localPosition = pos;
        
        // 玩家碰到死
        EntityMgr.GetAll<IPlayer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var p = list[i];
            if (p is IGrid grid && grid.InRange(Renderer.bounds))
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
            effect.Target = EntityMgr.Player;
            effect.StartPosition = Renderer.bounds.center;
            LastTime = Time.realtimeSinceStartup;
            AudioMgr.PlaySound(Game.BulletShootSound);
        }
    }
}