using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingletonManager<TilemapManager>
{
    [SerializeField]
    private Tilemap walkableTilemap;

    [SerializeField]
    private Tilemap overlayTilemap;

    [SerializeField]
    private Tilemap[] unreachableTilemap;

    [SerializeField]
    private Sprite placeholderTileSprite;

    public Tilemap WalkableTilemap => walkableTilemap;
    public Tilemap OverlayTilemap => overlayTilemap;
    public Tilemap[] UnReachableTilemap => unreachableTilemap;
    public Sprite PlaceholderTileSprite => placeholderTileSprite;
    private Pathfinding pathfinding;
    public Tilemap PathfindingTilemap => walkableTilemap;

    private void Start()
    {
        pathfinding = new Pathfinding(this);
    }

    public List<Node> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        return pathfinding.FindPath(startPosition, endPosition);
    }

    public Node FindNode(Vector3 position)
    {
        return pathfinding.FindNode(position);
    }

    public bool CanWalkAtTile(Vector3Int tilePosition)
    {
        return walkableTilemap.HasTile(tilePosition) && !IsItUnreachableTailmap(tilePosition);
    }

    public bool IsItUnreachableTailmap(Vector3Int tilePostion)
    {
        foreach (var tilemap in unreachableTilemap)
        {
            if (tilemap.HasTile(tilePostion))
                return true;
        }
        return false;
    }

    public bool IsTileValid(Vector3Int tilePostion)
    {
        if (!walkableTilemap.HasTile(tilePostion))
            return false;

        //Check for gameobject
        return IsBlockByGameObject(tilePostion);
    }

    private bool IsBlockByGameObject(Vector3Int tilePostion)
    {
        Vector2 center = walkableTilemap.CellToWorld(tilePostion) + walkableTilemap.cellSize * 0.5f;
        Vector2 tileSize = walkableTilemap.cellSize * 0.5f; // matches exact tile size
        float angle = 0f;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, tileSize, angle);

        foreach (var hit in hits)
        {
            if (hit.tag != "Ground")
                return false;
        }
        return true;
    }
}
