using UnityEngine;

public class InputManager : SingletonManager<InputManager>
{
    public Vector2 initialTouchPosition;

    public Vector3 inputHoldWorldPosition;

    public Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        bool hasTouchInput = Input.touchCount > 0;
        Touch touch = hasTouchInput ? Input.GetTouch(0) : default;
        Vector2 inputPosition = hasTouchInput ? touch.position : Input.mousePosition;

        if (Input.GetMouseButtonDown(0) || (hasTouchInput && touch.phase == TouchPhase.Began))
        {
            initialTouchPosition = inputPosition;
        }

        if (Input.GetMouseButtonUp(0) || (hasTouchInput && touch.phase == TouchPhase.Ended))
        {
            if (Vector2.Distance(initialTouchPosition, inputPosition) < 10)
            {
                GameManager.Instance.DetectClick(inputPosition);
            }
        }
        inputHoldWorldPosition =
            hasTouchInput ? mainCamera.ScreenToWorldPoint(touch.position)
            : Input.GetMouseButton(0) ? mainCamera.ScreenToWorldPoint(Input.mousePosition)
            : Vector2.zero;
    }
}
