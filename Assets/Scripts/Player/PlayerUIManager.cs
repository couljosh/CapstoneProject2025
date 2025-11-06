using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    //used to assign the prefab
    //Debug.Log("test");
    [Header("UI Prefab")]
    public GameObject kickChargeUIPrefab;

    [Header("UI Canvas")]
    public Canvas mainCanvas;

    public Vector3 uiOffset = new Vector3(0f, 2f, 0f);

    private void Start()
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>(); //find canvas
            if (mainCanvas == null)
            {
                //Debug.LogError("PlayerUIManager no canvas found");
                enabled = false;
                return;
            }
        }
    }

    //called by the SpawnPlayers script when a new player is successfully spawned.
    public void RegisterPlayer(GameObject playerObject)
    {
        PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
        if (playerMove == null)
        {
            Debug.LogError("PlayerUIManager: Spawned player does not have a PlayerMove component.");
            return;
        }

        //instantiate UI prefab
        GameObject uiInstance = Instantiate(kickChargeUIPrefab, mainCanvas.transform);
        KickChargeUI kickUI = uiInstance.GetComponent<KickChargeUI>();

        if (kickUI == null)
        {
            Debug.LogError("PlayerUIManager: KickChargeUIPrefab is missing the KickChargeUI script.");
            return;
        }

        //assign UI to PlayerMove script
        playerMove.SetKickChargeUI(kickUI);

        WorldToScreenUI worldToScreen = uiInstance.AddComponent<WorldToScreenUI>();
        worldToScreen.target = playerObject.transform;
        worldToScreen.barElement = uiInstance.GetComponent<RectTransform>();
        worldToScreen.uiOffset = uiOffset;
    }
}