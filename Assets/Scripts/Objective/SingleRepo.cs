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
    public float intensity = 15;
    public float flickerSpeed;
    public Color originalColor;

    private List<GameObject> enteredPlayersTeam1 = new List<GameObject>();
    private List<GameObject> enteredPlayersTeam2 = new List<GameObject>();
    private bool canDepositContinue = true;
    private int currentlyDepositingTeam;

    void Start()
    {
        repoLight.intensity = intensity;
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;

        score = GameObject.Find("SceneManager").GetComponent<SceneChange>();
    }


    void Update()
    {
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
                elaspedTime += Time.deltaTime;
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
            }
            if (currentlyDepositingTeam == 2 && enteredPlayersTeam2.Count == 0)
            {
                elaspedTime = 0;
                progressBar.fillAmount = 0;
                repoLight.color = originalColor;

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
        
    }

    public void ProgressSignifer(PlayerMove playerMove)
    {
        if (playerMove.playerNum > 2 && canDepositContinue == true)
        {
            repoLight.color = Color.Lerp(originalColor, Color.blue, (float)(depositTime - 0.5));
            progressBar.color = Color.blue;
        }

        else if (playerMove.playerNum <= 2 && canDepositContinue == true)
        {
            repoLight.color = Color.Lerp(originalColor, Color.red, (float)(depositTime - 0.5));
            progressBar.color = Color.red;
        }
    }

    public void SetLight()
    {
        //Active Repository Signifier
        if (!active)
        {
            repoLight.enabled = false;
        }
        else
        {
            repoLight.enabled = true;
        }
    }



}
