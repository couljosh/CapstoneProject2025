using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Compilation;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using System.ComponentModel;

public class SceneChange : MonoBehaviour
{
    public static event System.Action OnGameStart;
    public bool canRunTimer = false;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;

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
    public dynamiteTimer dynamiteTimer;
    public bool pointsAdded;
    public bool isTimeOut;
    public bool isOvertime;
    public float overtimeExeption;
    public float overtimeElapsed;

    public RepositoryLogic repositoryLogicScript;



    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        int initialMin = Mathf.FloorToInt(roundTime / 60);
        int initialSec = Mathf.FloorToInt(roundTime % 60);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", initialMin, initialSec);

        GameScore.redScoreBeforeRound = GameScore.redTotalScore;
        GameScore.blueScoreBeforeRound = GameScore.blueTotalScore;

        StartCoroutine(CountdownRoutine());
        overtimeBar.gameObject.SetActive(false);
    }

    IEnumerator CountdownRoutine()
    {

        int count = 3;
        while (count > 0)
        {
            if (countdownText != null) countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        if (countdownText != null) countdownText.text = "GO!";

        OnGameStart?.Invoke();
        StartRoundTimer();

        yield return new WaitForSeconds(0.7f);
        countdownText.gameObject.SetActive(false);
    }

    private void StartRoundTimer()
    {
        canRunTimer = true;
        if (dynamiteTimer != null)
        {
            dynamiteTimer.SendMessage("StartAnimation");
        }

    }

    void Update()
    {


        //Show the score for both teams
        if(redScore && blueScore != null)
        {
            redScore.text = redRoundTotal.ToString();
            blueScore.text = blueRoundTotal.ToString();
        }
        if (!canRunTimer) return;

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


        if (isTimeOut && !pointsAdded)
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

        if (overtimeElapsed <= 0 && !pointsAdded)
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
