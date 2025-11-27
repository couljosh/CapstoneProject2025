using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{

    public Image image;               // Reference to the Image component
    public float fadeDuration = 1f;   // Duration for the fade-out effect

    private Color startColor;         // Initial color with alpha set to 1
    private Color targetColor;        // Target color with alpha set to 0

    public AnimationCurve fadeCurve;

    void Start()
    {
        // Get the Image component if not assigned
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        // Initialize colors
        startColor = image.color;
        targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Start the fade-out effect
        StartCoroutine(FadeOutAlpha());
    }

    IEnumerator FadeOutAlpha()
    {
        float elapsedTime = 0f;

        // Ensure the starting alpha is 1
        image.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / fadeDuration;
            float curvedT = fadeCurve.Evaluate(normalizedTime);

            float alpha = Mathf.Lerp(1f, 0f, curvedT);
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Ensure alpha is set to 0 at the end
        image.color = targetColor;
    }
}