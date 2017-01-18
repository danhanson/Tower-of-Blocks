using UnityEngine;
using System.Collections.Generic;
using System;

public class Circle : Block
{
    public float radius;

    private const int edgesPerRadius = 1;

    public override float Height
    {
        get
        {
            return radius + transform.position.y;
        }
    }

    protected override Collider2D AddCollider()
    {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = radius;
        return collider;
    }

    protected override Mesh MakeMesh()
    {
        int triangleCount = (int) (edgesPerRadius * radius);
        List<Vector3> vertexList = new List<Vector3>(triangleCount + 1);
        List<int> triangleList = new List<int>(triangleCount); 
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, 360f / triangleCount);

        vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));
        vertexList.Add(new Vector3(0.0f, radius, 0.0f));
        vertexList.Add(quaternion * vertexList[1]);     // 3. First vertex on circle outline rotated by angle)
                                                        // Add triangle indices.
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);
        for (int i = 0; i < triangleCount; i++)
        {
            triangleList.Add(0);                      // Index of circle center.
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);
            vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        return mesh;
    }

    public override BlockData ToBlockData()
    {
        BlockData data = new BlockData();
        data.radius = radius;
        data.position = transform.position;
        data.angle = transform.eulerAngles.z;
        data.polygon = null;
        data.material = materialType;
        return data;
    }
}
