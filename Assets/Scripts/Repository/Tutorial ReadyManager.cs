using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialReadyManager : MonoBehaviour
{
    public GameObject player1ReadyUI;
    public GameObject player2ReadyUI;
    public GameObject player3ReadyUI;
    public GameObject player4ReadyUI;

    public Transform player1Spot;
    public Transform player2Spot;
    public Transform player3Spot;
    public Transform player4Spot;

    public GameObject player1Repo;
    public GameObject player2Repo;
    public GameObject player3Repo;
    public GameObject player4Repo;

    public GameObject player1Icon;
    public GameObject player2Icon;
    public GameObject player3Icon;
    public GameObject player4Icon;

    public GameObject readySignifier;

    private int readyPlayers = 0;
    private int playersConnected;

    public LevelRef sceneListSO;

    private void Awake()
    {
        player1ReadyUI.SetActive(false);
        player2ReadyUI.SetActive(false);
        player3ReadyUI.SetActive(false);
        player4ReadyUI.SetActive(false);
    }

    public void ReadyUp(int playerNumber)
    {
        //for displaying ready text
        switch (playerNumber)
        {
            case 1:
                //player1Icon.SetActive(false);
                player1ReadyUI.SetActive(true);
                //GameObject.Instantiate(readySignifier, player1Spot);
                Destroy(player1Repo);
                break;

            case 2:
               // player3Icon.SetActive(false);
                player3ReadyUI.SetActive(true);
                //GameObject.Instantiate(readySignifier, player2Spot);
                Destroy(player3Repo);
                break;

            case 3:
              //  player2Icon.SetActive(false);
                player2ReadyUI.SetActive(true);
                //GameObject.Instantiate(readySignifier, player3Spot);
                Destroy(player2Repo);
                break;

            case 4:
             //   player4Icon.SetActive(false);
                player4ReadyUI.SetActive(true);
                //GameObject.Instantiate(readySignifier, player4Spot);
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
        LoadRandomScene();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);  //ONLY IF IT SHOULD LOAD NEXT SCENE, NOT RANDOM

        yield return null;
    }

    void LoadRandomScene()
    {
        int randInd = Random.Range(0, sceneListSO.sceneList.Count);
        SceneManager.LoadScene(sceneListSO.sceneList[randInd]);
    }
}
