using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GemCollection : MonoBehaviour
{
    [Header("Gem Collection Customization")]
    public float radius;
    public float collectTime;
    private float elapsedTime;
    public float pickupDelay;
    private float releasedTime;
    public ParticleSystem sparkleVFX;

    [Header("Sparkle Timing")]
    public float activeDuration = 3.0f;
    public float minWaitTime = 3.0f;
    public float maxWaitTime = 10.0f;

    [Header("Density Logic")]
    public float detectionRadius = 5.0f;
    public float speedUpPerNeighbor = 1.5f;
    public int minNeighborsToSparkle = 2;
    public LayerMask gemLayer;

    [Header("Checks")]
    public bool isReleased;
    public bool isCollecting;

    [Header("Stored References")]
    private GameObject Collecter;
    public GameObject gemPrefab;
    public Rigidbody rb;
    public SphereCollider sphereCollider;
    public GameObject collectionParticle;

    [Header("Layer To Detect")]
    public LayerMask terrainMask;

    private Vector3 collectedPos;
    private Coroutine sparkleRoutine;

    void Start()
    {
        //start sparkle cycle if the gem is lodged
        if (!isReleased)
        {
            sparkleRoutine = StartCoroutine(LodgedGemSparkle());
        }
    }

    void Update()
    {
        //Release Gem on Check
        if (!isReleased)
        {
            sphereCollider.enabled = false;

            Collider[] hitblocks = Physics.OverlapSphere(transform.position, radius, terrainMask, QueryTriggerInteraction.Ignore);

            if (hitblocks.Length == 0)
            {
                ReleaseGem();
                isReleased = true;
            }
        }

        //Recieve Gem on Check
        if (Collecter != null && isCollecting)
        {
            RecieveGem();
        }
    }

    public IEnumerator LodgedGemSparkle()
    {
        //wait one frame to ensure everything is setup
        yield return null;

        //spread out the initial start times slightly so they don't sync up
        yield return new WaitForSeconds(Random.Range(0.1f, 2f));

        while (!isReleased)
        {
            //overlap sphere to check how dense the gems are together
            Collider[] neighbors = Physics.OverlapSphere(transform.position, detectionRadius, gemLayer);
            int neighborCount = 0;
            foreach (var col in neighbors)
            {
                GemCollection other = col.GetComponent<GemCollection>();
                if (other != null && !other.isReleased && other != this) neighborCount++;
            }

            //only emit if denisty/neighbor count meets requirement
            if (neighborCount >= minNeighborsToSparkle)
            {
                //turn sparkle on
                var emission = sparkleVFX.emission;
                emission.enabled = true;
                sparkleVFX.Play();

                yield return new WaitForSeconds(activeDuration);

                //turn sparkle off
                emission.enabled = false;
                sparkleVFX.Stop();
            }

            //random delay with neighbor speed-up and an offset min/max time
            float randomWait = Random.Range(minWaitTime, maxWaitTime);
            float reduction = neighborCount * speedUpPerNeighbor;
            float chaos = Random.Range(-0.75f, 0.75f);

            yield return new WaitForSeconds(Mathf.Max(0.5f, (randomWait - reduction) + chaos));
        }
    }

    //Release Gem
    public void ReleaseGem()
    {
        if (sparkleRoutine != null) StopCoroutine(sparkleRoutine);
        var emission = sparkleVFX.emission;
        emission.enabled = false;
        sparkleVFX.Stop();

        sphereCollider.enabled = true;
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

                //play gem collection noise
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Gems/GemsCollection", gameObject.transform.position);
            }
        }
    }

    //Player Who Collected Check
    private void OnTriggerEnter(Collider other)
    {
        if (isReleased)
        {
            //if the object is a player and not in the drill (cannot find drill logic script)
            if (other.gameObject.tag == "ObjectDestroyer" && !other.gameObject.GetComponentInChildren<DrillLogic>())
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
        PlayerDeath pd = Collecter.GetComponent<PlayerDeath>();

        pd.collectedGems.Add(gemPrefab.gameObject);
        rb.useGravity = true;

        //update ui
        if (pd.gemsHeldUI != null)
        {
            pd.gemsHeldUI.AnimateToValue(pd.collectedGems.Count);
        }

        GameObject particle = Instantiate(collectionParticle, transform.position, Quaternion.identity);
        particle.transform.parent = Collecter.transform;

        var ps = particle.GetComponent<ParticleSystem>().main;
        ps.startColor = gameObject.GetComponent<MeshRenderer>().material.color;

        gameObject.SetActive(false);
    }
}