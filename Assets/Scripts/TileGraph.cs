using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph : MonoBehaviour
{
    public AIBehaviour penguin_script;
    public TileBaseNode tile_based_node_script;
    public Vector3 map_size;
    public float radius;
    public Vector3 tile_size;
    public float tile_size_density;


    bool execute_once_flag = false;

    public TileBaseNode start_node;
    public TileBaseNode end_node;

    // lists to hold our node based on its value (base/pathing/open/closed)
    public List<TileBaseNode> tile_base_node_list = new List<TileBaseNode>();
    public List<TileBaseNode> tile_path_list = new List<TileBaseNode>();
    public List<TileBaseNode> tile_open_list = new List<TileBaseNode>();
    public List<TileBaseNode> tile_closed_list = new List<TileBaseNode>();
    public List<TileBaseNode> room1_tile_nodes = new List<TileBaseNode>();
    public List<TileBaseNode> room2_tile_nodes = new List<TileBaseNode>();
    public List<TileBaseNode> room3_tile_nodes = new List<TileBaseNode>();

    public List<List<float>> tile_cluster_lookup = new List<List<float>>();
    public TileBaseNode target_tile_node;

    public bool tile_base_mode = true;

    public TileBaseNode random_tile_node;

    int closet;
    int target_closet;

    public enum Modes { DIJSKTRA, EUCLIDEAN, CLUSTER };
    public Modes current_mode = Modes.DIJSKTRA;

    void Start()
    {
        map_size = new Vector3(80, 0, 80);
        tile_size = new Vector3(map_size.x / tile_size_density, 0, map_size.z / tile_size_density);
        radius = tile_size.x / 2;

        // generate the tiles
        CreateTile();
        ClearTile();
        FindStartNode();
        //FindEndNode(room);

    }
    int counter = 0;    // count the number of node reached
    void Update()
    {
        if(tile_base_mode)
        {
            foreach(TileBaseNode node in tile_base_node_list)
            {
                node.NodeVisible();
                //node.GetComponent<Renderer>().material.color = Color.white;
            }
            // unvisited node.
            foreach(TileBaseNode node in tile_open_list)
            {
                node.GetComponent<Renderer>().material.color = Color.yellow;
            }
            // closed node.
            foreach(TileBaseNode node in tile_closed_list)
            {
                node.GetComponent<Renderer>().material.color = Color.grey;
            }
            // selected path for our penguin.
            foreach(TileBaseNode node in tile_path_list)
            {
                node.GetComponent<Renderer>().material.color = Color.green;
            }
            // starting and ending point.
            start_node.GetComponent<Renderer>().material.color = Color.red;
            end_node.GetComponent<Renderer>().material.color = Color.red;

            CalculateTileGraphPath();
        }
    }


    void CreateTile()
    {
        GameObject[] tile_based_node = GameObject.FindGameObjectsWithTag("tile_based_node");

        foreach(GameObject go in tile_based_node)
        {
            tile_base_node_list.Add(go.GetComponent<TileBaseNode>());
        }

        for(int i = 0; i < tile_base_node_list.Count; i++)
        {
            CreateNeighbours(tile_base_node_list[i]);
        }
    }

    void CreateNeighbours(TileBaseNode node)
    {
        node.neighbours[(int)TileBaseNode.Neighbours.upper] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.upper_right] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.right] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, 0)));
        node.neighbours[(int)TileBaseNode.Neighbours.lower_right] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, -tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.lower] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.lower_left] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, -tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.left] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, 0)));
        node.neighbours[(int)TileBaseNode.Neighbours.upper_left] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, tile_size.z)));
    }

    void ClearTile()
    {
        tile_open_list.Clear();
        tile_closed_list.Clear();
        tile_path_list.Clear();

        foreach(TileBaseNode node in tile_base_node_list)
        {
            node.ResetPathfindingValue();
        }
    }
    void FindStartNode() {
        // find the starting according to my penguin position
    }

    private void FindEndNode(int room)
    {
        while(room == target_closet)
        {
            target_closet = UnityEngine.Random.Range(0, 3);
        }
        if(target_closet == 0)
        {
            target_tile_node = room1_tile_nodes[UnityEngine.Random.Range(0, room2_tile_nodes.Count)];
        }
        else if(target_closet == 1)
        {
            target_tile_node = room2_tile_nodes[UnityEngine.Random.Range(0, room2_tile_nodes.Count)];
        } else
        {
            target_tile_node = room3_tile_nodes[UnityEngine.Random.Range(0, room3_tile_nodes.Count)];
        }
    }
    private void CalculateTileGraphPath()
    {
        // clear the current list to re-calculate the path.
        tile_open_list.Clear();
        tile_closed_list.Clear();
        tile_path_list.Clear();

        counter = 0;    // set the counter to zero
        start_node = tile_base_node_list[0];
        start_node.SetCostSoFar(0);

        //foreach(TileBaseNode node in tile_base_node_list)
        //{
        //    if(Cost(penguin_script.transform, node.transform) < Cost(penguin_script.transform, start_node.transform))
        //    {
        //        start_node = node;
        //    }
        //}
        closet = target_closet;

        calculateDijkstraTileMode();
    }

    void calculateDijkstraTileMode()
    {
        tile_open_list.Add(start_node); // add the starting node to open node list.
        float heuristic_temp = Cost(start_node.transform, end_node.transform);
        start_node.SetHeuristic(heuristic_temp);
        float total_estimated_value_temp = start_node.CostSoFar() + start_node.HeuristicValue();
        start_node.SetTotalEstimatedValue(total_estimated_value_temp);

    }

    float Cost(Transform node,Transform neighbours)
    {
        float distance = (node.position - neighbours.position).magnitude;
        return distance;
    }
}

