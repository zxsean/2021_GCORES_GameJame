using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartView : MonoBehaviour
{
    private Button startBtn;
    
    private void Awake()
    {
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(Game.Start);
    }

}
