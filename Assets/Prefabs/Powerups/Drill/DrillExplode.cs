using UnityEngine;
using UnityEngine.TerrainUtils;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Filters;
using FMODUnity;
using FMOD.Studio;

public class DrillExplode : MonoBehaviour
{
    [Header("Layers To Detect")]
    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
    public LayerMask bedrock;

    [Header("Explosion Variables")]
    [HideInInspector] public bool hasExploded;
    private float radius;
    public float innerRadius;
    private float explosionCount = 0;
    public float elapsedTime;
    public int sphereIterations;
    public float sphereIncrease;
    private bool isFinishedClearing = true;
    public float timeToScan;

    public float forceStrengthGem;
    public float forceStrengthCart;
    public float forceStrengthBomb;
    public float forceStrengthLargeGem;

    public float drillForceMultplier;
    public float drillForceMultplierCart;

    private bool hasInitialRumbled = false;
    private bool isRumbling = false;

    [Header("References")]
    public GameObject explosionParticle;
    private PlayerMove playerMove;
    public DrillLogic drillLogicScript;
    public GameObject explodePos;

    [Header("Audio")]
    public FMODUnity.EventReference engineStartEvent;
    public EventInstance engineStartInstance;


    private void Start()
    {
        radius = sphereIterations * sphereIncrease;
        engineStartInstance = RuntimeManager.CreateInstance(engineStartEvent);

        engineStartInstance.start();

        playerMove = GetComponentInParent<PlayerMove>();
    }

    private void Update()
    {
        if (!isFinishedClearing)
        {
            ClearTerrain();

        }
    }

    // Determines all interactions that happen when the driving drill hits something
    private void OnTriggerEnter(Collider other)
    {
        //NOTE: For reasons beyond me, the first explosion from the drill will NOT pick up any collisions whatsoever. Every subsequent one can, but the first cannot.
        //Therefore, we must actually do two explosions in quick succession in order to actually pick up anything. The first explosion is REQUIRED for the second explosion to see anything
        //We do not know if this is a unity bug, or some huge oversight in what we're doing. God help us all. 
        if ((other.gameObject.tag == "Bedrock") && explosionCount < 2 && drillLogicScript.isDrillMoving)
        {
            explosionCount++;
            hasExploded = true; 
            Explosion();    

        }

         if(other.gameObject.tag == "Repository" && explosionCount < 2 && drillLogicScript.isDrillMoving)
        {
            explosionCount++;

            //Triggers the explosion twice to account for the issue where the first explosion doesnt detect anything 
            Explosion();    
            Explosion();
        }

        if (other.gameObject.tag == "ObjectDestroyer" && drillLogicScript.isDrillMoving)
        {
            other.gameObject.GetComponent<PlayerDeath>().PlayerDie();
        }

        if (other.gameObject.tag == "Cart")
        {
            Vector3 forceVector = other.gameObject.gameObject.transform.position - transform.position;
            other.gameObject.gameObject.GetComponent<Rigidbody>().AddForce((forceVector * forceStrengthBomb) * drillForceMultplierCart, ForceMode.Impulse);
        }

        if (other.gameObject.tag == "Bomb")
        {
            Vector3 forceVector = other.gameObject.gameObject.transform.position - transform.position;
            other.gameObject.gameObject.GetComponent<Rigidbody>().AddForce((forceVector * forceStrengthBomb) * drillForceMultplier, ForceMode.Impulse);
        }

    }


    public void Explosion()
    {

        Collider[] objectsDec = Physics.OverlapSphere(explodePos.transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask, QueryTriggerInteraction.Ignore);
        Explode(objectsDec);

        //NOTE: Like the above note, since there are two explosions, we only want to be showing one effect and playing one noise. This code does so, as only one explosion will have happened when
        //this code fires. 
        if (explosionCount < 2)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Bomb/BombExplosion", gameObject.transform.position);

            GameObject.Instantiate(explosionParticle, gameObject.transform.position, Quaternion.identity);

            StartCoroutine(destroyDrillAfterDelay(0.4f));
        }

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
                        if (raycastHit.collider.gameObject != playerMove.gameObject)
                            raycastHit.collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
                    }

                    if (raycastHit.collider.tag == "LargeGem")
                    {
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

    //if other scripts need to destroy the drill (coroutines cannot be called from other scripts)
    public void destroyDrillFromOtherScript(float waitTime)
    {
        StartCoroutine(destroyDrillAfterDelay(waitTime));
    }

    public IEnumerator destroyDrillAfterDelay(float waitTime)
    {
        //do not move player until the delay ends
        playerMove.canAct = false;

        yield return new WaitForSeconds(waitTime);

        //re-enable player movement when they're allowed to move again
        playerMove.canAct = true;

        //reset to basic movement
        playerMove.powerUpPickupScript.activePowerup = null;

        CaveInManager.isPowerupInPlay = false;
        engineStartInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        drillLogicScript.drillInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //destroy drill object, so that you respawn normally
        Destroy(gameObject.transform.parent.gameObject);

        yield return null;
    }

}


