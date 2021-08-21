using UnityEngine;

public enum ShootDirection
{
    Up,
    Down,
    Left,
    Right
}

public class BulletMonsterData : GridData
{
    public int hp;
    public int interval;
    public float bulletSpeed;
    public int bulletDamage;
    public ShootDirection direction;
}