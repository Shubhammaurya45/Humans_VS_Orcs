using UnityEngine;

public class TextPopupController : MonoBehaviour
{
    [SerializeField]
    private TextPopup textPopupPrefab;

    public void Spawn(string popupText, Vector3 popupPosition, Color PopupColor)
    {
        var textPopup = Instantiate(textPopupPrefab);
        textPopup.transform.position = popupPosition;
        textPopup.SetText(popupText, PopupColor);
    }
}
