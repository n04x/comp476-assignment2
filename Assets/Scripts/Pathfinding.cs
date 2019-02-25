using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pathfinding : MonoBehaviour
{
    public Penguin penguin;
    //public Node tile_based_node_script;
    public Vector3 map_size;
    //float radius;
    public enum Algorithms { DIJSKTRA, EUCLIDEAN, CLUSTER };
    public Algorithms current_algorithm = Algorithms.DIJSKTRA;
    public bool rgtg_mode = true;  // false for pov.
    public Text textui_mode;
    public Text textui_counter;
    public Text textui_algorithm;
    // ==============================================================================
    // all rgtg variable and list needed for our pathfinding 
    // look in README for meaning of rgtg
    // ==============================================================================
    public Vector3 rgtg_node_size;
    public float rgtg_density;
    public Node rgtg_start_node;
    public Node rgtg_target_node;
    public Node rgtg_random_node;
    public List<Node> rgtg_node_list = new List<Node>();
    public List<Node> rgtg_path_list = new List<Node>();
    public List<Node> rgtg_open_list = new List<Node>();
    public List<Node> rgtg_closed_list = new List<Node>();
    public List<Node> rgtg_closet1_nodes = new List<Node>();
    public List<Node> rgtg_closet2_nodes = new List<Node>();
    public List<Node> rgtg_closet3_nodes = new List<Node>();
    public List<Node> rgtg_closet4_nodes = new List<Node>();
    public List<List<float>> rgtg_cluster = new List<List<float>>();
    // ==============================================================================
    // all povg variable and list needed for our pathfinding 
    // look in README for meaning of pov
    // ==============================================================================
    public Node povg_start_node;
    public Node povg_target_node;
    public List<Node> povg_node_list = new List<Node>();
    public List<Node> povg_path_list = new List<Node>();
    public List<Node> povg_open_list = new List<Node>();
    public List<Node> povg_closed_list = new List<Node>();
    public List<Node> povg_closet1_node = new List<Node>();
    public List<Node> povg_closet2_node = new List<Node>();
    public List<Node> povg_closet3_node = new List<Node>();
    public List<Node> povg_closet4_node = new List<Node>();
    public Node povg_random_node;
    public List<List<float>> povg_cluster_lookup = new List<List<float>>();

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
            penguin.transform.position = new Vector3(rgtg_random_node.transform.position.x, penguin.transform.position.y, rgtg_random_node.transform.position.z);
        } else if(closet == 1)
        {
            int node_pos = UnityEngine.Random.Range(0, rgtg_closet2_nodes.Count);
            rgtg_random_node = rgtg_closet2_nodes[node_pos];
            penguin.transform.position = new Vector3(rgtg_random_node.transform.position.x, penguin.transform.position.y, rgtg_random_node.transform.position.z);
        }
        else if(closet == 2)
        {
            int node_pos = UnityEngine.Random.Range(0, rgtg_closet3_nodes.Count);
            rgtg_random_node = rgtg_closet3_nodes[node_pos];
            penguin.transform.position = new Vector3(rgtg_random_node.transform.position.x, penguin.transform.position.y, rgtg_random_node.transform.position.z);
        }

        //LookUpTable(6);
        ClearRgtg();
        ClearPov();
        FindStartNode();
        //if(rgtg_mode)
        //{
        //    FindRgtgEndNode(closet);
        //} else
        //{
        //    FindPovEndNode(closet);
        //}
        FindTargetNode(closet);
        rgtg_start_node.SetCostSoFar(0);

    }

    void Update()
    {
        // update the displayed text in the UI.
        UpdateTextUI(rgtg_mode);
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            // todo
        } else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            // todo
        } else if(Input.GetKeyDown(KeyCode.Alpha3))
        {

        }

        if(rgtg_mode)
        {
            // color nodes tiles.
            foreach (Node node in rgtg_node_list)
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
                node.GetComponent<Renderer>().material.color = Color.black;
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
                if(Vector3.Angle(penguin.transform.forward, (rgtg_path_list[counter].transform.position - penguin.transform.position)) > 20) {
                    penguin.Stop();
                    penguin.AlignTowardTarget(rgtg_path_list[counter].transform.position);
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
                Debug.Log("inside the else to set rgtg_mode to false...");
                rgtg_mode = false;
                foreach(Node node in rgtg_path_list) {
                    node.TurnNodeInvisible();
                }
                foreach (Node node in povg_path_list)
                {
                    node.TurnNodeVisible();   
                }
                CalculatePoVPath();
            }

        } 
        else {
            Debug.Log("iside the PoV side");
            foreach (Node node in povg_node_list)
            {
                node.TurnNodeVisible();
            }
            // unvisited node.
            foreach (Node node in povg_open_list)
            {
                node.GetComponent<Renderer>().material.color = Color.yellow;
            }
            // closed node.
            foreach (Node node in povg_closed_list)
            {
                node.GetComponent<Renderer>().material.color = Color.grey;
            }
            // selected path for our penguin.
            foreach (Node node in povg_path_list)
            {
                node.GetComponent<Renderer>().material.color = Color.green;
            }
            // starting and target point.
            povg_start_node.GetComponent<Renderer>().material.color = Color.blue;
            povg_target_node.GetComponent<Renderer>().material.color = Color.red;

            if (povg_path_list.Count > counter && povg_target_node == povg_path_list[povg_path_list.Count - 1]) {
                if(Vector3.Angle(penguin.transform.forward, (povg_path_list[counter].transform.position - penguin.transform.position)) > 20) {
                    penguin.Stop();
                    penguin.AlignTowardTarget(povg_path_list[counter].transform.position);
                } else {
                    if(counter == povg_path_list.Count - 1) {
                        penguin.Move(povg_path_list[counter].transform.position, true);
                    } else {
                        penguin.Move(povg_path_list[counter].transform.position, false);
                    }
                }
                bool node_reached = false;
                Collider[] collision_array = Physics.OverlapSphere(penguin.transform.position, 0.2f);
                for (int i = 0; i < collision_array.Length; i++)
                {
                    if(collision_array[i].GetComponent(typeof(Node)) == povg_path_list[counter]) {
                        node_reached = true;
                    }
                }

                if(node_reached) {
                    counter++;
                }
            } else {
                rgtg_mode = true;
                foreach (Node node in povg_node_list)
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

    // ===========================================================
    // General-purpose pathfinding script functions.
    // ===========================================================
    void BuildGraph()
    {
        // Build graph for regular grid tile
        GameObject[] rgtg_node_graph = GameObject.FindGameObjectsWithTag("temp_tag");

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
                rgtg_closet1_nodes.Add(node);
            } else if(pos.x <= -16 && pos.z >= 28)
            {
                rgtg_closet2_nodes.Add(node);
            } else if(pos.x >= 30 && pos.z >= 20)
            {
                rgtg_closet3_nodes.Add(node);
            } else if(pos.x >= 28 && pos.z <= -28)
            {
                rgtg_closet4_nodes.Add(node);
            }
        }

        // Build graph for point of visibility.
        GameObject[] povg_node_graph = GameObject.FindGameObjectsWithTag("povg_node");
        foreach (GameObject go in povg_node_graph)
        {
            povg_node_list.Add(go.GetComponent<Node>());
        }
        foreach (Node node in povg_node_list)
        {
            Vector3 pos = node.transform.position;        
            if(pos.x <= -30 && pos.z <= -32)
            {
                node.GetComponent<Renderer>().material.color = Color.blue;
                povg_closet1_node.Add(node);
            } else if(pos.x <= -16 && pos.z >= 28)
            {
                node.GetComponent<Renderer>().material.color = Color.cyan;
                povg_closet2_node.Add(node);
            } else if(pos.x >= 30 && pos.z >= 20)
            {
                node.GetComponent<Renderer>().material.color = Color.yellow;
                povg_closet3_node.Add(node);
            } else if(pos.x >= 28 && pos.z <= -28)
            {
                node.GetComponent<Renderer>().material.color = Color.black;
                povg_closet4_node.Add(node);
            }
        }
    }
    float Cost(Transform node, Transform neighbours)
    {
        float distance = (node.position - neighbours.position).magnitude;
        return distance;
    }
    void UpdateTextUI(bool mode)
    {
        if (mode)
        {
            textui_mode.text = "Pathfinding graph: RGTG";
            textui_counter.text = "Counter: " + counter;
            textui_algorithm.text = "A* algorithm: " + current_algorithm;
        }
    }
    void FindStartNode()
    {
        // find the starting according to my penguin position
        for (int i = 0; i < rgtg_node_list.Count; i++)
        {
            if (i == 0)
            {
                rgtg_start_node = rgtg_node_list[i];
            }
            else
            {
                if ((penguin.transform.position - rgtg_node_list[i].transform.position).magnitude < (penguin.transform.position - rgtg_start_node.transform.position).magnitude)
                {
                    rgtg_start_node = rgtg_node_list[i];
                }
            }
        }
    }
    void FindTargetNode(int room)
    {
        do
        {
            target_closet = UnityEngine.Random.Range(0, 3);
        } while (room == target_closet);

        if (rgtg_mode)
        {
            if (target_closet == 0)
            {
                rgtg_target_node = rgtg_closet1_nodes[UnityEngine.Random.Range(0, rgtg_closet1_nodes.Count)];
            }
            else if (target_closet == 1)
            {
                rgtg_target_node = rgtg_closet2_nodes[UnityEngine.Random.Range(0, rgtg_closet2_nodes.Count)];
            }
            else
            {
                rgtg_target_node = rgtg_closet3_nodes[UnityEngine.Random.Range(0, rgtg_closet3_nodes.Count)];
            }
        }
        else
        {
            do
            {
                target_closet = UnityEngine.Random.Range(0, 3);
            } while (room == target_closet);

            if (target_closet == 0)
            {
                povg_target_node = povg_closet1_node[UnityEngine.Random.Range(0, povg_closet1_node.Count)];
            }
            else if (target_closet == 1)
            {
                povg_target_node = povg_closet2_node[UnityEngine.Random.Range(0, povg_closet2_node.Count)];
            }
            else
            {
                povg_target_node = povg_closet3_node[UnityEngine.Random.Range(0, povg_closet3_node.Count)];
            }
        }
    }
    // used for cluster heuristic.
    //void LookUpTable(int layers)
    //{
    //    // regular grid tile graph table
    //    for (int i = 0; i < layers; i++)
    //    {
    //        rgtg_cluster.Add(new List<float>());
    //        for (int j = 0; j < layers; j++)
    //        {
    //            if (i == j)
    //            {
    //                rgtg_cluster[i].Add(0);
    //            }
    //            else
    //            {
    //                int start = LayerMask.NameToLayer("cluster" + i);
    //                int end = LayerMask.NameToLayer("cluster" + j);
    //                rgtg_start_node = GetRgtgNodeCluster(start);
    //                rgtg_target_node = GetRgtgNodeCluster(end);
    //                ClearRgtg();
    //                CalculateDijkstraRGTG();
    //                float weight = RgtgClusterWeight(rgtg_path_list, start, end);
    //                rgtg_cluster[i].Add(weight * 1000000);
    //            }
    //        }
    //    }
    //}
    // ===========================================================
    // Regular grid tile graph pathfinding script functions.
    // ===========================================================
    void CreateNeighbours(Node node)
    {
        node.rgtg_neighbours[0] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, rgtg_node_size.z)));
        node.rgtg_neighbours[1] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(rgtg_node_size.x, 0, rgtg_node_size.z)));
        node.rgtg_neighbours[2] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(rgtg_node_size.x, 0, 0)));
        node.rgtg_neighbours[3] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(rgtg_node_size.x, 0, -rgtg_node_size.z)));
        node.rgtg_neighbours[4] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -rgtg_node_size.z)));
        node.rgtg_neighbours[5] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-rgtg_node_size.x, 0, -rgtg_node_size.z)));
        node.rgtg_neighbours[6] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-rgtg_node_size.x, 0, 0)));
        node.rgtg_neighbours[7] = rgtg_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-rgtg_node_size.x, 0, rgtg_node_size.z)));
    }
    void ClearRgtg()
    {
        rgtg_open_list.Clear();
        rgtg_closed_list.Clear();
        rgtg_path_list.Clear();

        foreach(Node node in rgtg_node_list)
        {
            node.ResetValue();
        }
    }
    void CalculateRGTGPath()
    {
        // clear the current list to re-calculate the path.
        rgtg_open_list.Clear();
        rgtg_closed_list.Clear();
        rgtg_path_list.Clear();

        counter = 0;    // set the counter to zero

        rgtg_start_node = rgtg_node_list[0];
        rgtg_start_node.SetCostSoFar(0);

        foreach (Node node in rgtg_node_list)
        {
            if (Cost(penguin.transform, node.transform) < Cost(penguin.transform, rgtg_start_node.transform))
            {
                rgtg_start_node = node;
            }
        }

        closet = target_closet;
        FindTargetNode(closet);
        CalculateDijkstraRGTG();
    }
    void CalculateDijkstraRGTG()
    {
        rgtg_open_list.Add(rgtg_start_node); // add the starting node to open node list.

        while (rgtg_open_list.Count > 0 || rgtg_closed_list.Count != rgtg_node_list.Count)
        {

            Node current_node = rgtg_open_list[0];
            foreach (Node possible_node in rgtg_open_list)
            {
                if (possible_node.TotalEstimateValue() < current_node.TotalEstimateValue())
                {
                    current_node = possible_node;
                }
            }
            rgtg_open_list.Remove(current_node);
            rgtg_closed_list.Add(current_node);

            foreach (Node neighbour in current_node.rgtg_neighbours)
            {
                if (neighbour == null)
                {
                    continue;
                }
                bool inside_open_list = false;
                bool inside_closed_list = false;

                if (rgtg_closed_list.Contains(neighbour))
                {
                    inside_closed_list = true;
                }
                else if (rgtg_open_list.Contains(neighbour))
                {
                    inside_open_list = true;
                }

                float new_cost_so_far = (current_node.CostSoFar() + Cost(current_node.transform, neighbour.transform));

                if (rgtg_closed_list.Contains(neighbour) && new_cost_so_far < neighbour.CostSoFar())
                {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                    rgtg_closed_list.Remove(neighbour);
                    rgtg_open_list.Add(neighbour);
                }
                else if (inside_open_list && new_cost_so_far < neighbour.CostSoFar())
                {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                }
                else if (!inside_open_list && !inside_closed_list)
                {
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
            int size = rgtg_path_list.Count - 1;
            if (rgtg_path_list[rgtg_path_list.Count - 1].previous == rgtg_start_node)
            {
                rgtg_path_list.Add(rgtg_path_list[rgtg_path_list.Count - 1].previous);
                rgtg_path_list.Reverse();
                return;
            }
            else
            {
                rgtg_path_list.Add(rgtg_path_list[size].previous);
            }
        }
    }
    // ===========================================================
    // Point of visibility graph pathfinding scrip functions.
    // ===========================================================
    void ClearPov() {
        povg_open_list.Clear();
        povg_closed_list.Clear();
        povg_path_list.Clear();

        foreach(Node node in povg_node_list){
            node.ResetValue();
        }
    }
    void CalculatePoVPath() {
        povg_open_list.Clear();
        povg_closed_list.Clear();
        povg_path_list.Clear();

        counter = 0;    // set the counter to zero.
        povg_start_node = povg_node_list[0];
        povg_start_node.SetCostSoFar(0);
        foreach(Node node in povg_node_list)
        {
            if(Cost(penguin.transform, node.transform) < Cost(penguin.transform, povg_start_node.transform))
            {
                povg_start_node = node;
            }
        }
        closet = target_closet;
        FindTargetNode(closet);
        CalculateDijkstraPoV();
    }
    void CalculateDijkstraPoV() {
        povg_open_list.Add(povg_start_node);  // add the starting node to open node list.

        while(povg_open_list.Count > 0 || povg_closed_list.Count != povg_node_list.Count) {
            Node current_node = povg_open_list[0];   // hold the current node.
            foreach (Node possible_node in povg_open_list)
            {
                if(possible_node.TotalEstimateValue() < current_node.TotalEstimateValue()) {
                    current_node = possible_node;
                }
            }

            povg_open_list.Remove(current_node);
            povg_closed_list.Add(current_node);

            foreach (Node neighbour in current_node.pov_neighbours)
            {
                if(neighbour == null) {
                    continue;
                }
                bool inside_open_list = false;
                bool inside_closed_list = false;

                if(povg_closed_list.Contains(neighbour)) {
                    inside_closed_list = true;
                } else if(povg_open_list.Contains(neighbour)) {
                    inside_open_list = true;
                }

                float new_cost_so_far = (current_node.CostSoFar() + Cost(current_node.transform, neighbour.transform));

                if(inside_closed_list && new_cost_so_far < neighbour.CostSoFar()) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                    povg_closed_list.Remove(neighbour);
                    povg_open_list.Add(neighbour);
                } else if(inside_open_list && new_cost_so_far < neighbour.CostSoFar()) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.SetTotalEstimatedValue(new_estimated_value);
                    neighbour.previous = current_node;
                } else if(!inside_open_list && !inside_closed_list) {
                    neighbour.SetCostSoFar(new_cost_so_far);
                    float new_estimated_value = neighbour.CostSoFar() + neighbour.HeuristicValue();
                    neighbour.previous = current_node;
                    povg_open_list.Add(neighbour);
                }
            }
        }
        
        povg_path_list.Add(povg_target_node);
        while(true) {
            if(povg_path_list[povg_path_list.Count - 1].previous == povg_start_node) {
                Node temp_node = povg_path_list[povg_path_list.Count - 1].previous;
                povg_path_list.Add(temp_node);
                povg_path_list.Reverse();
                return;
            } else
            {
                povg_path_list.Add(povg_path_list[povg_path_list.Count - 1].previous);
            }
        }
    }
    
 
}

