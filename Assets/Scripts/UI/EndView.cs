using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndView : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Text text;
    private RectTransform textTrans;
    

    private void Awake()
    {
        text = transform.Find("Text").GetComponent<Text>();
        textTrans = text.transform as RectTransform;
    }

    private void Start()
    {
        AudioMgr.PlayMusic(Game.EndMusic);
    }

    private void Update()
    {
        var pos = textTrans.anchoredPosition;
        pos.y += speed * Time.deltaTime;
        textTrans.anchoredPosition = pos;
    }
}
