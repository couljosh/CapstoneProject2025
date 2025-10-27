using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class RepositoryLogic : MonoBehaviour
{
    [Header("Script References")]
    public SceneChange score;
    public PlayerDeath depositor;
    public RepoMover repoMoverScript;

    [Header("UI References")]
    public Image progressBar;
    public Image radiusImg;
    public Image timerProgress;
    public Light repoLight;

    [Header("Object References")]
    public GameObject repoAlarm;
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
    public bool active = true;
    public int teamlastDepo;
    public int singleCheck;

    [Header("Repository Customization")]
    public float depositTime;
    public float increaseMult;
    public float decreaseMult; //If a progress reduction is added when the radius is empty
    private float depositProgress;

    [Header("Color Customization")]
    public Color originalColor;
    public Color redTeamColor;
    public Color blueTeamColor;

    [Header("Repository State Checks")]
    public bool teamOneCanDepo;
    public bool teamTwoCanDepo;
    public bool isContested;
    public bool isEmpty;
    public LayerMask player;

    [Header("Sound Variables")]
    public EventReference depositRef;
    private FMOD.Studio.EventInstance instance;
    FMOD.Studio.PLAYBACK_STATE playBackState;


    void Start()
    {
        //Script References
        repoMoverScript = GameObject.Find("RepoMover").GetComponent<RepoMover>();
        score = GameObject.Find("SceneManager").GetComponent<SceneChange>();


        //Start with default repository settings
        timerProgress.fillAmount = repoMoverScript.switchInterval;
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;
        ClearStartingArea();

        //Sound References
        instance = FMODUnity.RuntimeManager.CreateInstance(depositRef);
    }


    void Update()
    {

        ConditionCheck();
        print(largeGemsInRadius.Count);

        // SYSTEM STRUCTURE //---------------------------------------------------------------------------------------
        progressBar.fillAmount = depositProgress / depositTime;
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

        }


        // TEAM 1 DEPOSITING //--------------------------------------------------------------------------------------
        if (teamOneCanDepo)
        {
            //Increase progress until full
            depositProgress += Time.deltaTime;

            //Team 1 Signifiers Active
            progressBar.color = Color.red;

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
            progressBar.color = Color.blue;

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

        if (other.gameObject.tag == "LargeGem" && active)
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
            depositProgress = 0;
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
            ConditionalSoundTrigger();
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
            ConditionalSoundTrigger();
        }
        else
        {
            teamTwoCanDepo = false;
        }
    }


    public void CompleteDeposit()
    {
        depositProgress = 0;

        //Add Red Score
        if (teamOneCanDepo)
        {
            score.redTotal += depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);
        }

        //Add Blue Score
        if (teamTwoCanDepo)
        {
            score.blueTotal += depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);
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


        repoMoverScript.elaspedTime = repoMoverScript.switchInterval;

        teamOneCanDepo = false;
        teamTwoCanDepo = false;
    }


    public void DisableRepo()
    {
        teamOneCanDepo = false;
        teamTwoCanDepo = false;
        isContested = false;
        isEmpty = false;

        depositor = null;
        enteredPlayersTeam1.Clear();
        enteredPlayersTeam2.Clear();

        singleCheck = 0;
        teamlastDepo = 0;

        timerProgress.enabled = false;
        radiusImg.enabled = false;
        repoLight.enabled = false;
        depositProgress = 0;
        repoLight.color = originalColor;
        timerProgress.fillAmount = repoMoverScript.switchInterval;
        repoAlarm.SetActive(false);
    }

    public void ActivateRepo()
    {
        repoLight.enabled = true;
        timerProgress.enabled = true;
        radiusImg.enabled = true;
        timerProgress.fillAmount -= 1f / repoMoverScript.switchInterval * Time.deltaTime;
        repoAlarm.SetActive(true);
        CheckWhenSetActive();
    }


    void ConditionalSoundTrigger()
    {
        bool isPaused;
        RESULT r = instance.getPaused(out isPaused);
        {
            if (r == RESULT.OK)
                //Sound for same team
                if (isPaused && teamlastDepo == singleCheck)
                {
                    instance.setPaused(false);
                }

            //Sound for new team
            if (teamlastDepo != singleCheck)
            {
                PlaySound();

            }
        }
    }


    void PlaySound()
    {
        instance.setPaused(false);
        instance.start();
    }


    void ClearStartingArea()
    {
        Instantiate(startingPocket, transform.position, Quaternion.identity);

    }


    void CheckWhenSetActive()
    {
        Collider[] playersHit = Physics.OverlapSphere(transform.position, depositRadius.radius, player, QueryTriggerInteraction.Ignore);

        if (playersHit.Length > 0)
        {
            foreach (var detectedPlayer in playersHit)
            {
                if (detectedPlayer != null)
                    addEnteredPlayer(detectedPlayer.gameObject);
            }
        }
    }


    void addEnteredPlayer(GameObject player)
    {
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
}
