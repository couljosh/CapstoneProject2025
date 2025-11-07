using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RepoMoveSystem : MonoBehaviour
{
    [Header("Stored Refenences")]
    public GameObject repository;
    public GameObject[] repoSpawnNodes;
    public GameObject currentLoc;

    [Header("Repository Customization")]
    public float activeDuration;
    public float elaspedTime;
    public float raisingElapsedTime;
    public float loweringElapsedTime;

    public int randInd;

    //Movement
    public float raiseDuration;
    public Vector3 raisedPos;
    public float raiseOffset;

    public float raiseLerpT;
    public float lowerLerpT;

    public bool isRaising;
    public bool isLowering;
    public bool isSwitching;

    public float minOffDelay;
    public float maxOffDelay;
    public float offElapsedTime = 0;



    void Start()
    {
        // SYSTEM SETUP //---------------------------------------------------------------------------------------
        //Find All Nodes
        repoSpawnNodes = GameObject.FindGameObjectsWithTag("RepoSpawn");

        //Ensure repo disabled
        repository.GetComponent<RepositoryLogic>().DisableRepo();

        //Find first spot
        FindNewSpot();
    }


    private void Update()
    {
        elaspedTime += Time.deltaTime;

        // Lower Check //---------------------------------------------------------------------------------------
        if (elaspedTime > activeDuration)
        {
            LowerRepo();
        }
        else
        {
            loweringElapsedTime = 0;
            lowerLerpT = 0;
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
            repository.GetComponent<RepositoryLogic>().ActivateRepo();
        }
    }


    // Lowering & Deactivating //---------------------------------------------------------------------------------------
    void LowerRepo()
    {
        //Deactivate it 
        repository.GetComponent<RepositoryLogic>().DisableRepo();

        //Raise Location
        if (loweringElapsedTime < raiseDuration)
        {
            loweringElapsedTime += Time.deltaTime;


            lowerLerpT = loweringElapsedTime / raiseDuration;

            repository.transform.position = Vector3.Lerp(raisedPos, currentLoc.transform.position, lowerLerpT);

        }
        else if(!isSwitching)
        {
            isSwitching = true;
            StartCoroutine(DelayedSwitch());
        }
    }

    // Delay //---------------------------------------------------------------------------------------
    IEnumerator DelayedSwitch()
    {

        float offDelayDuration = Random.Range(minOffDelay, maxOffDelay);
        yield return new WaitForSeconds(offDelayDuration);
        FindNewSpot();
        elaspedTime = 0;

    }
}