using UnityEngine;

public class BombAnim : MonoBehaviour
{
    [Header("Explosion Timing")]
    public float fuseTime = 3f;              // total time before explosion
    public float expandDuration = 0.75f;     // how long before detonation it starts expanding
    public float maxScaleMultiplier = 2f;    // final scale before exploding
    public AnimationCurve expandCurve;       // curve for expansion

    [Header("Spawn Grow Effect")]
    public float spawnGrowTime = 0.25f;      // time it takes to grow when pulled out
    public AnimationCurve spawnCurve;        // curve for spawn grow


    [Header("Flashing Settings")]
    public Material normalMaterial;
    public Material flashMaterial;
    public Renderer bombRenderer;

    [Header("Visual Only")]
    public Transform bombVisual;

    private float timer;
    private float flashTimer;
    private bool flashing;
    private float spawnTimer;
    private bool spawnedIn;

    void Start()
    {
        timer = fuseTime;
        flashTimer = 0f;
        flashing = false;
        spawnTimer = 0f;
        spawnedIn = false;

        if (bombRenderer == null && bombVisual != null)
            bombRenderer = bombVisual.GetComponent<Renderer>();

        if (bombVisual == null)
            bombVisual = transform; // fallback

        //start hidden and at 0
        bombVisual.localScale = Vector3.zero;  
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (!spawnedIn)
        {
            spawnTimer += Time.deltaTime;
            float t = Mathf.Clamp01(spawnTimer / spawnGrowTime);
            float scale = Mathf.Lerp(0f, 1f, spawnCurve.Evaluate(t));
            bombVisual.localScale = Vector3.one * scale;

            if (t >= 1f)
                spawnedIn = true;

            return; // don’t flash/expand until its fully spawned
        }

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

    // this shit work
    private void ToggleFlash()
    {
        flashing = !flashing;

        if (bombRenderer == null) return;

        if (flashing && flashMaterial != null)
        {
            bombRenderer.material = flashMaterial;
        }
            
        else if (normalMaterial != null)
        {
            bombRenderer.material = normalMaterial;
        }
            
    }
}
