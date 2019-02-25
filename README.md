# comp476-assignment2    
## Personal information    
COMP476 Assignment #1: Advanced Game Development, Concordia University    
Presented by: Thomas Backs    
ID: 27554524    
***
## Introduction    
 In our games, there is 1200 regular grid tile graph nodes (*click [here](#regular-grid-tile-graph-map) to see the map*), and 88 point of visibility graph nodes (*click [here](#point-of-visibility-graph-map) to see the map*). In our code we will use the abbreviation for each in order to clearly identify which graph that variable belong to and to facilitate identification as well. for our regular grid tile graph, we use **rgtg** and for our point of visibility graph node we use **povg**. To be able to clearly follow the pathing and direction, there is a camera attached to the penguin GameObject that is shown in bottom right corner. On top right, there is some useful information about the current run.    

## Map Layout    
Below is a picture of the map layout of the game, with number assigned to each *Room[#]* gameobject in unity. The obstacles has been removed from this picture in order to keep it clear! The red outline design the room, each room has at least one small room called *closet*. There is two corridor in the middle, one that lead to a dead-end. There is few obstacles in each room, to force the player to move around it, instead of straight line in the room.    
![Map Layout](Screenshots/map-layout.png = 250x250)
    
    
## Obstacles    
The obstacles is clearly defined by the color RED in the map layout. There is two type of obstacle, a 4-sided one and a triangular prism one. In all three (3) rooms there is some obstacles.

## Scripts file explanation    
### Node.cs
This script contains variable and functions that will be used by both TileNode and PoVNode prefabs. The reason for a single file for both is to simplify refactoring and for simplicity sake. It contains `public bool rgtg = true;` which set to true by default, when it is true it will initialize the array instead of the list, otherwise it will use the list for pour pov. It contains 3 variables used by both regular grid and point of visibility, the variables are of type float named: `cost_so_far, heuristic_value, total_estimated_value`. There is also variable respective to each mode as explained below. *NOTE: the variables are starting by either rgtg (regular grid tile graph) or pov (point of visibility)*.    
These variables are used for regular grid tile graph method, notice that they all start with rgtg to facilitate their identification and their reason for existing.    
`public Node[] rgtg_neighbours = new Node[8];` this array will hold all current node neighbours up to maximum of 8. each value correspond to a position of the map, the mapping is upper = 0 upper right = 1, right = 2, lower right = 3, lower = 4, lower left = 5, left = 6, and upper left = 7. This array will be use in our Pathfinding script in [Pathfinding.cs](#Pathfindingcs) in the CreateNeighbours(Node node) function.
* void Start()
    * `GetComponent<Renderer>().enabled = false;` To hide the rendering of the nodes from our map.
* public void TurnNodeVisible()
    * `GetComponent<Renderer>().enabled = true;` To enable the visibility of the nodes on the map.
* public void TurnNodeInvisible()
    * * `GetComponent<Renderer>().enabled = false;` To enable the visibility of the nodes on the map.
* public void ResetValue()
    * Function to reset cost-so-far, heuristic, and total estimate value to zero.
* Getter and setter for our three (3) float (`cost_so_far, heuristic_value, total_estimated_value`) in our file.
* public int Compare(Node node_a, Node node_b)
    * `int result = node_a.total_estimated_value.CompareTo(node_b.heuristic_value);` To compare the estimated cost from one node to another, again based on slides information of A* algorithm. CompareTo() function is a C# function.
    * return either the `result` or `node_a.heuristic_value.CompareTo(node_b.heuristic_value)` as integer value.    
* There is also various getter and setter for our float values `cost_so_far, heuristic_value, total_estimated_value`, I did not add them in documentation because I find that getter and setter are self-explanatory.    

### Pathfinding.cs
This scripts will handle pathfinding calculation and pathing connection as well for both regular grid tile graph and point of visibility graph. `public AIBehaviour penguin_script` is needed to trace the path for the penguin game object. `Vector3 map_size` to hold the size of our game map, it is set in Start(). It also has `public enum Modes { DIJSKTRA, EUCLIDEAN, CLUSTER }` to determine the current pathfinding mode, the current mode: `public Modes current_mode = Modes.DIJSKTRA`. it also has a boolean `public bool rgtg_mode = true` to determine if we use regular grid tile graph or point of visibility.     
These variables  are used for regular grid tile graph function, notice that they all start with rgtg to facilitate their identification and their reason for existing.    
`Vector3 rgtg_node_size` determine the node size, the value is calculated and set in the Start() function below. `float rgtg_density` is the divisor for our map size to determine the number of tile to have which gives us an evenly divided node size throughout our map.
`public Node [type]_start_node;` determine our starting node, replace type is either rgtg or pov. `public Node [type]_target_node` determine our target node. `public List<Node> [type]_node_list = new List<Node>()` will contains *all* available node belonging to its type. `public List<Node> [type]_path_list = new List<Node>()` will contain all node that AI will *cross* as path. `public List<Node> [type]_open_list = new List<Node>()` will contains all *open* node yet to be visited. `public List<Node> [type]_closed_list = new List<Node>()` will contains all *closed* node that won't be used for our path, `public List<Node> [type]_closet#_nodes = new List<Node>()`, contains all nodes that is in a closet, there is 4 closets in our map click [here](#closet-map) to go to map screenshot with number. `public Node [type]_random_node` to pick a random node inside a closet to be set as our target. We use the Node instead of GameObject for our [Pathfinding.cs](#pathfindingcs) file, because we will need to access to many function and variable inside the [Node.cs](#nodecs) script and creating a holder for both GameObject AND Script for all our needs is useless since we mostly just going to need the script of each node to do our work.

### Penguin.cs    
This scripts will handle our Penguin AI movement behaviour by using the Steering Arrive with Align. It has these float variables named `max_velocity, max_rotation_velocity, max_acceleration, max_rotation_acceleration, time_to_target, current_rotation_velocity, current_velocity, distance_from_target, current_acceleration`. These are variables needed for our functions mentionned below. There is alos two Vector3 variables which are `direction_vector` and `player_distance`. These functions below are called inside the `Update()` functions of [Pathfinding.cs](#pathfindingcs).    
* public void Move(Vector3 target_position, bool target_node)    
    * This function will calculate the arrive movement toward the toward using the Steering approach mentioned in our slides. The parameters required for this are the `Vector3 target_position` and the `bool target_node`. If it is set to true, it will slow down toward the goal target, otherwise it will apply the normal velocity movement.  
* public bool Stop()
    * This function will force the penguin to stop its action. and reset the `current_velocity` to 0.0f if true. otherwise it will keep moving toward the target. It is called if the angle between its target and the penguin is over the threshold.
* public bool AlignTowardTarget()
    * This function will be called to align the penguin toward its target node to ensure it is moving in the right direction, it will correct its trajectory if the angle between the target and the penguin is over the threshold, if so, it will correct it's alignment and rotates the penguin toward the target.    

## Screenshot    
### Closet Map
![Closet Map](Screenshots/closet-map.png = 250x250)    
### Regular Grid Tile Graph Map
![Regular Grid Tile Map](Screenshots/rgtg-layout.png = 250x250)    
### Point of Visibility Graph Map
![Point of Visibility Map](Screenshots/povg-layout.png = 250x250)    
## Sources 
The source for 3d penguin: https://free3d.com/3d-model/emperor-penguin-601811.html
