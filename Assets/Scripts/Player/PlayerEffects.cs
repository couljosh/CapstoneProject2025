using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    //references to gameobject
    private PlayerMove playerMove;

    //light effects
    public GameObject playerLight;
    private Light spotLight;
    private float initialRange;
    [HideInInspector] public bool chargingMaxKick = false;
    private float lightFlashTimer = 0;
    public float flashInterval;
    private bool flashOn = true;


    private void Start()
    { 
        initialRange = playerLight.GetComponent<Light>().range;
        spotLight = playerLight.GetComponent<Light>();
        playerMove = gameObject.GetComponent<PlayerMove>();
    }
    public void KickEffects(float currentRatio)
    {
        spotLight.range = initialRange * (currentRatio / 1);

        if (playerMove.chargingKick && currentRatio >= 1)
        {
            chargingMaxKick = true;
        }
        else
        {
            chargingMaxKick = false;
        }
            
    }

    private void Update()
    {
        //LIGHT FLASH AT FULL CHARGE
        if (chargingMaxKick)
        {
            lightFlashTimer += Time.deltaTime;

            if (lightFlashTimer > flashInterval)
            {
                lightFlashTimer = 0;
                flashOn = !flashOn;
            }
        }
        else
        {
            flashOn = true;
        }

        spotLight.enabled = flashOn;
    }
}
