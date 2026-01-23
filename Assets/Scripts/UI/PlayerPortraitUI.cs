using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPortraitUI : MonoBehaviour
{
    public Image portraitImage;
    public Color aliveColor = Color.white;
    public Color deadColor = new Color(0.3f, 0.3f, 0.3f, 1f); //gray colour

    [Header("Animation Settings")]
    public float popScale = 1.4f;
    public float animDuration = 0.2f;

    private Vector3 originalScale;
    private Coroutine animRoutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetStatus(bool isDead)
    {
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(PopAnimation(isDead));
    }

    IEnumerator PopAnimation(bool isDead)
    {
        //pop up animation
        float elapsed = 0;
        Vector3 targetPop = originalScale * popScale;

        while (elapsed < animDuration / 2)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetPop, elapsed / (animDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        //change colour near the end of the pop
        portraitImage.color = isDead ? deadColor : aliveColor;

        //return back to regular size
        elapsed = 0;
        while (elapsed < animDuration / 2)
        {
            transform.localScale = Vector3.Lerp(targetPop, originalScale, elapsed / (animDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}