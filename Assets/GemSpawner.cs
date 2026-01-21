using System.Collections;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public int TeamNum;

    public GameObject[] gemPrefab;
    public int gemAmount;
    public int gemCurrentCount;

    public float totalSpawnTime = 5f;
    public float elapsedTime;
    public float spawnDelay;

    void Awake()
    {
        //Assigns the corrent amount of gems to the corrisponding spawner
        if(TeamNum == 1)
        {
            gemAmount = GameScore.redTotalScore;
        }
        else
        {
            gemAmount = GameScore.blueTotalScore;
        }

        //Ensures spawning completes at a specific time into the intermission screen
        spawnDelay = totalSpawnTime / gemAmount;
    }

     
    public void Update()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime > spawnDelay && gemCurrentCount < gemAmount)
        {
            gemCurrentCount ++;

            //Random position and rotation to ensure scattered drop of gems
            var bounds = gameObject.GetComponent<BoxCollider>().bounds;
            var px = Random.Range(bounds.min.x, bounds.max.x);
            var py = Random.Range(bounds.min.y, bounds.max.y);
            var pz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 pos = new Vector3(px, py, pz);
            Quaternion randRot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0,360));

            int chosenGem = Random.Range(0, gemPrefab.Length);
            GameObject.Instantiate(gemPrefab[chosenGem], pos, randRot);
                        
             elapsedTime = 0;
        }

    }
}
