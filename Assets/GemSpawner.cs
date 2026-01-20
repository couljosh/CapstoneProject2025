using System.Collections;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public int TeamNum;

    public GameObject gemPrefab;
    public GameObject gemSpawnloc;
    public int gemAmount;
    public int gemCurrentCount;


    public float elapsedTime;
    public float spawnDelay;

    void Awake()
    {
        if(TeamNum == 1)
        {
            gemAmount = GameScore.redTotalScore;
        }
        else
        {
            gemAmount = GameScore.blueTotalScore;
        }
    }


    public void Update()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime > spawnDelay && gemCurrentCount < gemAmount)
        {
            gemCurrentCount ++; 
            Quaternion randRot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0,360));
             GameObject.Instantiate(gemPrefab, transform.position, randRot);
            elapsedTime = 0;
        }

    }
}
