using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTile : MonoBehaviour
{
    GameObject[] rgtg_node_graph;
    // Start is called before the first frame update
    void Start()
    {
        rgtg_node_graph = GameObject.FindGameObjectsWithTag("rgtg_node");
        Debug.Log("node list size: " + rgtg_node_graph.Length);

    }

    // Update is called once per frame
    void Update()
    {
        DebugGraph(rgtg_node_graph);
    }
    void DebugGraph(GameObject[] nodes)
    {
        Debug.Log("Debug Graph is called!");
        for(int i = 0; i < nodes.Length; i++)
        {
            for(int j = 0; j < nodes.Length; j++)
            {
                if(i != j)
                {
                    if(nodes[i].transform.position == nodes[j].transform.position)
                    {
                        nodes[i].GetComponent<Renderer>().material.color = Color.blue;
                        nodes[j].GetComponent<Renderer>().material.color = Color.blue;
                    }
                }
            }
        }
    }
}
