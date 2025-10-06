using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{

    //references to animators
    [HideInInspector] public Animator copperAnimator;

    //references to gameobject
    private PlayerMove playerMove;

    public Light spotLight;
    private float initialRange;
    [HideInInspector] public bool chargingMaxKick = false;
    private float lightFlashTimer = 0;
    public float flashInterval;
    private bool flashOn = true;


    private void Start()
    {
        initialRange = spotLight.range;
        playerMove = gameObject.GetComponent<PlayerMove>();
        copperAnimator = GetComponent<Animator>();

        //Set all bool animators to false
        copperAnimator.SetBool("isRunning", false);
        copperAnimator.SetBool("isCharging", false);
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
        if (playerMove.moveAmt.magnitude > 0)
        {
            copperAnimator.SetBool("isRunning", true);
        }
        else if (playerMove.moveAmt.magnitude == 0)
        {

            copperAnimator.SetBool("isRunning", false);
        }

        spotLight.enabled = flashOn;
    }
}
