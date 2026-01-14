using Unity.VisualScripting;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [HideInInspector] public Animator copperAnimator;
    private PlayerMove playerMove;

    public Light spotLight;
    private float initialRange;
    [HideInInspector] public bool chargingMaxKick = false;
    private float lightFlashTimer = 0;
    public float flashInterval;
    private bool flashOn = true;

    public ParticleSystem chargeableKick;
    private ParticleSystem.MainModule kickMain;
    private ParticleSystem.EmissionModule kickEmission;

    [Header("Kick Release VFX")]
    public ParticleSystem releaseVFX1;
    public ParticleSystem releaseVFX2;


    private void Start()
    {
        initialRange = spotLight.range;
        playerMove = gameObject.GetComponent<PlayerMove>();
        copperAnimator = GetComponent<Animator>();

        copperAnimator.SetBool("isRunning", false);
        copperAnimator.SetBool("isCharging", false);

        kickMain = chargeableKick.main;
        kickEmission = chargeableKick.emission;

        kickEmission.rateOverTime = 0f;
        kickMain.startSizeMultiplier = 0.5f;
        kickMain.startSpeedMultiplier = -9f;
    }

    public void KickEffects(float currentRatio)
    {
        spotLight.range = initialRange * (currentRatio / 1);

        if (playerMove.chargingKick && currentRatio >= 1)
        {
            chargingMaxKick = true;

            kickEmission.rateOverTime = 40f;
            kickMain.startSizeMultiplier = 1f;
            kickMain.startSpeedMultiplier = -18f;

            if (!chargeableKick.isPlaying)
                chargeableKick.Play();
        }
        else
        {
            chargingMaxKick = false;

            kickEmission.rateOverTime = 0f;
            kickMain.startSizeMultiplier = 0.4f;
            kickMain.startSpeedMultiplier = -9f;
        }
    }

    private void Update()
    {
        if (playerMove.chargingKick && chargingMaxKick == false)
        {
            kickEmission.rateOverTime = 20f;

            if (!chargeableKick.isPlaying)
                chargeableKick.Play();
        }
        else if (chargingMaxKick == false)
        {
            kickEmission.rateOverTime = 0f;

            if (chargeableKick.isPlaying)
                kickChargeableStop(chargeableKick);
        }
        else
        {
            kickEmission.rateOverTime = 40f;
        }

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

        if (playerMove.moveAmt.magnitude > 0 && playerMove.powerUpPickupScript.activePowerup == null) //only animate movement normally if player is not in a powerup
        {
            copperAnimator.SetBool("isRunning", true);
        }
        else if (playerMove.moveAmt.magnitude == 0 || playerMove.powerUpPickupScript.activePowerup != null) //kill anim if powerup is entered
        {
            copperAnimator.SetBool("isRunning", false);
        }

        spotLight.enabled = flashOn;
    }

    //playing and scaling of VFX2
    public void PlayKickReleaseVFX(float chargeRatio)
    {
        const float minimumMultiplier = 0.05f;
        const float maximumMultiplier = 0.25f;

        float finalSizeMultiplier = Mathf.Lerp(minimumMultiplier, maximumMultiplier, chargeRatio);

        //always plays, size is always scaled
        if (releaseVFX1 != null)
        {
            var main1 = releaseVFX1.main;
            main1.startSizeMultiplier = finalSizeMultiplier;

            releaseVFX1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            releaseVFX1.Play();
        }

        //only if 50% charged or more
        if (chargeRatio >= 0.5f)
        {
            if (releaseVFX2 != null)
            {
                var main2 = releaseVFX2.main;
                main2.startSizeMultiplier = finalSizeMultiplier;

                releaseVFX2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                releaseVFX2.Play();
            }
        }
        //if charge is less than 50%  stop VFX2
        else
        {
            if (releaseVFX2 != null)
            {
                releaseVFX2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }


    private void kickChargeableStop(ParticleSystem ps)
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}