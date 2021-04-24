using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtMouse : MonoBehaviour {
    [SerializeField] private Camera _camera;
    [SerializeField] private Texture2D _cursorTexture;

    void Awake() {
        if (_camera == null) {
            _camera = GameObject.FindObjectOfType<Camera>();
        }
    }

    private void Update() {
        if (_cursorTexture != null) {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Cursor.SetCursor(_cursorTexture, mousePos, CursorMode.ForceSoftware);
        }
    }
}
