using FMODUnity;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("Reference Reference")]
    public PlayerStats playerStats;
    private PlayerEffects playerEffects;
    private PlayerDeath playerDeath;
    public int playerNum;

    private KickChargeUI kickChargeUI; //ref to ui component for charge kick

    [Header("Input Variables")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction kickAction;
    private InputAction spawnBombAction;

    [Header("Movement Variables")]
    [HideInInspector] public Vector3 moveAmt = Vector3.zero;
    private Rigidbody rb;
    private float finalMoveSpeed;
    private float coyoteTimer = 0;

    [Header("Kick Variables")]
    public GameObject rayStartPosOne;
    public GameObject rayStartPosTwo;
    public GameObject rayStartPosThree;
    public LayerMask kickable;
    public LayerMask player;
    public LayerMask floor;

    private float currentKickStrength;
    private float kickStrengthTimer = 0;
    [HideInInspector] public bool chargingKick = false;
    private bool chargedEnough;

    [Header("Controller Variables")]
    private float normalizedRumble;


    private void Awake()
    {
        moveAction = inputActions.FindActionMap("Player1").FindAction("Move");
        spawnBombAction = inputActions.FindActionMap("Player1").FindAction("Spawn Bomb");
        kickAction = inputActions.FindActionMap("Player1").FindAction("Kick");

        rb = GetComponent<Rigidbody>();
        playerEffects = GetComponent<PlayerEffects>();
        playerDeath = GetComponent<PlayerDeath>();

        // kickChargeBackground.enabled = false; 
    }

    public void SetKickChargeUI(KickChargeUI ui)
    {
        kickChargeUI = ui;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveAmt = context.ReadValue<Vector2>();
    }


    //Called when the player presses down the kick button
    public void KickPerformed(InputAction.CallbackContext context)
    {
        if (context.performed && !playerDeath.isPlayerDead)
        {
            playerEffects.copperAnimator.SetBool("isCharging", false);
            playerEffects.copperAnimator.SetTrigger("Kick");
            chargingKick = true;
        }
        else if (context.canceled)
        {
            KickCanceled(context);
        }
    }

    //Called when the player releases the kick button
    private void KickCanceled(InputAction.CallbackContext context)
    {
        //Gamepad.current.SetMotorSpeeds(0, 0);
        RaycastHit playerHit;
        if (Physics.Raycast(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward), out playerHit, playerStats.kickDectDist, player, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward), out playerHit, playerStats.kickDectDist, player, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward), out playerHit, playerStats.kickDectDist, player, QueryTriggerInteraction.Ignore))
        {
            if (chargedEnough)
                playerHit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * (currentKickStrength * playerStats.playerForceMultiplier));
        }

        RaycastHit hit;
        if (Physics.Raycast(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward), out hit, playerStats.kickDectDist, kickable, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward), out hit, playerStats.kickDectDist, kickable, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward), out hit, playerStats.kickDectDist, kickable, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * playerStats.kickDectDist, Color.green);
            Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * playerStats.kickDectDist, Color.green);
            Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * playerStats.kickDectDist, Color.green);

            if (hit.collider.gameObject.tag == "Bomb")
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * currentKickStrength);
            }

            if (hit.collider.gameObject.tag == "Cart")
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * (currentKickStrength * playerStats.cartForceMultiplier));
            }

            if (hit.collider.gameObject.tag == "LargeGem")
            {
                hit.collider.GetComponentInParent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * currentKickStrength);
            }

            if (hit.collider.gameObject.tag == "RockObstacle")
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * currentKickStrength * playerStats.rockForceMultiplier);
            }
        }

        //reset to normal light length
        playerEffects.KickEffects(1);
        playerEffects.chargingMaxKick = false;

        //reset charge tracking
        chargingKick = false;
        currentKickStrength = 0;
        kickStrengthTimer = 0;

        playerEffects.copperAnimator.ResetTrigger("Kick");
        playerEffects.copperAnimator.SetBool("isCharging", false);

        if (kickChargeUI != null)
        {
            kickChargeUI.UpdateChargeBar(0f, false);
        }
    }


    void Update()
    {
        Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * playerStats.kickDectDist, Color.red);
        Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * playerStats.kickDectDist, Color.red);
        Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * playerStats.kickDectDist, Color.red);

        if (chargingKick)
        {
            //kickChargeBackground.enabled = true; // Moved to KickChargeUI.UpdateChargeBar

            playerEffects.copperAnimator.SetBool("isCharging", true);
            kickStrengthTimer += Time.deltaTime;
            kickStrengthTimer = Mathf.Clamp(kickStrengthTimer, 0, playerStats.timeToMaxStrength);

            //normalize charge for UI and strength
            float normalizedCharge = kickStrengthTimer / playerStats.timeToMaxStrength;

            currentKickStrength = playerStats.initialKickStrength * (playerStats.maximumKickMultiplier * normalizedCharge);

            playerEffects.KickEffects(normalizedCharge);

            //normalizedRumble = ((currentKickStrength / 2 - 0) / ((initialKickStrength * maximumKickMultiplier) - 0)) / 10;
            //Gamepad.current.SetMotorSpeeds(normalizedRumble, normalizedRumble);

            if (kickChargeUI != null)
            {
                kickChargeUI.UpdateChargeBar(normalizedCharge, true);
            }

            if (normalizedCharge >= 0.45f)
            {
                chargedEnough = true;
                playerEffects.chargingMaxKick = normalizedCharge >= 0.85f;
            }
            else
            {
                chargedEnough = false;
                playerEffects.chargingMaxKick = false;
            }
        }
        else
        {
            playerEffects.copperAnimator.SetBool("isCharging", false);

            if (kickChargeUI != null)
            {
                kickChargeUI.UpdateChargeBar(0f, false);
            }

        }
    }


    public void FixedUpdate()
    {
        if (!playerDeath.isPlayerDead)
        {
            Move(moveAmt);
        }
        else
        {
            chargingKick = false;
        }
    }


    public void Move(Vector3 direction)
    {
        //progressively dampen move speed by charging a kick
        if (kickStrengthTimer > playerStats.timeBeforePlayerSlowWhenCharge && chargingKick)
        {
            finalMoveSpeed = playerStats.initialMoveSpeed - (playerStats.initialMoveSpeed * (kickStrengthTimer / playerStats.timeToMaxStrength));
            finalMoveSpeed = Mathf.Clamp(finalMoveSpeed, playerStats.maxPlayerChargeSlowdown, Mathf.Infinity);
        }
        else
        {
            finalMoveSpeed = playerStats.initialMoveSpeed;
        }

        if (rb.linearVelocity.magnitude < finalMoveSpeed)
        {
            rb.AddForce(new Vector3(direction.x, 0f, direction.y) * finalMoveSpeed, ForceMode.VelocityChange);
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * playerStats.rotateSpeed);

        }

        //simulate gravity - if there is no floor under the CENTER of the player, to give a bit of leniency
        if (!Physics.BoxCast(gameObject.transform.position, transform.localScale * 0.5f, Vector3.down, Quaternion.identity, 3, floor))
        {
            coyoteTimer += Time.deltaTime;

            if (coyoteTimer > playerStats.coyoteTimeThreshold)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -playerStats.gravity, rb.linearVelocity.z);
            }
        }
        else
            coyoteTimer = 0;
    }
}