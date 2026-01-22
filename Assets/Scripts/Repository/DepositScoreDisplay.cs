using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DepositScoreDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI depositedText;
    public Image gemIcon;

    [Header("Display Settings")]
    public float holdDuration = 0.25f;
    public float floatDuration = 1.25f;
    public float floatDistance = 1.2f;
    public float floatDepthOffset = 0.5f;

    [Header("Pop Animations")]
    public float popInScale = 1.3f;
    public float popInDuration = 0.12f;
    public float popOutScale = 0.7f;
    public float popOutDuration = 0.12f;

    [Header("Opacity Curve")]
    public bool useOpacityCurve = false;
    public AnimationCurve opacityCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private Vector3 initialLocalPosition;
    private float initialWorldZ;
    private Vector3 originalScale;

    void Awake()
    {
        if (depositedText == null)
        {
            Debug.LogError("DepositScoreDisplay requires a TextMeshPro component to be assigned.");
            enabled = false;
        }

        initialLocalPosition = depositedText.transform.localPosition;
        originalScale = depositedText.transform.localScale;
        depositedText.enabled = false;

    }
    private void Start()
        {
            gemIcon.enabled = false;
        }


    public void ShowScore(int scoreValue, Color teamColor, float repoInitialWorldZ)
    {
        StopAllCoroutines();

        depositedText.text = $"+{scoreValue}";
        depositedText.color = new Color(teamColor.r, teamColor.g, teamColor.b, 1f);
        depositedText.transform.localPosition = initialLocalPosition;
        depositedText.transform.localScale = originalScale;

        initialWorldZ = repoInitialWorldZ;

        StartCoroutine(DisplayScoreCoroutine());
    }

    private IEnumerator DisplayScoreCoroutine()
    {
        depositedText.enabled = true;
        gemIcon.enabled = true;

        yield return StartCoroutine(PopAnimation(originalScale, popInScale, popInDuration));

        yield return new WaitForSeconds(holdDuration);

        Vector3 startPos = depositedText.transform.localPosition;

        float elapsed = 0f;
        Vector3 targetPos = startPos + new Vector3(0, floatDistance, floatDepthOffset);

        Transform repoTransform = transform.parent != null ? transform.parent : transform;
        float startLoopParentZ = repoTransform.position.z;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / floatDuration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

            float currentRepoZ = repoTransform.position.z;
            float zMovementSinceStart = currentRepoZ - startLoopParentZ;
            pos.z -= zMovementSinceStart;

            depositedText.transform.localPosition = pos;

            var c = depositedText.color;
            var a = gemIcon.color;

            if (useOpacityCurve)
            {
                c.a = opacityCurve.Evaluate(t);
                a.a = opacityCurve.Evaluate(t);
            }
            else
            {
                c.a = Mathf.Lerp(1f, 0f, t);
                a.a = Mathf.Lerp(1f, 0f, t);
            }

            depositedText.color = c;
            gemIcon.color = a;

            yield return null;
        }

        yield return StartCoroutine(PopAnimation(originalScale, popOutScale, popOutDuration));

        depositedText.enabled = false;
        gemIcon.enabled = false;
        depositedText.transform.localScale = originalScale;
        depositedText.transform.localPosition = initialLocalPosition;

        var resetCol = depositedText.color;
        resetCol.a = 1f;
        depositedText.color = resetCol;
    }

    private IEnumerator PopAnimation(Vector3 baseScale, float scaleMultiplier, float duration)
    {
        Vector3 start = baseScale;
        Vector3 peak = baseScale * scaleMultiplier;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            depositedText.transform.localScale = Vector3.Lerp(start, peak, t);
            yield return null;
        }

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            depositedText.transform.localScale = Vector3.Lerp(peak, baseScale, t);
            yield return null;
        }

        depositedText.transform.localScale = baseScale;
    }
}