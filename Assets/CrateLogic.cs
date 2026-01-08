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
}
