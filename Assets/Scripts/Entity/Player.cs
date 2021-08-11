﻿using System.Collections.Generic;
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
        // 处理状态
        ProcessStates();
        
        // 处理输入
        ProcessInputs();
    }

    protected virtual void ProcessStates()
    {
        if (Hp <= 0)
        {
            gameObject.SetActive(false);
            IsActive = false;
            // 玩家死亡 游戏结束
            IsDestroy = true;
        }
    }

    protected virtual void ProcessInputs()
    {
        var offsetX = 0.0f;
        var offsetY = 0.0f;
        
        // 方向
        if (Input.GetKey(KeyCode.W))
        {
            offsetY += CurSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            offsetY -= CurSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            offsetX -= CurSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            offsetX += CurSpeed * Time.deltaTime;
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
    }
}