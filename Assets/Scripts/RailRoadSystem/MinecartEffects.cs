using FMOD;
using UnityEngine;

public class MinecartEffects : MonoBehaviour
{
    private Material defaultMaterial;
    public Material playerKillSpeedMaterial;

    private CartContact cartContact;
    private Rigidbody rb;
    public MeshRenderer modelMeshRenderer;

    public Outline outline;
    public Color32 lowSpeedOutlineColor;
    public Color32 highSpeedOutlineColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultMaterial = modelMeshRenderer.material;
        cartContact = gameObject.GetComponentInChildren<CartContact>();
        rb = gameObject.GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.magnitude > cartContact.playerThreshold)
        {
            modelMeshRenderer.material = playerKillSpeedMaterial;
            outline.OutlineColor = highSpeedOutlineColor;

        }
        else
        {
            modelMeshRenderer.material = defaultMaterial;
            outline.OutlineColor = lowSpeedOutlineColor;
        }
    }
}
