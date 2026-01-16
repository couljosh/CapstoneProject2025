using FMOD.Studio;
using FMODUnity;
using System.Collections;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TerrainUtils;

public class DrillLogic : MonoBehaviour
{
    [Header("Variables")]
    public float startDelay;
    public float movingElapsedTime;
    public float startElapsedTime;
    public float maxSpeed;
    public float minTurn;
    public float timeToFullSpeed;
    private float currentSpeedUpProgress;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentTurn;

    [Header("Checks")]
    public bool isDrillMoving = false;
    public bool isDrill = true;

    [Header("References")]
    public AnimationCurve speedUpCurve;
    public AnimationCurve startupRumbleCurve;
    private DrillExplode drillExplode;
    public PlayerDeath playerDeath;
    private SceneChange sceneChange;
    //public AnimationCurve slowTurnCurve; //if we want turn speed to gradually slow down

    [Header("Audio")]
    public FMODUnity.EventReference drillEvent;
    public EventInstance drillInstance;

    //rumble
    bool finalExplosionPerformed = false; 


    void Start()
    {
        currentSpeed = 0;
        drillInstance = RuntimeManager.CreateInstance(drillEvent);

        drillExplode = gameObject.GetComponentInChildren<DrillExplode>();
        playerDeath = gameObject.transform.parent.GetComponent<PlayerDeath>();
        sceneChange = GameObject.FindFirstObjectByType<SceneChange>();

        Invoke("PlayDrillSound", 1f);
    }

    void Update()
    {
        if (startElapsedTime < startDelay)
        {
            startElapsedTime += Time.deltaTime;
            initialRumble();

        }
        else
        {
            isDrillMoving = true;
            movingElapsedTime += Time.deltaTime;

            //rumble logic
            if (!drillExplode.hasExploded && !(sceneChange.isTimeOut && !sceneChange.isOvertime)) //only rumble when hasn't exploded, and time has run out without overtime
                MovingRumble();
            else
                EndRumble();

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

        //if it hits a bomb that is lodged (kinematic)
        if (collision.gameObject.tag == "Bomb" && collision.gameObject.GetComponent<Rigidbody>().isKinematic == true)
        {
            //explode it instantly
            collision.gameObject.GetComponent<BombExplode>().ExplodeWithoutDelay();
        }
    }


    private void initialRumble()
    {
        float rumbleCurve = startupRumbleCurve.Evaluate(startElapsedTime);
        playerDeath.playerGamepad.SetMotorSpeeds(rumbleCurve, rumbleCurve);
    }

    private void MovingRumble()
    {
        playerDeath.playerGamepad.SetMotorSpeeds(0.08f, 0.08f);
    }

    public void EndRumble()
    {
        if(!finalExplosionPerformed)
        StartCoroutine(explosionRumble());

    }

    public IEnumerator explosionRumble()
    {
        finalExplosionPerformed = true;
        playerDeath.playerGamepad.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.1f);
        playerDeath.playerGamepad.SetMotorSpeeds(0.0f, 0.0f);
        yield return null;
    }

    void PlayDrillSound()
    {
        drillInstance.start();

    }
}