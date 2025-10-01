using UnityEngine;
using UnityEngine.TerrainUtils;

public class CavePocket : MonoBehaviour
{
    [Header("Cave-Pocket Size Customization")]
    public float minRadius;
    public float maxRadius;
   
    [Header("Layer To Detect")]
    public LayerMask terrainMask;

    void Start()
    {
        ClearTerrain();
    }

    //Clears Pocket Terrain
    public void ClearTerrain()
    {
        Collider[] terrainPieces = Physics.OverlapSphere(transform.position, Random.Range(minRadius,maxRadius), terrainMask);

        foreach (Collider collider in terrainPieces)
        {
            collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }
    }
}
