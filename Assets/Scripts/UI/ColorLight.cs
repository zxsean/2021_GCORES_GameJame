using System.Collections.Generic;
using UnityEngine;

public class ColorLight : MonoBehaviour
{
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = BuildMesh();
    }

    private Mesh BuildMesh()
    {
        var mesh = new Mesh();
        const int length = 30;

        var a = new Vector3(-50.0f, 0.0f, 0.0f);
        var b = new Vector3(0.0f, 5f, 0.0f);
        var c = new Vector3(50.0f, 0.0f, 0.0f);
        var vertices = new Vector3[length];
        var normals = new Vector3[length];
        vertices[0] = a;
        vertices[length - 1] = c;
        normals[0] = Vector3.left;
        normals[length - 1] = Vector3.right;
        var count = (length - 2) / 2 + 1;
        for (var i = 1; i < count; ++i)
        {
            var t = i * (1.0f / count);
            var upVec = Bezier(a, b, c, t);
            var downVec = Bezier(a, -b, c, t);
            vertices[i * 2 - 1] = upVec;
            vertices[i * 2] = downVec;
            normals[i * 2 - 1] = Vector3.up; //Bezier(Vector3.left, Vector3.up, Vector3.right, t).normalized;
            normals[i * 2] = Vector3.down; //Bezier(Vector3.left, Vector3.down, Vector3.right, t).normalized;
        }

        var triangles = new List<int>();
        for (var i = 0; i < length - 2; ++i)
        {
            triangles.Add(i);
            triangles.Add(i % 2 == 0 ? i + 1 : i + 2);
            triangles.Add(i % 2 == 0 ? i + 2 : i + 1);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals;
        return mesh;
    }

    private Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var d = Vector3.Lerp(a, b, t);
        var e = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(d, e, t);
    }
}