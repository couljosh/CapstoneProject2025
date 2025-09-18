using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class GemCollection : MonoBehaviour
{

    public float radius;
    public LayerMask terrainMask;
    public Rigidbody rb;

    public float collectTime;
    private float elapsedTime;
    private bool isReleased;
    public bool isCollecting;

    public GameObject gemPrefab;


    private Vector3 collectedPos;

    private GameObject Collecter;
    public float pickupDelay;

    private float releasedTime;

    void Start()
    {

    }

    void Update()
    {


        if (!isReleased)
        {
            Collider[] hitblocks = Physics.OverlapSphere(transform.position, radius, terrainMask, QueryTriggerInteraction.Ignore);

            if (hitblocks.Length == 0)
            {
                ReleaseGem();
                isReleased = true;
            }
        }

        if (Collecter != null && collectedPos != null)
        {
             RecieveGem();
        }

    }

    public void ReleaseGem()
    {
        rb.isKinematic = false;
        releasedTime = Time.time;
    }

    public void RecieveGem()
    {   
        if(Time.time > releasedTime + pickupDelay && isCollecting)
        {
        elapsedTime += Time.deltaTime;

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        
        float lerpPercent = elapsedTime / collectTime;


        transform.position = Vector3.Slerp(collectedPos, Collecter.transform.position, lerpPercent);
        float gemDist = Vector3.Distance(transform.position, Collecter.transform.position);

            if (lerpPercent >= 1 || gemDist <= 0.5)
        {
                GemCollected();
                isCollecting = false;
        } 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReleased)
        {
            if (other.gameObject.tag == "ObjectDestroyer")
            {                
                collectedPos = gameObject.transform.position;
                Collecter = other.gameObject;
                isCollecting = true;
            }
        }
    }

    

    void GemCollected()
    {
        Collecter.GetComponent<PlayerDeath>().collectedGems.Add(gemPrefab.gameObject);
        rb.useGravity = true;
        gameObject.SetActive(false);
    }
}
