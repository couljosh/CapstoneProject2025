using UnityEngine;

//note
//this script handles the spawning and managing of the player charge kick bar and bomb ammo bars.


public class PlayerUIManager : MonoBehaviour
{
    [Header("Kick Charge UI")]
    public GameObject kickChargeUIPrefab;

    [Header("Bomb Ammo UI")]
    public GameObject bombAmmoBarPrefab; //base prefab which has all the content of the ammo bar to instantiate on the player.

    [Header("UI Canvas & Offset")]
    public Canvas mainCanvas;

    [Tooltip("Vertical offset of the UI bar, relative to screen height (0.01 = 1%)")]
    public float screenHeightOffsetValue = 0.04f;

    private void Start()
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
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

        //kick charge bar
        if (kickChargeUIPrefab != null)
        {
            GameObject kickUIInstance = Instantiate(kickChargeUIPrefab, mainCanvas.transform);
            KickChargeUI kickUI = kickUIInstance.GetComponent<KickChargeUI>();

            if (kickUI != null)
            {
                playerMove.SetKickChargeUI(kickUI);
                SetupWorldToScreen(kickUIInstance, playerObject.transform, this.screenHeightOffsetValue + 0.012f);
            }
            else
            {
                Destroy(kickUIInstance);
            }
        }
        //Debug.Log("test bomb")

        //bomb ammo bar 
        if (bombAmmoBarPrefab != null && bombSpawn != null)
        {
            GameObject bombUIInstance = Instantiate(bombAmmoBarPrefab, mainCanvas.transform);
            BombAmmoBar ammoBar = bombUIInstance.GetComponent<BombAmmoBar>();

            if (ammoBar != null)
            {
                ammoBar.Initialize(bombSpawn, bombSpawn.playerStats.bombRegenTime);

                SetupWorldToScreen(bombUIInstance, playerObject.transform, this.screenHeightOffsetValue + 0.03f);
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