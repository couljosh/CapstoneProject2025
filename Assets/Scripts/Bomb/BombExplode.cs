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
    public float radius;
    public float innerRadius;
    public float forceStrength;

    [Header("Layers to Detect")]
    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
    public LayerMask bedrock;

    [Header("Script References")]
    private BlockDestruction blockDestruction;
    private PlayerDeath playerDeathScript;

    void Start()
    {
        StartCoroutine(Explosion());
    }


    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(cookTime);

        Collider[] objectsDec = Physics.OverlapSphere(transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask);
        Explode(objectsDec);

        RuntimeManager.PlayOneShot("event:/SFX_Bomb/BombExplode");

        Destroy(gameObject);

    }


    public void Explode(Collider[] colliding)
    {

        // Interior Bomb Detection (avoids terrain inside the bomb from being missed)
        Collider[] interiorHits = Physics.OverlapSphere(transform.position, innerRadius, terrainMask);
        Debug.DrawRay(transform.position, Vector3.forward * innerRadius, Color.red, 5);

        foreach (Collider innerHit in interiorHits)
        {
            innerHit.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
        }


        //Bomb Detection
        foreach (Collider hit in colliding)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, hit.transform.position - transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask);
            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.collider.tag != null)
                {
                    Debug.DrawRay(transform.position, raycastHit.collider.gameObject.transform.position - transform.position, Color.green, 5);
                    if (raycastHit.collider.tag == "Bedrock")
                    {
                        break;
                    }

                    //Look for player
                    if (raycastHit.collider.tag == "ObjectDestroyer")
                    {
                        raycastHit.collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
                    }

                    //Loock for Gem / bombs
                    if (raycastHit.collider.tag == "Gem" || raycastHit.collider.tag == "Bomb")
                    {
                        Vector3 forceVector = raycastHit.collider.gameObject.gameObject.transform.position - transform.position;
                        raycastHit.collider.gameObject.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrength, ForceMode.Impulse);
                    }

                    //Look for Terrain
                    if (raycastHit.collider.tag == "Terrain")
                    {
                        raycastHit.collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
                    }
                }
            }
        }
    }
}
