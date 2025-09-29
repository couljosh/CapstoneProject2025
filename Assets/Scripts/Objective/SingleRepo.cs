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
    }


    void Update()
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


    private void OnTriggerEnter(Collider other)
    {
        elaspedTime = 0;

    }

    //Actively Depositing Check
    private void OnTriggerStay(Collider other)
    {
        PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();
        PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();

        if (other.gameObject.tag == "ObjectDestroyer")
        {
            elaspedTime += Time.deltaTime;
        }

        //Depositing Team Signifier
        if (elaspedTime >= depositTime && playerDeath != null && playerDeath.collectedGems.Count > 0 && active)
        {
            if (playerMove.playerNum > 2)
            {
                repoLight.color = Color.Lerp(originalColor, Color.blue, (float)(depositTime - 0.5));
            }

            else
            {
                repoLight.color = Color.Lerp(originalColor, Color.red, (float)(depositTime - 0.5));
            }


            //Deposit Gem Check
            if (depositAll)
            {
                //If the player is on the red team
                if (playerMove.playerNum <= 2)
                {
                    print("successfully scored");
                    score.redTotal += playerDeath.collectedGems.Count;

                }
                //If the player is on the blue team
                else
                {
                    score.blueTotal += playerDeath.collectedGems.Count;

                }

                playerDeath.collectedGems.Clear();
                playerDeath.gemCount = 0;
                elaspedTime = 0;
            }
            //Deposits one gem at a time
            else
            {
                playerDeath.collectedGems.RemoveAt(0);
                if (playerMove.playerNum <= 2)
                {
                    score.redTotal += 1;
                }
                else
                {
                    score.blueTotal += 1;
                }
                elaspedTime = 0;
            }
        }

        //Deposit Progress Signifier
        if (playerDeath != null && playerDeath.collectedGems.Count > 0)
        {
            repoLight.intensity = Mathf.PingPong(Time.time * flickerSpeed, 5);

            if (active)
            {
                progressBar.fillAmount = elaspedTime / depositTime;

            }
        }
        else
        {
            progressBar.fillAmount = 0;
        }
    }

    //Trigger Deposit Stopped
    private void OnTriggerExit(Collider other)
    {
        repoLight.intensity = intensity;
        repoLight.color = originalColor;
        progressBar.fillAmount = 0;
    }

}
