using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RepoMoveSystem : MonoBehaviour
{
    [Header("Stored Refenences")]
    public GameObject repository;
    public GameObject[] repoSpawnNodes;
    public GameObject currentLoc;
    private TerrainBlast terrainBlastScript;
    public DynamicCameraFollow dynamicCamera;

    [Header("Repository Customization")]
    public float activeDuration;
    private float raisingElapsedTime;
    private float loweringElapsedTime;

    //Movement
    public float raiseDuration;
    public float raiseOffset;
    public float minOffDelay;
    public float maxOffDelay;
    public float delayBeforeFirstActive;

    public float elaspedTime;

    private Vector3 raisedPos;
    private float raiseLerpT;
    private float lowerLerpT;
    private int randInd;

    public float timeLeftWhenAlarm;

    [Header("Repository Checks")]
    public bool isRaising;
    public bool isLowering;
    public bool isSwitching;
    public bool depositComplete = false;
    private bool retractStarted = false;
    private bool isGameActive = false;

    private void Awake()
    {
        SceneChange.OnGameStart += StartRepoSystem;
    }

    void Start()
    {
        //Find All Nodes
        repoSpawnNodes = GameObject.FindGameObjectsWithTag("RepoSpawn");

        terrainBlastScript = repository.GetComponent<TerrainBlast>();

        //Ensure repo disabled
        repository.GetComponent<RepositoryLogic>().DisableRepo();
    }

    private void StartRepoSystem()
    {
        // This method is called when the countdown 
        isGameActive = true;
        Invoke("FindNewSpot", delayBeforeFirstActive);
    }


    private void Update()
    {
        if (!isGameActive) return; //pauses logic until the game starts

        // Lower Check //---------------------------------------------------------------------------------------
        elaspedTime += Time.deltaTime;

        if (elaspedTime > activeDuration)
        {

            repository.GetComponent<RepositoryLogic>().DisableRepo();

            Animator anim = repository.GetComponent<RepositoryLogic>().repoAnimation;

            if (!retractStarted)
            {
                Debug.Log("retract starting");
                retractStarted = true;
                anim.SetBool("Appear", false);
                anim.SetBool("Retract", true);

            }

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("Repo Dissapear") && info.normalizedTime >= 1f)
            {
                isLowering = true;
            }

        }
        else
        {
            loweringElapsedTime = 0;
            lowerLerpT = 0;
            retractStarted = false;
            isLowering = false;
        }

        //after calculations are complete, lower if needed
        if (isLowering)
        {
            LowerRepo();
        }

        // Raise Check //---------------------------------------------------------------------------------------
        if (isRaising)
        {
            RaiseRepo();
            elaspedTime = 0;
        }
        else
        {
            raisingElapsedTime = 0;
            raiseLerpT = 0;
        }
    }


    // Choose New Spot //---------------------------------------------------------------------------------------
    void FindNewSpot()
    {
        //Pick random starting spawn loc
        randInd = Random.Range(0, repoSpawnNodes.Length - 1);
        currentLoc = repoSpawnNodes[randInd];

        //Moves Repo to that location
        repository.transform.position = currentLoc.transform.position;

        isSwitching = false;
        isRaising = true;
    }


    // Raising & Activating //---------------------------------------------------------------------------------------
    void RaiseRepo()
    {
        repository.GetComponent<RepositoryLogic>().repoAnimation.SetBool("Appear", true);
        repository.GetComponent<RepositoryLogic>().repoAnimation.SetBool("Retract", false);

        raisedPos = currentLoc.transform.position + new Vector3(0, raiseOffset, 0);

        if (raisingElapsedTime < raiseDuration)
        {
            raisingElapsedTime += Time.deltaTime;

            raiseLerpT = raisingElapsedTime / raiseDuration;

            repository.transform.position = Vector3.Lerp(currentLoc.transform.position, raisedPos, raiseLerpT);

        }
        else
        {

            raisingElapsedTime = 0;
            isRaising = false;
            terrainBlastScript.isFinishedClearing = false;
            repository.GetComponent<RepositoryLogic>().ActivateRepo();

        }

        //add as transform for dynamic camera
        dynamicCamera.AddPlayer(repository.transform);

    }


    // Lowering & Deactivating //---------------------------------------------------------------------------------------
    void LowerRepo()
    {

        //Raise Location
        if (loweringElapsedTime < raiseDuration)
        {
            loweringElapsedTime += Time.deltaTime;


            lowerLerpT = loweringElapsedTime / raiseDuration;
            repository.transform.position = Vector3.Lerp(raisedPos, currentLoc.transform.position, lowerLerpT);

        }
        else if (!isSwitching)
        {
            isSwitching = true;
            depositComplete = false;
            StartCoroutine(DelayedSwitch());
        }

        //Remove transform from dynamic camera
        dynamicCamera.players.Remove(repository.transform);
    }

    // Delay //-------------------------------------------------------------------------------------------------------
    IEnumerator DelayedSwitch()
    {

        float offDelayDuration = Random.Range(minOffDelay, maxOffDelay);
        yield return new WaitForSeconds(offDelayDuration);
        FindNewSpot();
        elaspedTime = 0;

    }

}