using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI popupText;

    [SerializeField]
    private float textPopupDuration = 0.5f;

    [SerializeField]
    private AnimationCurve fontSizeCurve;

    [SerializeField]
    private AnimationCurve xOffsetCurve;

    [SerializeField]
    private AnimationCurve yOffsetCurve;

    [SerializeField]
    private AnimationCurve alphaCurve;

    private float elapsedTime;
    private int randomXDirection = 1;

    private void Start()
    {
        randomXDirection = Random.Range(-1, 2);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > textPopupDuration)
        {
            elapsedTime = 0;
            Destroy(gameObject);
        }

        var alpha = alphaCurve.Evaluate(textPopupDuration);
        popupText.fontSize += fontSizeCurve.Evaluate(textPopupDuration) / 2;
        popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, alpha);
        float xOffset = xOffsetCurve.Evaluate(textPopupDuration) * 1.1f * randomXDirection;
        float yOffset = yOffsetCurve.Evaluate(textPopupDuration) * 1.1f;

        transform.position += new Vector3(xOffset, yOffset, 0) * Time.deltaTime;
    }

    public void SetText(string text, Color color)
    {
        popupText.text = text;
        popupText.color = color;
    }
}
