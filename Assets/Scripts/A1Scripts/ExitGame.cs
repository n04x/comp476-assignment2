using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// =================================================================
// Handle the game exit button to exit and close the game 
// =================================================================
public class ExitGame : MonoBehaviour
{
    public void Exit() {
        Application.Quit();
    }
}
