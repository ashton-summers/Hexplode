using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RedWinsController : MonoBehaviour {

    /// <summary>
    /// Method to be called if the main menu button is clicked
    /// </summary>
	public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
