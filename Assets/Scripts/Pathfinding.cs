﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Penguin penguin;
    //public Node tile_based_node_script;
    public Vector3 map_size;
    //float radius;
    public enum Modes { DIJSKTRA, EUCLIDEAN, CLUSTER };
    public Modes current_mode = Modes.DIJSKTRA;
    public bool rgtg_mode = true;  // false for pov.

    // ==============================================================================
    // all rgtg variable and list needed for our pathfinding 
    // look in README for meaning of rgtg
    // ==============================================================================
    public Vector3 rgtg_node_size;
    public float rgtg_density;
    public Node rgtg_start_node;
    public Node rgtg_target_node;
    public List<Node> rgtg_node_list = new List<Node>();
    public List<Node> rgtg_path_list = new List<Node>();
    public List<Node> rgtg_open_list = new List<Node>();
    public List<Node> rgtg_closed_list = new List<Node>();
    public List<Node> rgtg_closet1_nodes = new List<Node>();
    public List<Node> rgtg_closet2_nodes = new List<Node>();
    public List<Node> rgtg_closet3_nodes = new List<Node>();
    public List<Node> rgtg_closet4_nodes = new List<Node>();
    //public Node rgtg_target_node_indicator;
    public Node rgtg_random_node;
    public List<List<float>> rgtg_cluster_lookup = new List<List<float>>();
    // ==============================================================================
    // all pov variable and list needed for our pathfinding 
    // look in README for meaning of pov
    // ==============================================================================
    public Node pov_start_node;
    public Node pov_target_node;
    public List<Node> pov_node_list = new List<Node>();
    public List<Node> pov_path_list = new List<Node>();
    public List<Node> pov_open_list = new List<Node>();
    public List<Node> pov_closed_list = new List<Node>();
    public List<Node> pov_closet1_node = new List<Node>();
    public List<Node> pov_closet2_node = new List<Node>();
    public List<Node> pov_closet3_node = new List<Node>();
    public List<Node> pov_closet4_node = new List<Node>();
    //public Node pov_target_node_indicator;
    public Node pov_random_node;
    public List<List<float>> pov_cluster_lookup = new List<List<float>>();

    int closet;
    int target_closet;
    int counter = 0;    // count the number of node reached



    void Start()
    {
        map_size = new Vector3(80, 0, 80);
        rgtg_density = 40.0f;
        rgtg_node_size = new Vector3(map_size.x / rgtg_density, 0, map_size.z / rgtg_density);
        //radius = rgtg_node_size.x / 2;

        // generate the tiles
        BuildGraph();

        closet = UnityEngine.Random.Range(0, 3);
        if(closet == 0)
        {
            int node_pos = UnityEngine.Random.Range(0, rgtg_closet1_nodes.Count);
            rgtg_random_node = rgtg_closet1_nodes[node_pos];
            penguin.transform.position = new Vector3(rgtg_random_node.transform.position.x, rgtg_random_node.transform.position.y, rgtg_random_node.transform.position.z);
        } else if(closet == 1)
        {
            int node_pos = UnityEngine.Random.Range(0, rgtg_closet2_nodes.Count);
            rgtg_random_node = rgtg_closet2_nodes[node_pos];
            penguin.transform.position = new Vector3(rgtg_random_node.transform.position.x, rgtg_random_node.transform.position.y, rgtg_random_node.transform.position.z);
        }
        else if(closet == 2)
        {
            int node_pos = UnityEngine.Random.Range(0, rgtg_closet3_nodes.Count);
            rgtg_random_node = rgtg_closet3_nodes[node_pos];
            penguin.transform.position = new Vector3(rgtg_random_node.transform.position.x, rgtg_random_node.transform.position.y, rgtg_random_node.transform.position.z);
        }

        ClearTile();
        ClearPov();
        FindStartNode();
        FindEndNode(closet);

        rgtg_start_node.SetCostSoFar(0);

    }
    void Update()
    {
        if(rgtg_mode)
        {
            // color nodes tiles.
            foreach(Node node in rgtg_node_list)
            {
                node.TurnNodeVisible();
                //node.GetComponent<Renderer>().material.color = Color.white;
            }
            // open node.
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
            // starting and target point.
            rgtg_start_node.GetComponent<Renderer>().material.color = Color.blue;
            rgtg_target_node.GetComponent<Renderer>().material.color = Color.red;

            // check if the penguin is on the path.
            if(rgtg_path_list.Count > counter && rgtg_target_node == rgtg_path_list[rgtg_path_list.Count - 1]) {
                if(Vector3.Angle(penguin.transform.forward, (rgtg_path_list[counter].transform.position - penguin.transform.position)) > 10) {
                    penguin.Stop();
                    penguin.RotateTowardTarget(rgtg_path_list[counter].transform.position);
                } else {
                    if(counter == rgtg_path_list.Count - 1) {
                        penguin.Move(rgtg_path_list[counter].transform.position, true);
                    } else {
                        penguin.Move(rgtg_path_list[counter].transform.position, false);
                    }
                }
                bool node_reached = false;
                Collider[] collision_array = Physics.OverlapSphere(penguin.transform.position, 0.2f);
                for(int i = 0; i < collision_array.Length; i++) {
                    if(collision_array[i].GetComponent(typeof(Node)) == rgtg_path_list[counter]) {
                        node_reached = true;
                    }
                }
                if(node_reached) {
                    counter++;
                }
            }

            else {
                rgtg_mode = false;
                foreach(Node node in rgtg_path_list) {
                    node.TurnNodeInvisible();
                }
                foreach (Node node in pov_path_list)
                {
                    node.TurnNodeVisible();   
                }
                CalculatePoVPath();
            }

            // CalculateRGTGPath();
        } 
        else {
            foreach (Node node in pov_node_list)
            {
                node.TurnNodeVisible();
            }
            // unvisited node.
            foreach (Node node in pov_open_list)
            {
                node.GetComponent<Renderer>().material.color = Color.yellow;
            }
            // closed node.
            foreach (Node node in pov_closed_list)
            {
                node.GetComponent<Renderer>().material.color = Color.grey;
            }
            // selected path for our penguin.
            foreach (Node node in pov_path_list)
            {
                node.GetComponent<Renderer>().material.color = Color.green;
            }
            // starting and target point.
            pov_start_node.GetComponent<Renderer>().material.color = Color.blue;
            pov_target_node.GetComponent<Renderer>().material.color = Color.red;

            // CalculatePoVPath();
            if(pov_path_list.Count > counter && pov_target_node == pov_path_list[pov_path_list.Count - 1]) {
                if(Vector3.Angle(penguin.transform.forward, (pov_path_list[counter].transform.position - penguin.transform.position)) > 10) {
                    penguin.Stop();
                    penguin.RotateTowardTarget(pov_path_list[counter].transform.position);
                } else {
                    if(counter == pov_path_list.Count - 1) {
                        penguin.Move(pov_path_list[counter].transform.position, true);
                    } else {
                        penguin.Move(pov_path_list[counter].transform.position, false);
                    }
                }
                bool node_reached = false;
                Collider[] collision_array = Physics.OverlapSphere(penguin.transform.position, 0.2f);
                for (int i = 0; i < collision_array.Length; i++)
                {
                    if(collision_array[i].GetComponent(typeof(Node)) == pov_path_list[counter]) {
                        node_reached = true;
                    }
                }

                if(node_reached) {
                    counter++;
                }
            } else {
                rgtg_mode = true;
                foreach (Node node in pov_node_list)
                {
                    node.TurnNodeInvisible();
                }
                foreach (Node node in rgtg_node_list)
                {
                    node.TurnNodeVisible();
                }
                CalculateRGTGPath();

            }
        }
    }


    void BuildGraph()
    {
        // Build graph for regular grid tile
        GameObject[] rgtg_node_graph = GameObject.FindGameObjectsWithTag("rgtg_node");

        foreach(GameObject go in rgtg_node_graph)
        {
            rgtg_node_list.Add(go.GetComponent<Node>());
        }

        for(int i = 0; i < rgtg_node_list.Count; i++)
        {
            CreateNeighbours(rgtg_node_list[i]);
        }

        foreach(Node node in rgtg_node_list)
        {
            Vector3 pos = node.transform.position;        
            if(pos.x <= -30 && pos.z <= -32)
            {
                // node.GetComponent<Renderer>().material.color = Color.blue;
                rgtg_closet1_nodes.Add(node);
            } else if(pos.x <= -16 && pos.z >= 28)
            {
                // node.GetComponent<Renderer>().material.color = Color.cyan;
                rgtg_closet2_nodes.Add(node);
            } else if(pos.x >= 30 && pos.z >= 20)
            {
                // node.GetComponent<Renderer>().material.color = Color.yellow;
                rgtg_closet3_nodes.Add(node);
            } else if(pos.x >= 28 && pos.z <= -28)
            {
                // node.GetComponent<Renderer>().material.color = Color.black;
                rgtg_closet4_nodes.Add(node);
            }
        }

        // Build graph for point of visibility.
        GameObject[] pov_node_graph = GameObject.FindGameObjectsWithTag("pov_node");

        foreach (Node node in pov_node_list)
        {
            Vector3 pos = node.transform.position;        
            if(pos.x <= -30 && pos.z <= -32)
            {
                node.GetComponent<Renderer>().material.color = Color.blue;
                pov_closet1_node.Add(node);
            } else if(pos.x <= -16 && pos.z >= 28)
            {
                node.GetComponent<Renderer>().material.color = Color.cyan;
                pov_closet2_node.Add(node);
            } else if(pos.x >= 30 && pos.z >= 20)
            {
                node.GetComponent<Renderer>().material.color = Color.yellow;
                pov_closet3_node.Add(node);
            } else if(pos.x >= 28 && pos.z <= -28)
            {
                node.GetComponent<Renderer>().material.color = Color.black;
                pov_closet4_node.Add(node);
            }
        }
    }

    void CreateNeighbours(Node node)
    {
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.upper] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, tile_size.z)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.upper_right] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, tile_size.z)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.right] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, 0)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.lower_right] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, -tile_size.z)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.lower] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -tile_size.z)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.lower_left] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, -tile_size.z)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.left] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, 0)));
        //node.rgtg_neighbours[(int)Node.RGTG_Neighbours.upper_left] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, tile_size.z)));

        node.rgtg_neighbours[0] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, rgtg_node_size.z)));
        node.rgtg_neighbours[1] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(rgtg_node_size.x, 0, rgtg_node_size.z)));
        node.rgtg_neighbours[2] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(rgtg_node_size.x, 0, 0)));
        node.rgtg_neighbours[3] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(rgtg_node_size.x, 0, -rgtg_node_size.z)));
        node.rgtg_neighbours[4] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -rgtg_node_size.z)));
        node.rgtg_neighbours[5] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-rgtg_node_size.x, 0, -rgtg_node_size.z)));
        node.rgtg_neighbours[6] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-rgtg_node_size.x, 0, 0)));
        node.rgtg_neighbours[7] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-rgtg_node_size.x, 0, rgtg_node_size.z)));
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
    void ClearPov() {
        pov_open_list.Clear();
        pov_closed_list.Clear();
        pov_path_list.Clear();

        foreach(Node node in pov_node_list){
            node.ResetValue();
        }
    }

    void FindStartNode() {
        // find the starting according to my penguin position
    }

    private void FindEndNode(int room)
    {
        do {
            target_closet = UnityEngine.Random.Range(0, 3);
        } while(room == target_closet);

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
    private void CalculateRGTGPath()
    {
        // clear the current list to re-calculate the path.
        rgtg_open_list.Clear();
        rgtg_closed_list.Clear();
        rgtg_path_list.Clear();

        counter = 0;    // set the counter to zero

        rgtg_start_node = rgtg_node_list[0];
        rgtg_start_node.SetCostSoFar(0);

        // foreach(Node node in rgtg_path_list)
        // {
        //    if(Cost(penguin.transform, node.transform) < Cost(penguin.transform, rgtg_start_node.transform))
        //    {
        //        rgtg_start_node = node;
        //    }
        // }
        closet = target_closet;
        // FindEndNode(closet);
        CalculateDijkstraRGTG();
    }

    void CalculatePoVPath() {
        pov_open_list.Clear();
        pov_closed_list.Clear();
        pov_path_list.Clear();

        counter = 0;    // set the counter to zero.
        pov_start_node = pov_node_list[0];
        pov_start_node.SetCostSoFar(0);

        closet = target_closet;

        CalculateDijkstraPoV();
    }
    void CalculateDijkstraRGTG()
    {
        rgtg_open_list.Add(rgtg_start_node); // add the starting node to open node list.

        while(rgtg_open_list.Count > 0 || rgtg_closed_list.Count != rgtg_node_list.Count) {
            Node current_node = rgtg_open_list[0];
            foreach (Node possible_node in rgtg_open_list)
            {
                if(possible_node.TotalEstimateValue() < current_node.TotalEstimateValue()) {
                    current_node = possible_node;
                }
            }
            rgtg_open_list.Remove(current_node);
            rgtg_closed_list.Add(current_node);

            foreach (Node neighbour in current_node.rgtg_neighbours)
            {
                if(neighbour == null) {
                    continue;
                }
                bool inside_open_list = false;
                bool inside_closed_list = false;

                if(rgtg_closed_list.Contains(neighbour)) {
                    inside_closed_list = true;
                } else if(rgtg_open_list.Contains(neighbour)){
                    inside_open_list = true;
                }

                float new_cost_so_far = (current_node.CostSoFar() + Cost(current_node.transform, neighbour.transform));

                if(inside_closed_list && new_cost_so_far < neighbour.CostSoFar()) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                    rgtg_closed_list.Remove(neighbour);
                    rgtg_open_list.Add(neighbour);
                } else if(inside_open_list && new_cost_so_far < neighbour.CostSoFar()) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                } else if(!inside_open_list && !inside_closed_list) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                    rgtg_open_list.Add(neighbour);
                }

            }
        }

        rgtg_path_list.Add(rgtg_target_node);
        while (true)
        {
            if(rgtg_path_list[rgtg_path_list.Count - 1].previous == rgtg_start_node) {
                rgtg_path_list.Add(rgtg_path_list[rgtg_path_list.Count - 1].previous);
                rgtg_path_list.Reverse();
                return;
            } else {
                rgtg_path_list.Add(rgtg_path_list[rgtg_path_list.Count - 1].previous);
            }
        }
        // float heuristic_temp = Cost(rgtg_start_node.transform, rgtg_target_node.transform);
        // rgtg_start_node.SetHeuristicValue(heuristic_temp);
        // float total_estimated_value_temp = rgtg_start_node.CostSoFar() + rgtg_start_node.HeuristicValue();
        // rgtg_start_node.SetTotalEstimatedValue(total_estimated_value_temp);

    }
    void CalculateDijkstraPoV() {
        pov_open_list.Add(pov_start_node);  // add the starting node to open node list.

        while(pov_open_list.Count > 0 || pov_closed_list.Count != pov_node_list.Count) {
            Node current_node = pov_open_list[0];   // hold the current node.
            foreach (Node possible_node in pov_open_list)
            {
                if(possible_node.TotalEstimateValue() < current_node.TotalEstimateValue()) {
                    current_node = possible_node;
                }
            }

            pov_open_list.Remove(current_node);
            pov_closed_list.Add(current_node);

            foreach (Node neighbour in current_node.pov_neighbours)
            {
                if(neighbour == null) {
                    continue;
                }
                bool inside_open_list = false;
                bool inside_closed_list = false;

                if(pov_closed_list.Contains(neighbour)) {
                    inside_closed_list = true;
                } else if(pov_open_list.Contains(neighbour)) {
                    inside_open_list = true;
                }

                float new_cost_so_far = (current_node.CostSoFar() + Cost(current_node.transform, neighbour.transform));

                if(inside_closed_list && new_cost_so_far < neighbour.CostSoFar()) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                    pov_closed_list.Remove(neighbour);
                    pov_open_list.Add(neighbour);
                } else if(inside_open_list && new_cost_so_far < neighbour.CostSoFar()) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                } else if(!inside_open_list && !inside_closed_list) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.previous = current_node;
                    pov_open_list.Add(neighbour);
                }
            }
        }
        
        pov_path_list.Add(pov_target_node);
        while(true) {
            if(pov_path_list[pov_path_list.Count - 1].previous == pov_start_node) {
                Node temp_node = pov_path_list[pov_path_list.Count - 1].previous;
                pov_path_list.Add(temp_node);
                pov_path_list.Reverse();
                return;
            } else
            {
                pov_path_list.Add(pov_path_list[pov_path_list.Count - 1].previous);
            }
        }
    }
    float Cost(Transform node,Transform neighbours)
    {
        float distance = (node.position - neighbours.position).magnitude;
        return distance;
    }
}
