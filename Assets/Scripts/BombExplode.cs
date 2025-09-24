using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BombExplode : MonoBehaviour
{

    public float cookTime;
    public float radius;
    public LayerMask terrainMask;
    public LayerMask kickableMask;
    public LayerMask playerMask;
    public LayerMask gemMask;
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

        GetExplosionDetection(transform.position);
        GetExplosionDetection(new Vector3(0,rayOffset,0));

        RuntimeManager.PlayOneShot("event:/SFX_Bomb/BombExplode");

        Destroy(gameObject);

    }

    public void GetExplosionRadius(Vector3 center)
    {
        Collider[] hitblocks = Physics.OverlapSphere(center, radius, terrainMask | kickableMask | playerMask | gemMask);
        foreach (Collider col in hitblocks)
        {
            //look for destructible objects
            if (col.gameObject.tag != "ObjectDestroyer" && col.gameObject.GetComponent<BlockDestroy>())
            {
                col.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
            }

            //look for player
            if (col.gameObject.GetComponent<PlayerMove>()) //a script only players have
            {
                col.gameObject.GetComponent<PlayerDeath>().PlayerDie();
            }

            //look for other bombs
            if (col.gameObject.tag == "Bomb" || col.gameObject.tag == "Gem")
            {
                print("bomb found");
                Vector3 forceVector = col.gameObject.transform.position - transform.position;
                col.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrength, ForceMode.Impulse);
            }



        }
    }

    public void GetExplosionDetection(Vector3 rayPos)
    {         
        int i = 0;

        for (int angle = 0; angle < 360; angle += degreeShift)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;  

            if(Physics.Raycast(rayPos, direction, out rays[i], rayLength,terrainMask | kickableMask | playerMask | gemMask))
            {
                Debug.DrawRay(rayPos, direction * rayLength, Color.green);

                //look for player
                if (rays[i].collider.gameObject.GetComponent<PlayerMove>()) //a script only players have
                {
                    rays[i].collider.gameObject.GetComponent<PlayerDeath>().PlayerDie();
                }

                //look for other bombs or gems
                if (rays[i].collider.gameObject.gameObject.tag == "Bomb" || rays[i].collider.gameObject.gameObject.tag == "Gem")
                {
                    print("bomb found");
                    Vector3 forceVector = rays[i].collider.gameObject.gameObject.transform.position - transform.position;
                    rays[i].collider.gameObject.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrength, ForceMode.Impulse);
                }
            }
        else
            {
                Debug.DrawRay(rayPos, direction * rayLength, Color.red);
            }

            i++;
        }




        Collider[] hitblocks = Physics.OverlapSphere(transform.position, radius, terrainMask);
        foreach (Collider col in hitblocks)
        {
            //look for destructible objects
            if (col.gameObject.tag != "ObjectDestroyer" && col.gameObject.GetComponent<BlockDestroy>())
            {
                col.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
            }

        }
    }

}
