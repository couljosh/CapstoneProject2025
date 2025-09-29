using TMPro;
using UnityEngine;

public class TextNotification : MonoBehaviour
{
    [Header("Notification Customization")]
    private float timer = 0;
    public float timeToFlash;
    public int flashesBeforeDestroy;
    private int flashes = 0;
    public Color32 firstFlashColor;
    public Color32 secondFlashColor;

    // Notification Sequence
    void Update()
    {
        timer += Time.deltaTime;
        if(flashes > flashesBeforeDestroy)
        {
            Destroy(gameObject);
        }

        if(timer > timeToFlash)
        {
            TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();

            if (text.color == firstFlashColor)
            {
                text.color  = secondFlashColor;
            }
            else
            {
                text.color = firstFlashColor;
            }

            flashes++;
            timer = 0;
        }
    }
}
