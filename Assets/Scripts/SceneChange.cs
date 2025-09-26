using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class SceneChange : MonoBehaviour
{
    public Scene scene;
    public int sceneNumber;

    public float roundTime;
    public float endOfRoundDelay;

   
    public int redTotal;
    public int blueTotal;

    public GameObject redRepository;
    public GameObject blueRepository;
    //public GameObject singleRepository;

    public GameObject teamOneWinText;
    public GameObject teamTwoWinText;
    public GameObject drawText;
    public GameObject tint;

    public TextMeshProUGUI timerText;

    public TextMeshProUGUI redScore;
    public TextMeshProUGUI blueScore;


    void Start()
    {

        teamOneWinText.SetActive(false);
        teamTwoWinText.SetActive(false);
        drawText.SetActive(false);
        tint.SetActive(false);

    }

    void Update()
    {
        //Show the score for both teams
        redScore.text = redTotal.ToString();
        blueScore.text = blueTotal.ToString();

        //Timer Counts down
        roundTime -= Time.deltaTime;

        int min = Mathf.FloorToInt(roundTime / 60);
        int sec = Mathf.FloorToInt(roundTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", min,sec);

        if(roundTime <= 20)
        {
            timerText.color = Color.yellow;
        } 
        
        if(roundTime <= 10)
        {
            timerText.color = Color.red;
        }



        if (roundTime <= 0) //change this to when the game is finished.
        {
            StartCoroutine(checkScore());
        }
    }

    public IEnumerator checkScore()
    {
        tint.SetActive(true);
        timerText.gameObject.SetActive(false);

        if (redRepository.GetComponent<Repository>().depositTotal > blueRepository.GetComponent<Repository>().depositTotal || redTotal > blueTotal)
        {
            teamOneWinText.SetActive(true);
        }
        else if (redRepository.GetComponent<Repository>().depositTotal < blueRepository.GetComponent<Repository>().depositTotal || redTotal < blueTotal)
        {
            teamTwoWinText.SetActive(true);
        }
        else
        {
            drawText.SetActive(true);
        }

            yield return new WaitForSeconds(endOfRoundDelay);
        SceneManager.LoadScene(sceneNumber);

    }


}
