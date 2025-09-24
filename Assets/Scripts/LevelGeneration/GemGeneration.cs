using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GemGeneration : MonoBehaviour
{
    public List<GameObject> clusterPrefabs = new List<GameObject>();
    public List<GameObject> gemPrefabs = new List<GameObject>();
    public List<GameObject> spawnLocations = new List<GameObject>();

    public GameObject[] terrainPieces;

    public int clusterSpawnAmt;

    

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
        for(int i = 0;  i < clusterSpawnAmt; i++)
        {
            int chosenIndex = Random.Range(0, terrainPieces.Length);
            GameObject chosenPiece = terrainPieces[chosenIndex];
            spawnLocations.Add(chosenPiece);
            Debug.Log("reached");
        }
    }

    public void SpawnClusters()
    {
        foreach(GameObject location in spawnLocations)
        {
            int i = Random.Range(0, 3);
            Instantiate(clusterPrefabs[i], location.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            Debug.Log(transform.position);
        }
    }
  
}
