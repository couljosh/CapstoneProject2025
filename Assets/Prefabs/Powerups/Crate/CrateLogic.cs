using UnityEngine;

public class CrateLogic : MonoBehaviour
{
    public GameObject[] powerUps;
    public GameObject chosenPowerup;
    private BoxCollider collider; 

    private DynamicCameraFollow dynamicCamera;
    public float timeAfterSpawnToBeAccessible;
    private float timer; 

    void Awake()
    {
        dynamicCamera = Object.FindFirstObjectByType<DynamicCameraFollow>();
    }

    void Start()
    {
        //disable collider for a bit before players can actually pick the box up
        collider = gameObject.GetComponent<BoxCollider>();
        collider.enabled = false; 

        int randInd = Random.Range(0, powerUps.Length);
        chosenPowerup = powerUps[randInd];

        if (dynamicCamera != null)
        {
            dynamicCamera.AddPlayer(transform);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > timeAfterSpawnToBeAccessible ) 
        {
            collider.enabled = true; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Repository"))
        {
            CaveInManager.isPowerupInPlay = false;
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
