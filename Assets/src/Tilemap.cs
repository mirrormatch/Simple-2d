using UnityEngine;
using System.Collections;

public class Tilemap : MonoBehaviour {
 
    public Material tileMat;
    public int width;
    public int height;
    private int oldWidth;
    private int oldHeight;
    public int tileWidth;
    public int[] data = null;
 
    public void HandleDataChange(bool editMode) {
        if(width < 1 || height < 1 || 
         tileWidth < 1 || tileWidth < 1 || 
         tileMat == null) {
            // We can't gen data if the dimensions are invalid
            return;
        }
        UpdateAndCopyData();
        UpdateMesh(editMode);
        UpdateCollider();
    }
 
    void UpdateCollider() {
        var bc = gameObject.GetComponent<BoxCollider>();
        if(bc == null) {
            bc = gameObject.AddComponent<BoxCollider>();
        }
        bc.size = new Vector3(width, height, 0.1f);
        bc.isTrigger = true;
    }
 
    void UpdateAndCopyData() {
        var newData = new int[width * height];
        for(var i = 0; i < width * height; i++) {
            newData[i] = 0;
        }
        if(data != null) {
            for(int x = 0; x < oldWidth; x++) {
                for(int y = 0; y < oldHeight; y++) {
                    if(x >= width || y >= height) {
                        continue;
                    }
                    newData[y * width + x] = data[y * oldWidth + x];
                }
            }
        }
        data = newData;
        oldWidth = width;
        oldHeight = height;
    }
 
    void UpdateMesh(bool editMode) {
        // Create the MeshRender, MeshFilter, and Mesh as necessary
     
        var mr = gameObject.GetComponent<MeshRenderer>();
        if(mr == null) {
            mr = gameObject.AddComponent<MeshRenderer>();
        }
        mr.material = this.tileMat;
     
        var mf = gameObject.GetComponent<MeshFilter>();
        if(mf == null) {
            mf = gameObject.AddComponent<MeshFilter>();
        }
     
        Mesh m = GetMesh(editMode);
     
        RecreateMeshGrid(m);
    }
 
    Mesh GetMesh(bool editMode) {
        var mf = gameObject.GetComponent<MeshFilter>();
        Mesh m;
        if(editMode) {
            m = mf.sharedMesh;
            if(m == null) {
                mf.sharedMesh = new Mesh();
                m = mf.sharedMesh;
            }
        } else {
            m = mf.mesh;
            if(m == null) {
                mf.mesh = new Mesh();
                m = mf.mesh;
            }
        }
     
        return m;
    }
 
    void RecreateMeshGrid(Mesh m) {
        Vector3[] verts = new Vector3[width * height * 4];
        Vector2[] uvs = new Vector2[width * height * 4];
        int[] triangles = new int[width * height * 6];
     
        float xBase = -width / 2.0f;
        float yBase = -height / 2.0f;
     
        float uvWidth = GetUVTileWidth();
        float uvHeight = GetUVTileHeight();
        Debug.Log("UV incs: " + uvWidth + ", " + uvHeight);
     
        int vIndex = 0;
        int tIndex = 0;
        for(var x = 0; x < width; x++) {
            for(var y = 0; y < height; y++) {
                if(GetSquare(x, y) == -1) {
                    continue;
                }
                verts[vIndex + 0] = new Vector3(xBase + x, yBase + y, 0.0f);
                verts[vIndex + 1] = new Vector3(xBase + x + 1, yBase + y, 0.0f);
                verts[vIndex + 2] = new Vector3(xBase + x, yBase + y + 1, 0.0f);
                verts[vIndex + 3] = new Vector3(xBase + x + 1, yBase + y + 1, 0.0f);
             
                Vector2 uvBase = UVBaseForTile(x, y);
             
                uvs[vIndex + 0] = new Vector2(uvBase.x, uvBase.y);
                uvs[vIndex + 1] = new Vector2(uvBase.x + uvWidth, uvBase.y);
                uvs[vIndex + 2] = new Vector2(uvBase.x, uvBase.y + uvHeight);
                uvs[vIndex + 3] = new Vector2(uvBase.x + uvWidth, uvBase.y + uvHeight);              
             
                triangles[tIndex + 0] = vIndex + 2;
                triangles[tIndex + 1] = vIndex + 3;
                triangles[tIndex + 2] = vIndex + 0;
                triangles[tIndex + 3] = vIndex + 3;
                triangles[tIndex + 4] = vIndex + 1;
                triangles[tIndex + 5] = vIndex + 0;
             
                vIndex += 4;
                tIndex += 6;
            }
        }
     
        m.Clear();
        m.vertices = verts;
        m.uv = uvs;
        m.triangles = triangles;
    }
 
    float GetUVTileWidth() {
        Texture t = tileMat.mainTexture;
        return 1.0f / (t.width / tileWidth);
    }
 
    float GetUVTileHeight() {
        Texture t = tileMat.mainTexture;
        return 1.0f / (t.height / tileWidth);
    }
 
    Vector2 UVBaseForTile(int x, int y) {
        Vector2 toReturn = new Vector2();
     
        toReturn.x = GetTileCol(x, y) * GetUVTileWidth();
        toReturn.y = 1.0f - (GetTileRow(x, y) + 1) * GetUVTileHeight();
     
        return toReturn;
    }
 
    int GetTileCol(int x, int y) {
        int idx = GetSquare(x, y);
        Texture t = tileMat.mainTexture;
        return idx % (t.width / tileWidth);
    }
 
    int GetTileRow(int x, int y) {
        int idx = GetSquare(x, y);
        Texture t = tileMat.mainTexture;
        return idx / (t.width / tileWidth);
    }
 
    public void UpdateSquareAtPoint(Vector3 v, int idx) {
        v = transform.InverseTransformPoint(v);
        int x = (int)(v.x + (width / 2));
        int y = (int)((height / 2) + v.y);
        Debug.Log("Updating tile: " + x + ", " + y);
        SetSquare(x, y, idx);
    }
 
    public int GetSquare(int x, int y) {
        return data[y * width + x];
    }
 
    public void SetSquare(int x, int y, int idx) {
        if(data == null) {
            return;
        }
        data[y * width + x] = idx;
        RecreateMeshGrid(GetMesh(true));
    }
 
    public void FillWith(int idx) {
        for(int i = 0; i < data.Length; i++) {
            data[i] = idx;
        }
        RecreateMeshGrid(GetMesh(true));
    }
}
