using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class CaveInManager : MonoBehaviour
{
    [Header("References")]
    public GameObject floor;
    public GameObject canvas;
    public GameObject notificationPrefab;
    public GemGeneration gemGenerationScript;
    public LayerMask terrainMask;


    [Header("Cave-In Customization")]
    public float chancePerSecond;
    public float radius;
    private float timer;
    private float timeSinceLastCaveIn = 0;
    public float minTimeBetweenCaveIn;
    private float randomNum;
    public int terrainChunksToSpawn;

    public float gemVeritcalOffest;

    private ReadOnlyArray<Gamepad> gamepadArray;



        private void Start()
        {
           gamepadArray = Gamepad.all;
        }

    void Update()
    {
        timer += Time.deltaTime;
        timeSinceLastCaveIn += Time.deltaTime;

        //Cave-In Chance Check
        if(timer > 1f)
        {
            timer = 0;
            randomNum = Random.Range(0, 100);

            //Trigger Cave-In Sequence & Notification
            if (randomNum < chancePerSecond && timeSinceLastCaveIn > minTimeBetweenCaveIn)
            {
                GameObject.Instantiate(notificationPrefab, canvas.transform);
                timeSinceLastCaveIn = 0;
                StartCoroutine(spawnTerrain());
            }
        }
    }

    // Cave-In Sequence 
    public IEnumerator spawnTerrain()
    {
        foreach (var gamepad in gamepadArray)
        {
            gamepad.SetMotorSpeeds(0.1f, 0.1f);
        }    

        yield return new WaitForSeconds(3);

        foreach (var gamepad in gamepadArray)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }

        for (int i = 0; i < terrainChunksToSpawn; i++)
        {
            //Random Location based on floor bounds
            var bounds = floor.GetComponent<MeshCollider>().bounds;
            var px = Random.Range(bounds.min.x, bounds.max.x);
            var py = Random.Range(bounds.min.y, bounds.max.y);
            var pz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 pos = new Vector3(px, py + gemVeritcalOffest, pz);

            Collider[] hitblocks = Physics.OverlapSphere(pos, radius, terrainMask);
            foreach(Collider col in hitblocks)
            {
                if (col.gameObject.GetComponent<MeshRenderer>().enabled == false) //if the box is invisible
                {
                    col.gameObject.GetComponent<BlockDestroy>().enableCube();
                }    
               
            }

        int randIndex = Random.Range(0, gemGenerationScript.clusterPrefabs.Count);
        Instantiate(gemGenerationScript.clusterPrefabs[randIndex], pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }
}
