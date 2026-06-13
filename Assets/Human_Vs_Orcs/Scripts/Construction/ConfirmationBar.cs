using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBar : MonoBehaviour
{
    // private PlacementProcess placementProcess;
    [SerializeField] private Button confirmationBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_Text goldText; //Confirmation Bar goldText
    [SerializeField] private TMP_Text woodText;//Confiramtion bar woodText

    void OnDisable()
    {
        confirmationBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }

    public void SetupHook(UnityAction onConfirm,UnityAction onCancel)
    {
        confirmationBtn.onClick.AddListener(onConfirm);
        cancelBtn.onClick.AddListener(onCancel);    
    }

    public void ShowConfirmationBar(int goldCost,int woodCost)
    {
        goldText.text=goldCost.ToString();
        woodText.text=woodCost.ToString();
        UpdateColorRequirements(goldCost,woodCost);
        gameObject.SetActive(true);
    }

    public void HideConfirmationBar()
    {
        gameObject.SetActive(false);
    }

    void UpdateColorRequirements(int reqGold, int reqWood)
    {
        var greenColor = new Color(0, 0.8f, 0, 1f);
        goldText.color = BuildManager.Instance?.Gold >= reqGold ? greenColor : Color.red;
        woodText.color = BuildManager.Instance?.Wood >= reqWood ? greenColor : Color.red;
    }



}
