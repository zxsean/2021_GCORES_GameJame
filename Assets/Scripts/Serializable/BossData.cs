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
    public float speed;
    public Winding winding;
}