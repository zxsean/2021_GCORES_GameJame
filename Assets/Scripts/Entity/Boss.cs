using UnityEngine;

public class Boss : Grid, IEntity, IUpdatable
{
    private static readonly MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static readonly int BlurID = Shader.PropertyToID("_Blur");
    private int hp;


    public Boss(GameObject asset) : base(asset)
    {
        Game.BossView.gameObject.SetActive(true);

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

        Idle = data.idle;
        Attack = data.attack;
        Die = data.die;

        Renderer = transform.Find("GameObject/Render").GetComponent<SpriteRenderer>();
        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);
        Mpb.SetFloat(BlurID, 0);
        Renderer.SetPropertyBlock(Mpb);

        LastTime = Time.realtimeSinceStartup;
        CameraMgr.SetSize(9.25f);
    }

    private int Interval { get; }

    private float LastTime { get; set; }
    private float BulletSpeed { get; }
    private int BulletDamage { get; }
    private float BulletDuration { get; }
    private float Radius { get; }
    private float Speed { get; }
    private float BulletRevertSpeed { get; }
    private float BulletAcceleration { get; }
    private int Winding { get; }
    private float Length { get; }
    private bool IsHurt { get; set; }

    private Sprite Idle { get; }
    private Sprite Attack { get; }
    private Sprite Die { get; }

    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            IsHurt = true;
            var progress = (float) hp / ((BossData) RawData).hp;
            Game.BossView.SetProgress(progress);
        }
    }

    public bool IsDestroy { get; private set; }

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
            ChangeSprite(Die);
            Mpb.Clear();
            Renderer.GetPropertyBlock(Mpb);
            Mpb.SetFloat(BlurID, 0.5f);
            Renderer.SetPropertyBlock(Mpb);
            // 通关！
            IsDestroy = true;
            LevelMgr.CurLevel.Pause();
            Game.BossView.gameObject.SetActive(false);
            CameraMgr.Move(transform.localPosition, () => { LevelMgr.NextLevel(); });
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
            if (p is IGrid grid && InRange(grid.Renderer.bounds)) ((IEntity) p).Hp -= BulletDamage;
        }

        if (Time.realtimeSinceStartup - LastTime >= Interval * 0.85f ||
            Time.realtimeSinceStartup - LastTime <= Interval * 0.15f)
            ChangeSprite(Attack);
        else
            ChangeSprite(Idle);

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
            AudioMgr.PlaySound(Game.BulletShootSound, transform);
        }
    }

    private void ChangeSprite(Sprite sprite)
    {
        var sr = Renderer as SpriteRenderer;
        sr.sprite = sprite;
    }
}