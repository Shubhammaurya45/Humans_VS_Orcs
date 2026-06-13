using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI popupText;

    public void SetText(string text, Color color)
    {
        popupText.text = text;
        popupText.color = color;
    }
}
