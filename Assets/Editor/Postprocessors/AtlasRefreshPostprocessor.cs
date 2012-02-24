using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AtlasRefreshPostprocessor : AssetPostprocessor {

	static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom) {
        List<string> changed = new List<string>();
        changed.AddRange(imported);
        changed.AddRange(deleted);
        changed.AddRange(moved);
        changed.AddRange(movedFrom);
        
        TextureAtlasInfo[] atlases = GameObject.FindObjectsOfTypeIncludingAssets(typeof(TextureAtlasInfo)) as TextureAtlasInfo[];
        foreach(TextureAtlasInfo tai in atlases) {
            if(tai.Overlaps(changed)) {
                Debug.Log("Updating atlas for: " + tai.srcDirectory);
                AtlasGenerator ag = new AtlasGenerator();
                ag.Regenerate(tai);
                ag.RefreshDependentObjects(tai);
            }
        }
    }
}
