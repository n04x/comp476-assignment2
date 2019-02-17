using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoVBaseNode : MonoBehaviour
{
    public List<PoVBaseNode> neighbours = new List<PoVBaseNode>();

    // Pathfinding list in A* (see README.md for information)
    float cost_so_far;
    float heuristic_value;
    float total_estimated_value;

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    public void NodeVisible()
    {
        GetComponent<Renderer>().enabled = true;
    }

    public void NodeInvisible()
    {
        GetComponent<Renderer>().enabled = false;
    }

    public int Compare(PoVBaseNode node_a, PoVBaseNode node_b)
    {
        int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);
        if (result == 0)
        {
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
    public float CostSoFar()
    {
        return cost_so_far;
    }
    public void SetCostSoFar(float _cost_so_far)
    {
        cost_so_far = _cost_so_far;
    }

    public float HeuristicValue()
    {
        return heuristic_value;
    }
    public void SetHeuristic(float _heuristic_value)
    {
        heuristic_value = _heuristic_value;
    }

    public float TotalEstimateValue()
    {
        return total_estimated_value;
    }
    public void SetTotalEstimatedValue(float _total_estimated_value)
    {
        total_estimated_value = _total_estimated_value;
    }
}
