using UnityEngine;
using UnityEngine.UI;

public class WorldToScreenUI : MonoBehaviour
{
    public Transform target;
    public Camera mainCamera;
    public RectTransform barElement;
    public RectTransform barBG;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + Vector3.up * 2f); //convert to screen pos and lift it up
        barElement.position = screenPos;
        barBG.position = screenPos;

        
        
    }
}
