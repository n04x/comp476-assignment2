using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// =================================================================
// Handle border of the game and respawn on the other end.
// =================================================================
public class AIRespawn : MonoBehaviour
{
    // check if the attached object script is on the x-axis or not.
    public bool x_axis;
    public float offset;
    
    // =================================================================
    // OnTrigger function to handle the respawn on the other end mechanism
    // it uses an offset as well in order to respawn INSIDE the border.
    // =================================================================
    void OnTriggerEnter(Collider other) {
        if(x_axis) {
            other.transform.position = new Vector3(-other.transform.position.x + offset, other.transform.position.y, other.transform.position.z);
        } else
        {
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, -other.transform.position.z + offset);
            
        }
    }
}
