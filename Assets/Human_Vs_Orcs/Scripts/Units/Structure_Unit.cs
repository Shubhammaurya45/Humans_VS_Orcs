public class Structure_Unit : Units
{
    private BuildingProcess buildingProcess;
    public bool IsUnderConstruction => buildingProcess != null;

    private void Update()
    {
        if (IsUnderConstruction)
        {
            buildingProcess.Update();
        }
    }

    public void OnConstructionFinished() => buildingProcess = null;

    public void RegisterProcess(BuildingProcess process)
    {
        buildingProcess = process;
    }
}
