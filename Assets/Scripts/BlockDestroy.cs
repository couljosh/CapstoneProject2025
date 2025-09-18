using UnityEngine;
using System.Collections;


public class BlockDestroy : MonoBehaviour
{

    public float destroyDelay;

    private void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ObjectDestroyer")
        {
            disableCubeAfterDelay();
        }
    }

    public void disableCubeAfterDelay()
    {
        StartCoroutine(disableCube());
    }

    public IEnumerator disableCube()
    {
        yield return new WaitForSeconds(0.07f);

        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        //make box trigger only
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    public void enableCube()
    {
        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        //re-enable collision
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }
}
