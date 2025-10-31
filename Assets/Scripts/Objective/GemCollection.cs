using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class GemCollection : MonoBehaviour
{
    [Header("Gem Collection Customization")]
    public float radius;
    public float collectTime;
    private float elapsedTime;
    public float pickupDelay;
    private float releasedTime;

    [Header("Checks")]
    public bool isReleased;
    public bool isCollecting;

    [Header("Stored References")]
    private GameObject Collecter;
    public GameObject gemPrefab;
    public Rigidbody rb;

    [Header("Layer To Detect")]
    public LayerMask terrainMask;

    private Vector3 collectedPos;

    void Update()
    {
        //Release Gem on Check
        if (!isReleased)
        {
            Collider[] hitblocks = Physics.OverlapSphere(transform.position, radius, terrainMask, QueryTriggerInteraction.Ignore);

            if (hitblocks.Length == 0)
            {
                ReleaseGem();
                isReleased = true;
            }
        }

        //Recieve Gem on Check
        if (Collecter != null && collectedPos != null)
        {
            RecieveGem();
        }

    }


    //Release Gem
    public void ReleaseGem()
    {
        rb.isKinematic = false;
        releasedTime = Time.time;
    }


    //Recieve Gem
    public void RecieveGem()
    {
        if (Time.time > releasedTime + pickupDelay && isCollecting)
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
                elapsedTime = 0;
            }
        }
    }


    //Player Who Collected Check
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


    //Gem Collected
    void GemCollected()
    {
        Collecter.GetComponent<PlayerDeath>().collectedGems.Add(gemPrefab.gameObject);
        rb.useGravity = true;
        gameObject.SetActive(false);

        //Collecter.GetComponentInChildren<BagSize>().changeBagSize();
    }
}
