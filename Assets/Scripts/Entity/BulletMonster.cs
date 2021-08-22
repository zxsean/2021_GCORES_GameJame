using UnityEngine;

public class BulletMonster : Grid, IEntity, IUpdatable
{
    public int Hp { get; set; }
    public bool IsDestroy { get; private set; }
    
    private int Interval { get; set; }
    
    private float LastTime { get; set; }
    private float BulletSpeed { get; set; }
    private int BulletDamage { get; set; }
    private Vector3 Direction { get; set; }
    
    private static MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static int BlurID = Shader.PropertyToID("_Blur");

    public BulletMonster(GameObject asset) : base(asset)
    {
        var data = (BulletMonsterData) RawData;
        Hp = data.hp;
        Interval = data.interval;
        BulletDamage = data.bulletDamage;
        BulletSpeed = data.bulletSpeed;
        var sr = Renderer as SpriteRenderer;
        switch (data.direction)
        {
            case ShootDirection.Up:
                Direction = Vector3.up;
                sr.sprite = data.up;
                break;
            case ShootDirection.Down:
                Direction = Vector3.down;
                sr.sprite = data.down;
                break;
            case ShootDirection.Left:
                Direction = Vector3.left;
                sr.sprite = data.left;
                break;
            case ShootDirection.Right:
                Direction = Vector3.right;
                sr.sprite = data.right;
                break;
        }
        
        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);
        Mpb.SetFloat(BlurID, 0);
        Renderer.SetPropertyBlock(Mpb);
    }
    
    public void Update()
    {
        if (Hp <= 0)
        {
            // 红色
            ((SpriteRenderer)Renderer).color = Color.red;
            Renderer.GetPropertyBlock(Mpb);
            Mpb.SetFloat(BlurID, 0.5f);
            Renderer.SetPropertyBlock(Mpb);
            //gameObject.SetActive(false);
            IsDestroy = true;
            return;
        }
        
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
            var effect = EffectMgr.CreateEffect<DirectionBulletEffect>();
            effect.Damage = BulletDamage;
            effect.Speed = BulletSpeed;
            effect.Direction = Direction;
            var bounds = Renderer.bounds;
            var extents = bounds.extents;
            // add a bias
            var dir = Direction * 2.0f;
            extents.x *= dir.x;
            extents.y *= dir.y;
            extents.z *= dir.z;
            effect.StartPosition = bounds.center + extents;
            LastTime = Time.realtimeSinceStartup;
            AudioMgr.PlaySound(Game.BulletShootSound);
        }
    }
}