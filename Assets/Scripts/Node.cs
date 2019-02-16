using UnityEngine;
using System.Collections;

public class Node {
    public bool no_obstacle;
    public Vector3 position;
    public int grid_x;
    public int grid_y;

    public int cost_so_far;
    public int heuristic_cost;
    public Node parent;

    public Node(bool _no_obstacle, Vector3 _position, int _grid_x, int _grid_y)
    {
        no_obstacle = _no_obstacle;
        position = _position;
        grid_x = _grid_x;
        grid_y = _grid_y;
    }

    public int estimateTotalCost()
    {
        return cost_so_far + heuristic_cost;
    }
}
    