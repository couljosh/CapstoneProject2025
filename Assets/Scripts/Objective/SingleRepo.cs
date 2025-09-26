using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleRepo : MonoBehaviour
{
    //public int depositTotal;
    

    public SceneChange score;

    public float depositTime;
    public float elaspedTime;

    public Image progressBar;

    public GameObject gemCountText;
    public Light repoLight;
    public float intensity = 15;
    public float flickerSpeed;

    public bool isIncrease;

    public bool depositAll = false;
    public bool active = true;

    public Color originalColor;

    void Start()
    {
        repoLight.intensity = intensity;
        //depositTotal = 0;
        progressBar.fillAmount = 0;
    }
    void Update()
    {
        //gemCountText.GetComponent<TextMeshPro>().text = depositTotal.ToString();
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


    private void OnTriggerStay(Collider other)
    {
        PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();
        PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();

        
    


        if (other.gameObject.tag == "ObjectDestroyer")
        {
            elaspedTime += Time.deltaTime;

            
         }

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

        if (playerDeath != null && playerDeath.collectedGems.Count > 0)
        {
            repoLight.intensity = Mathf.PingPong(Time.time * flickerSpeed, 5);
            progressBar.fillAmount = elaspedTime / depositTime;
        }
        else
        {
            progressBar.fillAmount = 0;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        repoLight.intensity = intensity;
        repoLight.color = originalColor;
        progressBar.fillAmount = 0;
    }

}
