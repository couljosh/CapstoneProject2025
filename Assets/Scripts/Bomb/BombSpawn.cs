using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TerrainUtils;

public class BombSpawn : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    private PlayerDeath playerDeath;
    private PlayerEffects playerEffects;

    public GameObject bombPrefab;

    [Header("UI Reference")]
    public BombAmmoBar bombAmmoBarUI;

    [Header("Bomb Variables")]
    public float regenTimer;
    public float bombRad;
    private int bombsHeld;

    public LayerMask terrainMask;


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

            if (regenTimer >= playerStats.bombRegenTime)
            {
                regenTimer = playerStats.bombRegenTime;

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

                NotifyBombPlaced();

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

    public void NotifyBombPlaced()
    {
        if (bombAmmoBarUI != null)
        {
            bombAmmoBarUI.StartUsedPopEffect();
        }
    }
}