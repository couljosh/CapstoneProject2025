using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class Repository : MonoBehaviour
{
    public int depositTotal;
    public float depositTime;
    public float elaspedTime;

    public GameObject gemCountText;

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

            playerDeath.collectedGems.RemoveAt(0);
            depositTotal += 1;
            elaspedTime = 0;
        }
    }

}
