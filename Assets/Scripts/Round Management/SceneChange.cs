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
using Unity.VisualScripting.Antlr3.Runtime;
using System.Net.NetworkInformation;

public class SceneChange : MonoBehaviour
{
    public static bool gameHasStarted;

    public static event System.Action OnGameStart;
    public bool canRunTimer = false;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;

    [Header("Warning UI")]
    public TextMeshProUGUI warningNumberText;
    public TextMeshProUGUI warningWordText;
    private bool warningActive = false;

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


    private void Awake()
    {
        //needs to be in awake, as players calculate if they can act on start
        gameHasStarted = false;
    }

    void Start()
    {
        GameScore.redScoreBeforeRound = GameScore.redTotalScore;
        GameScore.blueScoreBeforeRound = GameScore.blueTotalScore;

        GameScore.roundNum++;
        print(GameScore.roundNum);

        string currentSceneName = SceneManager.GetActiveScene().name;

        int initialMin = Mathf.FloorToInt(roundTime / 60);
        int initialSec = Mathf.FloorToInt(roundTime % 60);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", initialMin, initialSec);


        StartCoroutine(CountdownRoutine());
        overtimeBar.gameObject.SetActive(false);
    }

    IEnumerator CountdownRoutine()
    {
        int count = 3;
        while (count > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = count.ToString();
                StartCoroutine(PunchText(countdownText));
            }
            yield return new WaitForSeconds(1f);
            count--;
        }

        if (countdownText != null)
        {
            countdownText.text = "GO!";
            StartCoroutine(PunchAndShakeText(countdownText, 0.7f));
        }

        gameHasStarted = true;
        OnGameStart?.Invoke();
        StartRoundTimer();

        yield return new WaitForSeconds(0.7f);
        countdownText.gameObject.SetActive(false);
    }
    IEnumerator PunchAndShakeText(TextMeshProUGUI textElement, float totalDuration)
    {
        float elapsed = 0f;
        Vector3 originalPos = textElement.transform.localPosition;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalDuration;

            // 1. "Throb" Scale - Pulse twice over the duration
            float pulse = Mathf.Sin(t * Mathf.PI * 2.5f) * 0.2f;
            textElement.transform.localScale = Vector3.one * (1.1f + pulse);

            // 2. Shake - Subtle high-frequency vibration
            float shakeIntensity = Mathf.Lerp(5f, 0f, t); // Fades out over time
            Vector3 shakeOffset = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                0) * shakeIntensity;

            textElement.transform.localPosition = originalPos + shakeOffset;

            // 3. Smooth Fade Out at the very end
            if (t > 0.7f)
            {
                float alpha = Mathf.Lerp(1, 0, (t - 0.7f) / 0.3f);
                textElement.canvasRenderer.SetAlpha(alpha);
            }

            yield return null;
        }

        // Reset
        textElement.transform.localPosition = originalPos;
        textElement.transform.localScale = Vector3.one;
    }

    // Helper coroutine for the punchy fade effect
    IEnumerator PunchText(TextMeshProUGUI textElement)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 punchScale = Vector3.one * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade alpha in
            textElement.canvasRenderer.SetAlpha(Mathf.Lerp(0, 1, t));
            // Punch scale
            textElement.transform.localScale = Vector3.Lerp(startScale, punchScale, Mathf.Sin(t * Mathf.PI));

            yield return null;
        }

        textElement.transform.localScale = Vector3.one;
        textElement.canvasRenderer.SetAlpha(1);
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
        if (redScore && blueScore != null)
        {
            redScore.text = redRoundTotal.ToString();
            blueScore.text = blueRoundTotal.ToString();
        }
        if (!canRunTimer) return;


        // DURING ROUND //-------------------------------------------------------------------------------------
        //Timer Counts down
        if (roundTime > 0.1f)
        {
            roundTime -= Time.deltaTime;
        }
        else
        {
            roundTime = 0f;
        }

        int min = Mathf.FloorToInt(roundTime / 60);
        int sec = Mathf.FloorToInt(roundTime % 60);

        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", min, sec);

        //Countdown Text Coloring
        if (roundTime <= 20)
        {
            timerText.color = Color.yellow;
        }

        if (roundTime <= 10)
        {
            timerText.color = Color.red;
        }

        //Trigger warning at 30 seconds
        if (roundTime <= 30f && roundTime > 25f && !warningActive)
        {
            StartCoroutine(PlayWarningSequence());
        }

        //Time hits 0
        if (roundTime <= 0 && !pointsAdded) //change this to when the game is finished.
        {
            isTimeOut = true;
        }

        // AFTER ROUND //-------------------------------------------------------------------------------------
        if (isTimeOut && !pointsAdded)
        {
            //Instant End (no overtime)
            if (repositoryLogicScript.isEmpty && !isOvertime)
            {
                checkScore();

            }

            //Overtime Trigger
            if (!repositoryLogicScript.isEmpty && !isOvertime)
            {
                isOvertime = true;

            }
        }

        // OVERTIME //-------------------------------------------------------------------------------------
        if (isOvertime)
        {
            overtimeBar.gameObject.SetActive(true);
            overtimeBarL.fillAmount = overtimeElapsed / overtimeExeption;
            overtimeBarR.fillAmount = overtimeElapsed / overtimeExeption;

            if (repositoryLogicScript.isEmpty && repositoryLogicScript.active)
            {
                overtimeElapsed -= Time.deltaTime;
            }
        }

        //Overtime Ended
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

        Invoke("LoadNextScene", 1f);
    }

    //IEnumerator LoadNextScene()
    //{
    //    yield return new WaitForSeconds(1);

    //    if (GameScore.roundNum == 1)
    //    {
    //        SceneManager.LoadScene("Intermission_1");

    //    }
    //    else if (GameScore.roundNum == 2)
    //    {
    //        SceneManager.LoadScene("Intermission_2");
    //    }
    //    else if (GameScore.roundNum == 3)
    //    {
    //        SceneManager.LoadScene("End Match");
    //    }
    //}

    public void LoadNextScene()
    {
        if (GameScore.roundNum == 1)
        {
            SceneManager.LoadScene("Intermission_1");

        }
        else if (GameScore.roundNum == 2)
        {
            SceneManager.LoadScene("Intermission_2");
        }
        else if (GameScore.roundNum == 3)
        {
            SceneManager.LoadScene("End Match");
        }
        else
        {
            print("failed to find scene");
        }
    }

    IEnumerator PlayWarningSequence() //plays the 30 second warning, and also has some text effects (found from a tutorial)
    {
        warningActive = true;

        warningNumberText.gameObject.SetActive(true);
        warningWordText.gameObject.SetActive(true);

        warningNumberText.canvasRenderer.SetAlpha(0);
        warningWordText.canvasRenderer.SetAlpha(0);

        Vector3 startScale = Vector3.one * 0.4f;
        Vector3 punchScale = Vector3.one * 1.2f;

        warningNumberText.transform.localScale = startScale;
        warningWordText.transform.localScale = startScale;

        //colours for flashing
        Color startColor = Color.white;
        Color flashColor = new Color(1f, 0.15f, 0.15f);

        float elapsed = 0;
        float duration = 0.15f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            float alpha = Mathf.Lerp(0, 1, t);
            warningNumberText.canvasRenderer.SetAlpha(alpha);
            warningWordText.canvasRenderer.SetAlpha(alpha);

            Vector3 currentScale = Vector3.Lerp(startScale, punchScale, easedT);
            warningNumberText.transform.localScale = currentScale;
            warningWordText.transform.localScale = currentScale;
            yield return null;
        }

        warningNumberText.transform.localScale = Vector3.one;
        warningWordText.transform.localScale = Vector3.one;

        //countdown loop
        while (roundTime > 25f)
        {
            warningNumberText.text = Mathf.CeilToInt(roundTime).ToString();
            warningWordText.text = "Seconds\nLeft";

            //PINGPONG CLUTCH
            float flashLerp = Mathf.PingPong(Time.time * 2f, 1f);
            warningNumberText.color = Color.Lerp(startColor, flashColor, flashLerp);

            yield return null;
        }

        //fade out
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float alpha = Mathf.Lerp(1, 0, t);
            warningNumberText.canvasRenderer.SetAlpha(alpha);
            warningWordText.canvasRenderer.SetAlpha(alpha);

            warningNumberText.transform.localScale = Vector3.Lerp(Vector3.one, startScale, t);
            warningWordText.transform.localScale = Vector3.Lerp(Vector3.one, startScale, t);
            yield return null;
        }

        // Reset color and disable
        warningNumberText.color = startColor;
        warningNumberText.gameObject.SetActive(false);
        warningWordText.gameObject.SetActive(false);
    }
}