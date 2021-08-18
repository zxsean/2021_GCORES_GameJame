using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndView : MonoBehaviour
{
    private Animation Anim { get; set; }

    private void Awake()
    {

    }

    private void Start()
    {
        AudioMgr.PlayMusic(Game.EndMusic);
    }
    
}
