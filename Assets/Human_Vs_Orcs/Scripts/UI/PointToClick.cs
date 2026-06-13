using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PointToClick : MonoBehaviour
{
    public float duration=1f;
    private float timer;
    private float freqTimer;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private AnimationCurve scaleCurve;

    private Vector3 initialScale;

    void Awake()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        initialScale=transform.localScale;

    }

    private void Update()
    {
        timer+=Time.deltaTime;
        freqTimer+=Time.deltaTime;

        float scaleMultiplier=scaleCurve.Evaluate(freqTimer);
        transform.localScale=initialScale*scaleMultiplier;

        if (freqTimer>= duration )
        {
            freqTimer=0; 
        }

        if (timer >= duration * 0.9f)
        {
            float fadeProgress=(timer-duration*0.9f)/(duration
            *0.1f);
            spriteRenderer.color=new Color(1,1,1,1-fadeProgress);
        }
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
