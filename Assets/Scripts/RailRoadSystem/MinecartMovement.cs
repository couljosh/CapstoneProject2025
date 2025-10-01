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
        track = FindAnyObjectByType<SplineContainer>();
        currentSpline = track.Splines[0];


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

        // Rotate the cart to align with the spline
        transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

        // Steer velocity toward spline direction without overriding it completely
        if (rb.linearVelocity.magnitude > 0.05f)
        {
            Vector3 forwardDir = transform.forward;

            // If the velocity is going the opposite direction, flip the forward
            if (Vector3.Dot(rb.linearVelocity, transform.forward) < 0)
            {
                forwardDir *= -1;

            }

            // Gradually steer velocity toward the forward direction
            Vector3 steeredVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity.magnitude * forwardDir, 0.1f);
            rb.linearVelocity = steeredVelocity;
        }
    }
}
