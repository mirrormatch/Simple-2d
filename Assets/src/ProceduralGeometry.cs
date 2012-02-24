using UnityEngine;
using System.Collections;

public class ProceduralGeometry {

	public static Mesh CreatePlane(float width, float height) {
        Mesh m = new Mesh();
        Vector3[] verts = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];
        
        float wd2 = width / 2.0f;
        float hd2 = height / 2.0f;
        
        verts[0] = new Vector3(-wd2,  hd2, 0);
        verts[1] = new Vector3( wd2,  hd2, 0);
        verts[2] = new Vector3(-wd2, -hd2, 0);
        verts[3] = new Vector3( wd2, -hd2, 0);
        
        normals[0] = new Vector3(0, 0, 1);
        normals[1] = new Vector3(0, 0, 1);
        normals[2] = new Vector3(0, 0, 1);
        normals[3] = new Vector3(0, 0, 1);
        
        uvs[0] = new Vector2(0, 1);
        uvs[1] = new Vector2(1, 1);
        uvs[2] = new Vector2(0, 0);
        uvs[3] = new Vector2(1, 0);
        
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;
        
        m.vertices = verts;
        m.normals = normals;
        m.uv = uvs;
        m.triangles = triangles;
        m.Optimize();
        
        return m;
    }
}
