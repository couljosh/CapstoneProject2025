using UnityEngine;
using UnityEngine.UI;

public class KickChargeUI : MonoBehaviour
{
    [Header("UI References")]
    public Image kickChargeBar;
    public Image kickChargeIcon;
    public RawImage kickChargeBackground;

    private void Awake()
    {
        kickChargeBackground.enabled = false;
        kickChargeIcon.enabled = false;
        kickChargeBar.fillAmount = 0f;
        kickChargeBar.color = Color.white;
    }

    public void UpdateChargeBar(float normalizedCharge, bool isCharging)
    {
        kickChargeIcon.enabled=isCharging;
        kickChargeBackground.enabled = isCharging;

        if (isCharging)
        {
            kickChargeBar.fillAmount = normalizedCharge;

            if ((normalizedCharge >= 0.45f) && (normalizedCharge < 0.85f))
            {
            }
            else if (normalizedCharge >= 0.85f)
            {
                kickChargeBar.color = Color.red;
            }
            else
            {
                kickChargeBar.color = Color.white;
            }
        }
        else
        {
            //reset
            kickChargeBar.fillAmount = 0f;
            kickChargeBar.color = Color.white;
        }
    }
}