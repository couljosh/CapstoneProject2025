using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Stored References")]
    public InputActionAsset playerControls;
    private InputAction startGameAction;
    public TextMeshProUGUI pressButtonText;

    [Header("Menu Customization")]
    public float fadingSpeed;
    public float fadingDelay;
    public int levelOneIndex;

    private void Start()
    {
        GameScore.ResetScore();
    }

    //Enable Input for Active Controllers
    private void OnEnable()
    {
        foreach (var controller in Gamepad.all)
        {
            startGameAction = playerControls.FindActionMap("Menu").FindAction("StartGame");
            startGameAction.Enable();
        }
    }


    //Scene Switch on Button Press
    private void Update()
    {
        if (startGameAction != null)
        {
            if (startGameAction.IsPressed())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            }
        }

        PressAnyButtonFading();
    }


    //Quit Applcation Event
    public void QuitGame()
    {
        Application.Quit();
    }


    //Button Flash Effect
    public void PressAnyButtonFading()
    {
        pressButtonText.alpha = Mathf.PingPong(Time.time * fadingSpeed, fadingDelay);

    }
}
