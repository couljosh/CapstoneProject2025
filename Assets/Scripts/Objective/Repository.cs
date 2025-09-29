using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Repository : MonoBehaviour
{
    [Header("Repository Customization")]
    public int depositTotal;
    public float depositTime;
    public float elaspedTime;
    public float flickerSpeed;

    [Header("Stored References")]
    public Image progressBar;
    public Light repoLight;

    [Header("Repository Checks")]
    public bool isIncrease;
    public bool depositAll = false;


    void Start()
    {
        depositTotal = 0;
        progressBar.fillAmount = 0;
    }

    //Reset Deposit Check
    private void OnTriggerEnter(Collider other)
    {
        elaspedTime = 0;

    }

    //Actively Depositing Check
    private void OnTriggerStay(Collider other)
    {

        PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();

        //Player Check
        if (other.gameObject.tag == "ObjectDestroyer")
        {
            elaspedTime += Time.deltaTime;
        }
        else
        {
            elaspedTime = 0;
        }

        //Deposit Gem Check
        if (elaspedTime >= depositTime && playerDeath != null && playerDeath.collectedGems.Count > 0)
        {
            if (depositAll)
            {

                depositTotal += playerDeath.collectedGems.Count;
                playerDeath.collectedGems.Clear();
                elaspedTime = 0;
            }
            else
            {
                playerDeath.collectedGems.RemoveAt(0);
                depositTotal += 1;
                elaspedTime = 0;
            }
        }

        //Depositing Signifiers
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

    //Cancelling Deposit Check
    private void OnTriggerExit(Collider other)
    {
        repoLight.intensity = 5;
        progressBar.fillAmount = 0;
    }
}
