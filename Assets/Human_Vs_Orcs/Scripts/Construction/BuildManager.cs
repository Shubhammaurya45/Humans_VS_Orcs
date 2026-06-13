using Unity.Mathematics;
using UnityEngine;

public class BuildManager : SingletonManager<BuildManager>
{
    private PlacementProcess placementProcess;

    [SerializeField]
    private ConfirmationBar buildConfirmationBar;

    [SerializeField]
    private Transform workerUnitParent;

    [SerializeField]
    private ParticleSystem constructionEffect;

    private int gold = 1000;
    private int wood = 1000;

    public int Gold => gold;
    public int Wood => wood;

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
        GameManager.Instance.cameraController.lockCamera = true;
        placementProcess = new PlacementProcess(buildAction);
        placementProcess.CreatePlacementOutline();
        buildConfirmationBar.ShowConfirmationBar(buildAction.GoldCost, buildAction.WoodCost);
        buildConfirmationBar.SetupHook(ConfirmBuildPlacement, CancelBuildPlacement);
    }

    public Units SpwanWorkerUnit(Units workerUnit, Vector3 buildPostion)
    {
        Vector3 workerUnitSpwanPostion = buildPostion + new Vector3(-1, 0, 0);
        return Instantiate(
            workerUnit,
            workerUnitSpwanPostion,
            quaternion.identity,
            workerUnitParent
        );
    }

    public void RemoveWorkerUnit(Units workerUnit)
    {
        Destroy(workerUnit.gameObject);
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

            new BuildingProcess(
                placementProcess.BuildAction,
                buildPosition,
                (Worker_Unit)GameManager.Instance?.activeUnit,
                constructionEffect
            );
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
