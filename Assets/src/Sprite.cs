using UnityEngine;
using System.Collections;

public class Sprite : MonoBehaviour {
    
    public TextureAtlasInfo atlas;
    public string staticSpriteFrame;

    public void Init() {
        gameObject.AddComponent<MeshRenderer>();
        var mf = gameObject.AddComponent<MeshFilter>();
        if(!Application.isPlaying) {
            mf.sharedMesh = ProceduralGeometry.CreatePlane(1.0f, 1.0f);
        } else {
            mf.mesh = ProceduralGeometry.CreatePlane(1.0f, 1.0f);
        }
    }
    
    public void HandleAtlasChange() {
        if(this.atlas != null) {
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = atlas.material;
        } else {
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = null;
        }
    }
    
    public void HandleStaticSpriteChange() {
        ModifyForCurrentFrame();
    }
    
    void ModifyForCurrentFrame() {
        int idx = atlas.GetSelectedIdx(staticSpriteFrame);
        if(idx == -1) {
            staticSpriteFrame = atlas.textureNames[0];
        }
        Rect r = atlas.RectForName(staticSpriteFrame);
        Rect uv = atlas.UVsForName(staticSpriteFrame);
        transform.localScale = new Vector3(r.width / 32, r.height / 32, 1);
        Mesh m = GetMesh();
        if(m == null) {
            Debug.Log(gameObject.name + " WTF NULL MESH");
        }
        Vector2[] uvs = m.uv;
        uvs[0] = new Vector2(uv.x, uv.y + uv.height);
        uvs[1] = new Vector2(uv.x + uv.width, uv.y + uv.height);
        uvs[2] = new Vector2(uv.x, uv.y);
        uvs[3] = new Vector2(uv.x + uv.width, uv.y);
        
        m.uv = uvs;
    }
    
    Mesh GetMesh() {
        if(!Application.isPlaying) {
            var mf = gameObject.GetComponent<MeshFilter>();
            if(SharedMeshInUse(mf.sharedMesh)) {
                mf.sharedMesh = ProceduralGeometry.CreatePlane(1.0f, 1.0f);
                return mf.sharedMesh;
            } else {
                return mf.sharedMesh;
            }
        }
        
        return gameObject.GetComponent<MeshFilter>().mesh;
    }
    
    bool SharedMeshInUse(Mesh toCheckFor) {
        var otherSprites = GameObject.FindObjectsOfType(typeof(Sprite)) as Sprite[];
        foreach(var s in otherSprites) {
            if(!this.Equals(s) && s.GetComponent<MeshFilter>().sharedMesh.Equals(toCheckFor)) {
                return true;
            }
        }
        return false;
    }
}
