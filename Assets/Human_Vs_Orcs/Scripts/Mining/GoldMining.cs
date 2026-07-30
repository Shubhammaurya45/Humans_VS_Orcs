using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoldMining : MonoBehaviour
{
    //
    [Header("Mining Settings")]
    [SerializeField]
    private float totalMiningTime = 10f; // time to complete one mining cycle

    [SerializeField]
    private int goldPerCycle = 50;

    [SerializeField]
    private int workerRequired = 3;

    [Header("UI")]
    [SerializeField]
    private SpriteRenderer mineSpriteRenderer;

    [SerializeField]
    private Sprite activeMineIcon;

    [SerializeField]
    private Sprite deactiveMineIcon;

    [SerializeField]
    private Slider progressBar; // optional visual feedback

    [SerializeField]
    private GameObject buttonPanel;

    [SerializeField]
    private Button mineButton;

    [SerializeField]
    private Button cancelButton;

    private float elapsedTime = 0f; // persists across cancel
    private bool isMining = false;
    private Coroutine miningRoutine;
    private bool justOpenedThisFrame;

    private void Awake()
    {
        mineButton.onClick.AddListener(StartMining);
        cancelButton.onClick.AddListener(CancelMining);
        UpdateProgressUI();
    }

    private void LateUpdate()
    {
        if (justOpenedThisFrame)
        {
            justOpenedThisFrame = false;
            return;
        }

        if (!buttonPanel.activeSelf)
            return;

        if (!PressedThisFrame())
            return;

        if (
            EventSystem.current.IsPointerOverGameObject(
                Input.touchCount > 0 ? Input.GetTouch(0).fingerId : -1
            )
        )
            return;

        CloseMiningUI();
    }

    private void OnMouseDown()
    {
        buttonPanel.SetActive(true);
    }

    bool PressedThisFrame()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;

        return Input.GetMouseButtonDown(0);
    }

    public void StartMining()
    {
        if (isMining)
            return; // already running, ignore double click

        CloseMiningUI();
        bool gotWorker = ResourceManager.Instance.Spend(ResourceType.Workers, workerRequired);
        if (gotWorker)
        {
            progressBar.gameObject.SetActive(true);
            mineSpriteRenderer.sprite = activeMineIcon;
            isMining = true;
            miningRoutine = StartCoroutine(MiningCountdown());
        }
    }

    public void CancelMining()
    {
        if (!isMining)
            return;

        CloseMiningUI();
        ResourceManager.Instance.Add(ResourceType.Workers, workerRequired);
        mineSpriteRenderer.sprite = deactiveMineIcon;
        isMining = false;
        if (miningRoutine != null)
        {
            StopCoroutine(miningRoutine);
            miningRoutine = null;
        }
        // elapsedTime is untouched here -> progress is preserved
    }

    private IEnumerator MiningCountdown()
    {
        while (elapsedTime < totalMiningTime)
        {
            elapsedTime += Time.deltaTime;
            UpdateProgressUI();
            yield return null;
        }

        CompleteMining();
    }

    private void CompleteMining()
    {
        isMining = false;
        elapsedTime = 0f;
        miningRoutine = null;
        UpdateProgressUI();
        progressBar.gameObject.SetActive(false);
        ResourceManager.Instance.Add(ResourceType.Workers, workerRequired);
        GiveGold(goldPerCycle);
    }

    private void GiveGold(int amount)
    {
        // hook into your resource/economy manager here
        Debug.Log($"Mined {amount} gold!");
        ResourceManager.Instance.Add(ResourceType.Coins, goldPerCycle);
    }

    private void UpdateProgressUI()
    {
        if (progressBar != null)
            progressBar.value = elapsedTime / totalMiningTime;
    }

    private void CloseMiningUI()
    {
        buttonPanel.SetActive(false);
    }
}
