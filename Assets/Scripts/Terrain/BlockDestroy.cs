using UnityEngine;
using System.Collections;

public class BlockDestroy : MonoBehaviour
{
    [Header("Terrain Destruction Customization")]
    public float destroyDelay;

    //Dectruction on Collsion
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ObjectDestroyer")
        {
            disableCubeAfterDelay(destroyDelay);
        }
    }


    public void disableCubeAfterDelay(float delay)
    {
        StartCoroutine(disableCube(delay));
        
    }


    public IEnumerator disableCube(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.tag = "InactiveTerrain";

        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        //make box trigger only
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }


    public void enableCube()
    {
        gameObject.tag = "ActiveTerrain";

        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        //re-enable collision
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }
}
