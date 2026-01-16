using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine.ProBuilder.Shapes;
using System;
using Unity.VisualScripting;

public class RepositoryLogic : MonoBehaviour
{
    [Header("Script References")]
    public SceneChange score;
    public PlayerDeath depositor;
    public RepoMoveSystem repoMoveSystemScript;
    public Outline outlineScript;
    public DynamicCamera dynamicCamera;
    public DepositScoreDisplay scoreDisplay;
    public Animator repoAnimation;
    private TerrainBlast terrainBlastScript;

    [Header("UI References")]
    public Image progressBar;
    public Image radiusImg;
    public Image timerProgress;
    public Light repoLight;

    [Header("Deposit Preview Text")]
    public DepositPreviewWorldText previewText;
    public int maxDepositCap = 100;
    private int previewTarget;
    private float previewValue;
    public float previewDecrementDuration = 1.0f; //time to reach 0
    public float baseScaleMultiplier = 0.5f;
    public float punchIntensity = 0.15f;
    private int lastPreviewInt;
    private int cachedTargetCap;
    private bool wasTeamOneLast;
    private Color greyColor = new Color(0.5f, 0.5f, 0.5f);
    private bool wasPreviewContested;
    public Color contestedPreviewColor = Color.red;

    [Header("Object References")]
    public GameObject repoAlarm;
    public Light repoAlarmLight;
    public Light repoAlarmLightTwo;
    public GameObject startingPocket;
    private List<GameObject> enteredPlayersTeam1 = new List<GameObject>();
    private List<GameObject> enteredPlayersTeam2 = new List<GameObject>();
    public List<GameObject> allEnteredPlayers = new List<GameObject>();
    public SphereCollider depositRadius;
    public List<GameObject> largeGemsInRadius = new List<GameObject>();
    public int largeGemValue;

    [Header("Deposit/Activity Checks")]
    public bool isIncrease;
    public bool depositAll = false;
    public bool active;
    public int teamlastDepo;
    public int singleCheck;

    [Header("Repository Customization")]
    public float depositTime;
    public float increaseMult;
    public float decreaseMult; //If a progress reduction is added when the radius is empty
    private float depositProgress;
    public ParticleSystem depositParticles;
    public float repoAlarmDuration;

    [Header("Color Customization")]
    public Color originalColor;
    public Color yellowTeamColor;
    public Color blueTeamColor;
    public Color yellowProgressColor;
    public Color blueProgressColor;

    [Header("Repository State Checks")]
    public bool teamOneCanDepo;
    public bool teamTwoCanDepo;
    public bool isContested;
    public bool isEmpty;
    public LayerMask player;
    public LayerMask kickable;

    [Header("Catch-Up Deposit Settings")]
    [Range(0f, 1f)]
    public float losingTeamDepositSpeedBonus = 0.10f; //10% faster
    public int catchupDeficitThreshold = 75;


    [Header("Sound Variables")]
    //public EventReference depositRef;
    private FMOD.Studio.EventInstance instance;
    FMOD.Studio.PLAYBACK_STATE playBackState;

    public Material repoMaterial;
    public Material contestedMaterial;
    public MeshRenderer mesh;

    private MeshRenderer[] meshRenderers;


    void Start()
    {

        dynamicCamera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();

        repoAnimation = GetComponentInChildren<Animator>();
        repoAnimation.SetBool("Appear", false);
        repoAnimation.SetBool("Retract", false);
        //Script References
        repoMoveSystemScript = GameObject.Find("RepoMover").GetComponent<RepoMoveSystem>();
        score = GameObject.Find("SceneManager").GetComponent<SceneChange>();

        timerProgress.fillAmount = repoMoveSystemScript.activeDuration;

        //Start with default repository settings
        progressBar.fillAmount = 0;
        repoLight.color = originalColor;
        ClearStartingArea();
        repoAlarm.SetActive(false);

        //Sound References
        //instance = FMODUnity.RuntimeManager.CreateInstance(depositRef);

        //Deposit Display Reference
        scoreDisplay = GetComponent<DepositScoreDisplay>();

        //depositParticles.enableEmission = false;

        //for keeping the repo terrain-free constantly
        terrainBlastScript = gameObject.GetComponent<TerrainBlast>();

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }


    void Update()
    {
        ConditionCheck();
        UpdateDepositPreview();

        //terrain clearing
        if (terrainBlastScript.isFinishedClearing == true)
        {
            //go again, keep the repo clear of terrain at all times
            terrainBlastScript.isFinishedClearing = false;
        }

        // SYSTEM STRUCTURE //---------------------------------------------------------------------------------------
        progressBar.fillAmount = depositProgress / depositTime;

        if (active)
            timerProgress.fillAmount -= 1f / repoMoveSystemScript.activeDuration * Time.deltaTime;

        instance.getPlaybackState(out playBackState);

        if (allEnteredPlayers.Count > 0)
            depositor = allEnteredPlayers[0].GetComponent<PlayerDeath>();


        // EMPTY //--------------------------------------------------------------------------------------------------
        if (isEmpty)
        {
            //Reduces Progress
            //if (depositProgress > 0)
            //{
            //    depositProgress -= Time.deltaTime * decreaseMult;
            //}

            if (instance.isValid())
                instance.setPaused(true);

            //Resets last team checks
            singleCheck = 0;
            teamlastDepo = 0;

        }


        // CONTESTED //----------------------------------------------------------------------------------------------
        if (isContested)
        {
            //repoAlarmLight.color = Color.yellow;
            float flickAmt = Mathf.PingPong(Time.time * 1000, 500);

            ChangeRepoMesh(contestedMaterial);

        }
        else
        {
            ChangeRepoMesh(repoMaterial);
        }


        // TEAM 1 DEPOSITING //--------------------------------------------------------------------------------------
        if (teamOneCanDepo)
        {

            //Increase progress until full (and with catch-up checks)
            float speedMult = 1f;

            if (CatchupActiveForTeam(isTeamOne: true))
                speedMult += losingTeamDepositSpeedBonus;

            depositProgress += Time.deltaTime * speedMult;


            //Team 1 Signifiers Active
            progressBar.color = yellowProgressColor;



            //Complete Deposit Check
            if (depositProgress >= depositTime)
            {
                CompleteDeposit();
            }
        }


        // TEAM 2 DEPOSITING //----------------------------------------------------------------------------------------
        if (teamTwoCanDepo)
        {
            //Increase progress until full (and with catch-up checks)
            float speedMult = 1f;

            if (CatchupActiveForTeam(isTeamOne: false))
                speedMult += losingTeamDepositSpeedBonus;

            depositProgress += Time.deltaTime * speedMult;

            //Team 2 Signifiers Active
            progressBar.color = blueProgressColor;



            //Complete Deposit Check
            if (depositProgress >= depositTime)
            {
                CompleteDeposit();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Tracks Players Entered
        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            addEnteredPlayer(other.gameObject);
        }

        if (other.gameObject.tag == "LargeGem" && active && other.gameObject.GetComponent<LargeGem>())
        {
            other.gameObject.GetComponent<LargeGem>().isInDepositRadius = true; 
            largeGemsInRadius.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Tracks Players Exited
        if (other.gameObject.tag == "ObjectDestroyer" && active)
        {
            removeEnteredPlayer(other.gameObject);
        }

        if (other.gameObject.tag == "LargeGem" && active)
        {
            //if its the collider child (more accurate hitbox)
            if(other.gameObject.GetComponent<LargeGem>())
            {
                other.gameObject.GetComponent<LargeGem>().isInDepositRadius = false;
                largeGemsInRadius.Remove(other.gameObject);
            }
            
        }
    }

    public void ConditionCheck()
    {
        // EMPTY //--------------------------------------------------------------------------------------------------
        if (enteredPlayersTeam1.Count <= 0 && enteredPlayersTeam2.Count <= 0)
        {
            depositor = null;
            depositProgress = 0;  //TURN ON IF NOT USING DEPOSIT DECREASE WHEN EMPTY
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }

        // CONTESTED //----------------------------------------------------------------------------------------------
        if (enteredPlayersTeam1.Count > 0 && enteredPlayersTeam2.Count > 0)
        {
            isContested = true;

            //Saves last team to determine what happens when deposit sound is played again
            instance.setPaused(true);
            singleCheck = teamlastDepo;

            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Repository/ContestedDeposit", gameObject.transform.position);


        }
        else
        {
            isContested = false;
        }

        // TEAM 1 DEPOSITING //--------------------------------------------------------------------------------------
        if (enteredPlayersTeam1.Count >= 1 && enteredPlayersTeam2.Count < 1 && depositor.collectedGems.Count > 0)
        {
            teamOneCanDepo = true;

            //Sets as last team 
            if (teamlastDepo == 2)
            {
                depositProgress = 0;
            }
            teamlastDepo = 1;

            //Deposit Sound Trigger
            //ConditionalSoundTrigger();
        }
        else
        {
            teamOneCanDepo = false;
        }

        // TEAM 2 DEPOSITING //----------------------------------------------------------------------------------------
        if (enteredPlayersTeam2.Count >= 1 && enteredPlayersTeam1.Count < 1 && depositor.collectedGems.Count > 0)
        {
            teamTwoCanDepo = true;

            //Sets as last team 
            if (teamlastDepo == 1)
            {
                depositProgress = 0;
            }
            teamlastDepo = 2;

            //Deposit Sound Trigger
            //ConditionalSoundTrigger();
        }
        else
        {
            teamTwoCanDepo = false;
        }
    }


    public void CompleteDeposit()
    {
        //int depositScore = depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);

        depositProgress = 0;

        //depositParticles.enableEmission = true;
        depositParticles.Clear();
        depositParticles.Play();

        // Capture the Z position just before the repo might move/lower.
        float currentRepoWorldZ = transform.position.z;


        //Add Red Score
        if (teamOneCanDepo)
        {
            //calculate cap
            int totalHeld = 0;
            foreach (GameObject p in enteredPlayersTeam1)
            {
                if (p != null) totalHeld += p.GetComponent<PlayerDeath>().collectedGems.Count;
            }

            //cap small gems at 100
            int smallGemsToDeposit = Mathf.Min(totalHeld, maxDepositCap);
            int gemsLeftToTake = smallGemsToDeposit;


            //score.redRoundTotal += depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);
            foreach (GameObject player in enteredPlayersTeam1)
            {
                PlayerDeath playerGems = player.GetComponent<PlayerDeath>();

                int amountToTake = 0;
                if (gemsLeftToTake > 0 && playerGems.collectedGems.Count > 0)
                {
                    amountToTake = Mathf.Min(playerGems.collectedGems.Count, gemsLeftToTake);

                    if (amountToTake == playerGems.collectedGems.Count)
                    {
                        playerGems.collectedGems.Clear(); // clear all if taking all
                    }
                    else
                    {
                        playerGems.collectedGems.RemoveRange(0, amountToTake);
                    }

                    playerGems.gemCount -= amountToTake;
                    gemsLeftToTake -= amountToTake;
                }
                // -----------------------------------------------------
            }

            //calculate final score
            int totalDepositScore = smallGemsToDeposit + (largeGemsInRadius.Count * largeGemValue);

            //counter display
            score.redRoundTotal += totalDepositScore;

            // PASS THE CURRENT WORLD Z POSITION
            scoreDisplay.ShowScore(totalDepositScore, yellowTeamColor, currentRepoWorldZ);
        }

        //Add Blue Score
        if (teamTwoCanDepo)
        {
            int totalHeld = 0;
            foreach (GameObject p in enteredPlayersTeam2)
            {
                if (p != null) totalHeld += p.GetComponent<PlayerDeath>().collectedGems.Count;
            }

            //cap the small gems at 100
            int smallGemsToDeposit = Mathf.Min(totalHeld, maxDepositCap);
            int gemsLeftToTake = smallGemsToDeposit;

            //score.blueRoundTotal += depositor.collectedGems.Count + (largeGemsInRadius.Count * largeGemValue);
            foreach (GameObject player in enteredPlayersTeam2)
            {
                PlayerDeath playerGems = player.GetComponent<PlayerDeath>();

                int amountToTake = 0;
                if (gemsLeftToTake > 0 && playerGems.collectedGems.Count > 0)
                {
                    amountToTake = Mathf.Min(playerGems.collectedGems.Count, gemsLeftToTake);

                    // Remove specific amount from inventory
                    if (amountToTake == playerGems.collectedGems.Count)
                    {
                        playerGems.collectedGems.Clear();
                    }
                    else
                    {
                        playerGems.collectedGems.RemoveRange(0, amountToTake);
                    }

                    playerGems.gemCount -= amountToTake;
                    gemsLeftToTake -= amountToTake;
                }
            }

            int totalDepositScore = smallGemsToDeposit + (largeGemsInRadius.Count * largeGemValue);

            //counter display
            score.blueRoundTotal += totalDepositScore;

            scoreDisplay.ShowScore(totalDepositScore, blueTeamColor, currentRepoWorldZ);
        }

        if (depositor != null && depositor.collectedGems.Count == 0)
        {
            depositor.gemCount = 0;
        }

        if (allEnteredPlayers.Count > 0)
            allEnteredPlayers.RemoveAt(0);

        //get rid of any large gems
        foreach (GameObject largeGem in largeGemsInRadius)
        {
            Destroy(largeGem);
        }
        largeGemsInRadius.Clear();

        if (score.isOvertime)
        {
            if (!score.pointsAdded)
            {
                score.checkScore();
            }
        }

        repoMoveSystemScript.elaspedTime = repoMoveSystemScript.activeDuration;

        teamOneCanDepo = false;
        teamTwoCanDepo = false;

        //reset preview text
        previewValue = 0;

        //play complete deposit noise
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_Repository/Deposit", gameObject.transform.position);

    }


    public void DisableRepo()
    {
        outlineScript.enabled = false;

        teamOneCanDepo = false;
        teamTwoCanDepo = false;
        isContested = false;
        isEmpty = true;

        depositor = null;
        enteredPlayersTeam1.Clear();
        enteredPlayersTeam2.Clear();
        allEnteredPlayers.Clear();

        singleCheck = 0;
        teamlastDepo = 0;

        timerProgress.enabled = false;
        radiusImg.enabled = false;
        repoLight.enabled = false;
        depositProgress = 0;
        repoLight.color = originalColor;

        //account for any large gems that were in the radius (because ontriggerexit isn't called when repos are disabled)
        foreach (GameObject largeGem in largeGemsInRadius)
        {
            largeGem.GetComponentInParent<LargeGem>().isInDepositRadius = false;
        }

        largeGemsInRadius.Clear();

        active = false;
        //reset zoom state for dynamic camera
        //dynamicCamera.ZoomOutTimer = 0;

    }

    public void ActivateRepo()
    {
        active = true;
        timerProgress.fillAmount = repoMoveSystemScript.activeDuration;
        outlineScript.enabled = true;
        repoLight.enabled = true;
        timerProgress.enabled = true;
        radiusImg.enabled = true;
        CheckWhenSetActive();

        StartCoroutine(SetLightValues(repoAlarmDuration));
    }


    //void ConditionalSoundTrigger()
    //{
    //    bool isPaused;
    //    RESULT r = instance.getPaused(out isPaused);
    //    {
    //        if (r == RESULT.OK)
    //            //Sound for same team
    //            if (isPaused && teamlastDepo == singleCheck)
    //            {
    //                instance.setPaused(false);
    //            }

    //        //Sound for new team
    //        if (teamlastDepo != singleCheck)
    //        {
    //            PlaySound();

    //        }
    //    }
    //}


    void PlaySound()
    {
        instance.setPaused(false);
        instance.start();
    }


    void ClearStartingArea()
    {
        // Instantiate(startingPocket, transform.position, Quaternion.identity);

    }


    void CheckWhenSetActive()
    {
        //check for players already in the radius
        Collider[] playersHit = Physics.OverlapSphere(transform.position, depositRadius.radius, player, QueryTriggerInteraction.Ignore);

        if (playersHit.Length > 0 && isEmpty)
        {
            foreach (var detectedPlayer in playersHit)
            {
                if (detectedPlayer != null)
                    addEnteredPlayer(detectedPlayer.gameObject);
            }
        }

        //check for large gems already in the radius
        Collider[] gemsHit = Physics.OverlapSphere(transform.position, depositRadius.radius, kickable, QueryTriggerInteraction.Ignore);

        if (gemsHit.Length > 0)
        {
            foreach (var detectedGem in gemsHit)
            {
                if (detectedGem != null && detectedGem.tag == "LargeGem")
                {
                    largeGemsInRadius.Add(detectedGem.gameObject.transform.parent.gameObject);
                    detectedGem.gameObject.GetComponentInParent<LargeGem>().isInDepositRadius = true;
                }


            }
        }
    }


    void addEnteredPlayer(GameObject player)
    {

        foreach (var detectedPlayer in allEnteredPlayers)
        {
            if (player.gameObject == detectedPlayer)
            {
                return;
            }
        }

        allEnteredPlayers.Add(player);

        //depositor != other.gameObject ||
        if (depositor == null)
        {
            depositor = player.GetComponent<PlayerDeath>();
        }

        //Add to list of players currently in range based on their team affiliation
        if (player.GetComponent<PlayerMove>().playerNum == 1 || player.GetComponent<PlayerMove>().playerNum == 2)
            enteredPlayersTeam1.Add(player.gameObject);
        else if (player.GetComponent<PlayerMove>().playerNum == 3 || player.GetComponent<PlayerMove>().playerNum == 4)
            enteredPlayersTeam2.Add(player.gameObject);

    }


    void removeEnteredPlayer(GameObject player)
    {
        allEnteredPlayers.Remove(player);

        if (depositor != null && allEnteredPlayers.Count > 0)
            depositor = allEnteredPlayers[0].GetComponent<PlayerDeath>();

        //remove leaving player from list
        if (player.GetComponent<PlayerMove>().playerNum == 1 || player.GetComponent<PlayerMove>().playerNum == 2)
            enteredPlayersTeam1.Remove(player.gameObject);
        else if (player.GetComponent<PlayerMove>().playerNum == 3 || player.GetComponent<PlayerMove>().playerNum == 4)
            enteredPlayersTeam2.Remove(player.gameObject);
    }

    IEnumerator SetLightValues(float alarmLength)
    {
        repoAlarm.SetActive(true);

        yield return new WaitForSeconds(alarmLength);

        repoAlarm.SetActive(false);

    }

    void ChangeRepoMesh(Material repoMaterial)
    {
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = repoMaterial;
        }
    }

    int CalculateSmallGemPreview(List<GameObject> players)
    {
        int total = 0;

        foreach (GameObject p in players)
        {
            PlayerDeath pd = p.GetComponent<PlayerDeath>();
            total += pd.collectedGems.Count;
        }

        return Mathf.Min(total, maxDepositCap);
    }

    bool IsTeamLosing(bool isTeamOne)
    {
        if (score == null) return false;

        int totalRed = GameScore.redTotalScore + score.redRoundTotal;
        int totalBlue = GameScore.blueTotalScore + score.blueRoundTotal;

        if (isTeamOne)
            return totalRed < totalBlue;
        else
            return totalBlue < totalRed;
    }

    bool CatchupActiveForTeam(bool isTeamOne)
    {
        if (score == null) return false;

        int totalRed = GameScore.redTotalScore + score.redRoundTotal;
        int totalBlue = GameScore.blueTotalScore + score.blueRoundTotal;

        int deficit = Mathf.Abs(totalRed - totalBlue);
        if (deficit < catchupDeficitThreshold)
            return false;

        return IsTeamLosing(isTeamOne);
    }



    void UpdateDepositPreview()
    {
        bool validDeposit = (teamOneCanDepo && !isContested) || (teamTwoCanDepo && !isContested);

        // ---------------- CONTESTED STATE ----------------
        if (isContested)
        {
            //freeze deposit value where it was
            previewText.SetVisible(previewValue > 0);

            int frozenInt = Mathf.RoundToInt(previewValue);
            previewText.SetValue(frozenInt, cachedTargetCap);

            //override color to contested
            previewText.SetColor(Color.Lerp(
                previewText.text.color,
                contestedPreviewColor,
                Time.deltaTime * 10f
            ));

            wasPreviewContested = true;
            return;
        }
        // --------------------------------------------------

        wasPreviewContested = false;

        // ---------------- ACTIVE DEPOSIT ----------------
        if (validDeposit)
        {
            List<GameObject> activeTeam = teamOneCanDepo ? enteredPlayersTeam1 : enteredPlayersTeam2;
            wasTeamOneLast = teamOneCanDepo;

            int realTotalHeld = 0;
            foreach (GameObject p in activeTeam)
            {
                if (p != null)
                    realTotalHeld += p.GetComponent<PlayerDeath>().collectedGems.Count;
            }

            cachedTargetCap = Mathf.Min(realTotalHeld, maxDepositCap);

            float destinationValue = cachedTargetCap * (depositProgress / depositTime);
            previewValue = Mathf.MoveTowards(previewValue, destinationValue, Time.deltaTime * 100f);
        }
        // ---------------- WALK-OFF / EMPTY ----------------
        else
        {
            if (previewValue > 0)
            {
                float dropSpeed = Mathf.Max(cachedTargetCap / previewDecrementDuration, 15f);
                previewValue = Mathf.MoveTowards(previewValue, 0f, Time.deltaTime * dropSpeed);
            }
        }

        // ---------------- VISIBILITY ----------------
        if (previewValue <= 0.05f && !validDeposit)
        {
            previewValue = 0;
            previewText.SetVisible(false);
            lastPreviewInt = 0;
        }
        else
        {
            previewText.SetVisible(true);

            int currentInt = Mathf.RoundToInt(previewValue);
            previewText.SetValue(currentInt, cachedTargetCap);

            Color teamCol = wasTeamOneLast ? yellowTeamColor : blueTeamColor;
            Color targetColor = validDeposit ? teamCol : greyColor;

            previewText.SetColor(Color.Lerp(
                previewText.text.color,
                targetColor,
                Time.deltaTime * 10f
            ));

            // scaling
            float visualRatio = cachedTargetCap > 0 ? (previewValue / cachedTargetCap) : 0;
            float growScale = Mathf.Lerp(0.8f, 1.2f, visualRatio) * baseScaleMultiplier;

            if (currentInt > lastPreviewInt && validDeposit)
            {
                previewText.transform.localScale =
                    Vector3.one * (growScale + punchIntensity);
                lastPreviewInt = currentInt;
            }

            previewText.transform.localScale = Vector3.Lerp(
                previewText.transform.localScale,
                Vector3.one * growScale,
                Time.deltaTime * 20f
            );
        }
    }


}

