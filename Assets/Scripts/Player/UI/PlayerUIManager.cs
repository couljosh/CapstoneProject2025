using UnityEngine;

//note
//this script handles the spawning and managing of the player charge kick bar and bomb ammo bars.

public class PlayerUIManager : MonoBehaviour
{
    [Header("Kick Charge UI")]
    public GameObject kickChargeUIPrefab;

    [Header("Bomb Ammo UI")]
    public GameObject bombAmmoBarPrefab;

    [Header("Gems Held Counter UI")]
    public GameObject gemsHeldCounterPrefab;

    [Header("UI Canvas & Offset")]
    public Canvas mainCanvas;

    [Tooltip("Vertical offset of the UI bar, relative to screen height (0.01 = 1%)")]
    public float screenHeightOffsetValue = 0.04f;

    [Header("Player Portraits")]
    public PlayerPortraitUI[] playerPortraits;
    private PlayerDeath[] trackedPlayers = new PlayerDeath[5];
    private bool[] lastDeathState = new bool[5];

    private void Start()
    {
        if (mainCanvas == null)
        {
            mainCanvas = Object.FindFirstObjectByType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogError("assign canvas in inspector");
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            if (trackedPlayers[i] != null)
            {
                bool currentState = trackedPlayers[i].isPlayerDead;

                //if the state changed (died or respawned)
                if (currentState != lastDeathState[i])
                {
                    //trigger the pop and color change on the portrait
                    if (playerPortraits[i - 1] != null)
                    {
                        playerPortraits[i - 1].SetStatus(currentState);
                    }
                    lastDeathState[i] = currentState;
                }
            }
        }
    }

    public void RegisterPlayer(GameObject playerObject, int playerID)
    {
        PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
        BombSpawn bombSpawn = playerObject.GetComponent<BombSpawn>();

        //setup for player portrait animation
        PlayerDeath pd = playerObject.GetComponent<PlayerDeath>();
        if (pd != null && playerID >= 1 && playerID <= 4)
        {
            trackedPlayers[playerID] = pd;
            lastDeathState[playerID] = pd.isPlayerDead;
        }

        if (playerMove != null)
        {
            playerMove.playerNum = playerID;
        }

        // ------------------ GEMS HELD COUNTER ------------------
        if (gemsHeldCounterPrefab != null)
        {
            GameObject gemsUIInstance = Instantiate(gemsHeldCounterPrefab, mainCanvas.transform);
            GemsHeldUI gemsUI = gemsUIInstance.GetComponent<GemsHeldUI>();

            if (gemsUI != null)
            {
                playerMove.GetComponent<PlayerDeath>().gemsHeldUI = gemsUI;

                SetupWorldToScreen(
                    gemsUIInstance,
                    playerObject.transform,
                    this.screenHeightOffsetValue + 0.055f
                );
            }
            else
            {
                Destroy(gemsUIInstance);
            }
        }

        // gems held counter
        if (gemsHeldCounterPrefab != null)
{
    GameObject gemsUIInstance = Instantiate(gemsHeldCounterPrefab, mainCanvas.transform);
    GemsHeldUI gemsUI = gemsUIInstance.GetComponent<GemsHeldUI>();

    if (gemsUI != null)
    {
        playerMove.GetComponent<PlayerDeath>().gemsHeldUI = gemsUI;

        SetupWorldToScreen(
            gemsUIInstance,
            playerObject.transform,
            this.screenHeightOffsetValue + 0.055f
        );
    }
    else
    {
        Destroy(gemsUIInstance);
    }
}

        // ------------------ KICK CHARGE BAR ------------------
        if (kickChargeUIPrefab != null)
        {
            GameObject kickUIInstance = Instantiate(kickChargeUIPrefab, mainCanvas.transform);
            KickChargeUI kickUI = kickUIInstance.GetComponent<KickChargeUI>();

            if (kickUI != null)
            {
                playerMove.SetKickChargeUI(kickUI);
                SetupWorldToScreen(kickUIInstance, playerObject.transform, screenHeightOffsetValue + 0.012f);
            }
            else
            {
                Destroy(kickUIInstance);
            }
        }

        // ------------------ BOMB AMMO BAR ------------------
        if (bombAmmoBarPrefab != null && bombSpawn != null)
        {
            GameObject bombUIInstance = Instantiate(bombAmmoBarPrefab, mainCanvas.transform);
            BombAmmoBar ammoBar = bombUIInstance.GetComponent<BombAmmoBar>();

            if (ammoBar != null)
            {
                //calculate team based on ID
                bool isBlueTeam = playerID == 2 || playerID == 4;

                //pass player id to ammo bar
                ammoBar.Initialize(bombSpawn, isBlueTeam, playerID);
                bombSpawn.bombAmmoBarUI = ammoBar;

                SetupWorldToScreen(bombUIInstance, playerObject.transform, screenHeightOffsetValue + 0.03f);
            }
            else
            {
                Destroy(bombUIInstance);
            }
        }
    }

    private void SetupWorldToScreen(GameObject uiInstance, Transform target, float screenOffset)
    {
        WorldToScreenUI worldToScreen = uiInstance.AddComponent<WorldToScreenUI>();

        RectTransform rectTransform = uiInstance.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            worldToScreen.target = target;
            worldToScreen.barElement = rectTransform;
            worldToScreen.screenHeightOffset = screenOffset;
        }
    }
}