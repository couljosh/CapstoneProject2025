using UnityEngine;

public class BridgeTriggerSend : MonoBehaviour
{
    public GameObject bridgeDestroyer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BreakBridge()
    {
        bridgeDestroyer.SendMessage("BridgeFall");
    }

}
