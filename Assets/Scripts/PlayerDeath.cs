using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeath : MonoBehaviour
{

    public float respawnDelay;
    public int spawnNum;

    public MeshRenderer playerMesh;
    public PlayerInput playerInput;
    public Light playerLight;
    public GameObject bombText;

   

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void PlayerDie()
    {
        StartCoroutine(PlayerDieOrder());   
    }

    public IEnumerator PlayerDieOrder()
    {
        playerMesh.enabled = false;
        playerLight.gameObject.SetActive(false);
        bombText.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
        playerMesh.enabled = true;
        playerLight.gameObject.SetActive(true);
        bombText.SetActive(true);

    }
}
