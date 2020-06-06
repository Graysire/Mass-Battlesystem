using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A pathfinding node used by our A*pathfinding
public class PathNode
{
    //the x and y positions of this node in the grid (discounting grid size and actual world space coordinates)
    public readonly int posX;
    public readonly int posY;
    //whether this node is obstructed for movement
    public bool isMoveObstructed;
    //whether this node is obstructed for line of sight
    public bool isSightObstructed;
    //the defense bonus this tile gives if used as cover
    public int coverBonus = 0;

    //the pathing node preceding this node when calculating a path
    public PathNode prevNode;
    //the facing this node was entered from
    public int prevFacing;

    //the facing required to move to this square from the adjacent node, facing 0 is up and increases clockwise, facing 5 being up-left
    public int facing;

    //gCost is the distance from the start of the path to this node
    public int gCost;
    //hCost is the distance from the end of the path to this node
    public float hCost;
    //the total cost of moving to this node
    public int FCost { get { return gCost + (int) hCost; } }

    //Constructor for the pathfinding node
    public PathNode(bool isObstructed, int gridX, int gridY)
    {
        this.isMoveObstructed = isObstructed;
        posX = gridX;
        posY = gridY;
    }

}
