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

    private void Start()
    {
        int i = 0;

        foreach (var gamePad in Gamepad.all)
        {
            PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);
            i++;
        }
    }
}