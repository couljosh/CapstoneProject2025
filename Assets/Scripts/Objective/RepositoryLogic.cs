using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepositoryLogic : MonoBehaviour
{
    [Header("Stored References")]
    public SceneChange score;
    public Image progressBar;
    public Image radiusImg;
    public GameObject gemCountText;
    public Light repoLight;

    [Header("Repository Checks")]
    public bool isIncrease;
    public bool depositAll = false;
    public bool active = true;

    [Header("Repository Customization")]
    public float depositTime;
    public float elaspedTime;
    public float intensity;
    public float flickerSpeed;
    public Color originalColor;

    public List<GameObject> enteredPlayersTeam1 = new List<GameObject>();
    public List<GameObject> enteredPlayersTeam2 = new List<GameObject>();
    private bool canDepositContinue = true;
    private int currentlyDepositingTeam;

    public Color redTeamColor;
    public Color blueTeamColor;
    public Image timerProgress;
    public RepoMover repoMoverScript;
    private float elapsedTimeLastFrame;

    [Header("Repository States")]
    public bool teamOneCanDepo;
    public bool teamTwoCanDepo;
    public bool isContested;
    public bool isEmpty;

    public float depositProgress;
    public float increaseMult;
    public float decreaseMult;

    public int teamlastDepo; //1 or 2

    public PlayerDeath depositor;
    public GameObject repoAlarm;

    public List<GameObject> allEnteredPlayers = new List<GameObject>();


    void Start()
    {
        repoMoverScript = GameObject.Find("RepoMover").GetComponent<RepoMover>();
        score = GameObject.Find("SceneManager").GetComponent<SceneChange>();

        timerProgress.fillAmount = repoMoverScript.switchInterval;
        repoLight.intensity = intensity;
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;

    }


    void Update()
    {
        // SYSTEM STRUCTURE //---------------------------------------------------------------------------------------
        progressBar.fillAmount = depositProgress / depositTime;
        SetLight();

        if(allEnteredPlayers.Count > 0)
        depositor = allEnteredPlayers[0].GetComponent<PlayerDeath>();


        // EMPTY //--------------------------------------------------------------------------------------------------
        if (isEmpty)
        {            
            //Reduce progress until 0
            if(depositProgress > 0)
            {
               // depositProgress -= Time.deltaTime * decreaseMult;
            }

            //Set Signifiers to Neutral
            repoLight.color = originalColor;
        }


        // CONTESTED //----------------------------------------------------------------------------------------------
        if (isContested)
        {

        }


        // TEAM 1 DEPOSITING //--------------------------------------------------------------------------------------
        if (teamOneCanDepo)
        {
            if(teamlastDepo == 2)
            {
                depositProgress = 0;
            }

            teamlastDepo = 1;

            //Increase progress until full
            depositProgress += Time.deltaTime;

            //Set signifiers to team one depositing
            repoLight.color = Color.Lerp(originalColor, redTeamColor, (float)(depositTime - 0.5));
            repoLight.intensity = intensity * 4;
            progressBar.color = Color.red;

            if(depositProgress >= depositTime)
            {
                CompleteDeposit();
            }
        }


        // TEAM 2 DEPOSITING //----------------------------------------------------------------------------------------
        if (teamTwoCanDepo)
        {
            if (teamlastDepo == 1)
            {
                depositProgress = 0;
            }

            teamlastDepo = 2;

            //Increase progress until full
            depositProgress += Time.deltaTime;

            //Set signifiers to team two depositing
            repoLight.color = Color.Lerp(originalColor, blueTeamColor, (float)(depositTime - 0.5));
            repoLight.intensity = intensity * 4;
            progressBar.color = Color.blue;
            
            if (depositProgress >= depositTime)
            {
                CompleteDeposit();
            }
        }   
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            allEnteredPlayers.Add(other.gameObject);

            if (depositor != other.gameObject || depositor == null)
            {
                depositor = other.GetComponent<PlayerDeath>();
            }

            //add to list of players currently in range based on their team affiliation
            if (other.GetComponent<PlayerMove>().playerNum == 1 || other.GetComponent<PlayerMove>().playerNum == 2)
                enteredPlayersTeam1.Add(other.gameObject);
            else if (other.GetComponent<PlayerMove>().playerNum == 3 || other.GetComponent<PlayerMove>().playerNum == 4)
                enteredPlayersTeam2.Add(other.gameObject);

            ConditionCheck();
        }
    }

    private void OnTriggerExit(Collider other)
    {   

        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            allEnteredPlayers.Remove(other.gameObject);

            //remove leaving player from list
            if (other.GetComponent<PlayerMove>().playerNum == 1 || other.GetComponent<PlayerMove>().playerNum == 2)
                enteredPlayersTeam1.Remove(other.gameObject);
            else if (other.GetComponent<PlayerMove>().playerNum == 3 || other.GetComponent<PlayerMove>().playerNum == 4)
                enteredPlayersTeam2.Remove(other.gameObject);

            ConditionCheck();

        }
        }

    public void ConditionCheck()
    {

        //EMPTY
        if(enteredPlayersTeam1.Count <= 0 && enteredPlayersTeam2.Count <= 0)
        {
            depositProgress = 0;
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }

        //CONTESTED
        if (enteredPlayersTeam1.Count > 0 && enteredPlayersTeam2.Count > 0)
        {
            isContested = true;
        }
        else
        {
            isContested = false;
        }

        //TEAM 1 DEPOSITING
        if (enteredPlayersTeam1.Count >= 1 && enteredPlayersTeam2.Count < 1 && depositor.collectedGems.Count > 0)
        {
            teamOneCanDepo = true;
        }
        else
        {
            teamOneCanDepo = false;
        }

        //TEAM 2 DEPOSITING
        if (enteredPlayersTeam2.Count >= 1 && enteredPlayersTeam1.Count < 1 && depositor.collectedGems.Count > 0)
        {
            teamTwoCanDepo = true;
        }
        else
        {
            teamTwoCanDepo = false;
        }
    }


    public void CompleteDeposit()
    {
        depositProgress = 0;

        if (teamOneCanDepo)
        {
            score.redTotal += depositor.collectedGems.Count;
        }

        if (teamTwoCanDepo)
        {
            score.blueTotal += depositor.collectedGems.Count;
        }

        depositor.collectedGems.Clear();
        depositor.gemCount = 0;

        teamOneCanDepo = false;
        teamTwoCanDepo = false;

        repoLight.intensity = intensity;
        repoLight.color = originalColor;
    }


    public void SetLight()
    {
        //Active Repository Signifier
        if (!active)
        {
            // Disable all checks
            teamOneCanDepo = false;
            teamTwoCanDepo = false;
            isContested = false;
            isEmpty = false;
            depositor = null;

            enteredPlayersTeam1.Clear();
            enteredPlayersTeam2.Clear();


            timerProgress.enabled = false;
            radiusImg.enabled = false;
            repoLight.enabled = false;
            depositProgress = 0;
            repoLight.color = originalColor;
            repoLight.intensity = intensity;
            timerProgress.fillAmount = repoMoverScript.switchInterval;
            repoAlarm.SetActive(false);
            



        }
        else
        {
            repoLight.enabled = true;
            timerProgress.enabled = true;
            radiusImg.enabled = true;
            timerProgress.fillAmount -= 1f / repoMoverScript.switchInterval * Time.deltaTime;
            repoAlarm.SetActive(true);
        }
    }

    void CheckDepositor()
    {
        
    }
}
