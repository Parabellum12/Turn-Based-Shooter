using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AstarPathing
{
    static int Move_Straight_Cost = 10;
    static int Move_Diagonal_Cost = 14;
    
    /*
    public static IEnumerator returnPath(Vector2Int start, Vector2Int end, GridClass<World_Handler_Script.WorldBuildTile> grid, bool adjacentOnly, System.Action<Vector2Int[]> callback)
    {
        return returnPathPriv(start, end, grid, adjacentOnly, callback);
        //return null;
    }
    */
    public static IEnumerator returnPath(Vector2Int start, Vector2Int end, GridClass<World_Handler_Script.WorldBuildTile> grid, bool adjacentOnly, List<Vector2Int> restrictedTiles, System.Action<Vector2Int[]> callback = null)
    {
       
        if (!grid.inBounds(end.x, end.y) ||!grid.getGridObject(end.x, end.y).IsWalkable())
        {
            callback.Invoke(null);
            yield break;
        }
        World_Handler_Script.WorldBuildTile startNode = grid.getGridObject(start.x, start.y);
        World_Handler_Script.WorldBuildTile endNode = grid.getGridObject(end.x, end.y);


        List<World_Handler_Script.WorldBuildTile> openList = new List<World_Handler_Script.WorldBuildTile>();
        openList.Add(startNode);
        List<World_Handler_Script.WorldBuildTile> closedList = new List<World_Handler_Script.WorldBuildTile>();

        for (int i = 0; i < grid.getWidth(); i++)
        {
            for (int j = 0; j < grid.getHeight(); j++)
            {
                World_Handler_Script.WorldBuildTile currentNode = grid.getGridObject(i, j);
                currentNode.gCost = int.MaxValue;
                currentNode.calcFCost();
                currentNode.LastNode = null;
            }
        }
 
        startNode.gCost = 0;
        startNode.hCost = calcDistCost(startNode, endNode);
        startNode.calcFCost();
        bool needToUpdateTime = true;
        float startTime = Time.realtimeSinceStartup;
        while (openList.Count > 0)
        {
            if (needToUpdateTime)
            {
                //Debug.Log("update");
                needToUpdateTime = false;
                startTime = Time.realtimeSinceStartup;
            }
            World_Handler_Script.WorldBuildTile currentNode = getLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                //end
                callback.Invoke(getReturnPath(grid.getGridObject(end.x, end.y)));
                yield break;
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (World_Handler_Script.WorldBuildTile curTile in getNodesAround(currentNode, grid, adjacentOnly, restrictedTiles))
            {
                if (closedList.Contains(curTile))// || !validTile(curTile.getXY(), currentNode.getXY(), adjacentOnly))
                {
                    continue;
                }
                if (!curTile.IsWalkable())
                {
                    closedList.Add(curTile);
                    continue;
                }

                int tempGCost = currentNode.gCost + calcDistCost(currentNode, curTile);
                if (tempGCost < curTile.gCost)
                {
                    curTile.gCost = tempGCost;
                    curTile.LastNode = currentNode;
                    curTile.hCost = calcDistCost(curTile, endNode);
                    curTile.calcFCost();

                    if (!openList.Contains(curTile))
                    {
                        openList.Add(curTile);
                    }
                }
            }
            if (Mathf.Abs(Time.realtimeSinceStartup - startTime) > 0.01)
            {
                needToUpdateTime = true;
                //Debug.Log("set to update");
                yield return null;
            }
            
        }

        callback.Invoke(null);
        yield break;

    }

    private static List<World_Handler_Script.WorldBuildTile> getNodesAround(World_Handler_Script.WorldBuildTile node, GridClass<World_Handler_Script.WorldBuildTile> grid, bool adjacentOnly, List<Vector2Int> restrictedTiles)
    {
        List<World_Handler_Script.WorldBuildTile> nodes = new List<World_Handler_Script.WorldBuildTile>();
        for (int i = node.getXY().x-1; i <= node.getXY().x+1; i++)
        {
            for (int j = node.getXY().y-1; j <= node.getXY().y+1; j++)
            {
                if (i >= 0 && i < grid.getWidth() && j >= 0 && j < grid.getHeight())
                {
                    if (validTile(new Vector2Int(i,j), node.getXY(), adjacentOnly, restrictedTiles, grid))
                    {
                        nodes.Add(grid.getGridObject(i, j));
                    }
                }
            }
        }
        return nodes;
    }

    private static bool validTile(Vector2Int cur, Vector2Int caller, bool adjacentOnly, List<Vector2Int> restrictedTiles, GridClass<World_Handler_Script.WorldBuildTile> grid)
    {

        //Debug.Log("ValidTile Debug: " + cur.ToString() + " :: " + caller.ToString() + " :: " + adjacentOnly);
        if (restrictedTiles.Contains(cur))
        {
            return false;
        }
        if (adjacentOnly)
        {
            bool x = cur.x == caller.x;
            bool y = cur.y == caller.y;
            if (!x && !y)
            {
                return false;
            }
            else
            {
                return true;
            }    
        }
        else
        {
            if (diagonalValidMove(caller, cur, grid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private static bool diagonalValidMove(Vector2Int original, Vector2Int MoveToTile, GridClass<World_Handler_Script.WorldBuildTile> grid)
    {
        if (original.x == MoveToTile.x || original.y == MoveToTile.y)
        {
            return true;
        }



        if (original.x < MoveToTile.x && original.y < MoveToTile.y)
        {
            //original bottom left, moveto top right slash
            return checkSlash(original, MoveToTile, grid);
        }
        else if (original.x > MoveToTile.x && original.y > MoveToTile.y)
        {
            //moveto bottom left, original top right slash
            return checkSlash(MoveToTile, original, grid);
        }
        else if (original.x < MoveToTile.x && original.y > MoveToTile.y)
        {
            //original top left, moveto bottom right backslash
            return checkBackSlash(original, MoveToTile, grid);
        }
        else if (original.x > MoveToTile.x && original.y < MoveToTile.y)
        {
            //original bottom right, moveto top left backslash
            return checkBackSlash(MoveToTile, original, grid);
        }
        else
        {
            Debug.LogError("unknown tileOrientation");
            return false;
        }

        //need to check left and right diagonal seperatly, need to setup function to take bottom left to top right or bottom right to top left and check tiles between
    }


    private static bool checkSlash(Vector2Int bottomLeft, Vector2Int topRight, GridClass<World_Handler_Script.WorldBuildTile> grid)
    {
        Vector2Int left = new Vector2Int(bottomLeft.x, topRight.y);
        Vector2Int right = new Vector2Int(topRight.x, bottomLeft.y);
        if (grid.getGridObject(left.x, left.y).IsWalkable() && grid.getGridObject(right.x, right.y).IsWalkable())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool checkBackSlash(Vector2Int topLeft, Vector2Int bottomRight, GridClass<World_Handler_Script.WorldBuildTile> grid)
    {
        Vector2Int left = new Vector2Int(topLeft.x, bottomRight.y);
        Vector2Int right = new Vector2Int(bottomRight.x, topLeft.y);
        if (grid.getGridObject(left.x, left.y).IsWalkable() && grid.getGridObject(right.x, right.y).IsWalkable())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private static Vector2Int[] getReturnPath(World_Handler_Script.WorldBuildTile endNode)
    {
        List<World_Handler_Script.WorldBuildTile> pathAsNodes = new List<World_Handler_Script.WorldBuildTile>();
        pathAsNodes.Add(endNode);
        World_Handler_Script.WorldBuildTile current = endNode;
        while (current.LastNode != null)
        {
            pathAsNodes.Add(current.LastNode);
            current = current.LastNode;
        }
        Vector2Int[] pathAsCoords = new Vector2Int[pathAsNodes.Count];
        int count = 0;
        for (int i = pathAsNodes.Count - 1; i >= 0; i--)
        {
            pathAsCoords[count] = pathAsNodes[i].getXY();
            count++;
        }
        return pathAsCoords;
    }

    private static int calcDistCost(World_Handler_Script.WorldBuildTile nodeA, World_Handler_Script.WorldBuildTile nodeB)
    {
        int xDist = Mathf.Abs(nodeA.getXY().x - nodeB.getXY().x);
        int yDist = Mathf.Abs(nodeA.getXY().y - nodeB.getXY().y);
        int remaining = Mathf.Abs(xDist - yDist);
        return Move_Diagonal_Cost * Mathf.Min(xDist, yDist) + Move_Straight_Cost * remaining;
    }

    private static World_Handler_Script.WorldBuildTile getLowestFCostNode(List<World_Handler_Script.WorldBuildTile> openList)
    {
        World_Handler_Script.WorldBuildTile currentLowest = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].fCost < currentLowest.fCost)
            {
                currentLowest = openList[i];
            }
        }
        return currentLowest;
    }


}
