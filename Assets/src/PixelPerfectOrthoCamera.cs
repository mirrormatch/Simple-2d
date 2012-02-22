using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class PixelPerfectOrthoCamera : MonoBehaviour {
 
    public int pixelsPerUnit = 32;

    // Use this for initialization
    void Start() {
        Camera c = gameObject.GetComponent<Camera>();
        c.orthographicSize = Screen.height / (pixelsPerUnit * 2);
    }
 
    // Update is called once per frame
    void Update() {
 
    }
}
