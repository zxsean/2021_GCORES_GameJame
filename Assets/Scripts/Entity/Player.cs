using System.Collections.Generic;
using UnityEngine;

public class Player : Grid, IEntity, IUpdatable, IEffectTarget, IPlayer
{
    public int Hp { get; set; }
    public bool IsDestroy { get; private set; }
    public bool IsActive { get; private set; }
    public float Speed { get; private set; }
    private float CurSpeed { get; set; }
    
    private Sprite Up { get; set; }
    private Sprite Down { get; set; }
    private Sprite Left { get; set; }
    private Sprite Right { get; set; }
    
    private static MaterialPropertyBlock Mpb = new MaterialPropertyBlock();
    private static int BlurID = Shader.PropertyToID("_Blur");

    public Player(GameObject asset) : base(asset)
    {
        var data = (PlayerData) RawData;
        Speed = data.speed;
        CurSpeed = Speed;
        Hp = data.hp;
        IsActive = true;

        Up = data.up;
        Down = data.down;
        Left = data.left;
        Right = data.right;
        
        Mpb.Clear();
        Renderer.GetPropertyBlock(Mpb);
        Mpb.SetFloat(BlurID, 0);
        Renderer.SetPropertyBlock(Mpb);
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
            // 红色
            ((SpriteRenderer)Renderer).color = Color.red;
            Renderer.GetPropertyBlock(Mpb);
            Mpb.SetFloat(BlurID, 0.5f);
            Renderer.SetPropertyBlock(Mpb);
            
            AudioMgr.PlaySound(Game.DieSound, transform);
            //gameObject.SetActive(false);
            IsActive = false;
            // 玩家死亡 游戏结束
            IsDestroy = true;
            LevelMgr.ReEnter();
        }
    }

    protected virtual void ProcessInputs()
    {
        if (!IsActive) return;
        
        var offsetX = 0.0f;
        var offsetY = 0.0f;

        var moved = false;
        // 方向
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            offsetY += CurSpeed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Up;
            moved = true;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            offsetY -= CurSpeed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Down;
            moved = true;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            offsetX -= CurSpeed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Left;
            moved = true;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            offsetX += CurSpeed * Time.deltaTime;
            ((SpriteRenderer) Renderer).sprite = Right;
            moved = true;
        }
        
        // 脚步声
        if (moved)
        {
            AudioMgr.PlayContinueSound(Game.FootStepSound, transform);
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
    }
}