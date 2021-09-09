using UnityEngine;

public enum Winding
{
    CW = -1,
    CCW = 1
}

public class BossData : GridData
{
    public Sprite attack;
    public float bulletAcceleration;
    public int bulletDamage;
    public float bulletDuration;
    public float bulletRevertSpeed;
    public float bulletSpeed;
    public Sprite die;
    public int hp;

    public Sprite idle;
    public int interval;
    public float speed;
    public Winding winding;
}