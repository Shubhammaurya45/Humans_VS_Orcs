using UnityEngine;
using UnityEngine.EventSystems;

public class Worker_Unit : Humanoid_Units, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.Actions.Length == 0)
            return;
        BuildManager.Instance.constructionMenu.ShowConstructionMenu();
        GameManager.Instance.activeUnit = this;
        foreach (var action in this.Actions)
        {
            var localAction = action;
            BuildManager.Instance.constructionMenu.SpawnConstructionButtons(
                localAction.buildingIcon,
                localAction.buildingName.ToString(),
                () => localAction.Execute()
            );
        }
    }

    public void SetAnimation(Animator animator)
    {
        Debug.Log(CurrentTask);
        switch (CurrentTask)
        {
            case UnitTask.Build:
                animator.SetBool("Build", true);
                break;
            case UnitTask.Chop:
                animator.SetBool("Chop", true);
                break;
            default:
                animator.SetBool("Idle", true);
                animator.SetBool("Chop", false);
                animator.SetBool("Build", false);
                break;
        }
    }
}
