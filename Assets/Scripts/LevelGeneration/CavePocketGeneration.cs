using System.Collections.Generic;
using UnityEngine;

public class CavePocketGeneration : MonoBehaviour
{
    [Header("Cave Pocket Customization")]
    public List<GameObject> pocketPrefabs = new List<GameObject>();
    public int pocketSpawnAmt;

    [Header("Stored References")]
    public List<GameObject> spawnLocations = new List<GameObject>();
    public GameObject[] terrainPieces;


    void Start()
    {
        terrainPieces = GameObject.FindGameObjectsWithTag("Terrain");

        //Trigger Pocket Generation In Order
        ChooseLocations();
        SpawnPockets();
    }

    //Chooses Pocket Locations
    public void ChooseLocations()
    {
        for (int i = 0; i < pocketSpawnAmt - 1; i++)
        {
            int chosenIndex = Random.Range(0, terrainPieces.Length);
            GameObject chosenPiece = terrainPieces[chosenIndex];
            spawnLocations.Add(chosenPiece);
        }
    }


    //Generates Pockets
    public void SpawnPockets()
    {
        foreach (GameObject location in spawnLocations)
        {
            int i = Random.Range(0, pocketPrefabs.Count);
            Instantiate(pocketPrefabs[i], location.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }
}
