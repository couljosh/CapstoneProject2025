using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BlockDestroy : MonoBehaviour
{
    [Header("Terrain Destruction Customization")]
    public float destroyDelay;

    public Tilemap terrainTileMap;
    private Vector3Int myCellLocation;

    public GameObject startingType;
    public GameObject emptyType;


    [Header("VFX")]
    public GameObject destructionVFX; 

    private void Start()
    {
        terrainTileMap = GameObject.Find("TerrainTileMap").GetComponent<Tilemap>();
        myCellLocation = terrainTileMap.WorldToCell(transform.position);
        startingType = this.gameObject;
    }

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

        if (destructionVFX != null)
        {
            GameObject vfx = Instantiate(destructionVFX, transform.position, Quaternion.identity);
            //Destroy(vfx, 2f); //auto-destroy after finished
        }

        gameObject.tag = "InactiveTerrain";
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;

        terrainTileMap.SetTile(myCellLocation, null);
    }

    public void disableCubeNonCoroutine()
    {
        if (destructionVFX != null)
        {
            GameObject vfx = Instantiate(destructionVFX, transform.position, Quaternion.identity);
            //Destroy(vfx, 2f);
        }

        gameObject.tag = "InactiveTerrain";
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        terrainTileMap.SetTile(myCellLocation, null);
    }

    public void enableCube()
    {
        gameObject.tag = "ActiveTerrain";
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }
}
