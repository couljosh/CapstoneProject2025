using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Compilation;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.InputSystem;


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

    public GameObject overtimeBar;
    public Image overtimeBarL;
    public Image overtimeBarR;

    public TextMeshProUGUI timerText;
    public bool pointsAdded;
    public bool isTimeOut;
    public bool isOvertime;
    public float overtimeExeption;
    public float overtimeElapsed;

    public RepositoryLogic repositoryLogicScript;



    void Start()
    {
       
       
        GameScore.redScoreBeforeRound = GameScore.redTotalScore;
        GameScore.blueScoreBeforeRound = GameScore.blueTotalScore;

        //  repoMoveSystemScript = GameObject.Find("MoveableRepository").GetComponent<RepositoryLogic>();

        overtimeBar.gameObject.SetActive(false);
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
            roundTime = 0.1f;

            if (repositoryLogicScript.isEmpty && !isOvertime)
            {
                checkScore();

            }

            if (!repositoryLogicScript.isEmpty && !isOvertime)
            {
                isOvertime = true;

            } 
            
             if (repositoryLogicScript.isEmpty && isOvertime && repositoryLogicScript.active)
             {
                overtimeElapsed -= Time.deltaTime;
             }
        }

        if (isOvertime)
        {
            overtimeBar.gameObject.SetActive(true);

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


       StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
