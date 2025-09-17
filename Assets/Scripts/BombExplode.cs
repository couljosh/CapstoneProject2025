using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
        GetExplosionRadius(gameObject.transform.position);
        
        Destroy(gameObject);
        
    }

    public void GetExplosionRadius(Vector3 center)
    {
        Collider[] hitblocks = Physics.OverlapSphere(center, radius, terrainMask | kickableMask | playerMask | gemMask);
        foreach (Collider col in hitblocks)
        {
            //look for destructible objects
            if(col.gameObject.tag != "ObjectDestroyer" && col.gameObject.GetComponent<BlockDestroy>())
            {        
                col.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
            }

            //look for player
            if(col.gameObject.GetComponent<PlayerMove>()) //a script only players have
            {
                col.gameObject.GetComponent<PlayerDeath>().PlayerDie();
            }

            //look for other bombs
            if(col.gameObject.tag == "Bomb" || col.gameObject.tag == "Gem")
            {
                print("bomb found");
                Vector3 forceVector = col.gameObject.transform.position - transform.position;
                col.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * forceStrength, ForceMode.Impulse);
            }
            
             

        }
    }

}
