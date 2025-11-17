using System.Collections;
using TMPro;
using UnityEngine;

public class DepositScoreDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI depositedText;

    [Header("Display Settings")]
    public float displayDuration = 1.5f;
    public float floatDistance = 1.2f;
    public float floatDepthOffset = 0.5f;

    private Vector3 initialLocalPosition;
    private float initialWorldZ;

    void Awake()
    {
        if (depositedText == null)
        {
            Debug.LogError("DepositScoreDisplay requires a TextMeshPro component to be assigned.");
            enabled = false;
        }

        //initial local position
        initialLocalPosition = depositedText.transform.localPosition;
        depositedText.enabled = false;
    }

    public void ShowScore(int scoreValue, Color teamColor, float repoInitialWorldZ)
    {
        //stop any previous display coroutine
        StopAllCoroutines();

        depositedText.text = $"+{scoreValue}";

        Color faceColor = teamColor;
        faceColor.a = 1f;
        depositedText.color = faceColor;

        depositedText.transform.localPosition = initialLocalPosition;
        initialWorldZ = repoInitialWorldZ;

        StartCoroutine(DisplayScoreCoroutine());
    }

    private IEnumerator DisplayScoreCoroutine()
    {
        depositedText.enabled = true;
        float elapsedTime = 0f;

        Vector3 targetLocalPosition = initialLocalPosition + new Vector3(0, floatDistance, floatDepthOffset);

        Transform repoTransform = transform.parent;
        if (repoTransform == null)
        {
            repoTransform = transform;
        }


        while (elapsedTime < displayDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / displayDuration;

            //get local pos for lerp
            Vector3 animatedLocalPos = Vector3.Lerp(initialLocalPosition, targetLocalPosition, t);

            //counteract the lowering of the repository so the text doesnt go into the ground
            float currentRepoWorldZ = repoTransform.position.z;
            float zDifference = currentRepoWorldZ - initialWorldZ;
            float zCorrection = -zDifference;
            zCorrection += -3f;

            animatedLocalPos.z += zCorrection;

            depositedText.transform.localPosition = animatedLocalPos;

            //fade out text
            Color color = depositedText.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            depositedText.color = color;

            yield return null;
        }

        depositedText.enabled = false;

        //restore opacity and pos
        Color finalColor = depositedText.color;
        finalColor.a = 1f;
        depositedText.color = finalColor;
        depositedText.transform.localPosition = initialLocalPosition;
    }
}