using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int levelOneIndex;

    public InputActionAsset playerControls;

    private InputAction startGameAction;

    public TextMeshProUGUI pressButtonText;

    public float fadingSpeed;
    public float fadingDelay;

    public void StartGame()
    {

    }

    private void OnEnable()
    {
        foreach (var controller in Gamepad.all)
        {
            startGameAction = playerControls.FindActionMap("Menu").FindAction("StartGame");
            startGameAction.Enable();
        }
    }

    private void Update()
    {
        if (startGameAction != null)
        {
            if (startGameAction.IsPressed())
            {
                SceneManager.LoadScene(levelOneIndex);

            }
        }

        PressAnyButtonFading();
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void PressAnyButtonFading()
    {
        pressButtonText.alpha = Mathf.PingPong(Time.time * fadingSpeed, fadingDelay);

    }
}
