using TMPro;
using UnityEngine;

public class TextNotification : MonoBehaviour
{
    [Header("Notification Customization")]
    private float elapsedTime = 0;
    public float displayTime;

    private Animator caveInAnimator;
    public RuntimeAnimatorController caveInAnimControl;

    private void Start()
    {
        caveInAnimator = GetComponent<Animator>();

        caveInAnimator.runtimeAnimatorController = caveInAnimControl;
    }

    // Notification Sequence
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= displayTime)
        {
            Destroy(gameObject);
        }
    }

}
