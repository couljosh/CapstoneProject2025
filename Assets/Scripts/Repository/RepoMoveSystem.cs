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
    public DynamicCameraFollow dynamicCamera; //dynamic camera script ref for adding and removing repo from it

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







    void Start()
    {
        //dynamicCamera = Camera.main.GetComponent<DynamicCameraFollow>();

        // SYSTEM SETUP //---------------------------------------------------------------------------------------
        //Find All Nodes
        repoSpawnNodes = GameObject.FindGameObjectsWithTag("RepoSpawn");

        terrainBlastScript = repository.GetComponent<TerrainBlast>();  

        //Ensure repo disabled
        repository.GetComponent<RepositoryLogic>().DisableRepo();

        //Find first spot
        Invoke("FindNewSpot", delayBeforeFirstActive);
    }


    private void Update()
    {

        // Lower Check //---------------------------------------------------------------------------------------
        elaspedTime += Time.deltaTime;

        if (elaspedTime > activeDuration)
        {

            repository.GetComponent<RepositoryLogic>().DisableRepo();

            Animator anim = repository.GetComponent<RepositoryLogic>().repoAnimation;

            if (!retractStarted) //check to make sure retract anim has not started
            {
                Debug.Log("retract starting");
                retractStarted = true;
                anim.SetBool("Appear", false); //retract animation play
                anim.SetBool("Retract", true);
                
            }

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0); //access current progress of repo animation

            if (info.IsName("Repo Dissapear") && info.normalizedTime >= 1f)
            {
                isLowering = true;
            }

        }
        else
        {
            loweringElapsedTime = 0;
            lowerLerpT = 0;
            retractStarted = false; //reset for next animation.
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

        //if (elaspedTime >= (activeDuration - timeLeftWhenAlarm))
        //{
        //    repository.GetComponent<RepositoryLogic>().repoAlarm.SetActive(true);
        //}


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
        //Raise Location

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
        //Deactivate it 

        //Raise Location
        if (loweringElapsedTime < raiseDuration)
        {
            loweringElapsedTime += Time.deltaTime;

            
            lowerLerpT = loweringElapsedTime / raiseDuration;
            //print(loweringElapsedTime);
            repository.transform.position = Vector3.Lerp(raisedPos, currentLoc.transform.position, lowerLerpT);

        }
        else if(!isSwitching)
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