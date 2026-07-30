using System.Collections;
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

    [SerializeField]
    private float chopTime = 3f;

    [SerializeField]
    private int woodPerChop = 10;

    [SerializeField]
    private float regrowTime = 15f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite fullTreeSprite;

    [SerializeField]
    private Collider2D treeCollider;

    private Animator anim;
    private bool isChopped = false;
    private Coroutine regrowRoutine;

    public float ChopTime => chopTime;
    public GameObject ChopButtonUI => chopButtonUI;
    public Button ChopButton => chopButton;

    public bool IsChopped => isChopped;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        chopButton.onClick.AddListener(TreeChopManager.Instance.StartChoping);
    }

    void OnMouseDown()
    {
        // Fires for mouse click AND touch tap - no extra mobile code needed
        TreeChopManager.Instance.SelectTree(this);
    }

    public void OnChopped(Worker_Unit worker)
    {
        // drop wood / play fall animation / etc.

        if (anim)
            anim.enabled = false;

        treeCollider.enabled = false;
        isChopped = true;
        ResourceManager.Instance.Add(ResourceType.Wood, woodPerChop);
        spriteRenderer.sprite = treeChoppedIcon;
        if (worker != null)
            worker.SetTask(UnitTask.None);
        regrowRoutine = StartCoroutine(RegrowAfterDelay());
    }

    private IEnumerator RegrowAfterDelay()
    {
        yield return new WaitForSeconds(regrowTime);
        Regrow();
    }

    private void Regrow()
    {
        isChopped = false;
        if (anim)
            anim.enabled = true;

        spriteRenderer.sprite = fullTreeSprite;

        if (treeCollider != null)
            treeCollider.enabled = true; // choppable again

        regrowRoutine = null;
    }

    private void OnDisable()
    {
        if (regrowRoutine != null)
            StopCoroutine(regrowRoutine);
    }
}
