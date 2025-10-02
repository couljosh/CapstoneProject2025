using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;
using static UnityEditor.FilePathAttribute;

public class CavePocketGeneration : MonoBehaviour
{
    [Header("Cave Pocket Customization")]
    public List<GameObject> pocketPrefabs = new List<GameObject>();
    public int pocketHalfAmt;

    [Header("Stored References")]
    public List<GameObject> spawnLocations = new List<GameObject>();
    public GameObject[] terrainPieces;



    [Header("References")]
    public GameObject leftFloor;
    public GameObject rightFloor;
    public LayerMask terrainMask;


    void Start()
    {
        SpawnPockets(leftFloor);
        SpawnPockets(rightFloor);

    }

    //Chooses Pocket Locations
    //public void ChooseLocations()
    //{
    //    for (int i = 0; i < pocketHalfAmt - 1; i++)
    //    {
    //        int chosenIndex = Random.Range(0, terrainPieces.Length);
    //        GameObject chosenPiece = terrainPieces[chosenIndex];
    //        spawnLocations.Add(chosenPiece);
    //    }
    //}

    ////Generates Pockets
    //public void SpawnPockets()
    //{
    //    foreach (GameObject location in spawnLocations)
    //    {
    //        int i = Random.Range(0, pocketPrefabs.Count);
    //        Instantiate(pocketPrefabs[i], location.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    //    }
    //}


    public void SpawnPockets(GameObject floor)
    {
        for (int i = 0; i < pocketHalfAmt; i++)
        {
            //Random Location based on floor bounds
            var bounds = floor.GetComponent<MeshCollider>().bounds;
            var px = Random.Range(bounds.min.x, bounds.max.x);
            var py = Random.Range(bounds.min.y, bounds.max.y);
            var pz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 pos = new Vector3(px, py, pz);


            int ind = Random.Range(0, pocketPrefabs.Count);
            Instantiate(pocketPrefabs[ind], pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }
}
