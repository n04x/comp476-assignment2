using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// =================================================================
// This class handle the home area of each team which is selected
// in the Scene view. It will handle any character that enter enemy
// area and exit enemy area but flipping the boolean variable.
// =================================================================
public class HomeArea : MonoBehaviour
{
    private AIBehaviour ai_script;
    // private AIBehaviour ai_script;
    public enum Team {RED, BLUE};
    public Team team;
    // =================================================================
    // OnTriggerEnter for when a character enters the enemy area
    // =================================================================
    void OnTriggerEnter(Collider other) {
        if(team == Team.RED) {
            if(other.gameObject.tag == "Blue") {
                ai_script = other.GetComponent<AIBehaviour>();
                ai_script.enemy_area = true;
            }
        } else if(team == Team.BLUE) {
            if(other.gameObject.tag == "Red") {
                ai_script = other.GetComponent<AIBehaviour>();
                ai_script.enemy_area = true;
            }
        } else {
            return;
        }
    }
    // =================================================================
    // OnTriggerExit for when a character exits the enemy area.
    // =================================================================
    void OnTriggerExit(Collider other) {
        if(team ==Team.RED) {
            if(other.gameObject.tag == "Blue") {
                ai_script = other.GetComponent<AIBehaviour>();
                if(ai_script.has_flag == false) {
                    ai_script.enemy_area = false;
                }
            } 
        } else if(team == Team.BLUE) {
            if(other.gameObject.tag == "Red") {
                ai_script = other.GetComponent<AIBehaviour>();
                if(ai_script.has_flag == false) {
                    ai_script.enemy_area = false;                
                }
            }
        } else {
            return;
        }    
    }
}
