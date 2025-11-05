using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayTeamScores : MonoBehaviour
{
    public TextMeshProUGUI redScore;
    public TextMeshProUGUI blueScore;

    public Loading loadingScript;

    private float countUpRed;
    private float countUpBlue;
    private float higherScore;
    public float countUpSpeed;
    public float secAfterScoreFinishedCounting;

    private void Start()
    {
        countUpSpeed = 0;
        higherScore = Mathf.Max(GameScore.redTotalScore, GameScore.blueTotalScore);
        countUpSpeed = higherScore / (loadingScript.loadingTime - secAfterScoreFinishedCounting);

        countUpRed = 0;
        countUpBlue = 0;


        //USE IF SCORE SHOULD RESUME COUNTING INSTEAD OF STARTING AT 0 TO COUNT UP
        // countUpRed = GameScore.redScoreBeforeRound;
        // countUpBlue = GameScore.blueScoreBeforeRound;

        print("higher score "+higherScore);
        print("countupSpeed "+countUpSpeed);
    }

    private void Update()
    {

        if (countUpRed < GameScore.redTotalScore)
        {
            countUpRed += Time.deltaTime * countUpSpeed;

        }
        else
        {
            countUpRed = GameScore.redTotalScore;
        }

        if (countUpBlue < GameScore.blueTotalScore)
        {
            countUpBlue += Time.deltaTime * countUpSpeed;

        }
        else
        {
            countUpBlue = GameScore.blueTotalScore;      
        }

        int timeRoundedRed = Mathf.RoundToInt(countUpRed);
        int timeRoundedBlue = Mathf.RoundToInt(countUpBlue);

        redScore.text = timeRoundedRed.ToString();
        blueScore.text = timeRoundedBlue.ToString();

    }
}
