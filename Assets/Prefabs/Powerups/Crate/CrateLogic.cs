using UnityEngine;

public class CrateLogic : MonoBehaviour
{
    public GameObject[] powerUps;
    public GameObject chosenPowerup;

    private DynamicCameraFollow dynamicCamera;

    void Awake()
    {
        dynamicCamera = Object.FindFirstObjectByType<DynamicCameraFollow>();
    }

    void Start()
    {
        int randInd = Random.Range(0, powerUps.Length);
        chosenPowerup = powerUps[randInd];

        if (dynamicCamera != null)
        {
            dynamicCamera.AddPlayer(transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Repository"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("ActiveTerrain"))
        {
            other.GetComponent<BlockDestroy>()
                 ?.disableCubeAfterDelay(0);
        }
    }

    private void OnDestroy()
    {
        if (dynamicCamera != null)
        {
            dynamicCamera.RemovePlayer(transform);
        }
    }
}
