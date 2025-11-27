using System.Collections;
using UnityEngine;
using UnityEngine.TerrainUtils;
using System.Threading;
//using static UnityEditor.PlayerSettings;

public class CartContact : MonoBehaviour
{
    public MinecartMovement minecartMovementScript;

    public LayerMask terrainMask;
    public LayerMask playerMask;

    public GameObject startingPocket;

    public Rigidbody rb;

    public float terrainThreshold;
    public float playerThreshold;

    public float terrainSpeedReduction;
    public float playerSpeedReduction;

    public bool isPowered;

    public bool isForwardOne;
    

    private void Start()
    {
        
        Invoke("ClearStartingArea", 0.1f);

    }

    private void OnTriggerEnter(Collider collision)
    {

        if(collision.gameObject.tag == "ActiveTerrain" && rb.linearVelocity.magnitude > terrainThreshold)
        {
            
            collision.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
            rb.linearVelocity = rb.linearVelocity * (1 - terrainSpeedReduction/100);
        }

        if (collision.gameObject.tag == "ObjectDestroyer" && rb.linearVelocity.magnitude > playerThreshold 
            && (isForwardOne && minecartMovementScript.isForward
            || !isForwardOne && !minecartMovementScript.isForward)
            || collision.gameObject.tag == "ObjectDestroyer" && isPowered)
        {
            collision.gameObject.GetComponent<PlayerDeath>().PlayerDie();
            rb.linearVelocity = rb.linearVelocity * (1 - playerSpeedReduction / 100);
            
        }
    }

    void ClearStartingArea()
    {
        
        Instantiate(startingPocket, transform.position, Quaternion.identity);

    }


}
