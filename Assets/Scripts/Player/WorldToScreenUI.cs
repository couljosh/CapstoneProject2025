using UnityEngine;
using UnityEngine.UI;

public class WorldToScreenUI : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public RectTransform barElement;
    [HideInInspector] public Vector3 uiOffset = new Vector3(0f, 2f, 0f); //offset

    private Camera mainCamera;

    void Start()
    {
        //find main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("WorldToScreenUI: Main Camera not found in the scene.");
            enabled = false;
        }

        if (target == null || barElement == null)
        {
            Debug.LogError("WorldToScreenUI: Target or barElement is not set. Check PlayerUIManager.");
            enabled = false;
        }
    }

    void LateUpdate() //LateUpdate to make sure player movement is complete
    {
        if (target != null && mainCamera != null)
        {
            //convert player world pos + offset to screen pos
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + uiOffset);

            //update UI position
            barElement.position = screenPos;
        }
    }
}