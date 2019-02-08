using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// =================================================================
// handle all behavior involved the Flag for both team. The right 
// team choice is chosen by the public enumerator Team in the Scene
// =================================================================

public class FlagBehavior : MonoBehaviour
{
    private GameController game_controller;
    public GameObject enemy;
    public GameObject flag_home_position;
    bool flag_captured = false;
    public enum Team {RED, BLUE};
    public Team team;
    void Start() {
        GameObject game_controller_object = GameObject.FindWithTag("GameController");
        if(game_controller_object != null) {
			game_controller = game_controller_object.GetComponent<GameController>();
		}	
		if(game_controller == null) {
			Debug.Log("cant find 'GameController' script");
		}
    }

    void Update() {
        if(flag_captured) {
            if(team == Team.BLUE) {
                transform.position = (enemy.transform.position - new Vector3(1.0f, 0.0f, 0.0f));
                if(enemy.GetComponent<AIBehaviour>().currentAction() == 4) {
                    flag_captured = false;
                }
            } else if(team == Team.RED) {
                transform.position = (enemy.transform.position + new Vector3(1.0f, 0.0f, 0.0f));
                 if(enemy.GetComponent<AIBehaviour>().currentAction() == 4) {
                    flag_captured = false;
                }
            }
            
        } else {
            transform.position = flag_home_position.transform.position;
        }    
        if(team == Team.BLUE && !flag_captured) {
            game_controller.blue_flag_captured = false;
        }
        if(team == Team.RED && !flag_captured) {
            game_controller.red_flag_captured = false;
        }
    }
    // =================================================================
    // OnTriggerEnter to do the proper action when an character captures
    // the flag and when the character drops the flag as well
    // =================================================================
    void OnTriggerEnter(Collider other) {
        if(team == Team.BLUE) {
            if(other.gameObject.tag == "Red") {
                game_controller.blue_flag_captured = true;
                flag_captured = true;
                other.gameObject.GetComponent<AIBehaviour>().has_flag = true;
                other.gameObject.GetComponent<AIBehaviour>().target = GameObject.FindGameObjectWithTag("RedBase");
                enemy = other.gameObject;
            }
        } else if(team == Team.RED) {
            if(other.gameObject.tag == "Blue") {
                game_controller.red_flag_captured = true;
                flag_captured = true;
                other.gameObject.GetComponent<AIBehaviour>().has_flag = true;
                other.gameObject.GetComponent<AIBehaviour>().target = GameObject.FindGameObjectWithTag("BlueBase");
                enemy = other.gameObject;
            }
        } else {
            return;
        }
        
    }
}
