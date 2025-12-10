using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;

public class SpawnPlayers : MonoBehaviour
{
    public InputActionAsset inputActions;
    public PlayerInputManager playerInputManager;
    public GameObject[] Players;
    private InputDevice[] currentInputDevices = new InputDevice[4];
    private GameObject[] currentPlayers = new GameObject[4];

    public float respawnDelay;

    private PlayerUIManager uiManager;
    private int currentPlayerCount;

    private void Awake()
    {
        //awake won't run a second time on do-not-destroy objects, so only newly instantiated versions will run this
        if(GameObject.FindObjectsByType<PlayerInputManager>(FindObjectsSortMode.None).Length > 1)
        {
            //find the other input manager and make it spawn the players again for the next level
            SpawnPlayers[] mainscript = GameObject.FindObjectsByType<SpawnPlayers>(FindObjectsSortMode.None);
            
            foreach(var p in mainscript)
            {
                if (p.gameObject == this.gameObject)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    //invoke spawn in other script
                    p.initialSpawn();
                    //p.uiManager = null;
                }
                    
            }

            
        }
            
        else //this is the only one, make it the main
            DontDestroyOnLoad(this.gameObject);

        uiManager = Object.FindFirstObjectByType<PlayerUIManager>();
    }

    private void Start()
    {
        initialSpawn();
    }

    public void initialSpawn()
    {
        uiManager = Object.FindFirstObjectByType<PlayerUIManager>();

        int i = 0;
        currentPlayerCount = Gamepad.all.Count;
        foreach (var gamePad in Gamepad.all)
        {

            PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);
            currentInputDevices[i] = gamePad;
            currentPlayers[i] = newPlayerInput.gameObject;

            if (uiManager != null)
            {
                
                uiManager.RegisterPlayer(newPlayerInput.gameObject);
            }

            i++;
        }
    }

    private void Update()
    {
        //this whole bird nest keeps parity between connected controllers and the players they controll using two arrays as its structure (currentinputdevices and currentplayers)
        InputSystem.onDeviceChange +=
            (sender, args) =>
            {
                //only spawn a player if the game is running, and it can find a gameobject only present in every level (so intermissions dont spawn player)
                if (Application.isPlaying && GameObject.Find("RepoMover"))
                {
                    switch (args)
                    {
                        //if a new input device has been added
                        case InputDeviceChange.Added:

                            //find newest player to add
                            for (int i = 0; i <= currentInputDevices.Length; i++)
                            {
                                //first empty controller space
                                if (currentInputDevices[i] == null)
                                {
                                    //if this player wasn't ingame already
                                    if (currentPlayers[i] == null)
                                    {
                                        //instantiate a new player
                                        PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: sender);
                                        currentPlayers[i] = newPlayerInput.gameObject;
                                        if (uiManager != null)
                                        {
                                            uiManager.RegisterPlayer(newPlayerInput.gameObject);
                                        }
                                    }

                                    currentInputDevices[i] = sender;

                                    break;
                                }
                            }

                            break;
                        
                        //if an input device is removed
                        case InputDeviceChange.Removed:

                            //find which input device it was
                            for (int i = 0; i <= currentInputDevices.Length; i++)
                            {
                                if (currentInputDevices[i] == sender)
                                {

                                    currentInputDevices[i] = null;
                                    break;
                                }
                            }

                            break;
                    }
                }
                
            };
    }
}