using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Sprite))]
public class SpriteInspector : Editor {

    protected SerializedObject m_object;
 
    public void OnEnable() {
        m_object = new SerializedObject(target);
    }
    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeInspector();
        Sprite s = m_object.targetObject as Sprite;
        HandleAtlasSelector(s);
        HandleSpriteDropdown(s);
    }
    
    void HandleAtlasSelector(Sprite s) {
        var newAtlas = EditorGUILayout.ObjectField("Texture Atlas:", s.atlas, typeof(TextureAtlasInfo), true) as TextureAtlasInfo;
        if(newAtlas != s.atlas) {
            s.atlas = newAtlas;
            s.HandleAtlasChange();
        }
    }
    
    void HandleSpriteDropdown(Sprite s) {
        if(s.atlas != null) {
            string[] dropdownList = s.atlas.textureNames;
            int selected = s.atlas.GetSelectedIdx(s.staticSpriteFrame);
            int newSelected = EditorGUILayout.Popup("Selected Sprite:", selected, dropdownList);
            if(newSelected != selected) {
                s.staticSpriteFrame = s.atlas.NameForIdx(newSelected);
                s.HandleStaticSpriteChange();
            }
        }
    }
}