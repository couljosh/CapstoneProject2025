using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpawnPlayers : MonoBehaviour
{
    public InputActionAsset inputActions;
    public PlayerInputManager playerInputManager;
    public GameObject[] Players;

    public float respawnDelay;

    private PlayerUIManager uiManager; // ref for chargekick ui

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        uiManager = FindObjectOfType<PlayerUIManager>(); //find ui maanger for chargekick
    }

    private void Start()
    {
        int i = 0;

        foreach (var gamePad in Gamepad.all)
        {
            PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);

            //register spawned player with UI manager
            if (uiManager != null)
            {
                uiManager.RegisterPlayer(newPlayerInput.gameObject);
            }

            i++;
        }
    }
}