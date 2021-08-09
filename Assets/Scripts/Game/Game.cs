﻿using UnityEngine;

public static class Game
{
    public static Transform Root { get; private set; }
    
    public static void Init()
    {
        Root = GameObject.Find("Game").transform;
    }

    public static void Start()
    {
        LevelMgr.EnterLevel(0);
    }

    public static void Update()
    {
        
    }

    public static void Destroy()
    {
        
    }
}