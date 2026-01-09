using System.Collections;
using TMPro;
using UnityEngine;

public class GemsHeldUI : MonoBehaviour
{
    [Header("Gem Text GameObject")]
    public TextMeshProUGUI gemText;

    [Header("Timing Variables")]
    public float baseCountDuration = 0.2f; // time to count 1 gem
    public float visibleDuration = 0.2f;

    [Header("Pop Animation")]
    public float popInScale = 1.25f;
    public float popInDuration = 0.1f;

    [Header("Per-Pip Pulse")]
    public float pipScale = 1.12f;
    public float pipDuration = 0.05f;

    private int currentValue;
    private int targetValue;

    private Vector3 originalScale;
    private Coroutine chaseRoutine;
    private Coroutine pipRoutine;
    private float hideTimer;

    private void Awake()
    {
        // setup size and text
        originalScale = gemText.transform.localScale;
        gemText.enabled = false;
    }

    // called by player when gem total changes
    public void AnimateToValue(int newValue)
    {
        // ---------- NEW: handle gem loss instantly ----------
        if (newValue < currentValue)
        {
            currentValue = newValue;
            targetValue = newValue;

            gemText.text = currentValue.ToString();
            gemText.enabled = true;
            hideTimer = visibleDuration;

            TriggerPipPulse();

            // restart hide timer cleanly
            if (chaseRoutine == null)
            {
                chaseRoutine = StartCoroutine(ChaseTarget());
                StartCoroutine(Pop());
            }

            return;
        }
        // ----------------------------------------------------

        targetValue = newValue;

        gemText.enabled = true;
        hideTimer = visibleDuration;

        if (chaseRoutine == null)
        {
            chaseRoutine = StartCoroutine(ChaseTarget());
            StartCoroutine(Pop());
        }
    }

    private IEnumerator ChaseTarget()
    {
        while (true)
        {
            int diff = targetValue - currentValue;

            // ---------- COUNT UP ----------
            if (diff > 0)
            {
                float speedMultiplier = Mathf.Clamp(diff, 1f, 6f);
                float stepTime = baseCountDuration / speedMultiplier;

                currentValue++;
                gemText.text = currentValue.ToString();

                // pulse EVERY increment
                TriggerPipPulse();

                yield return new WaitForSeconds(stepTime);
                continue;
            }

            // ---------- IDLE / HIDE ----------
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                gemText.enabled = false;
                chaseRoutine = null;
                yield break;
            }

            yield return null;
        }
    }

    private void TriggerPipPulse()
    {
        if (pipRoutine != null)
            StopCoroutine(pipRoutine);

        pipRoutine = StartCoroutine(PipPulse());
    }

    private IEnumerator PipPulse()
    {
        float time = 0f;
        Vector3 peak = originalScale * pipScale;

        while (time < pipDuration)
        {
            time += Time.deltaTime;
            gemText.transform.localScale =
                Vector3.Lerp(originalScale, peak, time / pipDuration);
            yield return null;
        }

        time = 0f;
        while (time < pipDuration)
        {
            time += Time.deltaTime;
            gemText.transform.localScale =
                Vector3.Lerp(peak, originalScale, time / pipDuration);
            yield return null;
        }

        gemText.transform.localScale = originalScale;
    }

    private IEnumerator Pop()
    {
        float time = 0f;
        Vector3 peak = originalScale * popInScale;

        while (time < popInDuration)
        {
            time += Time.deltaTime;
            gemText.transform.localScale =
                Vector3.Lerp(originalScale, peak, time / popInDuration);
            yield return null;
        }

        time = 0f;
        while (time < popInDuration)
        {
            time += Time.deltaTime;
            gemText.transform.localScale =
                Vector3.Lerp(peak, originalScale, time / popInDuration);
            yield return null;
        }

        gemText.transform.localScale = originalScale;
    }
}
