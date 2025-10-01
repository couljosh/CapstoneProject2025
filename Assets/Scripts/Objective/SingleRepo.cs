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


    void Start()
    {
        repoLight.intensity = intensity;
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;
    }


    void Update()
    {
        SetLight();
    }


    private void OnTriggerEnter(Collider other)
    {
        elaspedTime = 0;
        
    }

    //Actively Depositing Check
    private void OnTriggerStay(Collider other)
    {
        PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();
        PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();

        //If the repository is active and a player is near
        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            //Start increasing timer
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
        //Resets the Repo
        repoLight.intensity = intensity;
        repoLight.color = originalColor;
        progressBar.fillAmount = 0;

    }





    public void DepositeAll(PlayerMove move, PlayerDeath death)
    {
        //If the player is on the red team
        if (move.playerNum <= 2)
        {
            print("successfully scored");
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
    }

    public void ProgressSignifer(PlayerMove playerMove)
    {
        if (playerMove.playerNum > 2)
        {
            repoLight.color = Color.Lerp(originalColor, Color.blue, (float)(depositTime - 0.5));
            progressBar.color = Color.blue;
        }

        else
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
