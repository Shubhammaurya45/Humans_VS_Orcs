using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Keyboard movement")]
    [SerializeField]
    private float moveSpeed = 5f;

    [Header("Mouse drag")]
    [SerializeField]
    private float mouseDragSpeed = 1f;
    public Vector3 leftMouseClickPosition = new Vector3();

    [Header("Zoom")]
    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float minZoom = 3f;

    [SerializeField]
    private float maxZoom = 12f;

    [Header("Bounds (optional)")]
    [SerializeField]
    private bool useBounds = false;

    [SerializeField]
    private Vector2 minBounds;

    [SerializeField]
    private Vector2 maxBounds;

    // Set true from BuildManager when UI is open
    public bool lockCamera = false;

    private Camera cam;

    // Mouse drag state
    private Vector3 dragOrigin;

    // Touch state
    private Vector2 lastSingleTouchPos;
    private Vector2 touchStartPos;
    private float lastPinchDistance;
    private bool isTouchDragging = false;
    private const float TouchDragThreshold = 15f; // pixels

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (lockCamera)
            return;

        HandleKeyboard();
        HandleMouseClick();
        HandleMouseDrag();
        HandleScrollZoom();
        HandleTouch();

        if (useBounds)
            ClampPosition();
    }

    // ── Keyboard ─────────────────────────────────────────────────────────────

    private void HandleKeyboard()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrow
        float v = Input.GetAxisRaw("Vertical"); // W/S or Up/Down arrow

        if (h == 0 && v == 0)
            return;

        Vector3 dir = new Vector3(h, v, 0f).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // 0 = left click
        {
            // World position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0; // flatten z for 2D
            UIManager.Instance?.DisplayPointToClick(worldPos);
            leftMouseClickPosition = worldPos;
        }
    }

    // ── Mouse drag (right-click) ──────────────────────────────────────────────
    // Right-click is used for drag so it doesn't conflict with IPointer left-click

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(1)) // right mouse button
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 currentWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = dragOrigin - currentWorldPos;
            transform.position += delta * mouseDragSpeed;
        }
    }

    // ── Scroll zoom ───────────────────────────────────────────────────────────

    private void HandleScrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f)
            return;

        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    // ── Mobile touch ──────────────────────────────────────────────────────────

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
            HandleSingleTouch();
        else if (Input.touchCount == 2)
            HandlePinchZoom();
    }

    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);

        // Don't pan when touching a UI element (buttons, menus, etc.)
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                lastSingleTouchPos = touch.position;
                touchStartPos = touch.position;
                isTouchDragging = false;
                break;

            case TouchPhase.Moved:
                // Only start panning after moving past the threshold
                // so quick taps still fire IPointerClickHandler cleanly
                if (!isTouchDragging)
                {
                    if (Vector2.Distance(touch.position, touchStartPos) > TouchDragThreshold)
                        isTouchDragging = true;
                }

                if (isTouchDragging)
                {
                    Vector3 prev = cam.ScreenToWorldPoint(
                        new Vector3(lastSingleTouchPos.x, lastSingleTouchPos.y, 0)
                    );
                    Vector3 curr = cam.ScreenToWorldPoint(
                        new Vector3(touch.position.x, touch.position.y, 0)
                    );
                    transform.position += prev - curr;
                }

                lastSingleTouchPos = touch.position;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isTouchDragging = false;
                break;
        }
    }

    private void HandlePinchZoom()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float currentPinchDistance = Vector2.Distance(t0.position, t1.position);

        // Record baseline when the second finger lands
        if (t1.phase == TouchPhase.Began)
        {
            lastPinchDistance = currentPinchDistance;
            return;
        }

        float delta = lastPinchDistance - currentPinchDistance;
        cam.orthographicSize += delta * (zoomSpeed * 0.05f);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        lastPinchDistance = currentPinchDistance;
    }

    // ── Bounds ────────────────────────────────────────────────────────────────

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos;
    }
}
