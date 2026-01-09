using UnityEngine;

public class CrateLogic : MonoBehaviour
{
    public GameObject[] powerUps;
    public GameObject chosenPowerup; 
    
    void Start()
    {
        int randInd = Random.Range(0, powerUps.Length);
        chosenPowerup = powerUps[randInd];
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Repository")
        {
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "ActiveTerrain")
        {
            other.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }

    }
}
