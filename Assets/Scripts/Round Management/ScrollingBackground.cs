using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 0.02f;
    private Material backgroundMaterial;

    void Start()
    {
            backgroundMaterial = GetComponent<Image>().material;
    }

    void Update()
    {
        if (backgroundMaterial != null)
        {
            float offsetX = Time.time * scrollSpeed;
            backgroundMaterial.SetTextureOffset("_MainTex", new Vector2(offsetX, 0)); //replay the texture with the material on it.
        }
    }
}