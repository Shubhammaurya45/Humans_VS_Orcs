using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess 
{
    private BuildActionSO buildAction;
    private GameObject placementOutline;
    private Tilemap overlayTilemap;
    private Sprite placeholderTileSprite;
    private Vector3Int[] highlightPosition;
    private Vector2Int buildingSize;
    private Vector3 pivotPosition;
    private Vector3 lastOutlinePosition=Vector3.positiveInfinity;
    private static readonly Color validColor = new Color(0f, 1f, 0f, 0.4f);
    private static readonly Color invalidColor = new Color(1f, 0f, 0f, 0.8f); 
    private Tile validTile;
    private Tile invalidTile;
   
    [Header("Getter")]
    public BuildActionSO BuildAction=>buildAction;
    public int GoldCost=>buildAction.GoldCost;
    public int WoodCost=>buildAction.WoodCost;
    
    
    //Constructor
    public PlacementProcess (BuildActionSO buildAction)
    {
        this.buildAction=buildAction;
        buildingSize=buildAction.BuildingSize;
        //Set array size
        highlightPosition = new Vector3Int[buildingSize.x * buildingSize.y];

        overlayTilemap=TilemapManager.Instance.OverlayTilemap;
        placeholderTileSprite=TilemapManager.Instance.PlaceholderTileSprite;
        CreateTile();

     
    }
    public  void Update()
    {
        if (placementOutline != null)
        {
            HighlightTiles(placementOutline.transform.position);
        }
        PlacementOutlinePosition();
    }

    private void PlacementOutlinePosition()
    {
        if(placementOutline==null) return;
        
        if(GameManager.Instance.IsPointerOverUIElement()) return;

        Vector3 worldPosition=InputManager.Instance.inputHoldWorldPosition;

        if (worldPosition != Vector3.zero)
        {
            placementOutline.transform.position = SnapToGrid(worldPosition);
        }
    }

    //Create a init gameobject of  construction
    public void CreatePlacementOutline()
    {
        placementOutline=new GameObject("PlacementOutline");
        var renderer=placementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder=999;
        renderer.color=new Color(1,1,1,0.5f);
        renderer.sprite=buildAction.PlacementSprite;
    }

    //Place a gameobject excatly at grid  
    private Vector3 SnapToGrid(Vector3 worldPosition)
    {
        return new Vector3(Mathf.FloorToInt(worldPosition.x),Mathf.FloorToInt(worldPosition.y),0);
    }

    private void CreateTile()
    {
        validTile = ScriptableObject.CreateInstance<Tile>();
        validTile.sprite = placeholderTileSprite;
        validTile.color=validColor;

        invalidTile = ScriptableObject.CreateInstance<Tile>();
        invalidTile.sprite = placeholderTileSprite;
        invalidTile.color=invalidColor;
    }

    public void HighlightTiles(Vector3 outlinePosition)
    {
        if (outlinePosition == lastOutlinePosition) return; 

        lastOutlinePosition = outlinePosition;
        pivotPosition = outlinePosition + buildAction.HighlightTilemapOffset;

        ClearHighlight();

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                var pos=new Vector3Int((int)pivotPosition.x + x, (int)       pivotPosition.y + y, 0);
                highlightPosition[x + y * buildingSize.x] = pos;
                overlayTilemap.SetTile(pos, GetTile(pos));  
            }
        }
    }
    private Tile GetTile(Vector3Int tilePostion)
    {
        if(!TilemapManager.Instance.IsTileValid(tilePostion)) return invalidTile;

        return validTile;  
    }
      private bool IsPlacementAreaValid()
    {
         foreach (var tilePosition in highlightPosition)
        {
            if (!TilemapManager.Instance.IsTileValid(tilePosition)) return false;
        }

        return true;
    }

    public bool TryFinalizePlacement(out Vector3 buildPosition)
    {
        if (IsPlacementAreaValid())
        {
            FinalizePlacement(out buildPosition);
            return true; 
        }
        Debug.Log("Invalid Placement area");
        buildPosition=Vector3.zero;
        return false;
    }

    private void FinalizePlacement(out Vector3 buildPosition)
    {
        ClearHighlight();
        buildPosition = placementOutline.transform.position;
        Object.Destroy(placementOutline);
        
    }

   
    //Clear highlighted tiles
    private void ClearHighlight()
    {
        foreach(var tilePosition in highlightPosition)
        {
            overlayTilemap.SetTile(tilePosition,null);
        }
    }

    public void Cleanup()
    {
        ClearHighlight();
        Object.Destroy(placementOutline);
        Object.Destroy(validTile);
        Object.Destroy(invalidTile);
    }
  
    
    

    

    






}
