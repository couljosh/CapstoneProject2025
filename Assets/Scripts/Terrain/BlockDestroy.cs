using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using NUnit.Framework.Constraints;

public class BlockDestroy : MonoBehaviour
{
    [Header("Terrain Destruction Customization")]
    public float destroyDelay;

    public Tilemap terrainTileMap;
    private Vector3Int myCellLocation;

    public GameObject startingType;
    public GameObject emptyType;

    private TileBase tilebase;



    private void Start()
    {
        terrainTileMap = GameObject.Find("TerrainTileMap").GetComponent<Tilemap>();

        myCellLocation = terrainTileMap.WorldToCell(transform.position);
        //print(myCellLocation);

        startingType = this.gameObject;
    }

    //Dectruction on Collsion
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ObjectDestroyer")
        {
            disableCubeAfterDelay(destroyDelay);


        }
    }

    public void disableCubeAfterDelay(float delay)
    {
        StartCoroutine(disableCube(delay));
        
    }


    public IEnumerator disableCube(float delay)
    {

        yield return new WaitForSeconds(delay);

        gameObject.tag = "InactiveTerrain";


        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        //make box trigger only
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        terrainTileMap.SetTile(myCellLocation, null);

    }

    public void disableCubeNonCoroutine()
    {
        gameObject.tag = "InactiveTerrain";


        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        //make box trigger only
        gameObject.GetComponent<BoxCollider>().isTrigger = true;

        terrainTileMap.SetTile(myCellLocation, null);
    }


    public void enableCube()
    {
        gameObject.tag = "ActiveTerrain";

        //make mesh invisible
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        //re-enable collision
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }

    public void RefreshTile(ref TileData t)
    {

        for (int x = -1; x < 1; x++)
        {
            for (int y = 1; y > -1; y--)
            {

            }
        }

    }

    public void GetTileData()
    {

    }
}
