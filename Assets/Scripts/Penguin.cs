using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    float max_velocity;
    float max_rotation_velocity;
    float max_acceleration;
    float max_rotation_acceleration;
    float time_to_target;
    float current_rotation_velocity;
    float current_velocity;
    float distance_from_target;
    float current_acceleration;
    Vector3 direction_vector = new Vector3 (0, 0, 0);
    Vector3 player_distance;
    // =================================================================
    // Steering arrive movement function with Align capacity.
    // =================================================================
    public void Move(Vector3 target_position, bool target_node) {
        if(target_node) {
            distance_from_target = (target_position - transform.position).magnitude;
            // find the direction.
            direction_vector = (target_position - transform.position).normalized;
            // find the current rotation velocity.
            current_rotation_velocity = Mathf.Min(current_rotation_velocity + max_rotation_acceleration, max_rotation_velocity);
            //Create a goal velocity that is proportional to the distance to the target (interpolated from 0 to max)
            float goal_velocity = max_velocity * (distance_from_target / 15f);
            current_velocity = Mathf.Min(current_velocity + current_acceleration, max_velocity);
            //Calculate the current acceleration based on the goal velocity and the current velocity
            current_acceleration = Mathf.Min((goal_velocity - current_velocity) / time_to_target, max_acceleration);
            // Interpolate the oriention of the penguin.
            Quaternion target_rotation = Quaternion.LookRotation(direction_vector);
            transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, current_rotation_velocity * Time.deltaTime);
            // Update the position.
            Vector3 new_position = transform.position + (current_velocity * Time.deltaTime) * transform.forward.normalized;
            transform.position = new_position;
        } else {
            // find the direction.
            direction_vector = (target_position - transform.position).normalized;
            // find the current velocity.
            current_rotation_velocity = Mathf.Min(current_rotation_velocity + max_rotation_acceleration, max_rotation_velocity);
            current_velocity = Mathf.Min(current_velocity + max_acceleration, max_velocity - 10);
            // Interpolate the orientation of the penguin.
            Quaternion target_rotation = Quaternion.LookRotation(direction_vector);
            transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, current_rotation_velocity * Time.deltaTime);
            // Update the position.
            Vector3 new_position = transform.position + (current_velocity * Time.deltaTime) * transform.forward.normalized;
            transform.position = new_position;
        }
    }

    public bool Stop() {
        current_velocity = current_velocity - max_acceleration;
        if(current_velocity == 0) {
            return true;
        } else {
            Vector3 new_position = transform.position + (current_velocity * Time.deltaTime) * transform.forward.normalized;
            transform.position = new_position;
            return false;
        }
    }

    public bool RotateTowardTarget(Vector3 target_position) {
        Quaternion target_rotation = Quaternion.LookRotation(target_position - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, max_rotation_velocity * Time.deltaTime);
        if(transform.rotation == target_rotation) {
            return true;
        } else {
            return false;
        }
    }
}
