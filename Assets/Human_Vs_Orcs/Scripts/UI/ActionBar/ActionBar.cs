using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    [SerializeField]
    private ActionButton actionButtonPrefab;

    [SerializeField]
    private Transform actionButtonParent;
    public List<ActionButton> actionButtons = new();

    public void ShowActionBar()
    {
        gameObject.SetActive(true);
    }

    public void HideActionBar()
    {
        gameObject.SetActive(false);
        ClearActions();
    }

    public void RegisterAction(Sprite buidlingIcon, string buildingName, UnityAction action)
    {
        var actionButton = Instantiate(actionButtonPrefab, actionButtonParent);
        actionButton.Init(buidlingIcon, buildingName, action);
        actionButtons.Add(actionButton);
    }

    public void ClearActions()
    {
        for (int i = actionButtons.Count - 1; i >= 0; i--)
        {
            Destroy(actionButtons[i].gameObject);
            actionButtons.RemoveAt(i);
        }
    }
}
