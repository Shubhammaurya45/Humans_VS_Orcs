using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConstructionButton
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    public Image buildingIcon;
    public TMP_Text buildingName;

    public Button button;

    [SerializeField]
    private float hoverScale = 1.08f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * hoverScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BuildManager.Instance.constructionMenu.HideConstructionMenu();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    public void Init(Sprite icon, string buildingName, UnityAction action)
    {
        buildingIcon.sprite = icon;
        this.buildingName.text = buildingName;

        button.onClick.AddListener(action);
    }
}
