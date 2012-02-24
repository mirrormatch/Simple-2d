using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class MainMenu : MonoBehaviour {

    [MenuItem ("Simple 2D/Create Texture Atlas...")]
    static void CreateTextureAtlas() {
        var srcFolder = EditorUtility.OpenFolderPanel("Select Source Images Folder", "", "");
        var destFolder = EditorUtility.SaveFolderPanel("Select Location for Atlas", "", "");
        if(srcFolder.Contains("Assets") != true && destFolder.Contains ("Assets") != true) {
            Debug.LogError("Both source and destination folders must be in the project hierarchy");
            return;
        }
        
        var generator = new AtlasGenerator(srcFolder, destFolder);
        generator.ProcessAndSave();
    }
    
    [MenuItem ("Simple 2D/Create Sprite")]
    static void CreateSprite() {
        GameObject go = new GameObject("Sprite");
        Sprite s = go.AddComponent<Sprite>();
        s.Init();
        Resources.UnloadUnusedAssets();
    }
}
