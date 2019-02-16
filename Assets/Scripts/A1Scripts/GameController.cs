using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// =================================================================
// This class handles all actions and events that is shared during
// the game, it also handle all randomization events that occurs.
// it will also check the current state of the game.
// =================================================================
public class GameController : MonoBehaviour
{
    public GameObject[] red_team;
    public GameObject[] blue_team;
    public GameObject blue_flag;
    public GameObject red_flag;
    private AIBehaviour ai_script;
    // private AIBehaviour ai_script;
    // private AIBehaviour ai_script;
    
    int min = 0;    // minimum value for random range
    int max = 4;    // maximum value for random range
    enum Movement {WANDER, ARRIVE, FLEE, PURSUE, TAGGED, RESCUE}
    public GameObject red_tester;
    public GameObject blue_tester;
    
    bool red_tagged = false;
    bool blue_tagged = false;
    
    // variable used for Attacker().
    public bool red_attacker = false;
    public bool blue_attacker = false;

    // variable used for Rescue().
    public bool red_rescue = false;
    public bool blue_rescue = false;

    // variables used for Defender().
    public bool red_defender = false;
    public bool blue_defender = false;

    public bool red_flag_captured = false;
    public bool blue_flag_captured = false;

    // variable used for GameOver().
    public Text game_over_text;
    float restart_timer = 5.0f;
    public bool game_over = false;
    public bool blue_win = false;
    public bool red_win = false;
    // Update is called once per frame
    void Update()
    {
        // =================================================================
        // Use ESC Key to end the game.
        // =================================================================
        if(Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }
        Attacker();
        Defender();
        Rescue();
        if(game_over) {
            GameOver();
        }
    }
    // =================================================================
    // Function to select an attacker and to see if there is already one.
    // =================================================================
    void Attacker() {
        foreach (GameObject player in red_team) {
            if(player.GetComponent<AIBehaviour>().currentAction() == (int) Movement.ARRIVE) {
                red_attacker = true;
            }
        }
        foreach (GameObject player in blue_team) {
            if(player.GetComponent<AIBehaviour>().currentAction() == (int) Movement.ARRIVE) {
                blue_attacker = true;
            }
        }
        // Select a player from RED team to attack
        if(!red_attacker) {
             int index = Random.Range(min,max);
            ai_script = red_team[index].GetComponent<AIBehaviour>();
            if(ai_script.currentAction() == (int) Movement.WANDER) {
                ai_script.setActions(1);
                ai_script.target = blue_flag;
            }
        }
        // Select a player from BLUE team to attack
        if(!blue_attacker) {
            int index = Random.Range(min,max);
            ai_script = blue_team[index].GetComponent<AIBehaviour>();
            if(ai_script.currentAction() == (int) Movement.WANDER) {
                ai_script.setActions(1);
                ai_script.target = red_flag;
            }
        }
    }

    // =================================================================
    // Function to select a defender who IS NOT an attacker already.
    // =================================================================
    void Defender() {
        GameObject red_enemy_target = null;
        GameObject blue_enemy_target = null;
        // Check if there is already a pursuer for RED team
        foreach (GameObject player in red_team) {
            if(player.GetComponent<AIBehaviour>().currentAction() == (int) Movement.PURSUE) {
                red_defender = true;
            }
        }

        // Check if there is already a pursuer for BLUE team
        foreach (GameObject player in blue_team) {
            if(player.GetComponent<AIBehaviour>().currentAction() == (int) Movement.PURSUE) {
                blue_defender = true;
            }
        }
        // Check if there is a BLUE player in the RED home area
        foreach (GameObject target in blue_team) {
            if(target.GetComponent<AIBehaviour>().enemy_area && target.GetComponent<AIBehaviour>().currentAction() == (int) Movement.ARRIVE) {
                blue_enemy_target = target;
            }
        }
        // Check if there is a RED player in the BLUE home area
        foreach (GameObject target in red_team) {
            if(target.GetComponent<AIBehaviour>().enemy_area && target.GetComponent<AIBehaviour>().currentAction() == (int) Movement.ARRIVE) {
                red_enemy_target = target;
            }
        }

        // Set the Pursue action to the RED player randomly selected
        if(!red_defender && blue_enemy_target != null) {
            int index = Random.Range(min,max);
            ai_script = red_team[index].GetComponent<AIBehaviour>();
            if(ai_script.currentAction() == (int) Movement.WANDER && !ai_script.has_flag) {
                ai_script.setActions(3);
                ai_script.target = blue_enemy_target;
            }
        }
        // Set the Pursue action to the BLUE player randomly selected
        if(!blue_defender && red_enemy_target != null) {
            int index = Random.Range(min,max);
            ai_script = blue_team[index].GetComponent<AIBehaviour>();
            if(ai_script.currentAction() == (int) Movement.WANDER && !ai_script.has_flag) {
                ai_script.setActions(3);
                ai_script.target = red_enemy_target;
            }
        }
    }

    // =================================================================
    // Function to select a character to rescue the tagged characters.
    // =================================================================
    void Rescue() {
        int red_tagged_counter = 0;
        int blue_tagged_counter = 0;
        List<GameObject> tagged_red_player = new List<GameObject>();
        List<GameObject> tagged_blue_player = new List<GameObject>();
        GameObject rescue_target_red = null;
        GameObject rescue_target_blue = null;
        // =================================================================
        // Rescue a red player!
        // =================================================================
        foreach (GameObject player in red_team) {
            if(player.GetComponent<AIBehaviour>().currentAction() == (int) Movement.TAGGED) {
                red_tagged_counter++;
                tagged_red_player.Add(player);
            }
        }
        if(red_tagged_counter > 0) {
            red_tagged = true;
        } else {
            red_tagged = false;
        }
        foreach (GameObject rescuer in red_team) {
            if(rescuer.GetComponent<AIBehaviour>().currentAction() == (int) Movement.RESCUE) {
                red_rescue = true;
            }
        }
        if(red_tagged && !red_rescue) {
            int index = Random.Range(min,max);
            float closest_distance_red = float.PositiveInfinity;
            ai_script = red_team[index].GetComponent<AIBehaviour>();
            if(ai_script.currentAction() == (int) Movement.WANDER) {
                foreach (GameObject tagged_player in tagged_red_player) {
                    if((tagged_player.transform.position - red_team[index].transform.position).magnitude < closest_distance_red) {
                        closest_distance_red = (tagged_player.transform.position - red_team[index].transform.position).magnitude;
                        rescue_target_red = tagged_player;                        
                    }
                }
                ai_script.setActions(5);
                ai_script.target = rescue_target_red;
                red_rescue = true;
            }
        }
        // =================================================================
        // Rescue a blue player!
        // =================================================================
        foreach (GameObject player in blue_team) {
            if(player.GetComponent<AIBehaviour>().currentAction() == (int) Movement.TAGGED) {
                blue_tagged_counter++;
                tagged_blue_player.Add(player);
            }
        }
        if(blue_tagged_counter > 0) {
            blue_tagged = true;
        } else {
            blue_tagged = false;
        }
        foreach (GameObject rescuer in blue_team) {
            if(rescuer.GetComponent<AIBehaviour>().currentAction() == (int) Movement.RESCUE) {
                blue_rescue = true;
            }
        }
        if(blue_tagged && !blue_rescue) {
            int index = Random.Range(min,max);
            float closest_distance_blue = float.PositiveInfinity;
            ai_script = blue_team[index].GetComponent<AIBehaviour>();
            if(ai_script.currentAction() == (int) Movement.WANDER) {
                foreach (GameObject tagged_player in tagged_blue_player) {
                    if((tagged_player.transform.position - blue_team[index].transform.position).magnitude < closest_distance_blue) {
                        closest_distance_blue = (tagged_player.transform.position - blue_team[index].transform.position).magnitude;
                        rescue_target_blue = tagged_player;                        
                    }
                }
                ai_script.setActions(5);
                ai_script.target = rescue_target_blue;
                blue_rescue = true;
            }
        }
        // check if everyone is tagged.
        WholeTeamTagged(red_tagged_counter, blue_tagged_counter);
    }
    // =================================================================
    // Function named WholeTeamTagged that take two integers to 
    // check if the whole team has been tagged and apply the proper action
    // =================================================================
    void WholeTeamTagged(int red_counter, int blue_counter) {
        if(red_counter > 3 && blue_counter > 3) {
            game_over = true;
        } else if(red_counter > 3) {
            game_over = true;
            blue_win = true;
        } else if(blue_counter > 3) {
            game_over = true;
            red_win = true;
        }
    }
    // =================================================================
    // Function named GameOver for when the game end.
    // =================================================================
    public void GameOver() {
        restart_timer -= Time.deltaTime;
        if(restart_timer < 0) {
            Application.LoadLevel(Application.loadedLevel);
        }
        if(blue_win) {
            // blue team win
            game_over_text.text = "Blue team win! game restart in: " + (int) restart_timer;
        } else if(red_win) {
            // red team win
            game_over_text.text = "Red team win! game restart in: " + (int) restart_timer;

        } else {
            game_over_text.text = "Game ended with draw! game restart in: " + (int) restart_timer;
        }
    }
}
