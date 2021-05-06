using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUIManager : MonoBehaviour
{
    public void Awake() {
        // revert to default cursor
        Cursor.SetCursor(null, Camera.main.ScreenToWorldPoint(Input.mousePosition), CursorMode.ForceSoftware);
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
