using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GemGeneration : MonoBehaviour
{
    [Header("Gem Generation Customization")]
    public List<GameObject> clusterPrefabs = new List<GameObject>();
    public int clusterHalfAmt;

    [Header("Stored References")]
    public List<GameObject> spawnedGems = new List<GameObject>();
    public List<GameObject> spawnLocations = new List<GameObject>();
    public GameObject[] terrainPieces;

    public GameObject leftFloor;
    public GameObject rightFloor;

    public float verticalOffset;

    void Start()
    {
        SpawnGems(leftFloor);
        SpawnGems(rightFloor);

    }

    //Chooses Gem Locations
    //public void ChooseLocations()
    //{
    //    for(int i = 0;  i < clusterSpawnAmt -1; i++)
    //    {
    //        int chosenIndex = Random.Range(0, terrainPieces.Length);
    //        GameObject chosenPiece = terrainPieces[chosenIndex];
    //        spawnLocations.Add(chosenPiece);
    //    }
    //}

    ////Spawn Gems/Clusters
    //public void SpawnClusters()
    //{
    //    foreach(GameObject location in spawnLocations)
    //    {
    //        int i = Random.Range(0, clusterPrefabs.Count);
    //        Instantiate(clusterPrefabs[i], location.gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    //        spawnedGems.Add(clusterPrefabs[i]);
    //    }
    //}


    public void SpawnGems(GameObject floor)
    {
        for (int i = 0; i < clusterHalfAmt; i++)
        {
            //Random Location based on floor bounds
            var bounds = floor.GetComponent<MeshCollider>().bounds;
            var px = Random.Range(bounds.min.x, bounds.max.x);
            var py = Random.Range(bounds.min.y, bounds.max.y);
            var pz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 pos = new Vector3(px, py + verticalOffset, pz);


            int ind = Random.Range(0, clusterPrefabs.Count);
            Instantiate(clusterPrefabs[ind], pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }
}
