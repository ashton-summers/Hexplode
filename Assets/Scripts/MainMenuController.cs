using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    /// <summary>
    /// Method that will be called when the 'play game' button is pressed
    /// </summary>
	public void StartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
