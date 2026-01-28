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
    public Image warningBG;
    private bool warningActive = false;
    private int lastDisplayedSecond = -1;
    private int currentSecond;

    [Header("Round Customization")]
    public float roundTime;
    public float roundTimeInt;
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

    private DrillLogic drillLogicSctipt;


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

        string currentSceneName = SceneManager.GetActiveScene().name;

        //IF USING 00:00 FORMAT
        // int initialMin = Mathf.FloorToInt(roundTime / 60);
        //int initialSec = Mathf.FloorToInt(roundTime % 60);
        //if (timerText != null)
        //    timerText.text = string.Format("{0:00}:{1:00}", initialMin, initialSec);

        timerText.text = Mathf.RoundToInt(roundTime).ToString();

        StartCoroutine(CountdownRoutine());
        overtimeBar.gameObject.SetActive(false);
    }
    // GAME START COUNTDOWN //-------------------------------------------------------------------------------------
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
    // COUNTDOWN SHAKE EFFECT //-------------------------------------------------------------------------------------
    IEnumerator PunchAndShakeText(TextMeshProUGUI textElement, float totalDuration) // referenced from effect used in the deposit text animation
    {
        float elapsed = 0f;
        Vector3 originalPos = textElement.transform.localPosition;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalDuration;

            float pulse = Mathf.Sin(t * Mathf.PI * 2.5f) * 0.2f;
            textElement.transform.localScale = Vector3.one * (1.1f + pulse);

            float shakeIntensity = Mathf.Lerp(5f, 0f, t); //fades out over time
            Vector3 shakeOffset = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                0) * shakeIntensity;

            textElement.transform.localPosition = originalPos + shakeOffset;

            //fade out
            if (t > 0.7f)
            {
                float alpha = Mathf.Lerp(1, 0, (t - 0.7f) / 0.3f);
                textElement.canvasRenderer.SetAlpha(alpha);
            }

            yield return null;
        }

        //reset
        textElement.transform.localPosition = originalPos;
        textElement.transform.localScale = Vector3.one;
    }
    //coroutine for the punchy fade effect
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

            //fade alpha in
            textElement.canvasRenderer.SetAlpha(Mathf.Lerp(0, 1, t));
            //punch scale
            textElement.transform.localScale = Vector3.Lerp(startScale, punchScale, Mathf.Sin(t * Mathf.PI));

            yield return null;
        }

        textElement.transform.localScale = Vector3.one;
        textElement.canvasRenderer.SetAlpha(1);
    }

    IEnumerator PunchTextSofter(TextMeshProUGUI textElement)
    {
        float duration = 0.12f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 punchScale = Vector3.one * 1.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            //fade alpha in
            textElement.canvasRenderer.SetAlpha(Mathf.Lerp(0, 1, t));
            //punch scale
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
            //while true ()
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


        //Countdown Text Coloring
        if (roundTime <= 20)
        {
            timerText.color = Color.yellow;
        }

        if (roundTime <= 10)
        {
            timerText.color = Color.red;
        }

        currentSecond = Mathf.CeilToInt(roundTime);

        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;

            if (currentSecond == 30)
                StartCoroutine(PlayWarningSequence());

            if (currentSecond == 10)
                StartCoroutine(PlayTenSecondWarningSequence());
        }

        timerText.text = currentSecond.ToString();

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

        try
        {
            StopPowerupSound();
        }
        catch
        {
            Debug.Log("Problem ending sounds in fmod"); 
        }
        

        foreach (var gamepad in Gamepad.all)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }

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
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_30sSequence/AlarmSequence");
        
        warningActive = true;

        warningNumberText.gameObject.SetActive(true);
        warningWordText.gameObject.SetActive(true);
        warningBG.gameObject.SetActive(true);

        //warningNumberText.canvasRenderer.SetAlpha(100);
        warningWordText.canvasRenderer.SetAlpha(100);

        Vector3 startScale = Vector3.one * 0.4f;
        Vector3 punchScale = Vector3.one * 1.2f;

        warningNumberText.transform.localScale = startScale;
        warningWordText.transform.localScale = startScale;

        //colours for flashing
        Color startColor = Color.red;
        Color flashColor = new Color(1f, 0.9f, 0f);

        float elapsed = 0;
        float duration = 0.15f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            float alpha = Mathf.Lerp(0, 1, t);
            //warningNumberText.canvasRenderer.SetAlpha(alpha);
            warningWordText.canvasRenderer.SetAlpha(alpha);

            Vector3 currentScale = Vector3.Lerp(startScale, punchScale, easedT);
            warningNumberText.transform.localScale = currentScale;
            warningWordText.transform.localScale = currentScale;
            yield return null;
        }

        warningNumberText.transform.localScale = Vector3.one;
        warningWordText.transform.localScale = Vector3.one;

        //tracks it per second
        int lastSec = -1;

        //countdown loop
        while (currentSecond > 27)
        {
            int currentSec = currentSecond;

            //if second has changed, update text and trigger punch
            if (currentSec != lastSec)
            {
                lastSec = currentSec;
                warningNumberText.text = currentSec.ToString();
                StartCoroutine(PunchTextSofter(warningNumberText)); // Trigger punch on number change
            }

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
            //warningNumberText.canvasRenderer.SetAlpha(alpha);
            warningWordText.canvasRenderer.SetAlpha(alpha);

            warningNumberText.transform.localScale = Vector3.Lerp(Vector3.one, startScale, t);
            warningWordText.transform.localScale = Vector3.Lerp(Vector3.one, startScale, t);
            yield return null;
        }

        //reset color and disable
        warningNumberText.color = startColor;
        warningNumberText.text = ""; //clear number text so when the 10 second timer appears it doesnt show "27" briefly
        warningNumberText.gameObject.SetActive(false);
        warningWordText.gameObject.SetActive(false);
        warningBG.gameObject.SetActive(false);

        warningActive = false;
    }

    IEnumerator PlayTenSecondWarningSequence() //plays the 10 second warning
    {
        warningActive = true;

        warningNumberText.gameObject.SetActive(true);
        warningWordText.gameObject.SetActive(false);
        warningBG.gameObject.SetActive(true);

        //warningNumberText.canvasRenderer.SetAlpha(100);
        warningWordText.canvasRenderer.SetAlpha(100);

        Vector3 startScale = Vector3.one * 0.4f;
        Vector3 punchScale = Vector3.one * 1.2f;

        warningNumberText.transform.localScale = startScale;
        warningWordText.transform.localScale = startScale;

        //colours for flashing
        Color startColor = Color.red;
        Color flashColor = new Color(1f, 0.9f, 0);

        float elapsed = 0;
        float duration = 0.15f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 1f);

            float alpha = Mathf.Lerp(0, 1, t);
            //warningNumberText.canvasRenderer.SetAlpha(alpha);
            warningWordText.canvasRenderer.SetAlpha(alpha);

            Vector3 currentScale = Vector3.Lerp(startScale, punchScale, easedT);
            warningNumberText.transform.localScale = currentScale;
            warningWordText.transform.localScale = currentScale;
            yield return null;
        }

        warningNumberText.transform.localScale = Vector3.one;
        warningWordText.transform.localScale = Vector3.one;

        //tracks it per second
        int lastSec = -1;

        //countdown loop
        while (currentSecond > 0)
        {
            int currentSec = currentSecond;

            //if second has changed, update text and trigger punch
            if (currentSec != lastSec)
            {
                lastSec = currentSec;
                warningNumberText.text = currentSec.ToString();
                StartCoroutine(PunchTextSofter(warningNumberText)); // Trigger punch on number change
            }

            //warningWordText.text = "Seconds\nLeft";

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
            //warningNumberText.canvasRenderer.SetAlpha(alpha);
            warningWordText.canvasRenderer.SetAlpha(alpha);

            warningNumberText.transform.localScale = Vector3.Lerp(Vector3.one, startScale, t);
            warningWordText.transform.localScale = Vector3.Lerp(Vector3.one, startScale, t);
            yield return null;
        }

        //reset color and disable
        warningNumberText.color = startColor;
        warningNumberText.gameObject.SetActive(false);
        warningWordText.gameObject.SetActive(false);
        warningBG.gameObject.SetActive(false);
        warningActive = false;
    }

    void StopPowerupSound()
    {
        //WHERE ALL POEWRUP SOUNDS WILL NEED TO BE STOPPED ON ROUND END
        GameObject drill = GameObject.FindGameObjectWithTag("Drill");
        if (drill != null)
        {
            if(drill.GetComponent<DrillLogic>())
            drill.GetComponent<DrillLogic>().drillInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if(drill.GetComponentInChildren<DrillExplode>())
            drill.GetComponentInChildren<DrillExplode>().engineStartInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}