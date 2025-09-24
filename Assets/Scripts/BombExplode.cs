using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BombExplode : MonoBehaviour
{

    public float cookTime;
    public float radius;
    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
    public LayerMask bedrock;
    private BlockDestruction blockDestruction;
    private PlayerDeath playerDeathScript;
    public float forceStrength;

    public float rayLength = 3;
    public int degreeShift = 10;

    private RaycastHit[] rays = new RaycastHit[36];

    public float rayOffset;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    void Update()
    {

    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(cookTime);
        //GetExplosionRadius(gameObject.transform.position);

        //GetExplosionDetection(transform.position);
        //GetExplosionDetection(new Vector3(0,rayOffset,0));

        Collider[] objectsDec = Physics.OverlapSphere(transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask);
        Explode(objectsDec);

        RuntimeManager.PlayOneShot("event:/SFX_Bomb/BombExplode");

        Destroy(gameObject);

    }

    //OLD EXPLOSION METHOD - GetExplosionRadius only uses OverlapSphere & GetExplosionDetection uses OverlapSphere + Raycasts

    //public void GetExplosionRadius(Vector3 center)
    //{
    //    Collider[] hitblocks = Physics.OverlapSphere(center, radius, terrainMask | kickableMask | playerMask | gemMask);
    //    foreach (Collider col in hitblocks)
    //    {
    //        //look for destructible objects
    //        if (col.gameObject.tag != "ObjectDestroyer" && col.gameObject.GetComponent<BlockDestroy>())
    //        {
    //            col.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
    //        }

    //        //look for player
    //        if (col.gameObject.GetComponent<PlayerMove>()) //a script only players have
    //        {
    //            col.gameObject.GetComponent<PlayerDeath>().PlayerDie();
    //        }

    //        //look for other bombs
    //        if (col.gameObject.tag == "Bomb" || col.gameObject.tag == "Gem")
    //        {
    //            print("bomb found");
    //            Vector3 forceVector = col.gameObject.transform.position - transform.position;
    //            col.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrength, ForceMode.Impulse);
    //        }



    //    }
    //}

    //public void GetExplosionDetection(Vector3 rayPos)
    //{         
    //    int i = 0;

    //    for (int angle = 0; angle < 360; angle += degreeShift)
    //    {
    //        Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;  

    //        if(Physics.Raycast(rayPos, direction, out rays[i], rayLength,terrainMask | kickableMask | playerMask | gemMask))
    //        {
    //            Debug.DrawRay(rayPos, direction * rayLength, Color.green);

    //            //look for player
    //            if (rays[i].collider.gameObject.GetComponent<PlayerMove>()) //a script only players have
    //            {
    //                rays[i].collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
    //            }

    //            //look for other bombs or gems
    //            if (rays[i].collider.gameObject.gameObject.tag == "Bomb" || rays[i].collider.gameObject.gameObject.tag == "Gem")
    //            {
    //                print("bomb found");
    //                Vector3 forceVector = rays[i].collider.gameObject.gameObject.transform.position - transform.position;
    //                rays[i].collider.gameObject.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrength, ForceMode.Impulse);
    //            }
    //        }
    //    else
    //        {
    //            Debug.DrawRay(rayPos, direction * rayLength, Color.red);
    //        }

    //        i++;
    //    }




    //    Collider[] hitblocks = Physics.OverlapSphere(transform.position, radius, terrainMask);
    //    foreach (Collider col in hitblocks)
    //    {
    //        //look for destructible objects
    //        if (col.gameObject.tag != "ObjectDestroyer" && col.gameObject.GetComponent<BlockDestroy>())
    //        {
    //            col.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
    //        }

    //    }
    //}

    public void Explode(Collider[] colliding)
    {

        foreach(Collider hit in colliding)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, hit.transform.position - transform.position, radius, terrainMask | bedrock | kickableMask | playerMask | gemMask);
            foreach(RaycastHit raycastHit in hits)
            {
                if(raycastHit.collider.tag != null)
                {
                    Debug.DrawRay(transform.position, raycastHit.collider.gameObject.transform.position - transform.position, Color.green, 5);
                    if (raycastHit.collider.tag == "Bedrock")
                    {
                        break;
                    }

                    //Look for player
                    if(raycastHit.collider.tag == "ObjectDestroyer")
                    {
                        raycastHit.collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
                    }

                    //Loock for Gem / bombs
                    if(raycastHit.collider.tag == "Gem" || raycastHit.collider.tag == "Bomb")
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
