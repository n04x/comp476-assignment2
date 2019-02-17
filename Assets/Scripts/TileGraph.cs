using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph : MonoBehaviour
{
    public AIBehaviour penguin_script;
    public Node tile_based_node_script;
    public Vector3 map_size;
    public float radius;
    public Vector3 tile_size;
    public float tile_size_density;
    public enum Modes { DIJSKTRA, EUCLIDEAN, CLUSTER };
    public Modes current_mode = Modes.DIJSKTRA;
    public bool rgtg_base_mode = true;  // false for pov.

    // ==============================================================================
    // all rgtg variable and list needed for our pathfinding 
    // look in README for meaning of rgtg
    // ==============================================================================
    public Node rgtg_start_node;
    public Node rgtg_end_node;
    public List<Node> rgtg_node_list = new List<Node>();
    public List<Node> rgtg_path_list = new List<Node>();
    public List<Node> rgtg_open_list = new List<Node>();
    public List<Node> rgtg_closed_list = new List<Node>();
    public List<Node> rgtg_closet1_nodes = new List<Node>();
    public List<Node> rgtg_closet2_nodes = new List<Node>();
    public List<Node> rgtg_closet3_nodes = new List<Node>();
    public Node rgtg_target_node;
    public Node rgtg_random_node;
    public List<List<float>> rgtg_cluster_lookup = new List<List<float>>();

    // ==============================================================================
    // all pov variable and list needed for our pathfinding 
    // look in README for meaning of pov
    // ==============================================================================
    public Node pov_start_node;
    public Node pov_end_node;
    public List<Node> pov_node_list = new List<Node>();
    public List<Node> pov_path_list = new List<Node>();
    public List<Node> pov_open_list = new List<Node>();
    public List<Node> pov_closed_list = new List<Node>();
    public List<Node> pov_closet1_node = new List<Node>();
    public List<Node> pov_closet2_node = new List<Node>();
    public List<Node> pov_closet3_node = new List<Node>();
    public Node pov_target_node;
    public Node pov_random_node;
    public List<List<float>> pov_cluster_lookup = new List<List<float>>();




    int closet;
    int target_closet;


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
        if(rgtg_base_mode)
        {
            foreach(Node node in rgtg_node_list)
            {
                node.TurnNodeVisible();
                //node.GetComponent<Renderer>().material.color = Color.white;
            }
            // unvisited node.
            foreach(Node node in rgtg_open_list)
            {
                node.GetComponent<Renderer>().material.color = Color.yellow;
            }
            // closed node.
            foreach(Node node in rgtg_closed_list)
            {
                node.GetComponent<Renderer>().material.color = Color.grey;
            }
            // selected path for our penguin.
            foreach(Node node in rgtg_path_list)
            {
                node.GetComponent<Renderer>().material.color = Color.green;
            }
            // starting and ending point.
            rgtg_start_node.GetComponent<Renderer>().material.color = Color.red;
            rgtg_end_node.GetComponent<Renderer>().material.color = Color.red;

            CalculateTileGraphPath();
        }
    }


    void CreateTile()
    {
        GameObject[] tile_based_node = GameObject.FindGameObjectsWithTag("tile_based_node");

        foreach(GameObject go in tile_based_node)
        {
            rgtg_node_list.Add(go.GetComponent<Node>());
        }

        for(int i = 0; i < rgtg_node_list.Count; i++)
        {
            CreateNeighbours(rgtg_node_list[i]);
        }
    }

    void CreateNeighbours(Node node)
    {
        node.rgtg_neighbours[(int)Node.Neighbours.upper] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, tile_size.z)));
        node.rgtg_neighbours[(int)Node.Neighbours.upper_right] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, tile_size.z)));
        node.rgtg_neighbours[(int)Node.Neighbours.right] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, 0)));
        node.rgtg_neighbours[(int)Node.Neighbours.lower_right] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, -tile_size.z)));
        node.rgtg_neighbours[(int)Node.Neighbours.lower] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -tile_size.z)));
        node.rgtg_neighbours[(int)Node.Neighbours.lower_left] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, -tile_size.z)));
        node.rgtg_neighbours[(int)Node.Neighbours.left] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, 0)));
        node.rgtg_neighbours[(int)Node.Neighbours.upper_left] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, tile_size.z)));
    }

    void ClearTile()
    {
        rgtg_open_list.Clear();
        rgtg_closed_list.Clear();
        rgtg_path_list.Clear();

        foreach(Node node in rgtg_node_list)
        {
            node.ResetValue();
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
            rgtg_target_node = rgtg_closet1_nodes[UnityEngine.Random.Range(0, rgtg_closet1_nodes.Count)];
        }
        else if(target_closet == 1)
        {
            rgtg_target_node = rgtg_closet2_nodes[UnityEngine.Random.Range(0, rgtg_closet2_nodes.Count)];
        } else
        {
            rgtg_target_node = rgtg_closet3_nodes[UnityEngine.Random.Range(0, rgtg_closet3_nodes.Count)];
        }
    }
    private void CalculateTileGraphPath()
    {
        // clear the current list to re-calculate the path.
        rgtg_open_list.Clear();
        rgtg_closed_list.Clear();
        rgtg_path_list.Clear();

        counter = 0;    // set the counter to zero
        rgtg_start_node = rgtg_node_list[0];
        rgtg_start_node.SetCostSoFar(0);

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
        rgtg_open_list.Add(rgtg_start_node); // add the starting node to open node list.
        float heuristic_temp = Cost(rgtg_start_node.transform, rgtg_end_node.transform);
        rgtg_start_node.SetHeuristicValue(heuristic_temp);
        float total_estimated_value_temp = rgtg_start_node.CostSoFar() + rgtg_start_node.HeuristicValue();
        rgtg_start_node.SetTotalEstimatedValue(total_estimated_value_temp);

    }

    float Cost(Transform node,Transform neighbours)
    {
        float distance = (node.position - neighbours.position).magnitude;
        return distance;
    }
}

