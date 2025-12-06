using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private int readyPlayers = 0;
    private int playersConnected;

    public void ReadyUp(int playerNumber)
    {
        //for displaying ready text
        switch (playerNumber)
        {
            case 1:
                GameObject.Instantiate(readySignifier, player1Spot);
                Destroy(player1Repo);
                break;

            case 2:
                GameObject.Instantiate(readySignifier, player2Spot);
                Destroy(player2Repo);
                break;

            case 3:
                GameObject.Instantiate(readySignifier, player3Spot);
                Destroy(player3Repo);
                break;

            case 4:
                GameObject.Instantiate(readySignifier, player4Spot);
                Destroy(player4Repo);
                break;
        }

        readyPlayers++;
    }



    private void Update()
    {
        playersConnected = Gamepad.all.Count;

        if (readyPlayers >= playersConnected)
        {
            StartCoroutine(SceneChange());
        }
    }

    public IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        yield return null;
    }
}
