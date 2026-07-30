using UnityEngine;

public class ClickableActionPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject actionPanel; // buttons container (world-space canvas or child GameObject)

    public bool IsOpen => actionPanel != null && actionPanel.activeSelf;

    private void OnMouseDown()
    {
        //// ignore click if it's over UI (so clicking a button doesn't also re-trigger this)
        //if (
        //    UnityEngine.EventSystems.EventSystem.current != null
        //    && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
        //)
        //    return;

        ClickManager.Instance.ToggleOrOpen(this);
    }

    public void Show()
    {
        if (actionPanel != null)
            actionPanel.SetActive(true);
    }

    public void Hide()
    {
        if (actionPanel != null)
            actionPanel.SetActive(false);
    }
}
