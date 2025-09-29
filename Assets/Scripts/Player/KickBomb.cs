using UnityEngine;
using UnityEngine.TerrainUtils;

public class KickBomb : MonoBehaviour
{

    public float rayLength;
    public GameObject rayStartPos;
    public LayerMask kickable;
    public float kickStrength;
    public float initialStrength;
    public float maxChargeStrength;

    public int playerNum;


    void Start()
    {

    }

    private void Update()
    {
        Debug.DrawRay(rayStartPos.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(rayStartPos.transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, kickable))
        {
            Debug.DrawRay(rayStartPos.transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);

            if(playerNum == 1 && Input.GetButtonDown("KickP1"))
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * kickStrength);
            } 
            else if((playerNum == 2 && Input.GetButtonDown("KickP2")))
            {
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.forward) * kickStrength);
            }
        }


    }
}

