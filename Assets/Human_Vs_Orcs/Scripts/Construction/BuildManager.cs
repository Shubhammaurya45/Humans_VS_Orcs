using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class BuildManager : SingletonManager<BuildManager>
{
    private PlacementProcess placementProcess;
    public BuildActionSO buildAction;

    public ConstructionMenu constructionMenu;

    [SerializeField]
    private ConfirmationBar buildConfirmationBar;

    [SerializeField]
    private GameObject workerUnitPrefab;

    public GameObject WorkerPrefab => workerUnitPrefab;

    [SerializeField]
    private ParticleSystem constructionEffect;

    private int gold = 1000;
    private int wood = 1000;

    public int Gold => gold;
    public int Wood => wood;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (placementProcess != null)
        {
            placementProcess.Update();
        }
    }

    public bool IsPlacementProcessBegin()
    {
        if (placementProcess != null)
            return true;
        return false;
    }

    public void StartBuildProcess(BuildActionSO buildAction)
    {
        if (placementProcess != null)
            return;
        this.buildAction = buildAction;
        //constructionMenu.HideConstructionMenu();

        //GameManager.Instance.cameraController.lockCamera = true;
        placementProcess = new PlacementProcess(buildAction);

        placementProcess.CreatePlacementOutline();
        buildConfirmationBar.ShowConfirmationBar(buildAction.GoldCost, buildAction.WoodCost);
        buildConfirmationBar.SetupHook(ConfirmBuildPlacement, CancelBuildPlacement);
    }

    private void ConfirmBuildPlacement()
    {
        //Check player has enough resources
        if (!CheckEnoughResources(placementProcess.GoldCost, placementProcess.WoodCost))
        {
            Debug.Log("Not Enough Resorces");
            return;
        }

        //place the outline
        if (placementProcess.TryFinalizePlacement(out Vector3 buildPosition))
        {
            ReduceResources(placementProcess.GoldCost, placementProcess.WoodCost);
            buildConfirmationBar.HideConfirmationBar();

            new BuildingProcess(placementProcess.BuildAction, buildPosition, constructionEffect);
            placementProcess = null;
            GameManager.Instance.cameraController.lockCamera = false;
        }
    }

    private void CancelBuildPlacement()
    {
        Debug.Log("CancelBuildProcess");
        buildConfirmationBar.HideConfirmationBar();
        placementProcess.Cleanup();
        placementProcess = null;
        GameManager.Instance.cameraController.lockCamera = false;
    }

    private bool CheckEnoughResources(int goldCost, int woodCost)
    {
        if (gold >= goldCost && wood >= woodCost)
            return true;

        return false;
    }

    private void ReduceResources(int goldCost, int woodCost)
    {
        gold -= goldCost;
        wood -= woodCost;
    }
}
