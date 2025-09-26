using UnityEngine;
using UnityEngine.TerrainUtils;

public class CavePocket : MonoBehaviour
{
    public float minRadius;
    public float maxRadius;

    public LayerMask terrainMask;

    void Start()
    {
        ClearTerrain();
    }

    void Update()
    {
        
    }

    public void ClearTerrain()
    {
        Collider[] terrainPieces = Physics.OverlapSphere(transform.position, Random.Range(minRadius,maxRadius), terrainMask);

        foreach (Collider collider in terrainPieces)
        {
            collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay();
        }
    }
}
