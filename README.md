# comp476-assignment2    
## Personal information    
COMP476 Assignment #1: Advanced Game Development, Concordia University    
Presented by: Thomas Backs    
ID: 27554524    
***
## Introduction    
`//todo`

## Map Layout    
Below is a picture of the map layout of the game, with number assigned to each *Room[#]* gameobject in unity. The obstacles has been removed from this picture in order to keep it clear! The red outline design the room, each room has a small room called *closet*. There is two corridor in the middle, one that lead to a dead-end.    
![Map Layout](/map-layout.png)
    
    
## Obstacles    
The obstacles is clearly defined by the color RED in the map layout. There is two type of obstacle, a 4-sided one and a triangular prism one. In all three (3) rooms there is some obstacles.

## Scripts file explanation    
### TileBaseNode.cs
This file contains the script for Nodes creation and linking all the nodes in our map. We have a enumerator called `Neighbours` that is responsible of identifying and linking all neighbouring nodes (up to 8). We also have three (3) float variable called `cost_so_far`, `heuristic_value`, and `total_estimated_cost`, these are variable needed in our calculation of A* pathfinding based on Slide #37 of Week 03. There is also enumerator that will be used for determining neighbours, `public enum Neighbours { upper = 0, upper_right = 1, right = 2, lower_right = 3, lower = 4, lower_left = 5, left = 6, upper_left = 7 };`    

* void Start()
    * `GetComponent<Renderer>().enabled = false;` To hide the rendering of the node from our map.
* void NodeVisible()
    * `GetComponent<Renderer>().enabled = true;` To enable the visibility of the node on the map.
* public int Compare(Node node_a, Node node_b)
    * `int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);` To compare the estimated cost from one node to another, again based on slides information of A* algorithm. CompareTo() method is a C# method.
    * return either the `result` or `node_a.heuristic_value.CompareTo(node_b.heuristic_value)` as integer value.    
* public void ResetPathfindingValue()
    * function to reset cost-so-far, heuristic, and total estimate value to zero.

### TileGraph.cs
This file contains the scripts to create connection and to calculate the pathfinding algorithm for the *regular grid (Tile) graph*. It will stores all available nodes in a `List`, the list variable are shown like this:`public List<TileBaseNode> [name] = new List<TileBaseNode>();`. Where the *name* is the variable name of the list. We also have a single end node which is named `public TileBaseNode target_tile_node` which will contains the target node of this algorithm. There is also a public boolean value to enable the mode: `public bool tile_base_mode = true` it is set to true since it is the default one that will be used for our AI. there is two integers for rooms as well, it will allows us to select which room will be the target. 

* void CreateTile()
    * function to create tilemap and link all neighbours nodes to each other. inside this function, it will call another function called **CreateNeighbours** which will create the neighbours for each nodes (can contains up to 8 neighbours).    
    
* void CreateNeighbours(TileBaseNode node)
    * function to connect each neighbours of our Tile Base Graph, it will check all 8 possible connection which are `upper, upper_right, right, lower, lower_right, left, lower_left`. More information for these enumerator in **TileBaseNode.cs**. `        node.neighbours[(int)TileBaseNode.Neighbours.[Neighbours]] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3([value], 0, [value])));` This will check to see if there is any neighbouring node that correspond to our requirement, and if it found one it will return with its position value as n,the value in square bracket correspond to one of these three possible value which are `tilebase.x`, `-tilebase.x` or `0` and for the second one, we replace x by z. The `[Neighbours]` correspond to the enumerator position that can be found in **TileBaseNode.cs**, and format it as integer value as well. Then we create an anonymous function which is Lambda Expression, to return a node position that satisfy the condition.    

* void ClearTile()
    * function to clear all status of node and then call the `ResetPathfindingValue` function of the **TileBaseNode.cs** to reset its cost and value.
    

## Sources 
The source for 3d penguin: https://free3d.com/3d-model/emperor-penguin-601811.html
