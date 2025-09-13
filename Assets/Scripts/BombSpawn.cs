using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BombSpawn : MonoBehaviour
{
    public GameObject bombPrefab;
    public GameObject bombCountText;
    public int playerNum;
    public float bombRegenTime;
    public float bombUseCooldown;
    private float regenTimer;
    private float useTimer;

    private int bombsHeld;
    public int maxBombsHeld;

    void Start()
    {
        bombsHeld = 0;
       
    }

    void Update()
    {
        regenTimer += Time.deltaTime;
        //useTimer += Time.deltaTime;

        if (regenTimer > bombRegenTime && bombsHeld < maxBombsHeld)
        {
            bombsHeld++;
            regenTimer = 0;
        }

        bombCountText.GetComponent<TextMeshPro>().text = bombsHeld.ToString();
    }

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
