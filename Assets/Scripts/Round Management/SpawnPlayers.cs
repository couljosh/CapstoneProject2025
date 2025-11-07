using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

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

        uiManager = Object.FindFirstObjectByType<PlayerUIManager>();
    }

    private void Start()
    {
        int i = 0;

        foreach (var gamePad in Gamepad.all)
        {
            PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);

            if (uiManager != null)
            {
                uiManager.RegisterPlayer(newPlayerInput.gameObject);
            }

            i++;
        }
    }
}