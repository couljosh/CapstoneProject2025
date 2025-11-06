using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpawnPlayers : MonoBehaviour
{
    public InputActionAsset inputActions;
    public PlayerInputManager playerInputManager;
    public GameObject[] Players;

    public float respawnDelay;
    private PlayerUIManager uiManager;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        uiManager = FindObjectOfType<PlayerUIManager>();

        if (playerInputManager != null)
        {
            playerInputManager.DisableJoining();
        }
    }

    private void Start()
    {
        int i = 0;
        foreach (var gamePad in Gamepad.all)
        {
            if (i >= Players.Length) break;
            PlayerInput newPlayerInput = PlayerInput.Instantiate(
                Players[i],
                controlScheme: "Gamepad",
                pairWithDevice: gamePad
            );

            GameObject newPlayerObject = newPlayerInput.gameObject;

            newPlayerObject.GetComponent<PlayerMove>().playerNum = i + 1;
            if (uiManager != null)
            {
                uiManager.RegisterPlayer(newPlayerObject);
            }
            i++;
        }
    }
}