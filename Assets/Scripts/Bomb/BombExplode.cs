using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BombExplode : MonoBehaviour
{
    [Header("Bomb Explosion Variables")]
    public float cookTime;
    private float radius;
    public float innerRadius;
    public float forceStrengthGem;
    public float forceStrengthCart;
    public float forceStrengthBomb;
    public float forceStrengthLargeGem;
    public GameObject explosionParticle;
    public ParticleSystem fireParticles;
    public ParticleSystem smokeParticles;

    [Header("Layers to Detect")]
    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
    public LayerMask bedrock;

    [Header("Script References")]
    private BlockDestruction blockDestruction;
    private PlayerDeath playerDeathScript;

    public float elapsedTime;
    public int sphereIterations;
    public float sphereIncrease;
    private bool isFinishedClearing = true;
    public float timeToScan;
    private float deleteTimer = 0;

    public GameObject bombModel; 

    void Start()
    {
        StartCoroutine(Explosion());

        radius = sphereIterations * sphereIncrease;

        var emission = fireParticles.emission;
        emission.enabled = true;

        var smokeEmission = smokeParticles.emission;
        smokeEmission.enabled = true;
    }

    private void Update()
    {
        if (!isFinishedClearing)
        {
            ClearTerrain();
        }

        deleteTimer += Time.deltaTime;

        if(deleteTimer > 10) //destroy if alive for way too long
        {
            Destroy(gameObject);
        }

    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(cookTime);

        Collider[] objectsDec = Physics.OverlapSphere(transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask, QueryTriggerInteraction.Ignore);
        Explode(objectsDec);

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Bomb/BombExplosion", gameObject.transform.position);

        GameObject.Instantiate(explosionParticle, gameObject.transform.position, Quaternion.identity);
        

    }


    public void Explode(Collider[] colliding)
    {
        var emission = fireParticles.emission;
        emission.enabled = false;

        var smokeEmission = smokeParticles.emission;
        smokeEmission.enabled = false;

        // Interior Bomb Detection (avoids terrain inside the bomb from being missed)
        Collider[] interiorHits = Physics.OverlapSphere(transform.position, innerRadius, terrainMask);
        Debug.DrawRay(transform.position, Vector3.forward * innerRadius, Color.red, 5);

        foreach (Collider innerHit in interiorHits)
        {
            innerHit.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }


        //Bomb Detection
        foreach (Collider hit in colliding)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, hit.transform.position - transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.collider.tag != null)
                {
                    Debug.DrawRay(transform.position, raycastHit.collider.gameObject.transform.position - transform.position, Color.green, 5);
                    if (raycastHit.collider.tag == "Bedrock" || raycastHit.collider.tag == "Repository")
                    {
                        break;
                    }

                    //Look for player
                    if (raycastHit.collider.tag == "ObjectDestroyer")
                    {
                        raycastHit.collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
                    }

                    if (raycastHit.collider.tag == "LargeGem")
                    {
                        print("hit large gem");
                        Vector3 forceVector = raycastHit.collider.gameObject.gameObject.transform.position - transform.position;
                        raycastHit.collider.gameObject.gameObject.GetComponentInParent<Rigidbody>().AddForce(forceVector * forceStrengthLargeGem, ForceMode.Impulse);
                    }

                    //Loock for Gem
                    if (raycastHit.collider.tag == "Gem")
                    {
                        Vector3 forceVector = raycastHit.collider.gameObject.gameObject.transform.position - transform.position;
                        raycastHit.collider.gameObject.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrengthGem, ForceMode.Impulse);
                    }

                    //Look for Bombs
                    if (raycastHit.collider.tag == "Bomb")
                    {
                        Vector3 forceVector = raycastHit.collider.gameObject.gameObject.transform.position - transform.position;
                        raycastHit.collider.gameObject.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrengthBomb, ForceMode.Impulse);
                    }


                    //Loock for Cart
                    if (raycastHit.collider.tag == "Cart")
                    {
                        Vector3 forceVector = raycastHit.collider.gameObject.gameObject.transform.position - transform.position;
                        raycastHit.collider.gameObject.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrengthCart, ForceMode.Impulse);
                    }

                    //Look for Terrain
                    if (raycastHit.collider.tag == "ActiveTerrain")
                    {
                        isFinishedClearing = false;
                        
                    }
                }
            }
        }
        DisableBomb();

    }

    //Clears Pocket Terrain
    public void ClearTerrain()
    {
        elapsedTime += Time.deltaTime;

        

        for (int i = 1; i <= sphereIterations; i++)
        {
            StartCoroutine(SphereTrigger(i));
        }

        isFinishedClearing = true;
    }

    public IEnumerator SphereTrigger(int y)
    {
        
        yield return new WaitForSeconds(timeToScan * y);

        Collider[] terrainPieces = Physics.OverlapSphere(transform.position, y * sphereIncrease, terrainMask);

        foreach (Collider collider in terrainPieces)
        {
            collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }

        yield return new WaitForSeconds(0.1f);

        //destroy leftovers
        Collider[] piecesToDestroy = Physics.OverlapSphere(transform.position, (y * sphereIncrease) - 0.5f, terrainMask);
        foreach (Collider collider in piecesToDestroy)
        {
            Destroy(collider.gameObject);
        }
    }

    void DisableBomb()
    {
        //disable mesh and collider
        bombModel.SetActive(false);
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }
}
