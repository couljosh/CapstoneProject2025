using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BombSpawn : MonoBehaviour
{    
    [Header("References")]
    public PlayerStats playerStats;
    private PlayerDeath playerDeath;
    private PlayerEffects playerEffects;

    public GameObject bombPrefab;
    public GameObject bombCountText;

    [Header("Bomb Variables")]
    public float regenTimer;
    private int bombsHeld;


    void Start()
    {
        playerEffects = gameObject.GetComponent<PlayerEffects>();
        bombsHeld = 0;
        playerDeath = GetComponent<PlayerDeath>();
    }


    void Update()
    {
        regenTimer += Time.deltaTime;

        //Give Bomb based on Cooldown
        if (regenTimer > playerStats.bombRegenTime && bombsHeld < playerStats.maxBombsHeld)
        {
            bombsHeld++;
            regenTimer = 0;
        }

        bombCountText.GetComponent<TextMeshPro>().text = bombsHeld.ToString();
    }

    // Bomb Spawning //----------------------------------------------------------------------------------------
    public void OnSpawnBomb(InputAction.CallbackContext context)
    {
        playerEffects.copperAnimator.SetTrigger("Bomb");
        if (bombsHeld > 0 && context.performed && !playerDeath.isPlayerDead)
        {
            Instantiate(bombPrefab, transform.position + transform.forward, Quaternion.identity);
            bombsHeld--;
        }
        else
        {
            playerEffects.copperAnimator.ResetTrigger("Bomb");
        }
    }
}
