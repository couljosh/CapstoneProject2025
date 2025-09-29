using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GemGeneration : MonoBehaviour
{
    [Header("Gem Generation Customization")]
    public List<GameObject> clusterPrefabs = new List<GameObject>();
    public int clusterSpawnAmt;

    [Header("Stored References")]
    public List<GameObject> spawnedGems = new List<GameObject>();
    public List<GameObject> spawnLocations = new List<GameObject>();
    public GameObject[] terrainPieces;

    void Start()
    {
        terrainPieces = GameObject.FindGameObjectsWithTag("Terrain");
        
        //Trigger Gem Generation In Order
        ChooseLocations();
        SpawnClusters();
            
    }

    //Chooses Gem Locations
    public void ChooseLocations()
    {
        for(int i = 0;  i < clusterSpawnAmt -1; i++)
        {
            int chosenIndex = Random.Range(0, terrainPieces.Length);
            GameObject chosenPiece = terrainPieces[chosenIndex];
            spawnLocations.Add(chosenPiece);
        }
    }

    //Spawn Gems/Clusters
    public void SpawnClusters()
    {
        foreach(GameObject location in spawnLocations)
        {
            int i = Random.Range(0, clusterPrefabs.Count);
            Instantiate(clusterPrefabs[i], location.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            spawnedGems.Add(clusterPrefabs[i]);
        }
    }
}
