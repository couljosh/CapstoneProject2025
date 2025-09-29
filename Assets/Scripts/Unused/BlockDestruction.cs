using System.Collections;
using UnityEngine;

public class BlockDestruction : MonoBehaviour
{

    public float destroyDelay;

    void Start()
    {

    }

    void Update()
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
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public void enableCube()
    {
        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        //re-enable collision
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}
