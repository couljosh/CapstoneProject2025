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

    public void RegisterPlayer(GameObject playerObject)
    {
        PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
        BombSpawn bombSpawn = playerObject.GetComponent<BombSpawn>();

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
                PlayerMove pm = playerObject.GetComponent<PlayerMove>();
                bool isBlueTeam = pm.playerNum == 3 || pm.playerNum == 4;

                ammoBar.Initialize(bombSpawn, isBlueTeam);
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