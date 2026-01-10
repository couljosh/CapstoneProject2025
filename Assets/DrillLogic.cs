using UnityEngine;
using UnityEngine.TerrainUtils;
using FMODUnity;
using FMOD.Studio;

public class DrillLogic : MonoBehaviour
{
    public float startDelay;

    public float explodeRad;

    public float movingElapsedTime;
    public float startElapsedTime;

    public bool isDrillMoving = false;

    public bool isDrill = true;


    public float maxSpeed;
    public float minTurn;

    public float timeToFullSpeed;

    private float currentSpeedUpProgress;

    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentTurn;

    public FMODUnity.EventReference drillEvent;
    public EventInstance drillInstance;


    public AnimationCurve speedUpCurve;
    //public AnimationCurve slowTurnCurve; //if we want turn speed to gradually slow down


    void Start()
    {
        currentSpeed = 0;
        drillInstance = RuntimeManager.CreateInstance(drillEvent);

        Invoke("PlayDrillSound", 1f);
    }

    void Update()
    {
        if (startElapsedTime < startDelay)
        {
            startElapsedTime += Time.deltaTime;
        }
        else
        {
            isDrillMoving = true;
            movingElapsedTime += Time.deltaTime;       


            currentSpeedUpProgress = movingElapsedTime / timeToFullSpeed;
            float curvedT = speedUpCurve.Evaluate(currentSpeedUpProgress);

            currentSpeed = Mathf.Lerp(0, maxSpeed, curvedT);
            
        }

        //turn starts immediately so players can turn before it starts moving
        currentTurn = Mathf.Lerp(10, minTurn, currentSpeedUpProgress);
        
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "ActiveTerrain")
        {
            collision.gameObject.GetComponent<BlockDestroy>().disableCubeAfterDelay(0);
        }
    }

    void PlayDrillSound()
    {
        drillInstance.start();

    }
}