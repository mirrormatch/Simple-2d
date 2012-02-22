using UnityEngine;
using UnityEditor;
using System.Collections;

public class MainMenu : MonoBehaviour {

    [MenuItem ("Simple 2D/Create Texture Atlas...")]
    static void CreateTextureAtlas() {
        var srcFolder = EditorUtility.OpenFolderPanel("Select Source Images Folder", "", "");
        Debug.Log(srcFolder);
        var destFolder = EditorUtility.SaveFolderPanel("Select Location for Atlas", "", "");
        Debug.Log(destFolder);
    }
}
