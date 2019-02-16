using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// =================================================================
// This script is used for the Main Menu button to start the game.
// once it has been clicked it will load the game.
// =================================================================
public class LoadOnClick : MonoBehaviour
{
    public void LoadScene(int lvl) {
		SceneManager.LoadScene(lvl);
	}
}
