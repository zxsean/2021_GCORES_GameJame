using UnityEngine;

public enum Winding
{
    CW = -1,
    CCW = 1,
}

public class BossData : GridData
{
    public int hp;
    public int interval;
    public float bulletSpeed;
    public int bulletDamage;
    public float bulletDuration;
    public float bulletRevertSpeed;
    public float bulletAcceleration;
    public float speed;
    public Winding winding;
    
    public Sprite idle;
    public Sprite attack;
    public Sprite die;
}