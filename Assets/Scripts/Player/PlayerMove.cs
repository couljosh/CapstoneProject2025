using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerMove : MonoBehaviour
{

    [Header("Input Variables")]
    //Action Map
    public InputActionAsset inputActions;

    //Actions
    private InputAction moveAction;
    private InputAction kickAction;
    private InputAction spawnBombAction;

    //Movement Value
    [HideInInspector] public Vector3 moveAmt = Vector3.zero;
    private Rigidbody rb;
    public float initialMoveSpeed;
    public float rotateSpeed;
    private float finalMoveSpeed;

    public int playerNum;

    [Header("Kick Variables")]
    public float rayLength;
    public GameObject rayStartPosOne;
    public GameObject rayStartPosTwo;
    public GameObject rayStartPosThree;
    public LayerMask kickable;
    public float initialKickStrength;
    public float maximumKickMultiplier;
    public float timeToMaxStrength;
    public float timeBeforePlayerSlowWhenCharge;
    public float maxPlayerChargeSlowdown;
    private float currentKickStrength;
    private float kickStrengthTimer = 0;
    [HideInInspector] public bool chargingKick = false;

    //Effects Handling
    private PlayerEffects playerEffects;


    private void Awake()
    {
        moveAction = inputActions.FindActionMap("Player1").FindAction("Move");

        spawnBombAction = inputActions.FindActionMap("Player1").FindAction("Spawn Bomb");

        kickAction = inputActions.FindActionMap("Player1").FindAction("Kick");
        kickAction.performed += KickPerformed;
        kickAction.canceled += KickCanceled;

        rb = GetComponent<Rigidbody>();

        playerEffects = GetComponent<PlayerEffects>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveAmt = context.ReadValue<Vector2>();
    }

    //called when the player presses down the kick button
    public void KickPerformed(InputAction.CallbackContext context)
    {
        print("Kick!");
        playerEffects.copperAnimator.SetBool("isCharging", false);
        playerEffects.copperAnimator.SetTrigger("Kick");
        chargingKick = true;
    }

    //called when the player releases the kick button
    public void KickCanceled(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable) ||
            Physics.Raycast(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable) ||
            Physics.Raycast(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable))
        {
            Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);

            hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * currentKickStrength);
           
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
    }

    void Update()
    {
        Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
        Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
        Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);

        if(chargingKick)
        {
            print("Charging Kick...");
            playerEffects.copperAnimator.SetBool("isCharging", true);
            kickStrengthTimer += Time.deltaTime;
            currentKickStrength = initialKickStrength * (maximumKickMultiplier * kickStrengthTimer / timeToMaxStrength);
            //Mathf.Clamp(currentKickStrength, initialKickStrength, maximumKickMultiplier * initialKickStrength);
            kickStrengthTimer = Mathf.Clamp(kickStrengthTimer, 0, timeToMaxStrength);
            playerEffects.KickEffects(kickStrengthTimer/timeToMaxStrength);
        }
        else
        {
            playerEffects.copperAnimator.SetBool("isCharging", false);
        }
        
    }

    public void FixedUpdate()
    {
        Move(moveAmt);
    }

    public void Move(Vector3 direction)
    {
        //progressively dampen move speed by charging a kick
        if(kickStrengthTimer > timeBeforePlayerSlowWhenCharge && chargingKick)
        {
            finalMoveSpeed = initialMoveSpeed - (initialMoveSpeed * (kickStrengthTimer / timeToMaxStrength));
            finalMoveSpeed = Mathf.Clamp(finalMoveSpeed, maxPlayerChargeSlowdown, Mathf.Infinity);
        }
        else
        {
            finalMoveSpeed = initialMoveSpeed;
        }


            rb.linearVelocity = new Vector3(direction.x, 0, direction.y) * finalMoveSpeed;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);

        }

    }
}

