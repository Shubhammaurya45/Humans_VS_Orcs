using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Structure_Unit : Units, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private BuildingProcess buildingProcess;
    public bool IsUnderConstruction => buildingProcess != null;
    private bool isConstructionComplete = false;
    private Color originalColor;

    [SerializeField]
    private Color highlightColor = new Color(1f, 1f, 0.75f, 1f);

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        if (IsUnderConstruction)
        {
            buildingProcess.Update();
        }
    }

    public void SetSelectable(bool selectable)
    {
        isConstructionComplete = selectable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isConstructionComplete)
            return;
        spriteRenderer.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isConstructionComplete)
            return;
        spriteRenderer.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isConstructionComplete)
            return;

        // Ignore right click
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Place your selection logic here — show upgrade UI, display stats, etc.
        Debug.Log($"Selected: {gameObject.name}");
    }

    public void OnConstructionFinished() => buildingProcess = null;

    public void RegisterProcess(BuildingProcess process)
    {
        buildingProcess = process;
    }
}
