using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// =================================================================
// This script is used to check if the winning condition for each
// team (which are selected in Scene view) are respected before 
// setting game_over to true.
// =================================================================
public class WinningCondition : MonoBehaviour
{
    public enum Team {RED, BLUE}
    public Team team;
    private GameController game_controller;

    void Start() {
        GameObject game_controller_object = GameObject.FindWithTag("GameController");
        if(game_controller_object != null) {
			game_controller = game_controller_object.GetComponent<GameController>();
		}	
		if(game_controller == null) {
			Debug.Log("cant find 'GameController' script");
		}
    }
    // =================================================================
    // Check if the player that collide with game object [color]FlagHomePosition
    // has the flag and is the right team as well!
    // =================================================================    
    private void OnTriggerEnter(Collider other) {
        if(team == Team.BLUE) {
            if(other.gameObject.tag == "Blue" && other.gameObject.GetComponent<AIBehaviour>().has_flag) {
                game_controller.game_over = true;
                game_controller.blue_win = true;
            } else {
                return;
            }
        } else if(team == Team.RED) {
            if(other.gameObject.tag == "Red" && other.gameObject.GetComponent<AIBehaviour>().has_flag) {
                game_controller.game_over = true;
                game_controller.red_win = true;
            } else {
                return;
            }
        }
    }
}
