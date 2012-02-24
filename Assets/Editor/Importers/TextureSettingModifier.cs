using UnityEngine;
using UnityEditor;
using System.Collections;

public class TextureSettingModifier : AssetPostprocessor {
    
    void OnPreprocessTexture() {
        var importer = assetImporter as TextureImporter;
        importer.isReadable = true;
        importer.textureFormat = TextureImporterFormat.ARGB32;
        importer.mipmapEnabled = false;
        importer.textureType = TextureImporterType.Advanced;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Point;
    }
    
    void OnPostprocessTexture(Texture2D tex) {
    }
}
