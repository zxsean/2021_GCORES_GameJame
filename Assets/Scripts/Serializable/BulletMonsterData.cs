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
    public int bulletDamage;
    public float bulletSpeed;
    public ShootDirection direction;
    public Sprite down;
    public int hp;
    public int interval;
    public Sprite left;
    public Sprite right;
    public Sprite up;
}