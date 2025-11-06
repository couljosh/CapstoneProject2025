using UnityEngine;
using UnityEngine.UI;

public class WorldToScreenUI : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public RectTransform barElement;

    [Header("Screen Space Offset")]
    [HideInInspector] public float screenHeightOffset;

    private Camera mainCamera;

    void Start()
    {
        //main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("WorldToScreenUI: Main Camera not found in the scene.");
            enabled = false;
        }

    }

    //LateUpdate for after player movement
    void LateUpdate()
    {
        if (target != null && mainCamera != null && barElement != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(target.position);
            viewportPos.y += screenHeightOffset;

            Vector3 screenPos = mainCamera.ViewportToScreenPoint(viewportPos);
            barElement.position = screenPos;
        }
    }
}