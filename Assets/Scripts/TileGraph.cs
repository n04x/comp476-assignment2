using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph : MonoBehaviour
{
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
    public TileBaseNode end_tile_node;
    public List<List<float>> tile_clister_lookup = new List<List<float>>();
    public bool tile_base_mode = true;

    public TileBaseNode random_tile_node;

    void Start()
    {
        map_size = new Vector3(150, 0, 60);
        tile_size = new Vector3(map_size.x / tile_size_density / tile_size_density, 0, map_size.z / tile_size_density);
        radius = tile_size.x / 2;

        // generate the tiles
        CreateTile();
        ClearTile();
        FindStartNode();
        FindEndNode();
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
        node.neighbours[(int)TileBaseNode.Neighbours.up] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.up_right] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.right] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, 0)));
        node.neighbours[(int)TileBaseNode.Neighbours.down_right] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(tile_size.x, 0, -tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.down] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.down_left] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, -tile_size.z)));
        node.neighbours[(int)TileBaseNode.Neighbours.left] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, 0)));
        node.neighbours[(int)TileBaseNode.Neighbours.up_left] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3(-tile_size.x, 0, tile_size.z)));
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
}
