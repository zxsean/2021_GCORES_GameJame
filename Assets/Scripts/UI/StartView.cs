using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class StartView : MonoBehaviour
{
    public Text tipsTxt;

    private void Start()
    {
        AudioMgr.PlayContinueMusic(Game.NormalMusic);
    }

    private void Update()
    {
        var color = tipsTxt.color;
        color.a = Mathf.Sin(3 * Time.realtimeSinceStartup) * 0.5f + 0.5f;
        tipsTxt.color = color;
        if (Input.anyKeyDown)
        {
            Game.Start();
        }
    }
}
