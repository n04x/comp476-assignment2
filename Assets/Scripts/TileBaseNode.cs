using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBaseNode : MonoBehaviour
{
    // create enumerator for neighbours (up to 8)
    public enum Neighbours
    {
        up_left = 0, up = 1, up_right = 2, right = 3, down_right = 4, down = 5, down_left = 6, left = 7
    };
    public TileBaseNode[] neighbours = new TileBaseNode[8];

    // Pathfinding list in A* (see README.md for information)
    float cost_so_far;
    float heuristic_value;
    float total_estimated_value;

    void Start() {

        GetComponent<Renderer>().enabled = false;    
    }
    void NodeVisible() {
        GetComponent<Renderer>().enabled = true;
    }

    public int Compare(TileBaseNode node_a, TileBaseNode node_b) {
        int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);
        if(result == 0) {
            return node_a.heuristic_value.CompareTo(node_b.heuristic_value);
        }

        return result;
    }

}
