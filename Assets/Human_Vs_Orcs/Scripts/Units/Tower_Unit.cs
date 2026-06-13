using System.Collections;
using TMPro;
using UnityEngine;

public class Tower_Unit : Structure_Unit
{
    [SerializeField]
    private BuildActionSO buildAction;

    [SerializeField]
    private TMP_Text timerText;
    private float constructionTime;

    private void Start()
    {
        constructionTime = buildAction.ConstructionTime;
        StartCoroutine(TimeRoutine());
    }

    private IEnumerator TimeRoutine()
    {
        while (constructionTime > 0)
        {
            int minutes = Mathf.FloorToInt(constructionTime / 60);
            int second = Mathf.FloorToInt(constructionTime % 60);
            timerText.text = $"{minutes}:{second}";
            constructionTime -= Time.deltaTime;
            yield return null;
        }
        timerText.text = "00:00";
        OnTimerEnd();
    }

    private void OnTimerEnd()
    {
        timerText.gameObject.SetActive(false);
    }
}
