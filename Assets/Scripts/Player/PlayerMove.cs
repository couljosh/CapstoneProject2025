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
    private Vector3 moveAmt = Vector3.zero;
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
    public float cartForceMultiplier;
    public float maximumKickMultiplier;
    public float timeToMaxStrength;
    public float timeBeforePlayerSlowWhenCharge;
    public float maxPlayerChargeSlowdown;
    public float currentKickStrength;
    private float kickStrengthTimer = 0;
    [HideInInspector] public bool chargingKick = false;

    //Effects Handling
    private PlayerEffects playerEffects;
    private PlayerDeath playerDeath;

    private void Awake()
    {
        moveAction = inputActions.FindActionMap("Player1").FindAction("Move");

        spawnBombAction = inputActions.FindActionMap("Player1").FindAction("Spawn Bomb");

        kickAction = inputActions.FindActionMap("Player1").FindAction("Kick");
        
        
        

        rb = GetComponent<Rigidbody>();

        playerEffects = GetComponent<PlayerEffects>();

        playerDeath = GetComponent<PlayerDeath>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveAmt = context.ReadValue<Vector2>();
    }

    //called when the player presses down the kick button
    public void KickPerformed(InputAction.CallbackContext context)
    {
        
            if (context.performed && !playerDeath.isPlayerDead)
            {
                chargingKick = true;
            }
            else if (context.canceled)
            {
                KickCanceled(context);
            }
        
    }

    //called when the player releases the kick button
    private void KickCanceled(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            
            if(hit.collider.gameObject.tag == "Bomb")
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * currentKickStrength);

            }
            
            if(hit.collider.gameObject.tag == "Cart")
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * (currentKickStrength * cartForceMultiplier));

            }
        }

        //reset to normal light length
        playerEffects.KickEffects(1);
        playerEffects.chargingMaxKick = false; 
    
        //reset charge tracking
        chargingKick = false;
        currentKickStrength = 0;
        kickStrengthTimer = 0;
    }

    void Update()
    {
        //print(gameObject.name + " " + chargingKick);

        Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
        Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
        Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);

        if(chargingKick)
        {
            kickStrengthTimer += Time.deltaTime;
            currentKickStrength = initialKickStrength * (maximumKickMultiplier * kickStrengthTimer / timeToMaxStrength);
            //Mathf.Clamp(currentKickStrength, initialKickStrength, maximumKickMultiplier * initialKickStrength);
            kickStrengthTimer = Mathf.Clamp(kickStrengthTimer, 0, timeToMaxStrength);
            playerEffects.KickEffects(kickStrengthTimer/timeToMaxStrength);
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
            chargingKick = false ;
        }
    }

    public void Move(Vector3 direction)
    {
       
        //progressively dampen move speed by charging a kick
        if (kickStrengthTimer > timeBeforePlayerSlowWhenCharge && chargingKick)
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

