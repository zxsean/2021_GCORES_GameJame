using System.Collections.Generic;
using UnityEngine;

public class Player : Grid, IEntity, IUpdatable, IEffectTarget, IPlayer
{
    public int Hp { get; set; }
    public bool IsDestroy { get; private set; }
    public bool IsActive { get; private set; }
    public float Speed { get; private set; }
    private float CurSpeed { get; set; }

    public Player(GameObject asset) : base(asset)
    {
        var data = (PlayerData) RawData;
        Speed = data.speed;
        CurSpeed = Speed;
        Hp = data.hp;
        IsActive = true;
    }

    public void Update()
    {
        // 处理输入
        ProcessInputs();
        
        // 处理状态
        ProcessStates();
    }

    protected virtual void ProcessStates()
    {
        if (Hp <= 0)
        {
            AudioMgr.PlaySound(Game.DieSound);
            gameObject.SetActive(false);
            IsActive = false;
            // 玩家死亡 游戏结束
            IsDestroy = true;
            LevelMgr.ReEnter();
        }
    }

    protected virtual void ProcessInputs()
    {
        var offsetX = 0.0f;
        var offsetY = 0.0f;

        var moved = false;
        // 方向
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            offsetY += CurSpeed * Time.deltaTime;
            moved = true;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            offsetY -= CurSpeed * Time.deltaTime;
            moved = true;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            offsetX -= CurSpeed * Time.deltaTime;
            moved = true;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            offsetX += CurSpeed * Time.deltaTime;
            moved = true;
        }
        
        // 脚步声
        if (moved)
        {
            AudioMgr.PlayContinueSound(Game.FootStepSound);
        }
        else
        {
            AudioMgr.StopSound(Game.FootStepSound);
        }

        var bounds = Renderer.bounds;
        bounds.center = new Vector3(bounds.center.x + offsetX, bounds.center.y + offsetY);
        
        // check level contains
        if (!LevelMgr.CurLevel.Contains(bounds))
        {
            offsetX = 0;
            offsetY = 0;
        }
        
        // check barrier
        FloorMgr.GetAll<Barrier>(out var list);
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i].InRange(bounds))
            {
                offsetX = 0;
                offsetY = 0;
                break;
            }
        }

        CurPosX += offsetX;
        CurPosY += offsetY;
        var pos = transform.localPosition;
        pos.x = CurPosX;
        pos.y = CurPosY;
        transform.localPosition = pos;

        var camPos = Game.Camera.transform.localPosition;
        camPos.x = pos.x;
        camPos.y = pos.y;
        Game.Camera.transform.localPosition = camPos;
    }
}