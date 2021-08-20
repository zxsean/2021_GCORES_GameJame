using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndView : MonoBehaviour
{
    [SerializeField]
    private float speed;
    
    public RectTransform content;
    

    private void Start()
    {
        AudioMgr.PlayMusic(Game.EndMusic);
    }

    private void Update()
    {
        var pos = content.anchoredPosition;
        pos.y += speed * Time.deltaTime;
        content.anchoredPosition = pos;
    }
}
