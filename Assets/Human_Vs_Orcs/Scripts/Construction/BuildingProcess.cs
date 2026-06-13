using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO buildAction;
    private Units workerUnit;
    private Animator workerUnitAnim;
    private float progresTime;
    private Structure_Unit structure;
    private bool isFinished=false;
    private ParticleSystem constructionEffect;

    public BuildingProcess(BuildActionSO buildAction,Vector3 placementPosition,Worker_Unit worker,ParticleSystem constructionEffectPrefab)
    {
        this.buildAction = buildAction;
        constructionEffect=constructionEffectPrefab;
        StartConstruction(buildAction, placementPosition, worker);
        
        // this.constructionEffect=constructionEffectPrefab;
    }

    

    public void Update()
    {
        if(isFinished) return;

        progresTime+=Time.deltaTime;

        if (IsConstructionFinished) FinishConstruction();
    }

    private bool IsConstructionFinished=>progresTime>=buildAction.ConstructionTime;

    private void StartConstruction(BuildActionSO buildAction, Vector3 placementPosition, Worker_Unit worker)
    {
        structure = Object.Instantiate(buildAction.StructurePrefab);
        structure.SpriteRenderer.sprite = buildAction.FoundationSprite;
        structure.transform.position = placementPosition;
        structure.RegisterProcess(this);
        workerUnit = BuildManager.Instance.SpwanWorkerUnit(buildAction.WorkerUnitPrefab, placementPosition);
        workerUnitAnim = workerUnit.GetComponentInChildren<Animator>();
        worker.SetTask(UnitTask.Build, workerUnitAnim);

        constructionEffect=Object.Instantiate(constructionEffect,placementPosition,Quaternion.identity,structure.transform);
        constructionEffect.Play();
        
        
    }

    public void  FinishConstruction()
    {
        isFinished=true;
        structure.SpriteRenderer.sprite=buildAction.CompletionSprite;
        constructionEffect.Stop();
        workerUnit.SetTask(UnitTask.None,workerUnitAnim);
        BuildManager.Instance.RemoveWorkerUnit(workerUnit);
        structure.OnConstructionFinished();
        Object.Destroy(constructionEffect.gameObject);
        
    } 

    





    
}