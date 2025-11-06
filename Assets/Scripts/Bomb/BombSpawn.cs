using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BombSpawn : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    private PlayerDeath playerDeath;
    private PlayerEffects playerEffects;

    public GameObject bombPrefab;

    [Header("Bomb Variables")]
    private float regenTimer;
    private int bombsHeld;

    public int CurrentBombsHeld => bombsHeld;
    public float RegenerationProgress => regenTimer / playerStats.bombRegenTime;


    void Start()
    {
        playerEffects = gameObject.GetComponent<PlayerEffects>();
        bombsHeld = 0;
        playerDeath = GetComponent<PlayerDeath>();
    }


    void Update()
    {
        if (bombsHeld < playerStats.maxBombsHeld)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer > playerStats.bombRegenTime)
            {
                bombsHeld++;
                regenTimer = 0;
            }
        }
        else
        {
            regenTimer = 0;
        }
    }

    public void OnSpawnBomb(InputAction.CallbackContext context)
    {
        if (context.performed && !playerDeath.isPlayerDead)
        {
            if (bombsHeld > 0)
            {
                playerEffects.copperAnimator.SetTrigger("Bomb");
                Instantiate(bombPrefab, transform.position + transform.forward, Quaternion.identity);
                bombsHeld--;

                if (bombsHeld < playerStats.maxBombsHeld && regenTimer == 0)
                {
                    regenTimer = 0.001f;
                }
            }
            else
            {
                playerEffects.copperAnimator.ResetTrigger("Bomb");
            }
        }
    }
}