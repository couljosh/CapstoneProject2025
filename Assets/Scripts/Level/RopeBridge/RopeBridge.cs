using UnityEngine;

public class RopeBridge : MonoBehaviour
{
    private Vector3 newLocation;
    private Vector3 startingLocation;
    private bool fall = false;
    public float fallSpeed = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingLocation = transform.position;

        newLocation = new Vector3(transform.position.x, -20, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (fall)
        {
            transform.position = Vector3.MoveTowards(transform.position, newLocation, fallSpeed * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, newLocation) < 0.1f)
        {
            fall = false;
            transform.position = startingLocation;
        }

    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {



        if (other.gameObject.tag == "BridgeDestroyer")
        {
            
           fall = true;
            //transform.position = Vector3.Lerp(startingLocation, newLocation, 100);
           
        }
    }


}
