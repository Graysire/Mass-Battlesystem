using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class used for Pathfinding following the A* algorithm
public class Pathfinder
{
    PathGrid grid;

    //Vector3 used to debug pathfinding
    //Vector3 debugStart;
    //Vector3 debugTarget;

    public Pathfinder(PathGrid g)
    {
        grid = g;
    }


    //Finds the shortest path between two points, if one exists and puts the path into the grid
    public void FindPath(Vector3 startPos, Vector3 targetPos, int maxLength)
    {
        //convert the given positions into pathfinding nodes
        PathNode startNode = grid.WorldToNode(startPos);
        PathNode targetNode = grid.WorldToNode(targetPos);

        //reset gCost of the starting Node
        startNode.gCost = 0;
        //sets hCost of starting Node
        setHCost(startNode, targetNode, true);

        if (startNode == null)
        {
            Debug.Log("Starting Tile Out of Bounds");
            return;
        }
        else if (targetNode == null)
        {
            Debug.Log("Target Tile Out of Bounds");
            return;
        }
        else if (startNode == targetNode)
        {
            Debug.Log("Starting Tile and Target Tile are identical");
            return;
        }
        //checks if the target tile is farther than the max length of the path
        else if (startNode.hCost > maxLength)
        {
            Debug.Log("Target Tile out of movement range");
            return;
        }

        //list of pathing nodes that have not been checked yet
        List<PathNode> OpenList = new List<PathNode>();
        //set of pathing nodes that have been checked
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

        //add the first node to the unchecked nodes
        OpenList.Add(startNode);

        //while there are unchecked nodes, keep checking
        while (OpenList.Count > 0)
        {
            //start by looking at the first node
            PathNode currentNode = OpenList[0];
            //comapre to every other node
            for (int i = 0; i < OpenList.Count; i++)
            {
                //if the total cost of a node is lower, or the total cost is equal but node is closer to the target
                if (OpenList[i].FCost < currentNode.FCost || currentNode.FCost == OpenList[i].FCost && OpenList[i].hCost < currentNode.hCost)
                {
                    //that node becomes the new current node
                    currentNode = OpenList[i];
                }
            }
            //remove node from the unchecked nodes
            OpenList.Remove(currentNode);
            //add the node to the checked nodes
            ClosedList.Add(currentNode);

            //if the current node is the target
            if (currentNode == targetNode)
            {
                //startNode.isMoveObstructed = false;
                //targetNode.isMoveObstructed = true;
                //create a list to contain the final path
                List<PathNode> finalPath = new List<PathNode>();
                //go backwards from the current node until reaching the starting node
                while (currentNode != startNode)
                {
                    //add each node to the final path and look at the previous node
                    finalPath.Add(currentNode);
                    currentNode = currentNode.prevNode;
                }
                //add the starting node for debug purposes
                finalPath.Add(currentNode);

                //reverse the final path so that it goes from start to end, rather than end to start
                finalPath.Reverse();
                //send the final apth to the grid
                grid.finalPath = finalPath;

                return;
            }

            //look at each adjacent node
            foreach (PathNode adjacentNode in grid.GetAdjacentNodes(currentNode))
            {
                //if it has already been checked or is obstructed, skip it
                if (adjacentNode.isMoveObstructed || ClosedList.Contains(adjacentNode))
                {
                    continue;
                }
                else
                {
                    //if the adjacent node is more than 1 tile farther from the start than the current node
                    //or if the unchecked nodes list does not contain the adjacent node
                    if (!OpenList.Contains(adjacentNode) || currentNode.gCost + 1 < adjacentNode.gCost)
                    {
                        //the adjacent node's distance from the start node along this path is this node's distance + 1
                        adjacentNode.gCost = currentNode.gCost + 1;
                        //calculate the adjacent node's distance from the target using manhatten distance
                        //adjacentNode.hCost = Mathf.Abs(adjacentNode.posX - targetNode.posX) + Mathf.Abs(adjacentNode.posY - targetNode.posY);
                        //calculate the adjacent node's distance from the target using pythagorean theorem rounding down
                        setHCost(adjacentNode, targetNode, true);
                        //set the current node as the predecessor of the adjacent node
                        adjacentNode.prevNode = currentNode;
                        //if the unchecked nodes list does not contain the adjacent node and the adjacent node is not too far from the start, add it
                        if (!OpenList.Contains(adjacentNode) && adjacentNode.gCost <= maxLength)
                        {
                            OpenList.Add(adjacentNode);
                        }
                    }
                }
            }
        }
        return;
    }

    //Finds the shortest path between two points where turning counts as movement, if one exists and puts the path into the grid
    public void FindFormationPath(Vector3 startPos, Vector3 targetPos, float maxLength, int startFacing)
    {
        //convert the given positions into pathfinding nodes
        PathNode startNode = grid.WorldToNode(startPos);
        PathNode targetNode = grid.WorldToNode(targetPos);

        //reset gCost of the starting Node
        startNode.gCost = 0;
        //sets hCost of starting Node
        setHCost(startNode, targetNode, false);
        //sets the starting facing
        startNode.prevFacing = startFacing;

        if (startNode == null)
        {
            Debug.Log("Starting Tile Out of Bounds");
            return;
        }
        else if (targetNode == null)
        {
            Debug.Log("Target Tile Out of Bounds");
            return;
        }
        else if (startNode == targetNode)
        {
            Debug.Log("Starting Tile and Target Tile are identical");
            return;
        }
        //checks if the target tile is farther than the max length of the path
        else if (startNode.hCost > maxLength)
        {
            Debug.Log("Target Tile out of movement range");
            return;
        }

        //list of pathing nodes that have not been checked yet
        List<PathNode> OpenList = new List<PathNode>();
        //set of pathing nodes that have been checked
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

        //add the first node to the unchecked nodes
        OpenList.Add(startNode);

        //while there are unchecked nodes, keep checking
        while (OpenList.Count > 0)
        {
            //start by looking at the first node
            PathNode currentNode = OpenList[0];
            //compare to every other node
            //string debug ""; //declares debug string
            for (int i = 0; i < OpenList.Count; i++)
            {
                //if the total cost of a node is lower, or the total cost is equal but node is closer to the target
                if (OpenList[i].FCost < currentNode.FCost || currentNode.FCost == OpenList[i].FCost && OpenList[i].hCost < currentNode.hCost)
                {
                    //that node becomes the new current node
                    currentNode = OpenList[i];
                }
                //debug records each node in the open list and its cost
                //debug += "Node: " + OpenList[i].posX + "," + OpenList[i].posY + " Cost: " + OpenList[i].gCost + " " + OpenList[i].hCost + "\n";
            }
            //notes the node selected to be the next currentNode and prints the debug message
            //debug = "CURRENT: " + currentNode.posX + "," + currentNode.posY + " Cost: " + currentNode.gCost + " " + currentNode.hCost + "\n" + debug;
            //Debug.Log(debug);
            //remove node from the unchecked nodes
            OpenList.Remove(currentNode);
            //add the node to the checked nodes
            ClosedList.Add(currentNode);

            //if the current node is the target
            if (currentNode == targetNode)
            {
                //logs the movement cost
                //Debug.Log("Movement Cost: " + targetNode.gCost);

                startNode.isMoveObstructed = false;
                targetNode.isMoveObstructed = true;
                //create a list to contain the final path
                List<PathNode> finalPath = new List<PathNode>();
                //go backwards from the current node until reaching the starting node
                while (currentNode != startNode)
                {
                    //add each node to the final path and look at the previous node
                    finalPath.Add(currentNode);
                    currentNode = currentNode.prevNode;
                }
                //add the starting node for debug purposes
                finalPath.Add(currentNode);

                //reverse the final path so that it goes from start to end, rather than end to start
                finalPath.Reverse();
                //send the final apth to the grid
                grid.finalPath = finalPath;
                grid.pathCost = targetNode.gCost;

                return;
            }

            //look at each adjacent node
            foreach (PathNode adjacentNode in grid.GetAdjacentNodes(currentNode))
            {
                //if it has already been checked or is obstructed, skip it
                if (adjacentNode.isMoveObstructed || ClosedList.Contains(adjacentNode))
                {
                    continue;
                }
                else
                {
                    //calculate the facing costs assuming both clockwise turning and counter-clockwise turning
                    int facingCost = Mathf.Min(Mathf.Abs(adjacentNode.facing - currentNode.prevFacing), 6 - Mathf.Abs(adjacentNode.facing - currentNode.prevFacing));

                    //if the adjacent node is more than 1 tile farther from the start than the current node
                    //or if the unchecked nodes list does not contain the adjacent node
                    if (!OpenList.Contains(adjacentNode) || currentNode.gCost + 1 + facingCost < adjacentNode.gCost)
                    {
                        //the adjacent node's distance from the start node along this path is this node's distance + 1
                        adjacentNode.gCost = currentNode.gCost + 1 +facingCost;
                        //calculate the adjacent node's distance from the target using manhatten distance
                        //adjacentNode.hCost = Mathf.Abs(adjacentNode.posX - targetNode.posX) + Mathf.Abs(adjacentNode.posY - targetNode.posY);
                        //calculate the adjacent node's distance from the target using pythagorean theorem rounding down
                        setHCost(adjacentNode, targetNode, false);
                        //set the current node as the predecessor of the adjacent node
                        adjacentNode.prevNode = currentNode;
                        //set the facing as the predeccesor
                        adjacentNode.prevFacing = adjacentNode.facing;
                        //if the unchecked nodes list does not contain the adjacent node and the adjacent node is not too far from the start, add it
                        if (!OpenList.Contains(adjacentNode) && adjacentNode.gCost <= maxLength)
                        {
                            OpenList.Add(adjacentNode);
                        }
                    }
                }
            }
        }
        Debug.Log("Movement out of Range");
        return;
    }

    //Finds the distance between two points
    public int FindDistance(Vector3 startPos, Vector3 targetPos)
    {
        //convert the given positions into pathfinding nodes
        PathNode startNode = grid.WorldToNode(startPos);
        PathNode targetNode = grid.WorldToNode(targetPos);

        //reset gCost of the starting Node
        startNode.gCost = 0;
        //sets hCost of starting Node
        setHCost(startNode, targetNode, false);

        if (startNode == null)
        {
            Debug.Log("Starting Tile Out of Bounds");
            return -1;
        }
        else if (targetNode == null)
        {
            Debug.Log("Target Tile Out of Bounds");
            return -1;
        }
        else if (startNode == targetNode)
        {
            Debug.Log("Starting Tile and Target Tile are identical");
            return 0;
        }

        //list of pathing nodes that have not been checked yet
        List<PathNode> OpenList = new List<PathNode>();
        //set of pathing nodes that have been checked
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

        //add the first node to the unchecked nodes
        OpenList.Add(startNode);

        //while there are unchecked nodes, keep checking
        while (OpenList.Count > 0)
        {
            //start by looking at the first node
            PathNode currentNode = OpenList[0];
            //compare to every other node
            //string debug ""; //declares debug string
            for (int i = 0; i < OpenList.Count; i++)
            {
                //if the total cost of a node is lower, or the total cost is equal but node is closer to the target
                if (OpenList[i].FCost < currentNode.FCost || currentNode.FCost == OpenList[i].FCost && OpenList[i].hCost < currentNode.hCost)
                {
                    //that node becomes the new current node
                    currentNode = OpenList[i];
                }
                //debug records each node in the open list and its cost
                //debug += "Node: " + OpenList[i].posX + "," + OpenList[i].posY + " Cost: " + OpenList[i].gCost + " " + OpenList[i].hCost + "\n";
            }
            //notes the node selected to be the next currentNode and prints the debug message
            //debug = "CURRENT: " + currentNode.posX + "," + currentNode.posY + " Cost: " + currentNode.gCost + " " + currentNode.hCost + "\n" + debug;
            //Debug.Log(debug);
            //remove node from the unchecked nodes
            OpenList.Remove(currentNode);
            //add the node to the checked nodes
            ClosedList.Add(currentNode);

            //if the current node is the target
            if (currentNode == targetNode)
            {
                //return the distance from the start node to ending node
                return (int) targetNode.gCost;
            }

            //look at each adjacent node
            foreach (PathNode adjacentNode in grid.GetAdjacentNodes(currentNode))
            {
                //if it has already been checked skip it
                if (ClosedList.Contains(adjacentNode))
                {
                    continue;
                }
                else
                {

                    //if the adjacent node is more than 1 tile farther from the start than the current node
                    //or if the unchecked nodes list does not contain the adjacent node
                    if (!OpenList.Contains(adjacentNode) || currentNode.gCost + 1 < adjacentNode.gCost)
                    {
                        //the adjacent node's distance from the start node along this path is this node's distance + 1
                        adjacentNode.gCost = currentNode.gCost + 1;
                        //calculate the adjacent node's distance from the target using manhatten distance
                        //adjacentNode.hCost = Mathf.Abs(adjacentNode.posX - targetNode.posX) + Mathf.Abs(adjacentNode.posY - targetNode.posY);
                        //calculate the adjacent node's distance from the target using pythagorean theorem rounding down
                        setHCost(adjacentNode, targetNode, false);
                        //if the unchecked nodes list does not contain the adjacent node and the adjacent node is not too far from the start, add it
                        if (!OpenList.Contains(adjacentNode))
                        {
                            OpenList.Add(adjacentNode);
                        }
                    }
                }
            }
        }
        Debug.Log("Movement out of Range");
        return -1;
    }

    //Returns true if the straight line between two points is unobstructed, false otherwise
    public bool FindSightPath(Vector3 startPos, Vector3 targetPos, int maxLength)
    {
        //convert the given positions into pathfinding nodes
        PathNode startNode = grid.WorldToNode(startPos);
        PathNode targetNode = grid.WorldToNode(targetPos);

        //resets gCost of startNode to 0
        startNode.gCost = 0;
        //reserts hCost of startNode to be the distance to the target node
        setHCost(startNode, targetNode, true);

        if (startNode == null)
        {
            Debug.Log("Starting Tile Out of Bounds");
            return false;
        }
        else if (targetNode == null)
        {
            Debug.Log("Target Tile Out of Bounds");
            return false;
        }
        else if (startNode == targetNode)
        {
            Debug.Log("Starting Tile and Target Tile are identical");
            return true;
        }
        //checks if the target tile is farther than the max length of the path
        else if (startNode.hCost > maxLength)
        {
            Debug.Log("Target Tile out of sight/ability range");
            return false;
        }

        //list of pathing nodes that have not been checked yet
        List<PathNode> OpenList = new List<PathNode>();
        //set of pathing nodes that have been checked
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

        //add the first node to the unchecked nodes
        OpenList.Add(startNode);

        int strikes = 0;

        //debug string
        string debug = "";

        //while there are unchecked nodes, keep checking
        while (OpenList.Count > 0)
        {
            //start by looking at the first node
            PathNode currentNode = OpenList[0];
            //comapre to every other node
            for (int i = 0; i < OpenList.Count; i++)
            {
                //if the total cost of a node is lower, or the total cost is equal but node is closer to the target
                if (OpenList[i].hCost < currentNode.hCost || OpenList[i].hCost == currentNode.hCost && !OpenList[i].isSightObstructed)
                {
                    //that node becomes the new current node
                    currentNode = OpenList[i];
                }
            }
            //remove node from the unchecked nodes
            OpenList.Remove(currentNode);
            //add the node to the checked nodes
            ClosedList.Add(currentNode);


            debug += currentNode.posX + " " + currentNode.posY + " G:" + currentNode.gCost + " H:" + currentNode.hCost + " LoSBlock:" + currentNode.isSightObstructed + "\n";

            //if the path has backtracked, we are no longer finding a straight line
            if (currentNode.isSightObstructed)
            {
                strikes++;
                if (strikes >= 2)
                {
                    return false;
                }
            }

            //if the current node is the target
            if (currentNode == targetNode)
            {

                //create a list to contain the final path
                List<PathNode> finalPath = new List<PathNode>();
                //go backwards from the current node until reaching the starting node
                while (currentNode != startNode)
                {
                    //add each node to the final path and look at the previous node
                    finalPath.Add(currentNode);
                    currentNode = currentNode.prevNode;
                }
                //add the starting node for debug purposes
                finalPath.Add(currentNode);

                //reverse the final path so that it goes from start to end, rather than end to start
                finalPath.Reverse();
                //send the final apth to the grid
                grid.finalPath = finalPath;

                Debug.Log(debug);
                return true;
            }

            //look at each adjacent node
            foreach (PathNode adjacentNode in grid.GetAdjacentNodes(currentNode))
            {
                //if it has already been checked or is obstructed, skip it
                if (currentNode.isSightObstructed || ClosedList.Contains(adjacentNode))
                {
                    continue;
                }
                else
                {
                    //if the adjacent node is more than 1 tile farther from the start than the current node
                    //or if the unchecked nodes list does not contain the adjacent node
                    if (!OpenList.Contains(adjacentNode) || currentNode.gCost + 1 < adjacentNode.gCost)
                    {
                        //the adjacent node's distance from the start node along this path is this node's distance + 1
                        adjacentNode.gCost = currentNode.gCost + 1;
                        //calculate the adjacent node's distance from the target using manhatten distance
                        //adjacentNode.hCost = Mathf.Abs(adjacentNode.posX - targetNode.posX) + Mathf.Abs(adjacentNode.posY - targetNode.posY);
                        //calculate the adjacent node's distance from the target using pythagorean theorem rounding down
                        setHCost(adjacentNode, targetNode, false);
                        //set the current node as the predecessor of the adjacent node
                        adjacentNode.prevNode = currentNode;
                        //if the unchecked nodes list does not contain the adjacent node and the adjacent node is not too far from the start, add it
                        if (!OpenList.Contains(adjacentNode) && adjacentNode.gCost <= maxLength)
                        {
                            OpenList.Add(adjacentNode);
                        }
                    }
                }
            }
        }
        Debug.Log(debug);
        //Debug.Log("No possible path to reach target");
        return false;
    }

    //sets the HCost of startNode to the distance between startNode and TargetNode, optionally rounds it to an integer
    public void setHCost(PathNode startNode, PathNode targetNode, bool makeInt)
    {
        if (makeInt)
        {
            startNode.hCost = (int) Mathf.Sqrt(Mathf.Pow(startNode.posX - targetNode.posX, 2f) + Mathf.Pow(startNode.posY - targetNode.posY, 2f));
        }
        else
        {
            startNode.hCost = Mathf.Sqrt(Mathf.Pow(startNode.posX - targetNode.posX, 2f) + Mathf.Pow(startNode.posY - targetNode.posY, 2f));

            //alternate method of determining hCost

            //Vector3 startPos = grid.NodeToWorld(startNode);
            //Vector3 targetPos = grid.NodeToWorld(targetNode);

            //startNode.hCost = Mathf.Sqrt(Mathf.Pow(startPos.x - targetPos.x, 2f) + Mathf.Pow(startPos.y - targetPos.y, 2f));
        }
    }
}
