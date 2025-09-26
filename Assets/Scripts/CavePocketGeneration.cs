using System.Collections.Generic;
using UnityEngine;

public class CavePocketGeneration : MonoBehaviour
{
    public List<GameObject> pocketPrefabs = new List<GameObject>();
    public List<GameObject> spawnLocations = new List<GameObject>();

    public GameObject[] terrainPieces;

    public int pocketSpawnAmt;

    void Start()
    {
        terrainPieces = GameObject.FindGameObjectsWithTag("Terrain");

        ChooseLocations();

        SpawnClusters();
    }

    void Update()
    {
        
    }

    public void ChooseLocations()
    {
        for (int i = 0; i < pocketSpawnAmt; i++)
        {
            int chosenIndex = Random.Range(0, terrainPieces.Length);
            GameObject chosenPiece = terrainPieces[chosenIndex];
            spawnLocations.Add(chosenPiece);
        }
    }

    public void SpawnClusters()
    {
         

        foreach (GameObject location in spawnLocations)
        {
            int i = Random.Range(0, pocketPrefabs.Count);
            Instantiate(pocketPrefabs[i], location.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }
}
