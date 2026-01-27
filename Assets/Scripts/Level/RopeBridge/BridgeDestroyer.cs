using TMPro;
using UnityEngine;

public class BridgeDestroyer : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public float bridgeFallSpeed;
    private bool breakBridge = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (breakBridge)
        {
            BridgeFall();
        }
        if (Vector3.Distance(transform.position, endPos.position) < 0.1f)
        {
            breakBridge = false;
            transform.position = startPos.position;
        }
    }


    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.tag == "ObjectDestroyer")
        {
            
            breakBridge = true;
        }
    }

    public void BridgeFall()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPos.position, bridgeFallSpeed * Time.deltaTime);
    }



}
