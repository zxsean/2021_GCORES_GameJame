using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLiquidImage : BaseMeshEffect
{
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        vh.Clear();
        const int length = 30;
        var list = new List<UIVertex>(length);

        var a = new Vector3(-50.0f, 0.0f, 0.0f);
        var b = new Vector3(0.0f, 5f, 0.0f);
        var c = new Vector3(50.0f, 0.0f, 0.0f);
        var vertices = new Vector3[length];
        vertices[0] = a;
        vertices[9] = c;
        var count = (length - 2) / 2 + 1;
        for (var i = 1; i < count; ++i)
        {
            var t = i * (1.0f / count);
            vertices[i * 2 - 1] = Bezier(a, b, c, t);
            vertices[i * 2] = Bezier(a, -b, c, t);
        }

        var triangles = new List<int>();
        for (var i = 0; i < length - 2; ++i)
        {
            triangles.Add(i);
            triangles.Add(i % 2 == 0 ? i + 1 : i + 2);
            triangles.Add(i % 2 == 0 ? i + 2 : i + 1);
        }

        for (var i = 0; i < length; ++i)
        {
            var uiVertex = new UIVertex();
            uiVertex.position = vertices[i];
            list.Add(uiVertex);
        }

        vh.AddUIVertexStream(list, triangles);
    }

    private Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var d = Vector3.Lerp(a, b, t);
        var e = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(d, e, t);
    }
}