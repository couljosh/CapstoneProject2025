using UnityEngine;

public class BombAnim : MonoBehaviour
{
    [Header("Explosion Timing")]
    public float fuseTime = 3f;              // total time before explosion
    public float expandDuration = 0.75f;     // how long before detonation it starts expanding
    public float maxScaleMultiplier = 2f;    // final scale before exploding
    public AnimationCurve expandCurve;       // curve for expansion (e.g. ease-out)

    [Header("Flashing Settings")]
    public Material normalMaterial;
    public Material flashMaterial;
    public Renderer bombRenderer;

    [Header("Visual Only")]
    public Transform bombVisual;

    private float timer;
    private float flashTimer;
    private bool flashing;

    void Start()
    {
        timer = fuseTime;
        flashTimer = 0f;
        flashing = false;

        if (bombRenderer == null && bombVisual != null)
            bombRenderer = bombVisual.GetComponent<Renderer>();

        if (bombVisual == null)
            bombVisual = transform; // fallback
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // flashing
        float flashSpeed = Mathf.Lerp(1f, 12f, 1f - (timer / fuseTime)); // faster near end
        flashTimer += Time.deltaTime * flashSpeed;

        if (flashTimer >= 1f)
        {
            ToggleFlash();
            flashTimer = 0f;
        }

        // expand after certain time
        if (timer <= expandDuration)
        {
            float t = Mathf.Clamp01(1f - (timer / expandDuration));
            float scale = Mathf.Lerp(1f, maxScaleMultiplier, expandCurve.Evaluate(t));
            bombVisual.localScale = Vector3.one * scale;
        }
        else
        {
            bombVisual.localScale = Vector3.one; // stay static until last second
        }
    }

    // this shit doesnt work
    private void ToggleFlash()
    {
        flashing = !flashing;

        if (bombRenderer == null) return;

        if (flashing && flashMaterial != null)
        {
            print("flash made");
            bombRenderer.material = flashMaterial;
        }
            
        else if (normalMaterial != null)
        {
            bombRenderer.material = normalMaterial;
            print("flash off");
        }
            
    }
}
