using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InstructionsScreen : MonoBehaviour {
    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame() {
        SceneManager.LoadScene("Level_01");
    }
}
