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
    
    public AtlasGenerator(string src, string dst) {
        this.m_src = src;
        var splitPath = src.Split(new char[] {'/'});
        this.m_dst = dst;
        this.m_files = Directory.GetFiles(src, "*.png", SearchOption.AllDirectories);
        this.m_outputFile = dst + "/" + splitPath[splitPath.Length - 1] + ".png";
    }
    
    public void ProcessAndSave() {
        Debug.Log("Processing Atlas for folder: " + this.m_src);
        LoadAssets();
        Texture2D atlas = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
        Material newMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
        atlas.PackTextures(m_srcTextures.ToArray(), 0, 1024);
        //newMaterial.mainTexture = atlas;
        atlas.Apply();
        //EditorUtility.CompressTexture(atlas, TextureFormat.ARGB32, TextureCompressionQuality.Best);
        byte[] bytes = atlas.EncodeToPNG();
        File.WriteAllBytes(m_outputFile, bytes);
    }
    
    public void LoadAssets() {
        m_srcTextures = new List<Texture2D>();
        foreach(string path in m_files) {
            string assetPath = path.Substring(path.IndexOf("Assets"));
            Texture2D toAdd = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
            m_srcTextures.Add(toAdd);
        }
        Debug.Log("Loaded " + m_srcTextures.Count + " textures for processing");
    }
    
}
