using Unity.InferenceEngine;
using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO buildAction;
    private Animator workerUnitAnim;
    private float progresTime;
    private Structure_Unit structure;
    private bool isConstructionFinished = false;
    private ParticleSystem constructionEffect;

    private bool gotworker;
    private Worker_Unit worker;

    public BuildingProcess(
        BuildActionSO buildAction,
        Vector3 placementPosition,
        ParticleSystem constructionEffectPrefab
    )
    {
        this.buildAction = buildAction;
        if (gotworker) { }
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
        SpawnWorkerUnit(placementPosition);
        if (gotworker)
        {
            structure = Object.Instantiate(buildAction.StructurePrefab);
            var spriteRenderer = structure.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = buildAction.FoundationSprite;
            structure.transform.position = placementPosition;
            structure.RegisterProcess(this);
            constructionEffect = Object.Instantiate(
                constructionEffectPrefab,
                placementPosition,
                Quaternion.identity,
                structure.transform
            );
            constructionEffect.Play();
        }
    }

    public void SpawnWorkerUnit(Vector3 buildPostion)
    {
        int xRandomOffset = UnityEngine.Random.Range(-1, 2);
        int yRandomOffset = UnityEngine.Random.Range(0, 2);

        Vector3 workerUnitPostionOffset = new Vector3(xRandomOffset, yRandomOffset, 0);
        Vector3 workerUnitSpwanPostion = buildPostion + workerUnitPostionOffset;

        gotworker = PoolManager.Instance.TryGet<Worker_Unit>(
            BuildManager.Instance.WorkerPrefab,
            workerUnitSpwanPostion,
            Quaternion.identity,
            out worker
        );
        Debug.Log(gotworker);

        if (gotworker)
        {
            workerUnitAnim = worker.GetComponentInChildren<Animator>();
            worker.SetTask(UnitTask.Build);
            worker.SetAnimation(workerUnitAnim);
            var workerSprite = worker.GetComponentInChildren<SpriteRenderer>();
            if (xRandomOffset == 1)
                workerSprite.flipX = true;
        }
    }

    public void FinishConstruction()
    {
        if (isConstructionFinished)
            return;
        isConstructionFinished = true;
        structure.SetSelectable(isConstructionFinished);
        structure.SpriteRenderer.sprite = buildAction.CompletionSprite;
        constructionEffect.Stop();
        worker.SetTask(UnitTask.None);
        worker.SetAnimation(workerUnitAnim);
        RemoveWorkerUnit();
        structure.ShowHealthBar();
        structure.OnConstructionFinished();
    }

    public void RemoveWorkerUnit()
    {
        PoolManager.Instance.Release(BuildManager.Instance.WorkerPrefab, worker.gameObject);
    }
}
