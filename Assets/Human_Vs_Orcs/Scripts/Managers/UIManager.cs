using TMPro;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    public ActionBar actionBar;
    public GameObject pointToClick;

    [SerializeField]
    private TextPopupController textPopupController;

    protected override void Awake()
    {
        base.Awake();
        actionBar.HideActionBar();
    }

    public void DisplayPointToClick(Vector2 worldPoint)
    {
        Instantiate(pointToClick, worldPoint, quaternion.identity);
    }

    public void ShowTextPopup(string text, Vector3 postion, Color color)
    {
        textPopupController.Spawn(text, postion, color);
    }
}
