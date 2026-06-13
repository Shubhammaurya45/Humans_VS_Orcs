using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : SingletonManager<GameManager>
{
    public Units activeUnit;
    private PlacementProcess placementProcess;
    private ActionBar actionBar;
    private bool HasActiveUnit => activeUnit != null;
    public Camera mainCamera;

    public CameraController cameraController;

    [SerializeField]
    private float cameraSpeed = 100;

    [SerializeField]
    private float mobileCameraSpeed = 100;

    protected override void Awake()
    {
        base.Awake();
        cameraController = new CameraController(cameraSpeed, mobileCameraSpeed);
        mainCamera = Camera.main;
        actionBar = UIManager.Instance?.actionBar;
    }

    private void Update()
    {
        cameraController.Update();
    }

    public void DetectClick(Vector2 inputPostion)
    {
        if (IsPointerOverUIElement())
            return;
        if (BuildManager.Instance.IsPlacementProcessBegin())
            return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(inputPostion);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (HasClickOnUnit(hit, out var unit))
            HandleClickOnUnit(unit);
        else
            HandleClickOnGround(worldPoint);
    }

    //Detect clicks on Ground
    public void HandleClickOnGround(Vector2 worldPoint)
    {
        if (placementProcess != null)
            return;
        if (IsPointerOverUIElement())
            return;
        if (HasActiveUnit && IsHumanoid(activeUnit))
        {
            UIManager.Instance?.DisplayPointToClick(worldPoint);
            activeUnit.MoveTo(worldPoint);
        }
    }

    //Detect click on units and return true or false
    private bool HasClickOnUnit(RaycastHit2D hit, out Units unit)
    {
        if (hit.collider != null && hit.collider.TryGetComponent<Units>(out var clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }

        unit = null;
        return false;
    }

    // Detect wheater a player clicked on seleceted unit
    private bool HasClickedOnActiveUnit(Units currentUnit) => currentUnit == activeUnit;

    //select the new unit and deselect the active unit
    public void HandleClickOnUnit(Units unit)
    {
        if (HasClickedOnActiveUnit(unit))
        {
            activeUnit.Deselect();
            activeUnit = null;
            UIManager.Instance?.actionBar.HideActionBar();
            return;
        }
        SelectNewUnit(unit);
    }

    //select the new unit
    public void SelectNewUnit(Units unit)
    {
        if (HasActiveUnit)
        {
            activeUnit.Deselect();
            UIManager.Instance?.actionBar.HideActionBar();
        }
        activeUnit = unit;
        activeUnit.Select();
        if (unit.Actions.Length == 0)
            return;
        actionBar.ClearActions();
        actionBar.ShowActionBar();
        foreach (var action in unit.Actions)
        {
            var localAction = action;
            actionBar.RegisterAction(localAction.icon, () => localAction.Execute());
        }
    }

    //detect wheater a unit is humanoid or not
    private bool IsHumanoid(Units units) => units is Humanoid_Units;

    //Check is pointer over any UI element
    public bool IsPointerOverUIElement()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    public bool IsUnitChange()
    {
        if (HasClickedOnActiveUnit(activeUnit))
            return true;
        return false;
    }
}
