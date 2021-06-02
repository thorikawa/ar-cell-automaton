using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DepthUtils
{
    public static Mesh MakeEmptyMesh(int width, int height)
    {
        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        for (var j = 0; j < height; j++)
        {
            for (var i = 0; i < width; i++)
            {
                float u = (float)i / (width - 1f);
                float v = 1f - (float)j / (height - 1f);
                vertices.Add(new Vector3(u, 0, v));
                uvs.Add(new Vector2(u, v));
            }
        }

        int[] indices = new int[(width - 1) * (height - 1)];
        int count = 0;
        for (var i = 0; i < width - 1; i++)
        {
            for (var j = 0; j < height - 1; j++)
            {
                var v = j * width + i;
                //indices[count++] = v;
                //indices[count++] = v + 1;
                //indices[count++] = v + width;

                //indices[count++] = v + 1;
                //indices[count++] = v + 1 + width;
                //indices[count++] = v + width;
                indices[count++] = v;
            }
        }
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        //mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();

        return mesh;
    }
}
