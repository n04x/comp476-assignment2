using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    float max_seek_velocity;
    float max_rotation_velocity;
    float max_flee_velocity;
    float max_acceleration;
    float max_rotation_acceleration;
    float time_to_target;
    Vector3 direction_vector = new Vector3 (0, 0, 0);
    Vector3 player_distance;
    // =================================================================
    // Steering arrive movement function
    // =================================================================
    void SteeringArrive(Vector3 target_position, bool end_node) {

    }
}
