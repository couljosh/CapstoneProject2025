using UnityEngine;
using UnityEngine.TerrainUtils;

public class CartContact : MonoBehaviour
{

    public float DetectionDist;
    public float verticalOffset;
    public LayerMask terrainMask;
    public LayerMask playerMask;
    private Vector3 offsetPos;
    public GameObject rayPosLeft;
    public GameObject rayPosMid;
    public GameObject rayPosRight;

    public Rigidbody rb;

    public float terrainThreshold;
    public float playerThreshold;

    public float terrainSpeedReduction;
    public float playerSpeedReduction;

    private void Start()
    {
    }

    void Update()
    {
        //    RaycastHit hit;

        //    if (Physics.Raycast(rayPosLeft.transform.position, rayPosLeft.transform.TransformDirection(Vector3.forward), out hit, DetectionDist, terrainMask | playerMask, QueryTriggerInteraction.Ignore) |
        //        Physics.Raycast(rayPosMid.transform.position, rayPosMid.transform.TransformDirection(Vector3.forward), out hit, DetectionDist, terrainMask | playerMask, QueryTriggerInteraction.Ignore) |
        //        Physics.Raycast(rayPosRight.transform.position, rayPosRight.transform.TransformDirection(Vector3.forward), out hit, DetectionDist, terrainMask | playerMask, QueryTriggerInteraction.Ignore))
        //    {
        //        Debug.DrawRay(rayPosLeft.transform.position, rayPosLeft.transform.TransformDirection(Vector3.forward) * DetectionDist, Color.green);
        //        Debug.DrawRay(rayPosMid.transform.position, rayPosMid.transform.TransformDirection(Vector3.forward) * DetectionDist, Color.green);
        //        Debug.DrawRay(rayPosRight.transform.position, rayPosRight.transform.TransformDirection(Vector3.forward) * DetectionDist, Color.green);

        //        if(hit.collider != null)
        //        {
        //            hit.collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();

        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawRay(rayPosLeft.transform.position, rayPosLeft.transform.TransformDirection(Vector3.forward) * DetectionDist, Color.red);
        //        Debug.DrawRay(rayPosMid.transform.position, rayPosMid.transform.TransformDirection(Vector3.forward) * DetectionDist, Color.red);
        //        Debug.DrawRay(rayPosRight.transforwm.position, rayPosRight.transform.TransformDirection(Vector3.forward) * DetectionDist, Color.red);
        //    }
        //}

    }

    private void OnTriggerEnter(Collider collision)
    {

        if(collision.gameObject.tag == "ActiveTerrain" && rb.linearVelocity.magnitude > terrainThreshold)
        {
            collision.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
            rb.linearVelocity = rb.linearVelocity * (1 - terrainSpeedReduction/100);
        }

        if (collision.gameObject.tag == "ObjectDestroyer" && rb.linearVelocity.magnitude > playerThreshold)
        {
            print("HIT");
            collision.gameObject.GetComponent<PlayerDeath>().PlayerDie();
            rb.linearVelocity = rb.linearVelocity * (1 - playerSpeedReduction / 100);
        }
    }


}
