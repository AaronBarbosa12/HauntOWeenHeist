using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Pathfinding 
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int cellSize = 1;

    private Grid<PathNode> grid;
    private List<PathNode> openList; // Nodes to be queued up for searching
    private List<PathNode> closedList; // Nodes that have already been searched

    public Pathfinding()
    {
        Tilemap groundTileMap = GameObject.Find("Ground").GetComponent<Tilemap>();
        Tilemap terrainTileMap = GameObject.Find("Terrain").GetComponent<Tilemap>();
        groundTileMap.CompressBounds();
        terrainTileMap.CompressBounds();

        Vector3Int groundTileMapSize = groundTileMap.cellBounds.size;
        
        Func<Grid<PathNode>, int, int, PathNode> createGridObject = (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y);

        grid = new Grid<PathNode>(groundTileMapSize.x, groundTileMapSize.y, cellSize, new Vector2(0.5f, 0.5f), createGridObject, terrainTileMap);
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<Vector2> FindPath(Vector2 startWorldPosition, Vector2 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> pathNodes = FindPath(startX, startY, endX, endY);
        if (pathNodes == null)
        {
            return null;
        }
        else
        {
            List<Vector2> vectorPath = new List<Vector2>();
            foreach (PathNode pathNode in pathNodes)
            {
                Vector2 worldPosition = grid.GetWorldPosition(pathNode.x, pathNode.y);
                vectorPath.Add(worldPosition);
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        // Initialize Grid 
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        startNode.gCost = 0;
        startNode.hCost = CalculateNaiveDistance(startNode, endNode);
        startNode.CalculateFCost();

        /*
         * Keep going until:
         * current node == end node (end node is in openList)
         * OR openList is empty (no viable path)
         */
        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode) {
                // Reached Final Node 
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Cycle through neighbors of current node
            List<PathNode> neighbors = GetNeighborList(currentNode);

            foreach (PathNode neighborNode in neighbors)
            {
                if (closedList.Contains(neighborNode)) continue;

                Vector2 neighborWorldPosition = grid.GetWorldPosition(neighborNode.x, neighborNode.y);
                bool isWalkable = grid.isTileWalkable(neighborWorldPosition);
                if (!isWalkable){
                    closedList.Add(neighborNode);
                    continue;
                }
                int tentativeGCost = currentNode.gCost + CalculateNaiveDistance(currentNode, neighborNode);
                if (tentativeGCost < neighborNode.gCost){
                    // We've found a better path to this neighborNode 
                    // Update the path to it 
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateNaiveDistance(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    } 
                }
            
            }
        }
        // If we make it to this point, no valid path was found
        return null;
    }

    private int CalculateFCost(PathNode currentNode, PathNode neighborNode)
    {
        throw new NotImplementedException();
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        // Left Neighbors
        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y));
        }
        // Right Neighbors
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y));
        }
        // Top 
        if (currentNode.y + 1 < grid.GetHeight())
        {
            neighborList.Add(grid.GetGridObject(currentNode.x, currentNode.y + 1));
        }

        // Down
        if (currentNode.y - 1 >= 0)
        {
            neighborList.Add(grid.GetGridObject(currentNode.x, currentNode.y - 1));
        }
       
        return neighborList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathList = new List<PathNode> { endNode };

        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null){
            pathList.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        pathList.Reverse();
        return pathList;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) { 
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode; 
    }

    private int CalculateNaiveDistance(PathNode a, PathNode b)
    {
        /*
         * In a grid based coordinate system, diagonal moves cost more than straight 
         * moves: 
         *      cost_diag = sqrt(2) * cost_straight
         *
         * But moving diagonally once costs less than moving left once and up once:
         *      sqrt(2) < 1 + 1
         * 
         * More generally, if x = y, it's better to just move diagonally.
         *      sqrt(2)*x < x + y
         *      sqrt(2)*x < 2*x
         *      sqrt(2) < 2
         * 
         * We can take advantage of this in the case where x != y
         * 
         *              target
         *              /|         (1) Move as much as you can diagonally - min(x,y)
         *           d / |         (2) Then, move straight the rest of the way (|x - y)
         *            /  | y      
         *           /   |         
         *          /____|          
         *     source  x
         * 
         * Estimated cost = Amount we have to move diag + Amount we have to move straight
         * 
         * NOTE: This is all a consequence of a discrete coordinate system. In normal space, 
         * if we move 1 space at a 45 degree angle we end up at x, y = (sqrt(2), sqrt(2)). In 
         * contrast, in a grid based system we end up at (1,1). Therefore moving diagonally must have 
         * an additional cost compared to moving in a traditional coordinate system. Adding this 
         * cost leads to changes that can be counterintuituive.
         */
        int xDistance = Math.Abs(a.x - b.x);
        int yDistance = Math.Abs(a.y - b.y);

        int remaining = Math.Abs(xDistance - yDistance);

        //return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        return MOVE_STRAIGHT_COST * (xDistance + yDistance);
    }

}
