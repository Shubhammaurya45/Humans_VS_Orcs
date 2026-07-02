using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO buildAction;
    private Worker_Unit workerUnit;
    private Animator workerUnitAnim;
    private float progresTime;
    private Structure_Unit structure;
    private bool isConstructionFinished = false;
    private ParticleSystem constructionEffect;

    public BuildingProcess(
        BuildActionSO buildAction,
        Vector3 placementPosition,
        ParticleSystem constructionEffectPrefab
    )
    {
        this.buildAction = buildAction;
        StartConstruction(buildAction, placementPosition, constructionEffectPrefab);
    }

    public void Update()
    {
        if (isConstructionFinished)
            return;

        progresTime += Time.deltaTime;

        if (IsConstructionFinished)
            FinishConstruction();
    }

    private bool IsConstructionFinished => progresTime >= buildAction.ConstructionTime;

    private void StartConstruction(
        BuildActionSO buildAction,
        Vector3 placementPosition,
        ParticleSystem constructionEffectPrefab
    )
    {
        structure = Object.Instantiate(buildAction.StructurePrefab);
        var spriteRenderer = structure.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = buildAction.FoundationSprite;
        structure.transform.position = placementPosition;
        structure.RegisterProcess(this);
        workerUnit = BuildManager.Instance.SpawnWorkerUnit(placementPosition);
        workerUnitAnim = workerUnit.GetComponentInChildren<Animator>();
        workerUnit.SetTask(UnitTask.Build);
        workerUnit.SetAnimation(workerUnitAnim);

        constructionEffect = Object.Instantiate(
            constructionEffectPrefab,
            placementPosition,
            Quaternion.identity,
            structure.transform
        );
        constructionEffect.Play();
    }

    public void FinishConstruction()
    {
        if (isConstructionFinished)
            return;
        isConstructionFinished = true;
        structure.SetSelectable(isConstructionFinished);
        structure.SpriteRenderer.sprite = buildAction.CompletionSprite;
        constructionEffect.Stop();
        workerUnit.SetTask(UnitTask.None);
        workerUnit.SetAnimation(workerUnitAnim);
        BuildManager.Instance.RemoveWorkerUnit(workerUnit);
        structure.ShowHealthBar();
        structure.OnConstructionFinished();
    }
}
