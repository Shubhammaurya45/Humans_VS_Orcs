using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Image iconImage;
    public Button button;

    public void Init(Sprite icon,UnityAction action)
    {
        iconImage.sprite=icon;
        button.onClick.AddListener(action);
    }
}
