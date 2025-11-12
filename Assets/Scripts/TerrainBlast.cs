using UnityEngine;
using UnityEngine.TerrainUtils;
using System.Collections;

public class TerrainBlast : MonoBehaviour
{
    public int sphereIterations;
    public float sphereIncrease;
    public bool isFinishedClearing;
    public float timeToScan;
    public LayerMask terrainMask;

    private void Update()
    {

        if (!isFinishedClearing)
        {
            ClearTerrain();
        }

    }

    //Clears Pocket Terrain
    public void ClearTerrain()
    {

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


