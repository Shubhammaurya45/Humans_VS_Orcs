using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Image buildingIcon;
    public TMP_Text buildingName;

    public Button button;

    public void Init(Sprite icon, string buildingName, UnityAction action)
    {
        buildingIcon.sprite = icon;
        this.buildingName.text = buildingName;

        //button.onClick.AddListener(action);
    }
}
