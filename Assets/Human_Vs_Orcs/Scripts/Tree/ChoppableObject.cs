using UnityEngine;
using UnityEngine.UI;

public class ChoppableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject chopButtonUI;

    [SerializeField]
    private Button chopButton;

    [SerializeField]
    private Sprite treeChoppedIcon;

    private SpriteRenderer chopObjectSpriteRender;
    private Animator anim;

    [SerializeField]
    private float chopTime = 3f;
    public float ChopTime => chopTime;
    public GameObject ChopButtonUI => chopButtonUI;
    public Button ChopButton => chopButton;

    private void Start()
    {
        chopObjectSpriteRender = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        chopButton.onClick.AddListener(TreeChopManager.Instance.StartChoping);
    }

    void OnMouseDown()
    {
        // Fires for mouse click AND touch tap - no extra mobile code needed
        TreeChopManager.Instance.SelectTree(this);
    }

    public void OnChopped()
    {
        // drop wood / play fall animation / etc.

        if (anim)
            anim.enabled = false;
        chopObjectSpriteRender.sprite = treeChoppedIcon;
        TreeChopManager.Instance.worker.SetTask(UnitTask.None);
    }
}
