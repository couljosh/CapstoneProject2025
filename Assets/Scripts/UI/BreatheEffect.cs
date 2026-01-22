using System.Collections;
using UnityEngine;

public class BreatheEffect : MonoBehaviour
{
    public GameObject target;
    public float breathSpeed = 1f;
    public float breathAmount = 0.05f;
    public Vector3 originalScale;

    private float currentBreathSpeed;
    private float currentBreathAmount;
    private bool isGlitching = false;

    private void Start()
    {
        if (target!= null)
        {
            originalScale = target.transform.localScale;
        }

        currentBreathSpeed = breathSpeed;
        currentBreathAmount = breathAmount;

        StartCoroutine(RandomGlitchBreath());
    }

    private void Update()
    {
        if (target != null)
        {
            float breathingFactor = Mathf.Sin(Time.time * currentBreathSpeed) * currentBreathAmount;
            target.transform.localScale = originalScale + new Vector3(0f, breathingFactor, 0f);
        }
    }

    private IEnumerator RandomGlitchBreath()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

        }
    }
}
