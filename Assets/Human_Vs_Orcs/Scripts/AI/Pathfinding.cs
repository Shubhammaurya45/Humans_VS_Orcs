using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private TilemapManager tilemapManager;
    private Node[,] grid;
    public Node[,] Grid => grid;

    private int width;
    private int height;
    private Vector3Int gridOffset;

    public Pathfinding(TilemapManager tilemapManager)
    {
        this.tilemapManager = tilemapManager;
        tilemapManager.PathfindingTilemap.CompressBounds();
        var bounds = tilemapManager.PathfindingTilemap.cellBounds; // size of tilemap
        width = bounds.size.x; //Total number of cell in x
        height = bounds.size.y; //Total number of cell in y
        grid = new Node[width, height]; //Creates a null array of tilemap size
        gridOffset = tilemapManager.PathfindingTilemap.cellBounds.min;
        // Debug.Log(gridOffset);
        InitializeGrid();
    }

    //Fill the actual value in the grid
    private void InitializeGrid()
    {
        var cellSize = tilemapManager.PathfindingTilemap.cellSize;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Finding actual node position
                Vector3Int nodeLeftBottomPosition = new Vector3Int(
                    x + gridOffset.x,
                    y + gridOffset.y
                );
                bool isWalkable = tilemapManager.CanWalkAtTile(nodeLeftBottomPosition);
                //Storing it in node
                var node = new Node(nodeLeftBottomPosition, cellSize, isWalkable);
                grid[x, y] = node;
            }
        }
    }

    public Node FindNode(Vector3 position)
    {
        // Fit the to the cellsize
        Vector3Int flooredPosition = new Vector3Int(
            Mathf.FloorToInt(position.x),
            Mathf.FloorToInt(position.y)
        );
        int gridX = flooredPosition.x - gridOffset.x;
        int gridY = flooredPosition.y - gridOffset.y;

        //Store the position in grid
        if (gridX >= 0 && gridX < width && gridY >= 0 && gridY < height)
            return grid[gridX, gridY];

        return null;
    }

    public List<Node> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        Node startNode = FindNode(startPosition);
        Node endNode = FindNode(endPosition);

        if (startNode == null || endNode == null)
        {
            return new List<Node>();
        }

        List<Node> openList = new();
        HashSet<Node> closedList = new();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                var path = RetracePath(startNode, endNode);
                ResetNode(openList, closedList);
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neigbhour in GetNeighbours(currentNode))
            {
                if (!neigbhour.isWalkable || closedList.Contains(neigbhour))
                    continue;

                float tentativeG = currentNode.gCost + GetDistance(currentNode, neigbhour);

                if (tentativeG < neigbhour.gCost || !openList.Contains(neigbhour))
                {
                    neigbhour.gCost = tentativeG;
                    neigbhour.hCost = GetDistance(neigbhour, endNode);
                    neigbhour.fCost = neigbhour.gCost + neigbhour.hCost;
                    neigbhour.Parent = currentNode;

                    if (!openList.Contains(neigbhour))
                    {
                        openList.Add(neigbhour);
                    }
                }
            }
        }
        ResetNode(openList, closedList);
        return new List<Node>();
    }

    private Node GetLowestFCostNode(List<Node> openList)
    {
        Node lowestFCostNode = openList[0];

        foreach (var node in openList)
        {
            if (
                node.fCost < lowestFCostNode.fCost
                || (node.fCost == lowestFCostNode.fCost && node.hCost < lowestFCostNode.hCost)
            )
            {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neigbhours = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.x + x - gridOffset.x;
                int checkY = node.y + y - gridOffset.y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    var neigbhour = grid[checkX, checkY];
                    if (!neigbhour.isWalkable)
                        continue;
                    neigbhours.Add(neigbhour);
                }
            }
        }
        return neigbhours;
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.x - nodeB.x);
        int distanceY = Mathf.Abs(nodeA.y - nodeB.y);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Add(currentNode);
        path.Reverse();

        return path;
    }

    public void ResetNode(List<Node> openList, HashSet<Node> closeList)
    {
        foreach (var node in openList)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.fCost = 0;
        }

        foreach (var node in closeList)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.fCost = 0;
        }
        openList.Clear();
        closeList.Clear();
    }
}
