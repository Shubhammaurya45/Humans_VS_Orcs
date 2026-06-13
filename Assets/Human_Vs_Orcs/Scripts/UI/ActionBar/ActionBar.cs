using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    [SerializeField] private ActionButton actionButtonPrefab;
    public List<ActionButton> actionButtons=new ();

    public void ShowActionBar()
    {
        gameObject.SetActive(true);
    }
    public void HideActionBar()
    {
        gameObject.SetActive(false);
        ClearActions();
    }

    public void RegisterAction(Sprite icon,UnityAction action)
    {
        
            var actionButton=Instantiate(actionButtonPrefab,transform);
            actionButton.Init(icon,action);
            actionButtons.Add(actionButton);
            
        
    }
     public void ClearActions()
    {
        for(int i = actionButtons.Count - 1; i >= 0; i--)
        {
            Destroy(actionButtons[i].gameObject);
            actionButtons.RemoveAt(i);
        }
    }
}
