using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialReadyManager : MonoBehaviour
{
    public Transform player1Spot;
    public Transform player2Spot;
    public Transform player3Spot;
    public Transform player4Spot;

    public GameObject player1Repo;
    public GameObject player2Repo;
    public GameObject player3Repo;
    public GameObject player4Repo;

    public GameObject readySignifier;

    private bool isPlayer1Ready;
    private bool isPlayer2Ready;
    private bool isPlayer3Ready;
    private bool isPlayer4Ready;

    public void ReadyUp(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                GameObject.Instantiate(readySignifier, player1Spot);
                isPlayer1Ready = true;
                Destroy(player1Repo);
                break;

            case 2:
                GameObject.Instantiate(readySignifier, player2Spot);
                Destroy(player2Repo);
                isPlayer2Ready = true;
                break;

            case 3:
                GameObject.Instantiate(readySignifier, player3Spot);
                Destroy(player3Repo);
                isPlayer3Ready = true;
                break;

            case 4:
                GameObject.Instantiate(readySignifier, player4Spot);
                Destroy(player4Repo);
                isPlayer4Ready = true;
                break;
        }
    }

    private void Update()
    {
        if (isPlayer1Ready && isPlayer2Ready && isPlayer3Ready && isPlayer4Ready)
        {
            StartCoroutine(SceneChange());
        }

        //testing
        //if(isPlayer1Ready)
        //{
        //    StartCoroutine(SceneChange());
        //}

    }

    public IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        yield return null;
    }
}
