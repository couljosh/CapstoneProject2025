using System.Collections;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.TerrainUtils;
using UnityEngine.Tilemaps;
public class CaveInManager : MonoBehaviour
{
    [Header("References")]
    public GameObject floor;
    public GameObject canvas;
    public GameObject notificationPrefab;
    public GemGeneration gemGenerationScript;
    public LayerMask terrainMask;
    public Tilemap terrainTileMap;
    public RuleTile terrainTile;


    [Header("Cave-In Customization")]
    public float chancePerSecond;
    public float radius;
    private float timer;
    private float timeSinceLastCaveIn = 0;
    public float minTimeBetweenCaveIn;
    private float randomNum;
    public int terrainChunksToSpawn;
    public int minCaveInTileRadius;
    public int maxCaveInTileRadius;
    public float chanceDecreasePerTileOutwards;
    public float chanceToSpawnGemPerTile;

    public float gemVeritcalOffest;
    public int gemSpawnLimit;

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

            //OLD
            Collider[] hitblocks = Physics.OverlapSphere(pos, radius, terrainMask);

            //NEW
            //start at pos, cast increasingly outwards
            print(pos);
            StartCoroutine(terrainChunkSpawn(pos));
            foreach(Collider col in hitblocks)
            {     
                         
                if (col.gameObject.GetComponent<MeshRenderer>().enabled == false) //if the box is invisible
                {
                    col.gameObject.GetComponent<BlockDestroy>().enableCube();
                }                 
            }

            //for(int y = 0; y <= gemSpawnAmt; y++)
            //{
            //    int randIndex = Random.Range(0, gemGenerationScript.clusterPrefabs.Count);
            //    int randPiece = Random.Range(0, hitblocks.Length -1);
            //    Instantiate(gemGenerationScript.clusterPrefabs[randIndex], hitblocks[randPiece].transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));

            //}
        }
    }

    public IEnumerator terrainChunkSpawn(Vector3 pos)
    {
        int gemSpawnAmt = 0;

        //find the origin
        Vector3Int tilePos = terrainTileMap.WorldToCell(pos);

        //determine the magnitude of blocks outwards - how many tiles outwards will this cave in reach?
        int magnitudeToCycle = Random.Range(minCaveInTileRadius, maxCaveInTileRadius);
        
        //Tile testTile = terrainTileMap.

        //for each magnitude level
        for(int i = 1; i <= magnitudeToCycle; i++)
        {
            //max and min bounds of that magnitude, cycle through every tile
            for (int x = -i; x < i; x++)
            {
                for (int y = i; y > -i; y--)
                {
                    //this is now the targeted tile co-ordinate
                    //wait a bit between each placement
                    yield return new WaitForSeconds(Random.Range(0.001f, 0.005f));
                    
                    //lower chance to spawn the greater you go out from the center
                    float terrainSpawnRand = Random.Range(0, 100);

                    //if it hits the random chance, and there is no terrain in the spot
                    if (terrainSpawnRand < 100 - (chanceDecreasePerTileOutwards * (i - 1)) && terrainTileMap.GetTile(new Vector3Int(tilePos.x + x, tilePos.y + y)) == null)
                    //set the tile at that co-ordinate to terrain
                    terrainTileMap.SetTile(new Vector3Int(tilePos.x + x, tilePos.y + y), terrainTile);

                    float gemSpawnRand = Random.Range(0, 100);

                    if(gemSpawnRand <= chanceToSpawnGemPerTile && gemSpawnAmt <= gemSpawnLimit)
                    {
                        int randIndex = Random.Range(0, gemGenerationScript.clusterPrefabs.Count);
                        Instantiate(gemGenerationScript.clusterPrefabs[randIndex], terrainTileMap.CellToWorld(new Vector3Int (tilePos.x + x, tilePos.y + y)), Quaternion.Euler(0, Random.Range(0, 360), 0));
                        gemSpawnAmt++;
                    }

                }
            }
        }

        yield return null;
    }
}
