using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;

public class SingleRepo : MonoBehaviour
{
    public int depositTotal;
    public float depositTime;
    public float elaspedTime;

    public GameObject gemCountText;
    public Light repoLight;
    public float flickerSpeed;

    public bool isIncrease;

    public bool depositAll = false;

    public int depositTotalRed;
    public int depositTotalBlue;

    void Start()
    {
        depositTotal = 0;
    }
    void Update()
    {
        gemCountText.GetComponent<TextMeshPro>().text = depositTotal.ToString();

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
        else
        {
            elaspedTime = 0;
        }

        if (elaspedTime >= depositTime && playerDeath != null && playerDeath.collectedGems.Count > 0)
        {
            if (depositAll)
            {
                //Single Repo Test

                if (playerMove.playerNum <= 2)
                {
                    depositTotalRed += playerDeath.collectedGems.Count;
                }
                else
                {
                    depositTotalBlue += playerDeath.collectedGems.Count;
                }
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

        if (playerDeath != null && playerDeath.collectedGems.Count > 0)
        {
            repoLight.intensity = Mathf.PingPong(Time.time * flickerSpeed, 5);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        repoLight.intensity = 5;
    }

}