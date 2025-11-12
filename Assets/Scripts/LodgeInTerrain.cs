using UnityEngine;

public class LodgeInTerrain : MonoBehaviour
{
    public float radius;
    public LayerMask terrainMask;
    public Rigidbody rb;
    private Collider[] interiorHits;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Lodge();
    }

    void Update()
    {
        if(interiorHits != null &&  interiorHits.Length > 5)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    void Lodge()
    {
        // Interior Bomb Detection (avoids terrain inside the bomb from being missed)
        interiorHits = Physics.OverlapSphere(transform.position, radius, terrainMask);
        Debug.DrawRay(transform.position, Vector3.forward * radius, Color.red, 5);
    }
}
