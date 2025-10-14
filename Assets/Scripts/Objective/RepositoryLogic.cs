using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepositoryLogic : MonoBehaviour
{
    [Header("Stored References")]
    public SceneChange score;
    public Image progressBar;
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


    void Start()
    {

    }


    void Update()
    {
        if (isEmpty)
        {
            //Checks if no one is in radius 
            //Reduce progress until 0
            //Set Signifiers to Neutral
        }

        if (isContested)
        {
            //Checks if both teams array to see if it should contest
            //Pause progress
            //Set signifiers to contested
           
        }

        if (teamOneCanDepo)
        {
            //Checks if team one is in radius && they have gems
            //Increase progress until full
            //Set signifiers to team one depositing

            //
        }

        if (teamTwoCanDepo)
        {
            //Checks if team two is in radius && they have gems
            //Increase progress until full
            //Set signifiers to team two depositing
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ObjectDestroyer")
        {
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
        if (other.gameObject.tag == "ObjectDestroyer")
        {
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
        if (enteredPlayersTeam1.Count >= 1 && enteredPlayersTeam2.Count < 1)
        {
            teamOneCanDepo = true;
        }
        else
        {
            teamOneCanDepo = false;
        }

        //TEAM 2 DEPOSITING
        if (enteredPlayersTeam2.Count >= 1 && enteredPlayersTeam1.Count < 1)
        {
            teamTwoCanDepo = true;
        }
        else
        {
            teamTwoCanDepo = false;
        }
    }

    public void CompleteDeposit(PlayerMove move, PlayerDeath death)
    {
        //Progress set back to 0

    }
}
