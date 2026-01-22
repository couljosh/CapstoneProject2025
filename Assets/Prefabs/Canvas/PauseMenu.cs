using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool GameIsPaused = false;
    public Gamepad pauseUser;

    public RectTransform cursor;
    public float cursorSpeed;

    public InputActionAsset actionAsset;
    public InputAction pauseAction;

    private void Awake()
    {
        pauseMenu.SetActive(false);

        pauseAction = actionAsset.FindAction("UI/Pause");

    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPause;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPause;
        pauseAction.Disable();
    }

    void Update()
    {
        print(pauseUser);

        if (GameIsPaused && pauseUser != null)
        {           
            Vector2 stickValue = pauseUser.leftStick.ReadValue();
            cursor.anchoredPosition += stickValue * cursorSpeed * Time.unscaledDeltaTime;


        }
    }

     public void Pause()
    {
        GameIsPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        GameIsPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

    }

    void OnPause(InputAction.CallbackContext context)
    {
        if (context.control.device is Gamepad gamePad)
            pauseUser = gamePad;

        if (GameIsPaused)
            Resume();
        else       
            Pause();
    }

}
