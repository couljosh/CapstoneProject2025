using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class MinecartMovement : MonoBehaviour
{
    public SplineContainer track;
    public Rigidbody rb;
    public Spline currentSpline;


    void Start()
    {
         rb = GetComponent<Rigidbody>();
        currentSpline = track.Splines[0];

        rb.linearVelocity = Vector3.zero;

    }

    private void FixedUpdate()
    {
        //Snap cart to always go to the nearest point
        var native = new NativeSpline(currentSpline);
        SplineUtility.GetNearestPoint(native, transform.position, out float3 nearest, out float t);

        transform.position = nearest;

        //Get the Vector of towards that nearest point
        Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
        Vector3 up = native.EvaluateTangent(t);

        //Create Vectors to rotate cart to match track
        var remappedForward = new Vector3(0, 0, 1);
        var remappedup = new Vector3(0, 1, 0);
        var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedup));
        transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

        //Get the forward vector the cart is currently going
        Vector3 forwardDir = transform.forward;

        //Dot Product to check if cart is going backwards
        if(Vector3.Dot(rb.linearVelocity, forwardDir) < 0)
        {
            forwardDir *= -1;
        }

        //Move the rigid body based on the direction
        rb.linearVelocity = rb.linearVelocity.magnitude * forwardDir;

        Debug.Log(rb.linearVelocity);

        if(rb.linearVelocity.magnitude < 0.5)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
