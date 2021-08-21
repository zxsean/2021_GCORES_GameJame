using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class StartView : MonoBehaviour
{
    public Button startBtn;
    public Animation anim;
    
    private bool CanClicked { get; set; }
    
    private void Start()
    {
        CanClicked = false;
        AudioMgr.PlayContinueMusic(Game.NormalMusic);
        anim.Play("startview", () =>
        {
            CanClicked = true;
        });
        startBtn.onClick.AddListener(() =>
        {
            if (!CanClicked) return;
            Game.Start();
        });
    }
}
