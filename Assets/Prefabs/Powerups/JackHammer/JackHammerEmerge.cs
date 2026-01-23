using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class JackHammerEmerge : MonoBehaviour
{
    [Header("Layers To Detect")]
    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
    public LayerMask bedrock;

    [Header("Explosion Size/Checks")]
    public float innerRadius;
    public float radius;
    private float explosionCount = 0;
    private float elapsedTime;
    public int sphereIterations;
    public float sphereIncrease;
    public float timeToScan;
    private bool isFinishedClearing = true;
    [HideInInspector] public bool hasExploded;
    public bool isDoneEmerging = false;

    [Header("Explosion Forces")]
    public float forceStrengthGem;
    public float forceStrengthCart;
    public float forceStrengthBomb;
    public float forceStrengthLargeGem;

    //Rumble
    private bool hasInitialRumbled = false;
    private bool isRumbling = false;

    [Header("References")]
    public GameObject explosionParticle;
    private PlayerMove playerMoveScript;
    public JackHammerLogic jackHammerLogicScript;
    public GameObject explodePos;


    void Start()
    {
        playerMoveScript = GetComponentInParent<PlayerMove>();

        radius = sphereIterations * sphereIncrease;

        //engineStartInstance = RuntimeManager.CreateInstance(engineStartEvent);  //sound 
        //engineStartInstance.start();   //sound

    }


    void Update()
    {
        if (!isFinishedClearing)
        {
            ClearTerrain();

        }

        if (jackHammerLogicScript.hasEmerged && !isDoneEmerging)
        {
            Explosion();
            isDoneEmerging = true;
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

            StartCoroutine(destroyJackHammerAfterDelay(0.5f));
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
                        if (raycastHit.collider.gameObject != playerMoveScript.gameObject)
                            raycastHit.collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
                    }

                    if (raycastHit.collider.tag == "LargeGem")
                    {
                        GemExplode largeGemFound  = raycastHit.collider.gameObject.GetComponentInParent<GemExplode>();
                        largeGemFound.ExplodeSequence();


                        //Vector3 forceVector = raycastHit.collider.gameObject.gameObject.transform.position - transform.position;
                        //raycastHit.collider.gameObject.gameObject.GetComponentInParent<Rigidbody>().AddForce(forceVector * forceStrengthLargeGem, ForceMode.Impulse);
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

    //if other scripts need to destroy the jackhammer (coroutines cannot be called from other scripts)
    public void destroyJackHammerFromOtherScript(float waitTime)
    {
        StartCoroutine(destroyJackHammerAfterDelay(waitTime));
    }

    public IEnumerator destroyJackHammerAfterDelay(float waitTime)
    {

        //do not move player until the delay ends
        //playerMoveScript.canAct = false;

        yield return new WaitForSeconds(waitTime);

        //re-enable player movement when they're allowed to move again
        //playerMoveScript.canAct = true;

        jackHammerLogicScript.Emerge(Vector3.zero);

        //reset to basic movement
        playerMoveScript.powerUpPickupScript.activePowerup = null;

        CaveInManager.isPowerupInPlay = false;
        //engineStartInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);                        //Re-add with jackhamemr sfx
        //drillLogicScript.drillInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //destroy powerup object, so that you respawn normally
        Destroy(gameObject.gameObject);

        yield return null;
    }
}
