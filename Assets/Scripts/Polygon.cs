using System;
using UnityEngine;

public class Polygon : Block {

    public Vector2[] points;

    public override float Height
    {
        get
        {
            PolygonCollider2D col = GetComponent<PolygonCollider2D>();
            float max = float.NegativeInfinity;
            foreach(Vector2 p in col.points)
            {
                if(p.y > max)
                {
                    max = p.y;
                }
            }
            return max;
        }
    }

    public override BlockData ToBlockData()
    {
        BlockData data = new BlockData();
        data.angle = transform.eulerAngles.z;
        data.polygon = points;
        data.material = materialType;
        data.position = transform.position;
        data.radius = 0;
        return data;
    }

    protected override Collider2D AddCollider()
    {
        PolygonCollider2D collider = gameObject.AddMissingComponent<PolygonCollider2D>();
        collider.points = points;
        return collider;
    }

    protected override Mesh MakeMesh()
    {
        Triangulator t = new Triangulator(points);
        Vector3[] vertices = new Vector3[points.Length];
        Mesh m = new Mesh();
        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = points[i].To3D();
        }
        m.vertices = vertices;
        m.triangles = t.Triangulate();
        return m;
    }
}
