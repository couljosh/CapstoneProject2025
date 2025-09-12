using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayers : MonoBehaviour
{
    public InputActionAsset inputActions;
    public PlayerInputManager playerInputManager;
    public GameObject[] Players;

    public GameObject Spawn1;
    public GameObject Spawn2;
    public GameObject Spawn3;
    public GameObject Spawn4;

    void Start()
    {
        int i = 0;

        foreach (var gamePad in Gamepad.all)
        {
            print(gamePad.name);

            PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);
            i++;
        }
    }
}