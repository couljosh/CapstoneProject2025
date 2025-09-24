using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Events;
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
    public float moveSpeed;
    public float rotateSpeed;

    public int playerNum;


    [Header("Kick Variables")]
    public float rayLength;
    public GameObject rayStartPosOne;
    public GameObject rayStartPosTwo;
    public GameObject rayStartPosThree;
    public LayerMask kickable;
    public float kickStrength;



    private void Awake()
    {
        moveAction = inputActions.FindActionMap("Player1").FindAction("Move");

        spawnBombAction = inputActions.FindActionMap("Player1").FindAction("Spawn Bomb");

        kickAction = inputActions.FindActionMap("Player1").FindAction("Kick");

        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveAmt = context.ReadValue<Vector2>();
    }

    public void OnKick(InputAction.CallbackContext context)
    {
        RaycastHit hit;

        if (Physics.Raycast(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable) ||
            Physics.Raycast(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable) ||
            Physics.Raycast(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable))
        {
            Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
            Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
                Debug.Log("reached");
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * kickStrength);
            
        }
    }

    void Update()
    {
        Debug.DrawRay(rayStartPosOne.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
        Debug.DrawRay(rayStartPosTwo.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
        Debug.DrawRay(rayStartPosThree.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);

       
    }

    public void FixedUpdate()
    {
        Move(moveAmt);
    }

    public void Move(Vector3 direction)
    {
        rb.linearVelocity = new Vector3(direction.x, 0, direction.y) * moveSpeed;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }

    }
}

