using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChopSession
{
    public Worker_Unit worker; // which worker is doing this chop
    public Coroutine coroutine; // the running "countdown" for this chop
}

public class TreeChopManager : MonoBehaviour
{
    public static TreeChopManager Instance;

    [SerializeField]
    private GameObject workerPrefab;
    public Worker_Unit worker;

    private ChoppableObject selectedTree;

    private Dictionary<ChoppableObject, ChopSession> activeSessions =
        new Dictionary<ChoppableObject, ChopSession>();

    private Coroutine chopCountdown;
    private bool justOpenedThisFrame;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (justOpenedThisFrame)
        {
            justOpenedThisFrame = false;
            return;
        }

        if (selectedTree != null && !selectedTree.ChopButtonUI.activeSelf)
            return;

        if (!PressedThisFrame())
            return;

        if (
            EventSystem.current.IsPointerOverGameObject(
                Input.touchCount > 0 ? Input.GetTouch(0).fingerId : -1
            )
        )
            return;

        CloseChopUI();
    }

    bool PressedThisFrame()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;

        return Input.GetMouseButtonDown(0);
    }

    public void SelectTree(ChoppableObject tree)
    {
        if (selectedTree == tree && tree.ChopButtonUI.activeSelf)
            return;

        //StopChopingIfActive();
        selectedTree = tree;
        selectedTree.ChopButtonUI.SetActive(true);
        justOpenedThisFrame = true;
    }

    public void StartChoping()
    {
        if (selectedTree == null)
            return;

        // lock in which tree we mean, right now
        ChoppableObject tree = selectedTree;

        // If this exact tree already has a worker chopping it, don't start a second one
        if (activeSessions.ContainsKey(tree))
            return;

        Vector3 chopWorkerSpawnOffset = new Vector3(-0.8f, -0.8f, 0);

        // Ask the pool for a free worker.
        bool gotWorker = PoolManager.Instance.TryGet<Worker_Unit>(
            workerPrefab,
            selectedTree.transform.position + chopWorkerSpawnOffset,
            Quaternion.identity,
            out worker
        );

        if (!gotWorker)
        {
            Debug.LogWarning("Worker not available currently.");
            // e.g. show a UI popup/toast here instead of just a log
            return;
        }

        //selectedTree.ChopButton.gameObject.SetActive(false);

        Animator workeranim = worker.GetComponentInChildren<Animator>();
        worker.SetTask(UnitTask.Chop);
        worker.SetAnimation(workeranim);

        // Write the sticky note for this tree: "this worker is chopping it"
        ChopSession session = new ChopSession();
        session.worker = worker;
        session.coroutine = StartCoroutine(ChopCountdown(tree, session));

        activeSessions[tree] = session; // save the sticky note in the phone book

        CloseChopUI();
    }

    private IEnumerator ChopCountdown(ChoppableObject tree, ChopSession session)
    {
        float timePassed = 0f;

        while (timePassed < tree.ChopTime)
        {
            timePassed += Time.deltaTime;

            yield return null;
        }
        tree.OnChopped();
        StopChopingIfActive();
        FinishSession(tree, session);
    }

    // Cleans up after a tree is done being chopped (or cancelled)
    private void FinishSession(ChoppableObject tree, ChopSession session)
    {
        if (session.worker != null)
            PoolManager.Instance.Release(workerPrefab, session.worker.gameObject); // give worker back to the pool

        activeSessions.Remove(tree); // erase this tree's sticky note — it's done
    }

    private void StopChopingIfActive()
    {
        if (chopCountdown != null)
        {
            StopCoroutine(chopCountdown);
            chopCountdown = null;
        }
    }

    private void CloseChopUI()
    {
        if (selectedTree == null)
            return;

        selectedTree.ChopButtonUI.SetActive(false);
        selectedTree = null;
    }

    //
}
