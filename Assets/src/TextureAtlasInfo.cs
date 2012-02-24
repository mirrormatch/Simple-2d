using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextureAtlasInfo : ScriptableObject {
 
    public string srcDirectory;
    public string[] textureFiles;
    public string[] textureNames;
    public Rect[] images;
    public Texture2D atlas;
    public Material material;
    
    public void SetInitialData(string src, string[] texturePaths, Rect[] positions, Texture2D outputAtlas, Material m) {
        srcDirectory = src;
        atlas = outputAtlas;
        textureFiles = texturePaths;
        textureNames = new string[texturePaths.Length];
        images = positions;
        var splitPath = src.Split(new char[] { '/' });
        var endpoint = splitPath[splitPath.Length - 1];
        for(int i = 0; i < texturePaths.Length; i++) {
            var name = texturePaths[i].Substring(texturePaths[i].IndexOf(endpoint));
            textureNames[i] = name;
        }
        material = m;
    }
    
    public int GetSelectedIdx(string name) {
        return System.Array.IndexOf(textureNames, name);
    }
    
    public string NameForIdx(int idx) {
        return textureNames[idx];
    }
    
    public Rect RectForName(string name) {
        Rect r = images[GetSelectedIdx(name)];
        r = new Rect(r.x * atlas.width, r.y * atlas.height, r.width * atlas.width, r.height * atlas.height);
        return r;
    }
    
    public Rect UVsForName(string name) {
        Rect r = images[GetSelectedIdx(name)];
        return r;
    }
    
    public bool Overlaps(List<string> changedAssets) {
        foreach(string toCheck in changedAssets) {
            var path = System.IO.Directory.GetCurrentDirectory() + "/" + toCheck;
            if(Array.IndexOf(textureFiles, path) > -1 || path.IndexOf(srcDirectory) > -1) {
                return true;
            }
        }
        
        return false;
    }
}
