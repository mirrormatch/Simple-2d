using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AtlasGenerator {
    
    public string m_src;
    public string m_dst;
    public string[] m_files;
    public List<Texture2D> m_srcTextures;
    public string m_outputFile;
    public string m_atlasInfoOutputFile;
    public string m_materialOutputFile;
    
    public AtlasGenerator() {
    }
    
    public AtlasGenerator(string src, string dst) {
        this.m_src = src;
        this.m_dst = dst;
        PreparePaths();
    }
    
    public void Regenerate(TextureAtlasInfo tai) {
        this.m_src = tai.srcDirectory;
        var path = AssetDatabase.GetAssetPath(tai);
        this.m_dst = path.Substring(0, path.LastIndexOf('/'));
        PreparePaths();
        ProcessAndSave(tai.atlas, tai, tai.material, false);
    }
    
    public void PreparePaths() {
        var splitPath = m_src.Split(new char[] {'/'});
        var lastComponent = splitPath[splitPath.Length - 1];
        
        this.m_files = Directory.GetFiles(m_src, "*.png", SearchOption.AllDirectories);
        this.m_outputFile = m_dst + "/" + lastComponent + ".png";
        this.m_atlasInfoOutputFile = m_dst.Substring(m_dst.IndexOf("Assets")) + "/" + lastComponent + "_info.asset";
        this.m_materialOutputFile = m_dst.Substring(m_dst.IndexOf("Assets")) + "/" + lastComponent + ".mat";
    }
    
    public void ProcessAndSave() {
        Texture2D atlas = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
        TextureAtlasInfo tai = TextureAtlasInfo.CreateInstance<TextureAtlasInfo>();
        Material material = new Material(Shader.Find("Unlit/Transparent"));
        ProcessAndSave(atlas, tai, material, true);
    }
    
    void ProcessAndSave(Texture2D atlas, TextureAtlasInfo tai, Material material, bool isNew) {
        LoadAssets();
        Rect[] results = atlas.PackTextures(m_srcTextures.ToArray(), 0, 1024);
        atlas.Apply();
        byte[] bytes = atlas.EncodeToPNG();
        File.WriteAllBytes(m_outputFile, bytes);
        if(isNew) {
            AssetDatabase.ImportAsset(m_outputFile.Substring(m_outputFile.IndexOf("Assets")));
        }
        else {
            EditorUtility.SetDirty(atlas);
        }
        var a = AssetDatabase.LoadAssetAtPath(m_outputFile.Substring(m_outputFile.IndexOf("Assets")), typeof(Texture2D)) as Texture2D;
        
        material.mainTexture = a;
        if(isNew) {
            AssetDatabase.CreateAsset(material, m_materialOutputFile);
        }
        else {
            EditorUtility.SetDirty(material);
        }
        
        AssetDatabase.ImportAsset(m_materialOutputFile);
        material = AssetDatabase.LoadAssetAtPath(m_materialOutputFile, typeof(Material)) as Material;
        
        tai.SetInitialData(m_src, m_files, results, a, material);
        if(isNew) {
            AssetDatabase.CreateAsset(tai, m_atlasInfoOutputFile);
        }
        else {
            EditorUtility.SetDirty(tai);
        }
        UnloadAssets();
    }
    
    public void UnloadAssets() {
        Resources.UnloadUnusedAssets();
    }
    
    public void LoadAssets() {
        m_srcTextures = new List<Texture2D>();
        foreach(string path in m_files) {
            string assetPath = path.Substring(path.IndexOf("Assets"));
            Texture2D toAdd = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
            m_srcTextures.Add(toAdd);
        }
    }
    
    public void RefreshDependentObjects(TextureAtlasInfo tai) {
        // Check sprites
        Sprite[] s = GameObject.FindObjectsOfTypeIncludingAssets(typeof(Sprite)) as Sprite[];
        for(int i = 0; i < s.Length; i++) {
            if(s[i].atlas.Equals(tai)) {
                s[i].HandleStaticSpriteChange();
            }
        }
    }
    
}
