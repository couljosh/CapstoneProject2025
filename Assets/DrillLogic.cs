using UnityEngine;
using UnityEngine.TerrainUtils;


public class DrillLogic : MonoBehaviour
{
    public float startDelay;

    public float explodeRad;

    public float movingElapsedTime;
    public float startElapsedTime;

    public bool isDrillMoving;


    public float maxSpeed;
    public float minTurn;

    public float timeToFullSpeed;

    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentTurn;

    void Start()
    {
        isDrillMoving = true;
        currentSpeed = 0;
    }

    void Update()
    {
        if (startElapsedTime < startDelay)
        {
            startElapsedTime += Time.deltaTime;
        }
        else
        {
            movingElapsedTime += Time.deltaTime;

            float currentSpeedUpProgress = movingElapsedTime / timeToFullSpeed;
            currentSpeed = Mathf.Lerp(0, maxSpeed, currentSpeedUpProgress);
            currentTurn = Mathf.Lerp(10, minTurn, currentSpeedUpProgress);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "ActiveTerrain")
        {
            collision.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }
    }
}