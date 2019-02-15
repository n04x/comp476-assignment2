# comp476-assignment2    
## Personal information    
COMP476 Assignment #1: Advanced Game Development, Concordia University    
Presented by: Thomas Backs    
ID: 27554524    
***
## Introduction    
`//todo`

## Map Layout    
Below is a picture of the map layout of the game, with number assigned to each *Room[#]* gameobject in unity. The obstacles has been removed from this picture in order to keep it clear!    
![Map Layout](/map-layout.png)
    
    
## Obstacles    
The obstacles is clearly defined by the color RED in the map layout. There is two type of obstacle, a 4-sided one and a triangular prism one. In all three (3) rooms there is some obstacles.

## Scripts file explanation    
### Node.cs
This file contains the script for Nodes creation and linking all the nodes in our map. We have a enumerator called `Neighbours` that is responsible of identifying and linking all neighbouring nodes (up to 8). We also have three (3) float variable called `cost_so_far`, `heuristic_value`, and `total_estimated_cost`, these are variable needed in our calculation of A* pathfinding based on Slide #37 of Week 03.
* Start()
    * `GetComponent<Renderer>().enabled = false;` To hide the rendering of the node from our map.

## Sources 
The source for 3d penguin: https://free3d.com/3d-model/emperor-penguin-601811.html
