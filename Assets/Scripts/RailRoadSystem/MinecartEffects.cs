using FMOD;
using UnityEngine;

public class MinecartEffects : MonoBehaviour
{
    private Material defaultMaterial;
    public Material playerKillSpeedMaterial;

    private CartContact cartContact;
    private MinecartMovement minecartMovement;
    private Rigidbody rb;
    public MeshRenderer modelMeshRenderer;

    public Outline outline;
    public Color32 lowSpeedOutlineColor;
    public Color32 highSpeedOutlineColor;

    public bool isPowered;

    public Light frontLight;
    public Light backLight;
    public Color lowSpeedLight;
    public Color highSpeedLight;

    public float lowSpeedIntensity;
    public float highSpeedIntensity;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultMaterial = modelMeshRenderer.material;
        cartContact = gameObject.GetComponentInChildren<CartContact>();
        minecartMovement = gameObject.GetComponent<MinecartMovement>();
        rb = gameObject.GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (rb.linearVelocity.magnitude > cartContact.playerThreshold || isPowered)
        {
            modelMeshRenderer.material = playerKillSpeedMaterial;
            outline.OutlineColor = highSpeedOutlineColor;

            if (minecartMovement.isForward)
            {
                //danger lights
                frontLight.color = highSpeedLight;
                frontLight.intensity = highSpeedIntensity;

                //force opposite lights back to normal
                backLight.color = lowSpeedLight;
                backLight.intensity = lowSpeedIntensity;

            }

            if (!minecartMovement.isForward)
            {
                //danger lights
                backLight.color = highSpeedLight;
                backLight.intensity = highSpeedIntensity;

                //force opposite lights back to normal
                frontLight.color = lowSpeedLight;
                frontLight.intensity = lowSpeedIntensity;
            }
        }
        else
        {
            modelMeshRenderer.material = defaultMaterial;
            outline.OutlineColor = lowSpeedOutlineColor;

            //reset light values
            frontLight.color = lowSpeedLight;
            backLight.color = lowSpeedLight;
            frontLight.intensity = lowSpeedIntensity;
            backLight.intensity = lowSpeedIntensity;
        }
    }
}
