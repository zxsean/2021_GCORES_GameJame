using UnityEngine;

public class Monster : Grid, IEntity, IUpdatable, IMovatable
{
    public int Hp { get; set; }
    public bool IsDestroy { get; private set; }
    public float SpeedFactor { get; set; }
    public float SpeedDecayStartTime { get; set; }
    public float SpeedDecayTime { get; set; }
    public float Speed { get; private set; }
    public int Damage { get; private set; }

    private Matrix4x4 rotateMat;

    public Monster(GameObject asset) : base(asset)
    {
        var data = (MonsterData)RawData;
        Hp = data.hp;
        Speed = data.speed;
        Damage = data.damage;
        
        rotateMat = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 30.0f));
    }
    
    public void Update()
    {
        if (Hp <= 0)
        {
            gameObject.SetActive(false);
            IsDestroy = true;
            return;
        }
        
        // calc decay
        if (Time.realtimeSinceStartup - SpeedDecayStartTime >= SpeedDecayTime)
        {
            SpeedFactor = 1;
        }
        else
        {
            SpeedFactor = 0;
        }

        // chase player
        // 碰到illusion也会杀死illusion
        EntityMgr.GetAll<IPlayer>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            var p = list[i];
            if (p is Player)
            {
                var target = EntityMgr.Player;
                var pos = transform.localPosition;
                var bounds = target.Renderer.bounds;
                var dir = (bounds.center - pos).normalized;
                var offset = Speed * Time.deltaTime;
                var targetBounds = Renderer.bounds;
                targetBounds.center += dir * (offset * SpeedFactor * 5f);
                // check barrier
                FloorMgr.GetAll<Barrier>(out var bList);
                for (var j = 0; j < bList.Count; ++j)
                {
                    while (bList[j].InRange(targetBounds))
                    {
                        // 修正下方向
                        dir = rotateMat.MultiplyVector(dir);
                        targetBounds = Renderer.bounds;
                        targetBounds.center += dir * (offset * SpeedFactor * 5f);
                    }
                }
                
                pos += dir * (offset * SpeedFactor);
                transform.localPosition = pos;
            }
            if (InRange(p.Renderer.bounds))
            {
                ((IEntity)p).Hp -= Damage;
            }
        }
    }
}