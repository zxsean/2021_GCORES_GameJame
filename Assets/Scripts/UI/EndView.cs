using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndView : MonoBehaviour
{
    private Animation Anim { get; set; }
    private MeshRenderer Renderer { get; set; }
    private MeshFilter Filter { get; set; }
    
    private void Awake()
    {
        //Anim = GetComponent<Animation>();
        Renderer = GetComponent<MeshRenderer>();
        Filter = GetComponent<MeshFilter>();

        Filter.mesh = BuildCurveMesh();
    }

    private void Start()
    {
        //Anim.Play();
    }

    private Mesh BuildCurveMesh()
    {
        var mesh = new Mesh();
        var a = new Vector3(-5.0f, 0.0f, 0.0f);
        var b = new Vector3(0.0f, 0.5f, 0.0f);
        var c = new Vector3(5.0f, 0.0f, 0.0f);
        var vertices = new Vector3[10];
        vertices[0] = a;
        vertices[9] = c;
        for (var i = 1; i < 5; ++i)
        {
            var t = i * 0.2f;
            vertices[i * 2 - 1] = Bezier(a, b, c, t);
            vertices[i * 2] = Bezier(a, -b, c, t);
        }

        var triangles = new []
        {
            0, 1, 2,
            2, 1, 4,
            4, 1, 3,
            3, 6, 4,
            6, 3, 5,
            5, 8, 6,
            5, 7, 8,
            8, 7, 9
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }

    private Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var d = Vector3.Lerp(a, b, t);
        var e = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(d, e, t);
    }
}
