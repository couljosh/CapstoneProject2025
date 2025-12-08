using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine.ProBuilder.Shapes;
using System;
using Unity.VisualScripting;

public class RepositoryLogic : MonoBehaviour
{
    [Header("Script References")]
    public SceneChange score;
    public PlayerDeath depositor;
    public RepoMoveSystem repoMoveSystemScript;
    public Outline outlineScript;
    public DynamicCamera dynamicCamera;
    public DepositScoreDisplay scoreDisplay;
    public Animator repoAnimation;
    private TerrainBlast terrainBlastScript;

    [Header("UI References")]
    public Image progressBar;
    public Image radiusImg;
    public Image timerProgress;
    public Light repoLight;

    [Header("Object References")]
    public GameObject repoAlarm;
    public Light repoAlarmLight;
    public Light repoAlarmLightTwo;
    public GameObject startingPocket;
    private List<GameObject> enteredPlayersTeam1 = new List<GameObject>();
    private List<GameObject> enteredPlayersTeam2 = new List<GameObject>();
    public List<GameObject> allEnteredPlayers = new List<GameObject>();
    public SphereCollider depositRadius;
    public List<GameObject> largeGemsInRadius = new List<GameObject>();
    public int largeGemValue;

    [Header("Deposit/Activity Checks")]
    public bool isIncrease;
    public bool depositAll = false;
    public bool active;
    public int teamlastDepo;
    public int singleCheck;

    [Header("Repository Customization")]
    public float depositTime;
    public float increaseMult;
    public float decreaseMult; //If a progress reduction is added when the radius is empty
    private float depositProgress;
    public ParticleSystem depositParticles;
    public float repoAlarmDuration;

    [Header("Color Customization")]
    public Color originalColor;
    public Color yellowTeamColor;
    public Color blueTeamColor;
    public Color yellowProgressColor;
    public Color blueProgressColor;

    [Header("Repository State Checks")]
    public bool teamOneCanDepo;
    public bool teamTwoCanDepo;
    public bool isContested;
    public bool isEmpty;
    public LayerMask player;
    public LayerMask kickable;

    [Header("Sound Variables")]
    //public EventReference depositRef;
    private FMOD.Studio.EventInstance instance;
    FMOD.Studio.PLAYBACK_STATE playBackState;

    public Material repoMaterial;
    public Material contestedMaterial;
    public MeshRenderer mesh;

    private MeshRenderer[] meshRenderers;


    void Start()
    {

        dynamicCamera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();

        repoAnimation = GetComponentInChildren<Animator>();
        repoAnimation.SetBool("Appear", false);
        repoAnimation.SetBool("Retract", false);
        //Script References
        repoMoveSystemScript = GameObject.Find("RepoMover").GetComponent<RepoMoveSystem>();
        score = GameObject.Find("SceneManager").GetComponent<SceneChange>();

        timerProgress.fillAmount = repoMoveSystemScript.activeDuration;

        //Start with default repository settings
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;
        ClearStartingArea();
        repoAlarm.SetActive(false);

        //Sound References
        //instance = FMODUnity.RuntimeManager.CreateInstance(depositRef);

        //Deposit Display Reference
        scoreDisplay = GetComponent<DepositScoreDisplay>();

        //depositParticles.enableEmission = false;

        //for keeping the repo terrain-free constantly
        terrainBlastScript = gameObject.GetComponent<TerrainBlast>();

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }


    void Update()
    {
        ConditionCheck();

        //terrain clearing
        if (terrainBlastScript.isFinishedClearing == true)
        {
            //go again, keep the repo clear of terrain at all times
            terrainBlastScript.isFinishedClearing = false;
        }

        // SYSTEM STRUCTURE //---------------------------------------------------------------------------------------
        progressBar.fillAmount = depositProgress / depositTime;

        if (active)
            timerProgress.fillAmount -= 1f / repoMoveSystemScript.activeDuration * Time.deltaTime;

        instance.getPlaybackState(out playBackState);

        if (allEnteredPlayers.Count > 0)
            depositor = allEnteredPlayers[0].GetComponent<PlayerDeath>();


        // EMPTY //--------------------------------------------------------------------------------------------------
        if (isEmpty)
        {
            //Reduces Progress
            //if (depositProgress > 0)
            //{
            //    depositProgress -= Time.deltaTime * decreaseMult;
            //}

            if (instance.isValid())
                instance.setPaused(true);

            //Resets last team checks
            singleCheck = 0;
            teamlastDepo = 0;

        }


        // CONTESTED //----------------------------------------------------------------------------------------------
        if (isContested)
        {
            //repoAlarmLight.color = Color.yellow;
            float flickAmt = Mathf.PingPong(Time.time * 1000, 500);

            ChangeRepoMesh(contestedMaterial);

        }
        else
        {
            ChangeRepoMesh(repoMaterial);
        }


        // TEAM 1 DEPOSITING //--------------------------------------------------------------------------------------
        if (teamOneCanDepo)
        {

            //Increase progress until full
            depositProgress += Time.deltaTime;

            //Team 1 Signifiers Active
            progressBar.color = yellowProgressColor;



            //Complete Deposit Check
            if (depositProgress >= depositTime)
            {
                CompleteDeposit();
            }
        }


        // TEAM 2 DEPOSITING //----------------------------------------------------------------------------------------
        if (teamTwoCanDepo)
        {
            //Increase progress until full
            depositProgress += Time.deltaTime;

            //Team 2 Signifiers Active
            progressBar.color = blueProgressColor;



            //Complete Deposit Check
            if (depositProgress >= depositTime)
            {
                CompleteDeposit();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Tracks Players Entered
        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            addEnteredPlayer(other.gameObject);
        }

        if (other.gameObject.tag == "LargeGem" && active && other.gameObject.GetComponentInParent<LargeGem>().isReleased == true)
        {
            other.gameObject.transform.parent.gameObject.GetComponent<LargeGem>().isInDepositRadius = true;
            largeGemsInRadius.Add(other.gameObject.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Tracks Players Exited
        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            removeEnteredPlayer(other.gameObject);
        }

        if (other.gameObject.tag == "LargeGem" && active)
        {
            other.gameObject.transform.parent.gameObject.GetComponent<LargeGem>().isInDepositRadius = false;
            largeGemsInRadius.Remove(other.gameObject.transform.parent.gameObject);
        }
    }

    public void ConditionCheck()
    {
        // EMPTY //--------------------------------------------------------------------------------------------------
        if (enteredPlayersTeam1.Count <= 0 && enteredPlayersTeam2.Count <= 0)
        {
            depositor = null;
            depositProgress = 0;  //TURN ON IF NOT USING DEPOSIT DECREASE WHEN EMPTY
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }

        // CONTESTED //----------------------------------------------------------------------------------------------
        if (enteredPlayersTeam1.Count > 0 && enteredPlayersTeam2.Count > 0)
        {
            isContested = true;

            //Saves last team to determine what happens when deposit sound is played again
            instance.setPaused(true);
            singleCheck = teamlastDepo;

            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Repository/ContestedDeposit", gameObject.transform.position);


        }
        else
        {
            isContested = false;
        }

        // TEAM 1 DEPOSITING //--------------------------------------------------------------------------------------
        if (enteredPlayersTeam1.Count >= 1 && enteredPlayersTeam2.Count < 1 && depositor.collectedGems.Count > 0)
        {
            teamOneCanDepo = true;

            //Sets as last team 
            if (teamlastDepo == 2)
            {
                depositProgress = 0;
            }
            teamlastDepo = 1;

            //Deposit Sound Trigger
            //ConditionalSoundTrigger();
        }
        else
        {
            teamOneCanDepo = false;
        }

        // TEAM 2 DEPOSITING //----------------------------------------------------------------------------------------
        if (enteredPlayersTeam2.Count >= 1 && enteredPlayersTeam1.Count < 1 && depositor.collectedGems.Count > 0)
        {
            teamTwoCanDepo = true;

            //Sets as last team 
            if (teamlastDepo == 1)
            {
                depositProgress = 0;
            }
            teamlastDepo = 2;

            //Deposit Sound Trigger
            //ConditionalSoundTrigger();
        }
        else
        {
            teamTwoCanDepo = false;
        }
    }


    public void CompleteDeposit()
    {
        //int depositScore = depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);

        depositProgress = 0;

        //depositParticles.enableEmission = true;
        depositParticles.Clear();
        depositParticles.Play();

        // Capture the Z position just before the repo might move/lower.
        float currentRepoWorldZ = transform.position.z;


        //Add Red Score
        if (teamOneCanDepo)
        {
            int depositScore = 0;

            //score.redRoundTotal += depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);
            foreach (GameObject player in enteredPlayersTeam1)
            {
                PlayerDeath playerGems = player.GetComponent<PlayerDeath>();
                //counter display
                score.redRoundTotal += playerGems.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);

                //score display above repo
                depositScore += playerGems.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);

                //clear gems
                playerGems.collectedGems.Clear();
                playerGems.gemCount = 0;
            }
            // PASS THE CURRENT WORLD Z POSITION
            scoreDisplay.ShowScore(depositScore, yellowTeamColor, currentRepoWorldZ);
        }

        //Add Blue Score
        if (teamTwoCanDepo)
        {
            int depositScore = 0;

            //score.blueRoundTotal += depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);
            foreach (GameObject player in enteredPlayersTeam2)
            {
                PlayerDeath playerGems = player.GetComponent<PlayerDeath>();
                //counter display
                score.blueRoundTotal += playerGems.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);

                //score display above repo
                depositScore += playerGems.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);

                //clear gems
                playerGems.collectedGems.Clear();
                playerGems.gemCount = 0;
            }
            // PASS THE CURRENT WORLD Z POSITION
            scoreDisplay.ShowScore(depositScore, blueTeamColor, currentRepoWorldZ);
        }

        //Clear Inventory & Empty 
        depositor.collectedGems.Clear();
        depositor.gemCount = 0;

        if (allEnteredPlayers.Count > 0)
            allEnteredPlayers.RemoveAt(0);

        //get rid of any large gems
        foreach (GameObject largeGem in largeGemsInRadius)
        {
            Destroy(largeGem);
        }
        largeGemsInRadius.Clear();

        if (score.isOvertime)
        {
            if (!score.pointsAdded)
            {
                score.checkScore();
            }
        }
        else
        {
            repoMoveSystemScript.elaspedTime = repoMoveSystemScript.activeDuration;

        }

        teamOneCanDepo = false;
        teamTwoCanDepo = false;

        //play complete deposit noise
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Repository/Deposit", gameObject.transform.position);
        print("reached");

    }


    public void DisableRepo()
    {

        outlineScript.enabled = false;

        teamOneCanDepo = false;
        teamTwoCanDepo = false;
        isContested = false;
        isEmpty = true;

        depositor = null;
        enteredPlayersTeam1.Clear();
        enteredPlayersTeam2.Clear();
        allEnteredPlayers.Clear();

        singleCheck = 0;
        teamlastDepo = 0;

        timerProgress.enabled = false;
        radiusImg.enabled = false;
        repoLight.enabled = false;
        depositProgress = 0;
        repoLight.color = originalColor;

        //account for any large gems that were in the radius (because ontriggerexit isn't called when repos are disabled)
        foreach (GameObject largeGem in largeGemsInRadius)
        {
            largeGem.GetComponentInParent<LargeGem>().isInDepositRadius = false;
        }

        largeGemsInRadius.Clear();

        active = false;
        //reset zoom state for dynamic camera
        //dynamicCamera.ZoomOutTimer = 0;

    }

    public void ActivateRepo()
    {
        active = true;
        timerProgress.fillAmount = repoMoveSystemScript.activeDuration;
        outlineScript.enabled = true;
        repoLight.enabled = true;
        timerProgress.enabled = true;
        radiusImg.enabled = true;
        CheckWhenSetActive();

        StartCoroutine(SetLightValues(repoAlarmDuration));
    }


    //void ConditionalSoundTrigger()
    //{
    //    bool isPaused;
    //    RESULT r = instance.getPaused(out isPaused);
    //    {
    //        if (r == RESULT.OK)
    //            //Sound for same team
    //            if (isPaused && teamlastDepo == singleCheck)
    //            {
    //                instance.setPaused(false);
    //            }

    //        //Sound for new team
    //        if (teamlastDepo != singleCheck)
    //        {
    //            PlaySound();

    //        }
    //    }
    //}


    void PlaySound()
    {
        instance.setPaused(false);
        instance.start();
    }


    void ClearStartingArea()
    {
        // Instantiate(startingPocket, transform.position, Quaternion.identity);

    }


    void CheckWhenSetActive()
    {
        //check for players already in the radius
        Collider[] playersHit = Physics.OverlapSphere(transform.position, depositRadius.radius, player, QueryTriggerInteraction.Ignore);

        if (playersHit.Length > 0 && isEmpty)
        {
            foreach (var detectedPlayer in playersHit)
            {
                if (detectedPlayer != null)
                    addEnteredPlayer(detectedPlayer.gameObject);
            }
        }

        //check for large gems already in the radius
        Collider[] gemsHit = Physics.OverlapSphere(transform.position, depositRadius.radius, kickable, QueryTriggerInteraction.Ignore);

        if (gemsHit.Length > 0)
        {
            foreach (var detectedGem in gemsHit)
            {
                if (detectedGem != null && detectedGem.tag == "LargeGem")
                {
                    largeGemsInRadius.Add(detectedGem.gameObject.transform.parent.gameObject);
                    detectedGem.gameObject.GetComponentInParent<LargeGem>().isInDepositRadius = true;
                }


            }
        }
    }


    void addEnteredPlayer(GameObject player)
    {

        foreach (var detectedPlayer in allEnteredPlayers)
        {
            if (player.gameObject == detectedPlayer)
            {
                return;
            }
        }

        allEnteredPlayers.Add(player);

        //depositor != other.gameObject ||
        if (depositor == null)
        {
            depositor = player.GetComponent<PlayerDeath>();
        }

        //Add to list of players currently in range based on their team affiliation
        if (player.GetComponent<PlayerMove>().playerNum == 1 || player.GetComponent<PlayerMove>().playerNum == 2)
            enteredPlayersTeam1.Add(player.gameObject);
        else if (player.GetComponent<PlayerMove>().playerNum == 3 || player.GetComponent<PlayerMove>().playerNum == 4)
            enteredPlayersTeam2.Add(player.gameObject);

    }


    void removeEnteredPlayer(GameObject player)
    {
        allEnteredPlayers.Remove(player);

        if (depositor != null && allEnteredPlayers.Count > 0)
            depositor = allEnteredPlayers[0].GetComponent<PlayerDeath>();

        //remove leaving player from list
        if (player.GetComponent<PlayerMove>().playerNum == 1 || player.GetComponent<PlayerMove>().playerNum == 2)
            enteredPlayersTeam1.Remove(player.gameObject);
        else if (player.GetComponent<PlayerMove>().playerNum == 3 || player.GetComponent<PlayerMove>().playerNum == 4)
            enteredPlayersTeam2.Remove(player.gameObject);
    }

    IEnumerator SetLightValues(float alarmLength)
    {
        repoAlarm.SetActive(true);

        yield return new WaitForSeconds(alarmLength);

        repoAlarm.SetActive(false);

    }

    void ChangeRepoMesh(Material repoMaterial)
    {
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = repoMaterial;
        }
    }


}

