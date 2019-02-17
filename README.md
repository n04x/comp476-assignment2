# comp476-assignment2    
## Personal information    
COMP476 Assignment #1: Advanced Game Development, Concordia University    
Presented by: Thomas Backs    
ID: 27554524    
***
## Introduction    
`//todo`

## Map Layout    
Below is a picture of the map layout of the game, with number assigned to each *Room[#]* gameobject in unity. The obstacles has been removed from this picture in order to keep it clear! The red outline design the room, each room has a small room called *closet*. There is two corridor in the middle, one that lead to a dead-end. There is few obstacles in each room, to force the player to move around it, instead of straight line in the room.    
![Map Layout](/map-layout.png)
    
    
## Obstacles    
The obstacles is clearly defined by the color RED in the map layout. There is two type of obstacle, a 4-sided one and a triangular prism one. In all three (3) rooms there is some obstacles.

## Scripts file explanation    
### Node.cs
This script contains variable and functions that will be used by both TileNode and PoVNode prefabs. The reason for a single file for both is to simplify refactoring and for simplicity sake. It contains `public bool rgtg = true;` which set to true by default, when it is true it will initialize the array instead of the list, otherwise it will use the list for pour pov. It contains 3 variables used by both regular grid and point of visibility, the variables are of type float named: `cost_so_far, heuristic_value, total_estimated_value`. There is also variable respective to each mode as explained below. *NOTE: the variables are starting by either rgtg (regular grid tile graph) or pov (point of visibility)*.    
These variables are used for regular grid tile graph method, notice that they all start with rgtg to facilitate their identification and their reason for existing.    
`public Node[] rgtg_neighbours = new Node[8];` this array will hold all current node neighbours up to maximum of 8. each value correspond to a position of the map, the mapping is upper = 0 upper right = 1, right = 2, lower right = 3, lower = 4, lower left = 5, left = 6, and upper left = 7. This array will be use in our Pathfinding script in [Pathfinding](###Pathfinding.cs) in the CreateNeighbours(Node node) function.
* void Start()
    * `GetComponent<Renderer>().enabled = false;` To hide the rendering of the nodes from our map.
* public void TurnNodeVisible()
    * `GetComponent<Renderer>().enabled = true;` To enable the visibility of the nodes on the map.
* public void TurnNodeInvisible()
    * * `GetComponent<Renderer>().enabled = false;` To enable the visibility of the nodes on the map.
* public void ResetValue()
    * function to reset cost-so-far, heuristic, and total estimate value to zero.
* Getter and setter for our three (3) float (`cost_so_far, heuristic_value, total_estimated_value`) in our file.
* public int Compare(Node node_a, Node node_b)
    * `int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);` To compare the estimated cost from one node to another, again based on slides information of A* algorithm. CompareTo() method is a C# method.
    * return either the `result` or `node_a.heuristic_value.CompareTo(node_b.heuristic_value)` as integer value.    

### Pathfinding.cs
This file contains the scripts to create connection and to calculate the pathfinding algorithm for the *regular grid (Tile) graph*. It will stores all available nodes in a `List`, the list variable are shown like this:`public List<TileBaseNode> [name] = new List<TileBaseNode>();`. Where the *name* is the variable name of the list. We also have a single end node which is named `public TileBaseNode target_tile_node` which will contains the target node of this algorithm. There is also a public boolean value to enable the mode: `public bool tile_base_mode = true` it is set to true since it is the default one that will be used for our AI. there is two integers for rooms as well, it will allows us to select which room will be the target. 

* void CreateTile()
    * function to create tilemap and link all neighbours nodes to each other. inside this function, it will call another function called **CreateNeighbours** which will create the neighbours for each nodes (can contains up to 8 neighbours).    
    
* void CreateNeighbours(TileBaseNode node)
    * function to connect each neighbours of our Tile Base Graph, it will check all 8 possible connection which are `upper, upper_right, right, lower, lower_right, left, lower_left`. More information for these enumerator in **TileBaseNode.cs**. `        node.neighbours[(int)TileBaseNode.Neighbours.[Neighbours]] = tile_base_node_list.Find(n => (n.transform.position - node.transform.position) == (new Vector3([value], 0, [value])));` This will check to see if there is any neighbouring node that correspond to our requirement, and if it found one it will return with its position value as n,the value in square bracket correspond to one of these three possible value which are `tilebase.x`, `-tilebase.x` or `0` and for the second one, we replace x by z. The `[Neighbours]` correspond to the enumerator position that can be found in **TileBaseNode.cs**, and format it as integer value as well. Then we create an anonymous function which is Lambda Expression, to return a node position that satisfy the condition.    

* void ClearTile()
    * function to clear all status of node and then call the `ResetPathfindingValue` function of the **TileBaseNode.cs** to reset its cost and value.
    

## Sources 
The source for 3d penguin: https://free3d.com/3d-model/emperor-penguin-601811.html
