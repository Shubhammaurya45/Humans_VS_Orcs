using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool isWalkable;
    public float gCost;
    public float hCost;
    public float fCost;
    public Node Parent;

    public Node(Vector3Int position, Vector3 cellsize, bool isWalkable)
    {
        x = position.x;
        y = position.y;
        Vector3 halfCellSize = cellsize / 2;
        var nodeCenterPosition = position + halfCellSize;
        centerX = nodeCenterPosition.x;
        centerY = nodeCenterPosition.y;

        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }
}
