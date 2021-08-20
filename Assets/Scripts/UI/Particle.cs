using System;
using UnityEngine;
using UnityEngine.UI;

public class Particle : MonoBehaviour
{
    private int TintColorID = Shader.PropertyToID("_TintColor");

    private Material Material { get; set; }
    private RectTransform Trans { get; set; }
    private RawImage Image { get; set; }

    private void Awake()
    {
        Trans = transform as RectTransform;
        Image = GetComponent<RawImage>();
        Material = Image.material;
    }

    private void Update()
    {
        var y = Trans.position.y;
        var a = Mathf.Lerp(0f, 1f, (y - 200.0f) / 100.0f);
        a -= Mathf.Lerp(0f, 1f, (y - 300.0f) / 100.0f);
        var color = Image.color;
        color.a = a;
        Image.color = color;
    }
}