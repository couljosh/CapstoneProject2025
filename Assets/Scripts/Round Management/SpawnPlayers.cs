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
    private InputDevice[] currentInputDevices = new InputDevice[3];
    private GameObject[] currentPlayers = new GameObject[3];

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
                    break;
                }
                else
                {
                    //invoke spawn in other script
                    p.initialSpawn();
                    //p.uiManager = null;
                }
                    
            }
 
            Destroy(this.gameObject);
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
        //reset arrays
        //for (int g = 0; g < currentInputDevices.Length; g++)
        //{
        //    currentInputDevices[g] = null;
        //}

        //for (int g = 0; g < currentPlayers.Length; g++)
        //{
        //    currentPlayers[g] = null;
        //}
        uiManager = Object.FindFirstObjectByType<PlayerUIManager>();

        int i = 0;
        currentPlayerCount = Gamepad.all.Count;
        foreach (var gamePad in Gamepad.all)
        {
            //print(i);
            PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);
            currentInputDevices[i] = gamePad;
            currentPlayers[i] = newPlayerInput.gameObject;

            if (uiManager != null)
            {
                
                uiManager.RegisterPlayer(newPlayerInput.gameObject);
            }

            //currentPlayers[i].transform.position = GameObject.Find("Spawn" + i).transform.position;

            i++;
        }
    }

    private void Update()
    {
        //if(Gamepad.all.Count != currentPlayerCount)
        //{
            
        //    //int i = currentPlayerCount;
        //    //print(i);
        //    //foreach (var gamePad in Gamepad.all)
        //    //{
        //    //    PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: gamePad);

        //    //    if (uiManager != null)
        //    //    {
        //    //        uiManager.RegisterPlayer(newPlayerInput.gameObject);
        //    //    }

        //    //    i++;
        //    //}

        //    //currentPlayerCount = Gamepad.all.Count;
        //}

        InputSystem.onDeviceChange +=
            (sender, args) =>
            {
                switch (args)
                {
                    case InputDeviceChange.Added:

                        //find newest player to add
                        for (int i = 0; i <= currentInputDevices.Length; i++)
                        {
                            //first empty controller space
                            if(currentInputDevices[i] == null)
                            {
                                //if this player wasn't ingame already
                                if(currentPlayers[i] == null)
                                {
                                    //instantiate a new player
                                    PlayerInput newPlayerInput = PlayerInput.Instantiate(Players[i], controlScheme: "Gamepad", pairWithDevice: sender);

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

                    case InputDeviceChange.Removed:
                        
                        //find which input device it was
                        for(int i = 0; i <= currentInputDevices.Length; i++)
                        {
                            if(currentInputDevices[i] == sender)
                            {
                                currentInputDevices[i] = null;
                                break;
                            }
                        }

                        break;
                }
            };
    }
}