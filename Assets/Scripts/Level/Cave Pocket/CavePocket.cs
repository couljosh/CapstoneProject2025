using System.Collections;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class CavePocket : MonoBehaviour
{
    [Header("Cave-Pocket Size Customization")]
    public float minRadius;
    public float maxRadius;
    public float timeToScan;
    public float elapsedTime;

    [Header("Layer To Detect")]
    public LayerMask terrainMask;

    public int sphereIterations;
    public float sphereIncrease;

    public bool isFinishedClearing = false;

    void Start()
    {

    }

    private void Update()
    {
        

        if(!isFinishedClearing)
        {
            ClearTerrain();
        }

    }

    //Clears Pocket Terrain
    public void ClearTerrain()
    {
        elapsedTime += Time.deltaTime;

        for (int i = 1; i <= sphereIterations; i++)
        {
            StartCoroutine(SphereTrigger(i));
        }

        isFinishedClearing = true;
    }

    public IEnumerator SphereTrigger(int y)
    {
        yield return new WaitForSeconds(timeToScan * y);

        Collider[] terrainPieces = Physics.OverlapSphere(transform.position, y * sphereIncrease, terrainMask);

        foreach (Collider collider in terrainPieces)
        {
            try
            {
                collider.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
            }
            catch
            {
                continue;
            }
            
        }

        yield return new WaitForSeconds(0.1f);

        //destroy leftovers
        Collider[] piecesToDestroy = Physics.OverlapSphere(transform.position, (y * sphereIncrease) - 0.5f, terrainMask);
        foreach (Collider collider in piecesToDestroy)
        {
            Destroy(collider.gameObject);
        }
    }
}
