using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkable_mask;
    public Vector2 grid_size;
    public float node_radius;

    Node[,] grid;

    float node_diameter;
    int grid_x, grid_y;

    void Start()
    {
        node_diameter = node_radius * 2;
        grid_x = Mathf.RoundToInt(grid_size.x / node_diameter);
        grid_y = Mathf.RoundToInt(grid_size.y / node_diameter);
        Create();
    }

    private void Create()
    {
        grid = new Node[grid_x, grid_y];
        Vector3 world_down_left_corner = transform.position - Vector3.right * grid_size.x / 2 - Vector3.forward * grid_size.y / 2;
        for(int x = 0; x < grid_x; x++)
        {
            for(int y = 0; y < grid_y; y++)
            {
                Vector3 world_point = world_down_left_corner + Vector3.right * (x * node_diameter + node_radius) + Vector3.forward * (y * node_diameter + node_radius);
                bool no_obstacle = !(Physics.CheckSphere(world_point, node_radius, unwalkable_mask));
                grid[x, y] = new Node(no_obstacle, world_point, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int check_x = node.grid_x + x;
                int check_y = node.grid_y + y;

                if (check_x >= 0 && check_x < grid_x && check_y >= 0 && check_y < grid_y)
                {
                    neighbours.Add(grid[check_x, check_y]);
                }
            }
        }
        return neighbours;
    }
    public Node NodeFromWorldPoint(Vector3 world_pos)
    {
        float percent_x = (world_pos.x + grid_size.x / 2) / grid_size.x;
        float percent_y = (world_pos.y + grid_size.y / 2) / grid_size.y;
        percent_x = Mathf.Clamp01(percent_x);
        percent_y = Mathf.Clamp01(percent_y);

        int x = Mathf.RoundToInt((grid_x - 1) * percent_x);
        int y = Mathf.RoundToInt((grid_y - 1) * percent_y);

        return grid[x, y];
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(grid_size.x, 1, grid_size.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.no_obstacle) ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.position, Vector3.one * (node_diameter - .1f));
            }
        }
    }
}
