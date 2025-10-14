using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleRepo : MonoBehaviour
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


    void Start()
    {
        timerProgress.fillAmount = 1;
        repoLight.intensity = intensity;
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;

        score = GameObject.Find("SceneManager").GetComponent<SceneChange>();
        repoMoverScript = GameObject.Find("RepoMover").GetComponent<RepoMover>();

           }


    void Update()
    {
       timerProgress.fillAmount = repoMoverScript.elaspedTime / repoMoverScript.switchInterval;
        

        SetLight();

        if (enteredPlayersTeam1.Count == 0 && enteredPlayersTeam2.Count == 0)
        {
            elaspedTime = 0; progressBar.fillAmount = 0;
        }

        if(enteredPlayersTeam1.Count > 0 && enteredPlayersTeam2.Count == 0)
        {
            currentlyDepositingTeam = 1;
        }
        if(enteredPlayersTeam1.Count == 0 && enteredPlayersTeam2.Count > 0)
        {
            currentlyDepositingTeam = 2;
        }

        if(!active)
        {
            elaspedTime = 0;
            progressBar.fillAmount = 0;
            repoLight.color = originalColor;
            repoLight.intensity = intensity;
        }
       
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ObjectDestroyer")
        {
            //add to list of players currently in range based on their team affiliation
            if(other.GetComponent<PlayerMove>().playerNum == 1 || other.GetComponent<PlayerMove>().playerNum == 2)
            enteredPlayersTeam1.Add(other.gameObject);
            else if (other.GetComponent<PlayerMove>().playerNum == 3 || other.GetComponent<PlayerMove>().playerNum == 4)
            enteredPlayersTeam2.Add(other.gameObject);
        }
    }

    //Actively Depositing Check
    private void OnTriggerStay(Collider other)
    {
        PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();
        PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();

        //If the repository is active and a player is near
        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            if (enteredPlayersTeam1.Count > 0 && enteredPlayersTeam2.Count > 0)
            {
                canDepositContinue = false;
            }
            else
            {
                canDepositContinue = true;
            }

            if (canDepositContinue)
            {
                elaspedTime += Time.deltaTime;
            }

            //if (enteredPlayersTeam1.Count > 1 || enteredPlayersTeam2.Count > 1)
            //{
            //    elaspedTime -= Time.deltaTime/2;
            //}

        }

        //Deposit Progress Signifier
        if (playerDeath != null && playerDeath.collectedGems.Count > 0)
        {
            repoLight.intensity = Mathf.PingPong(Time.time * flickerSpeed, 5);

            if (active)
            {
                progressBar.fillAmount = elaspedTime / depositTime;
                ProgressSignifer(playerMove);
            }
        }


        //What happens for a successful deposit
        if (elaspedTime >= depositTime && playerDeath != null && playerDeath.collectedGems.Count > 0 && active)
        {
            //Deposit Gem Check
            DepositeAll(playerMove, playerDeath);
        } 
    }

    //Trigger Deposit Stopped
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "ObjectDestroyer")
        {
            //remove leaving player from list
            if (other.GetComponent<PlayerMove>().playerNum == 1 || other.GetComponent<PlayerMove>().playerNum == 2)
                enteredPlayersTeam1.Remove(other.gameObject);
            else if (other.GetComponent<PlayerMove>().playerNum == 3 || other.GetComponent<PlayerMove>().playerNum == 4)
                enteredPlayersTeam2.Remove(other.gameObject);

            //prevent opposing team inhereiting deposit progress if the previously depositing team fully leaves the radius
            if (currentlyDepositingTeam == 1 && enteredPlayersTeam1.Count == 0)
            {
                elaspedTime = 0;
                progressBar.fillAmount = 0;
                repoLight.color = originalColor;
                repoLight.intensity = intensity;
            }
            if (currentlyDepositingTeam == 2 && enteredPlayersTeam2.Count == 0)
            {
                elaspedTime = 0;
                progressBar.fillAmount = 0;
                repoLight.color = originalColor;
                repoLight.intensity = intensity;

            }
        }
        
    }

    public void DepositeAll(PlayerMove move, PlayerDeath death)
    {
        //If the player is on the red team
        if (move.playerNum <= 2)
        {
            score.redTotal += death.collectedGems.Count;
        }
        //If the player is on the blue team
        else
        {
            score.blueTotal += death.collectedGems.Count;
        }

        progressBar.color = Color.green;
        death.collectedGems.Clear();
        death.gemCount = 0;
        elaspedTime = 0;
        repoLight.intensity = intensity;

        //remove the player from being considered as still in the ring
        if (death.GetComponent<PlayerMove>().playerNum == 1 || death.GetComponent<PlayerMove>().playerNum == 2)
            enteredPlayersTeam1.Remove(death.gameObject);
        else if (death.GetComponent<PlayerMove>().playerNum == 3 || death.GetComponent<PlayerMove>().playerNum == 4)
            enteredPlayersTeam2.Remove(death.gameObject);

    }

    public void ProgressSignifer(PlayerMove playerMove)
    {
        if (playerMove.playerNum > 2 && canDepositContinue == true)
        {
            repoLight.color = Color.Lerp(originalColor, blueTeamColor, (float)(depositTime - 0.5));
            repoLight.intensity = intensity * 4;
            progressBar.color = Color.blue;
        }

        else if (playerMove.playerNum <= 2 && canDepositContinue == true)
        {
            repoLight.color = Color.Lerp(originalColor, redTeamColor, (float)(depositTime - 0.5));
            repoLight.intensity = intensity * 4;
            progressBar.color = Color.red;
        }
        else
            repoLight.intensity = intensity / 4;
    }

    public void SetLight()
    {
        //Active Repository Signifier
        if (!active)
        {
            timerProgress.enabled = false;
            repoLight.enabled = false;
        }
        else
        {
            repoLight.enabled = true;
            timerProgress.enabled = true;
        }
    }



}
