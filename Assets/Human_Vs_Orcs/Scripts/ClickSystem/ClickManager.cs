using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : SingletonManager<ClickManager>
{
    private ClickableActionPanel currentOpen;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if pointer is over UI (like the buttons themselves), don't close anything
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // raycast into the world to see what was clicked
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider == null)
            {
                // clicked empty space -> close whatever's open
                CloseCurrent();
            }
            // if hit.collider != null, OnMouseDown on that object will handle opening its own panel
            // (and it'll call ToggleOrOpen, which closes the old one automatically)
        }
    }

    public void ToggleOrOpen(ClickableActionPanel target)
    {
        if (currentOpen == target)
        {
            // clicking the same object again toggles it closed
            target.Hide();
            currentOpen = null;
            return;
        }

        CloseCurrent();
        target.Show();
        currentOpen = target;
    }

    private void CloseCurrent()
    {
        if (currentOpen != null)
        {
            currentOpen.Hide();
            currentOpen = null;
        }
    }
}
