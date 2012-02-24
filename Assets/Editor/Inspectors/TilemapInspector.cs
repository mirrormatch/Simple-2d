using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Tilemap))]
public class TilemapInspector : Editor {

    protected SerializedObject m_object;
    protected bool m_isDown = false;
    protected int selectedTile = 0;
 
    public void OnEnable() {
        m_object = new SerializedObject(target);
        Tilemap tm = m_object.targetObject as Tilemap;
        EditorUtility.SetSelectedWireframeHidden(tm.gameObject.renderer, true);
    }
 
    string[] TOOLS = { "Brush", "Eraser", "Fill" , "Clear"};
    int activeTool = 0;
 
    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeInspector();
        m_object.Update();
        Tilemap tm = m_object.targetObject as Tilemap;
        if(DrawDefaultInspector()) {
            tm.HandleDataChange(true);
        }
        if(tm.tileMat != null) {
            EditorGUILayout.BeginVertical();
            activeTool = GUILayout.Toolbar(activeTool, TOOLS);
            var rect = GUILayoutUtility.GetAspectRect(tm.tileMat.mainTexture.width / tm.tileMat.mainTexture.height);
            GUI.DrawTexture(rect, tm.tileMat.mainTexture);
            DrawSelectionRect(rect, selectedTile);
            if(Event.current.type == EventType.MouseDown) {
                if(rect.Contains(Event.current.mousePosition)) {
                    Debug.Log("Texture clicked!" + Event.current.mousePosition);
                    selectedTile = GetTileIndexForPoint(rect, Event.current.mousePosition);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
 
    int GetTileIndexForPoint(Rect texBounds, Vector3 point) {
        Tilemap tm = m_object.targetObject as Tilemap;
        float texToVisX = texBounds.width / tm.tileMat.mainTexture.width;
        float texToVisY = texBounds.height / tm.tileMat.mainTexture.height;
        float pxPerTileX = texToVisX * tm.tileWidth;
        float pxPerTileY = texToVisY * tm.tileWidth;
        int col = (int)((point.x - texBounds.x) / pxPerTileX);
        int row = (int)((point.y - texBounds.y) / pxPerTileY);
        return col + (row * tm.tileMat.mainTexture.width / tm.tileWidth);
    }
 
    void DrawSelectionRect(Rect texBounds, int idx) {
        Tilemap tm = m_object.targetObject as Tilemap;
        float texToVisX = texBounds.width / tm.tileMat.mainTexture.width;
        float texToVisY = texBounds.height / tm.tileMat.mainTexture.height;
     
        float pxPerTileX = texToVisX * tm.tileWidth;
        float pxPerTileY = texToVisY * tm.tileWidth;
     
        int col = GetTileCol(idx);
        int row = GetTileRow(idx);
     
        Vector3[] rectVerts = new Vector3[4];
        Vector3 bp = new Vector3(texBounds.x + col * pxPerTileX, texBounds.y + row * pxPerTileY, 0);
        rectVerts[0].x = bp.x;
        rectVerts[0].y = bp.y;
        rectVerts[1].x = bp.x + pxPerTileX;
        rectVerts[1].y = bp.y;
        rectVerts[2].x = bp.x + pxPerTileX;
        rectVerts[2].y = bp.y + pxPerTileY;
        rectVerts[3].x = bp.x;
        rectVerts[3].y = bp.y + pxPerTileY;
        Handles.DrawSolidRectangleWithOutline(rectVerts, new Color(1.0f, 1.0f, 1.0f, 0.15f), new Color(1.0f, 1.0f, 1.0f, 0.33f));
    }
 
    int GetTileCol(int idx) {
        Tilemap tm = m_object.targetObject as Tilemap;
        return idx % (tm.tileMat.mainTexture.width / tm.tileWidth);
    }
 
    int GetTileRow(int idx) {
        Tilemap tm = m_object.targetObject as Tilemap;
        return idx / (tm.tileMat.mainTexture.height / tm.tileWidth);
    }
 
    public void OnDisable() {
        Tilemap tm = m_object.targetObject as Tilemap;
        EditorUtility.SetSelectedWireframeHidden(tm.gameObject.renderer, true);
    }
 
    public void OnSceneGUI() {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        if(Event.current.type == EventType.MouseDown) {
            Tilemap tm = m_object.targetObject as Tilemap;
            Undo.RegisterSceneUndo(TOOLS[activeTool] + " " + tm.gameObject.name);
            HandleMouse(Event.current);
            m_isDown = true;
        } else if(Event.current.type == EventType.MouseDrag && m_isDown) {
            HandleMouse(Event.current);
        } else if(Event.current.type == EventType.MouseUp && m_isDown) {
            m_isDown = false;
        }
     
        if(Event.current.type == EventType.Layout) {
            HandleUtility.AddDefaultControl(controlId);
        }
    }
 
    void HandleMouse(Event e) {
        Tilemap tm = m_object.targetObject as Tilemap;
        Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        object output = HandleUtility.RaySnap(r);
        if(output != null) {
            RaycastHit rh = (RaycastHit)output;
     
            if(rh.collider != null && rh.collider.gameObject == tm.gameObject) {
                switch(activeTool) {
                case 0:
                    tm.UpdateSquareAtPoint(rh.point, selectedTile);
                    break;
                case 1:
                    tm.UpdateSquareAtPoint(rh.point, -1);
                    break;
                case 2:
                    tm.FillWith(selectedTile);
                    break;
				case 3:
					tm.FillWith(-1);
					break;
                }
            }
        }
    }
 
}
