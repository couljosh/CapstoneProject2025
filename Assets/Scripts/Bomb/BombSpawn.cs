using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BombSpawn : MonoBehaviour
{    
    public int playerNum;

    [Header("Gameobject References")]
    public GameObject bombPrefab;
    public GameObject bombCountText;

    [Header("Bomb Cooldown Variables")]
    public float bombRegenTime;
    public float bombUseCooldown;
    private float regenTimer;

    [Header("Bombs Held Variables")]
    private int bombsHeld;
    public int maxBombsHeld;

    void Start()
    {
        bombsHeld = 0;
    
    }

    void Update()
    {
        regenTimer += Time.deltaTime;

        //Give Bomb based on Cooldown
        if (regenTimer > bombRegenTime && bombsHeld < maxBombsHeld)
        {
            bombsHeld++;
            regenTimer = 0;
        }

        bombCountText.GetComponent<TextMeshPro>().text = bombsHeld.ToString();
    }

    //Spawn Bomb
    public void OnSpawnBomb(InputAction.CallbackContext context)
    {
        if (bombsHeld > 0 && context.performed)
        {
            Instantiate(bombPrefab, transform.position + transform.forward, Quaternion.identity);
            bombsHeld--;
            bombUseCooldown = 0;
        }
    }
}
