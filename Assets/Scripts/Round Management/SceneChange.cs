using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;


public class SceneChange : MonoBehaviour
{

    [Header("Round Customization")]
    public float roundTime;
    public float endOfRoundDelay;

    public int redRoundTotal;
    public int blueRoundTotal;

    [Header("Stored References")]

    [Header("UI References")]
    public TextMeshProUGUI redScore;
    public TextMeshProUGUI blueScore;

    public TextMeshProUGUI timerText;
    public bool pointsAdded;


    void Start()
    {

        GameScore.redScoreBeforeRound = GameScore.redTotalScore;
        GameScore.blueScoreBeforeRound = GameScore.blueTotalScore;
    }


    void Update()
    {

        //Show the score for both teams
        if(redScore && blueScore != null)
        {
            redScore.text = redRoundTotal.ToString();
            blueScore.text = blueRoundTotal.ToString();
        }
        

        //Timer Counts down
        roundTime -= Time.deltaTime;

        int min = Mathf.FloorToInt(roundTime / 60);
        int sec = Mathf.FloorToInt(roundTime % 60);

        if(timerText != null)
        timerText.text = string.Format("{0:00}:{1:00}", min,sec);

        if(roundTime <= 20)
        {
            timerText.color = Color.yellow;
        } 
        
        if(roundTime <= 10)
        {
            timerText.color = Color.red;
        }


        if (roundTime < 1 && !pointsAdded) //change this to when the game is finished.
        {
            roundTime = 0;
            checkScore();
        }
    }


    //Winning Team Display
    public void checkScore()
    {
        //tint.SetActive(true);
        //timerText.gameObject.SetActive(false);


        //if (redRoundTotal > blueRoundTotal)
        //{
        //    teamOneWinText.SetActive(true);
        //}
        //else if (redRoundTotal < blueRoundTotal)
        //{
        //    teamTwoWinText.SetActive(true);
        //}
        //else
        //{
        //    drawText.SetActive(true);
        //}

        GameScore.AddScore(redRoundTotal, blueRoundTotal);
        pointsAdded = true;


        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
