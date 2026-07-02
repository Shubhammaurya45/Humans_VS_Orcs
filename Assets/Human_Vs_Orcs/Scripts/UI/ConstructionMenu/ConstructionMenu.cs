using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConstructionMenu : MonoBehaviour
//IPointerEnterHandler,
//IPointerExitHandler,
//IPointerDownHandler,
//IPointerUpHandler
{
    [SerializeField]
    private ConstructionButton constructionButtonPrefab;

    [SerializeField]
    private Transform constructionButtonParent;

    public List<ConstructionButton> constructionButtons = new List<ConstructionButton>();

    public void ShowConstructionMenu()
    {
        gameObject.SetActive(true);
        GameManager.Instance.cameraController.lockCamera = true;
    }

    public void HideConstructionMenu()
    {
        gameObject.SetActive(false);

        foreach (var button in constructionButtons)
        {
            Destroy(button.gameObject);
        }
        constructionButtons.Clear();
        GameManager.Instance.activeUnit.Deselect();
        GameManager.Instance.cameraController.lockCamera = false;
    }

    public void SpawnConstructionButtons(
        Sprite buildingIcon,
        string buildingName,
        UnityAction action
    )
    {
        var constructionButton = Instantiate(constructionButtonPrefab, constructionButtonParent);
        constructionButton.Init(buildingIcon, buildingName, action);
        constructionButtons.Add(constructionButton);
    }
}
