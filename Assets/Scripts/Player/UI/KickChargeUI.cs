using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KickChargeUI : MonoBehaviour
{
    [Header("UI References")]
    public Image kickChargeBar;
    public Image kickChargeIcon;
    public RawImage kickChargeBackground;

    [Header("Color Settings")]
    public Color startColor = Color.white;
    public Color midColor = Color.red;
    public Color maxColorDark = new Color32(139, 0, 0, 255);

    [Header("Thresholds")]
    public float maxChargeThreshold = 0.8f;
    public float flashSpeed = 0.1f;

    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        kickChargeBackground.enabled = false;
        kickChargeIcon.enabled = false;
        kickChargeBar.fillAmount = 0f;
        kickChargeBar.color = startColor;
    }

    public void UpdateChargeBar(float normalizedCharge, bool isCharging)
    {
        kickChargeIcon.enabled = isCharging;
        kickChargeBackground.enabled = isCharging;

        if (isCharging)
        {
            kickChargeBar.fillAmount = normalizedCharge;

            if (normalizedCharge < maxChargeThreshold)
            {
                if (isFlashing)
                {
                    StopFlashing();
                }

                float t = Mathf.InverseLerp(0f, maxChargeThreshold, normalizedCharge);
                kickChargeBar.color = Color.Lerp(startColor, midColor, t);
            }
            else
            {
                if (!isFlashing)
                {
                    StartFlashing();
                }
            }
        }
        else
        {
            if (isFlashing)
            {
                StopFlashing();
            }

            kickChargeBar.fillAmount = 0f;
            kickChargeBar.color = startColor;
        }
    }

    private void StartFlashing()
    {
        isFlashing = true;
        flashCoroutine = StartCoroutine(FlashAnimation());
    }

    private void StopFlashing()
    {
        isFlashing = false;
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        kickChargeBar.color = midColor;
    }

    private IEnumerator FlashAnimation()
    {
        while (isFlashing)
        {
            kickChargeBar.color = midColor;
            yield return new WaitForSeconds(flashSpeed);

            kickChargeBar.color = maxColorDark;
            yield return new WaitForSeconds(flashSpeed);
        }
    }
}