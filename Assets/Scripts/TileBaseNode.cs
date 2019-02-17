using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBaseNode : MonoBehaviour
{
    // create enumerator for neighbours (up to 8)
    public enum Neighbours
    {
        up = 0, up_right = 1, right = 2, down_right = 3, down = 4, down_left = 5, left = 6, up_left = 7
    };
    public TileBaseNode[] neighbours = new TileBaseNode[8];

    // Pathfinding list in A* (see README.md for information)
    float cost_so_far;
    float heuristic_value;
    float total_estimated_value;

    void Start() {

        GetComponent<Renderer>().enabled = false;    
    }
    public void NodeVisible() {
        GetComponent<Renderer>().enabled = true;
    }

    public void NodeInvisible()
    {
        GetComponent<Renderer>().enabled = false;
    }

    public int Compare(TileBaseNode node_a, TileBaseNode node_b) {
        int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);
        if(result == 0) {
            return node_a.heuristic_value.CompareTo(node_b.heuristic_value);
        }

        return result;
    }
    public void ResetPathfindingValue()
    {
        cost_so_far = 0;
        heuristic_value = 0;
        total_estimated_value = 0;
    }
}
