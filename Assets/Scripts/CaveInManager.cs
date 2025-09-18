using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using System.Collections;
public class CaveInManager : MonoBehaviour
{
    public GameObject terrainPrefab;
    public GameObject floor;
    public GameObject canvas;
    public GameObject notificationPrefab;
    public float chancePerSecond;
    public LayerMask terrainMask;
    public float radius;
    private float timer;
    private float timeSinceLastCaveIn = 0;
    public float minTimeBetweenCaveIn;
    private float randomNum;
    public int terrainChunksToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timeSinceLastCaveIn += Time.deltaTime;

        if(timer > 1f)
        {
            timer = 0;
            randomNum = Random.Range(0, 100);

            if (randomNum < chancePerSecond && timeSinceLastCaveIn > minTimeBetweenCaveIn)
            {
                GameObject.Instantiate(notificationPrefab, canvas.transform);
                timeSinceLastCaveIn = 0;
                StartCoroutine(spawnTerrain());
            }
        }

        //debug spawn
        //if (Input.GetButtonDown("Fire3"))
        //{
        //    print("spawned terrain");
        //    GameObject.Instantiate(notificationPrefab, canvas.transform);
        //    StartCoroutine(spawnTerrain());
        //}
    }

    

    public IEnumerator spawnTerrain()
    {
        yield return new WaitForSeconds(3);

        for(int i = 0; i < terrainChunksToSpawn; i++)
        {
            var bounds = floor.GetComponent<MeshCollider>().bounds;
            var px = Random.Range(bounds.min.x, bounds.max.x);
            var py = Random.Range(bounds.min.y, bounds.max.y);
            var pz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 pos = new Vector3(px, py, pz);

            Collider[] hitblocks = Physics.OverlapSphere(pos, radius, terrainMask);
            foreach(Collider col in hitblocks)
            {
                if (col.gameObject.GetComponent<MeshRenderer>().enabled == false) //if the box is invisible
                {
                    col.gameObject.GetComponent<BlockDestroy>().enableCube();
                }    
               
            }

        }
    }
}
