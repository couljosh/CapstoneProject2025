using TMPro;
using UnityEngine;

public class DepositPreviewWorldText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float snapHideThreshold = 0.5f;

    private bool isVisible;

    void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();

        SetVisible(false);
    }

    //added largeBonus parameter and color
    public void SetValue(int current, int max, int largeBonus)
    {
        string smallGemText = $"{current}/{max}";

        if (largeBonus > 0)
        {
            text.text = $"{smallGemText} +{largeBonus}";
        }
        else
        {
            text.text = smallGemText;
        }
    }

    public void SetColor(Color c)
    {
        //turn the entire string to the target color
        text.color = c;
    }

    public void SetVisible(bool visible)
    {
        if (isVisible == visible)
            return;

        isVisible = visible;
        text.enabled = visible;
    }
}