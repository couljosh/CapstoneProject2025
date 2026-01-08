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

    public void SetValue(int current, int max)
    {
        if (max <= 0)
            return;

        text.text = $"{current}/{max}";
    }

    public void SetColor(Color c)
    {
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
