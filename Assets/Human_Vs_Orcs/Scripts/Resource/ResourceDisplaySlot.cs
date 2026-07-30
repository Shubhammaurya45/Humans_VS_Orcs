using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplaySlot : MonoBehaviour
{
    [SerializeField]
    private ResourceType resourceType;

    [SerializeField]
    private TextMeshProUGUI valueText;

    [SerializeField]
    private Image background; // 👈 new: the pill's background image

    [SerializeField]
    private Color flashColor = Color.red;

    [SerializeField]
    private float flashDuration = 0.15f;

    [SerializeField]
    private int flashCount = 6;

    private Color originalColor = Color.white;
    private Coroutine flashRoutine;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
            ResourceManager.Instance.OnInsufficientResource += HandleInsufficientResource;

            // Initialize immediately with current value
            UpdateText(ResourceManager.Instance.Get(resourceType));
        }
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
            ResourceManager.Instance.OnInsufficientResource -= HandleInsufficientResource;
        }
    }

    private void HandleResourceChanged(ResourceType type, int newValue)
    {
        if (type == resourceType)
            UpdateText(newValue);
    }

    private void HandleInsufficientResource(ResourceType type)
    {
        if (type != resourceType)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            background.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            background.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        flashRoutine = null;
        background.color = originalColor;
    }

    private void UpdateText(int value)
    {
        valueText.text = value.ToString("00"); // matches "00" style in your image
    }
}
