using TMPro;
using UnityEngine;

public class ResourceDisplaySlot : MonoBehaviour
{
    [SerializeField]
    private ResourceType resourceType;

    [SerializeField]
    private TextMeshProUGUI valueText;

    private void OnEnable()
    {
        ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
        // Initialize immediately with current value
        UpdateText(ResourceManager.Instance.Get(resourceType));
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
    }

    private void HandleResourceChanged(ResourceType type, int newValue)
    {
        if (type == resourceType)
            UpdateText(newValue);
    }

    private void UpdateText(int value)
    {
        valueText.text = value.ToString("00"); // matches "00" style in your image
    }
}
