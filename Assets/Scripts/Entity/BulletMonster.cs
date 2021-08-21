﻿using UnityEngine;

public class BulletMonster : Grid, IEntity, IUpdatable
{
    public int Hp { get; set; }
    public bool IsDestroy { get; private set; }
    
    private int Interval { get; set; }
    
    private float LastTime { get; set; }
    private float BulletSpeed { get; set; }
    private int BulletDamage { get; set; }
    private Vector3 Direction { get; set; }
    
    public BulletMonster(GameObject asset) : base(asset)
    {
        var data = (BulletMonsterData) RawData;
        Hp = data.hp;
        Interval = data.interval;
        BulletDamage = data.bulletDamage;
        BulletSpeed = data.bulletSpeed;
        switch (data.direction)
        {
            case ShootDirection.Up:
                Direction = Vector3.up;
                break;
            case ShootDirection.Down:
                Direction = Vector3.down;
                break;
            case ShootDirection.Left:
                Direction = Vector3.left;
                break;
            case ShootDirection.Right:
                Direction = Vector3.right;
                break;
        }
    }
    
    public void Update()
    {
        if (Hp <= 0)
        {
            gameObject.SetActive(false);
            IsDestroy = true;
            return;
        }
        
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