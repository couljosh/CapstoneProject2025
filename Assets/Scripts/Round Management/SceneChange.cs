using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Compilation;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;


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

    public Image overtimeBarL;
    public Image overtimeBarR;

    public TextMeshProUGUI timerText;
    public bool pointsAdded;
    public bool isTimeOut;
    public bool isOvertime;
    public float overtimeExeption;
    public float overtimeElapsed;

    public RepositoryLogic repoMoveSystemScript;


    void Start()
    {
       
        GameScore.redScoreBeforeRound = GameScore.redTotalScore;
        GameScore.blueScoreBeforeRound = GameScore.blueTotalScore;

        //  repoMoveSystemScript = GameObject.Find("MoveableRepository").GetComponent<RepositoryLogic>();

        overtimeBarL.gameObject.SetActive(false);
        overtimeBarR.gameObject.SetActive(false);
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
            isTimeOut = true;
        }


        if (isTimeOut)
        {

            if (repoMoveSystemScript.isEmpty && !isOvertime)
            {
                checkScore();

            }

            if (!repoMoveSystemScript.isEmpty && !isOvertime)
            {
                isOvertime = true;

            } 
            
             if (repoMoveSystemScript.isEmpty && isOvertime)
             {
                overtimeElapsed -= Time.deltaTime;
             }
        }

        if (isOvertime)
        {
            timerText.text = "OVERTIME";
            timerText.color = Color.yellow;

            overtimeBarL.gameObject.SetActive(true);
            overtimeBarR.gameObject.SetActive(true);

            overtimeBarL.fillAmount = overtimeElapsed / overtimeExeption;
            overtimeBarR.fillAmount = overtimeElapsed / overtimeExeption;

        }

        if (overtimeElapsed <= 0)
        {
            checkScore();
        }
    }


    //Winning Team Display
    public void checkScore()
    {


        GameScore.AddScore(redRoundTotal, blueRoundTotal);
        pointsAdded = true;


        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
