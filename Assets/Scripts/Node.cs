using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    // Pathfinding list in A* (see README.md for information)
    float cost_so_far;
    float heuristic_value;
    float total_estimated_value;
    public bool rgtg = true;
    // ==============================================================================
    // REGULAR GRID (TILE) GRAPH
    // using abbreviation rgtg -> regular grid tile graph
    // ==============================================================================
    public Node[] rgtg_neighbours; // create an array to hold all neighbours. see README for mapping value. 
    public Node rgtg_previous;
    // ==============================================================================
    // POINT OF VISIBILITY GRAPH
    // using abbreviation pov -> points of visibility
    // ==============================================================================
    public List<Node> pov_neighbours;
    public Node pov_previous;

    void Start()
    {
        if(rgtg)
        {
            rgtg_neighbours = new Node[8];
        } else
        {
            pov_neighbours = new List<Node>();
        }
        GetComponent<Renderer>().enabled = false;   // start by disabling it.
    }
    public void TurnNodeVisible()
    {
        GetComponent<Renderer>().enabled = true;
    }
    public void TurnNodeInvisible()
    {
        GetComponent<Renderer>().enabled = false;
    }
    
    public void ResetValue()
    {
        cost_so_far = 0;
        heuristic_value = 0;
        total_estimated_value = 0;
    }
    // Getter and Setter for cost-so-far.
    public float CostSoFar()
    {
        return cost_so_far;
    }
    public void SetCostSoFar(float value)
    {
        cost_so_far = value;
    }

    // Getter and Setter for heuristic value.
    public float HeuristicValue()
    {
        return heuristic_value;
    }
    public void SetHeuristicValue(float value)
    {
        heuristic_value = value;
    }

    // Getterand setter for total estimate value.
    public float TotalEstimateValue()
    {
        return total_estimated_value;
    }
    public void SetTotalEstimatedValue(float value)
    {
        total_estimated_value = value;
    }
    // Compare needed for regular grid tile graph to check
    public int Compare(Node node_a, Node node_b)
    {
        int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);
        if(result == 0)
        {
            return node_a.heuristic_value.CompareTo(node_b.heuristic_value);
        }
        return result;
    }
}
