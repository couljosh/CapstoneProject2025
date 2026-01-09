using UnityEngine;
using UnityEngine.TerrainUtils;
using System.Collections;
using System.Collections.Generic;

public class DrillExplode : MonoBehaviour
{
    //explode Variables
    private float radius;
    public float innerRadius;

    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
    public LayerMask bedrock;

    public GameObject explosionParticle;

    public float elapsedTime;
    public int sphereIterations;
    public float sphereIncrease;
    private bool isFinishedClearing = true;
    public float timeToScan;

    public float forceStrengthGem;
    public float forceStrengthCart;
    public float forceStrengthBomb;
    public float forceStrengthLargeGem;

    private PlayerMove playerMove;

    public GameObject explodePos;

    private bool isExplodedOnce = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (!isFinishedClearing)
        {
            print("ran");
            ClearTerrain();

            

            StartCoroutine(destroyDrillAfterDelay(0.4f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bedrock")
        {
            StartCoroutine(Explosion());        
            radius = sphereIterations * sphereIncrease;
        }
    }

    IEnumerator Explosion()
    {
        playerMove = GetComponentInParent<PlayerMove>();

        Collider[] objectsDec = Physics.OverlapSphere(explodePos.transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask, QueryTriggerInteraction.Ignore);
        Explode(objectsDec);

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Bomb/BombExplosion", gameObject.transform.position);

        GameObject.Instantiate(explosionParticle, gameObject.transform.position, Quaternion.identity);



        yield return null;
    }


    public void Explode(Collider[] colliding)
    {
        
        // Interior Bomb Detection (avoids terrain inside the bomb from being missed)
        Collider[] interiorHits = Physics.OverlapSphere(explodePos.transform.position, innerRadius, terrainMask);
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
                        //avoid killing the player who is in the drill
                        if(raycastHit.collider.gameObject != playerMove.gameObject)
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
                        print("hit terrain");
                        isFinishedClearing = false;

                    }
                }
            }
        }
    }

    //Clears Pocket Terrain
    public void ClearTerrain()
    {
        elapsedTime += Time.deltaTime;


        for (int i = 1; i <= sphereIterations; i++)
        {
            StartCoroutine(SphereTrigger(i));
            print(i);
        }
      

        isFinishedClearing = true;

    }

    public IEnumerator SphereTrigger(int y)
    {
        print("reached");
        yield return new WaitForSeconds(timeToScan * y);

        Collider[] terrainPieces = Physics.OverlapSphere(explodePos.transform.position, y * sphereIncrease, terrainMask);

        foreach (Collider collider in terrainPieces)
        {
            collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }

        yield return new WaitForSeconds(0.1f);

        //destroy leftovers
        Collider[] piecesToDestroy = Physics.OverlapSphere(explodePos.transform.position, (y * sphereIncrease) - 0.5f, terrainMask);
        foreach (Collider collider in piecesToDestroy)
        {
            Destroy(collider.gameObject);
        }
    }

    public IEnumerator destroyDrillAfterDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        //reset to basic movement
        playerMove.powerUpPickupScript.activePowerup = null;

        //destroy drill object, so that you respawn normally
        print("called destroy");
        Destroy(gameObject.transform.parent.gameObject);

        yield return null;
    }
}


