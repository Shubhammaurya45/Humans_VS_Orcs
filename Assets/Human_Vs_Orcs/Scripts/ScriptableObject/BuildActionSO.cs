using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildAction", menuName = "Actions/BuildAction")]
public class BuildActionSO : ActionSO
{
    [Header("Prefabs")]
    [SerializeField]
    private Units workerUnitPrefab;

    [SerializeField]
    private Structure_Unit structurePrefab;

    [Header("Sprite")]
    [SerializeField]
    private Sprite placementSprite;

    [SerializeField]
    private Sprite foundationSprite;

    [SerializeField]
    private Sprite completionSprite;

    [SerializeField]
    private Sprite buildingDestroyed;

    [Header("Building Details")]
    [SerializeField]
    private Vector2Int buildingSize = new Vector2Int();

    [SerializeField]
    private float constructionTime;

    [SerializeField]
    private Vector3 highlightTilemapOffset = new Vector3();

    [Header("Construction Req")]
    [SerializeField]
    private int goldCost;

    [SerializeField]
    private int woodCost;

    public Units WorkerUnitPrefab => workerUnitPrefab;
    public Structure_Unit StructurePrefab => structurePrefab;

    public Sprite PlacementSprite => placementSprite;
    public Sprite FoundationSprite => foundationSprite;
    public Sprite CompletionSprite => completionSprite;
    public Sprite BuildingDestroyed => buildingDestroyed;
    public Vector2Int BuildingSize => buildingSize;
    public float ConstructionTime => constructionTime;
    public Vector3 HighlightTilemapOffset => highlightTilemapOffset;
    public int GoldCost => goldCost;
    public int WoodCost => woodCost;

    public override void Execute()
    {
        BuildManager.Instance?.StartBuildProcess(this);
    }
}
