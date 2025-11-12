using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public class PlayerDeath : MonoBehaviour
{

    [Header("References")]
    public PlayerStats playerStats;
    public PlayerInput playerInput;
    private Gamepad playerGamepad;

    public MeshRenderer playerMesh;
    private MeshRenderer meshRenderer;

    public Collider playerCollider;
    public Light playerLight;
    public GameObject bombText;
    private GameObject deathPos;
    public GameObject copperModel;
    public GameObject impactSphere;
    public List<GameObject> collectedGems = new List<GameObject>();
    private DynamicCameraFollow dynamicCamera;

    [Header("Invinciblity Variables")]
    private bool isInvincible = false;
    private float invincibleTimer = 0;
    public float blinkInterval = 0.5f;

    [Header("Respawn Customizaiton/Check")]
    [HideInInspector] public bool isPlayerDead;
    public int spawnNum;

    [Header("Gem Drop Customization/Check")]
    public int gemCount;


    private void Start()
    {
        invincibleTimer = playerStats.invincibleDuration;
        meshRenderer = GetComponent<MeshRenderer>();
        deathPos = GameObject.Find("DeathChamber");

        playerGamepad = (Gamepad)playerInput.devices[0];

        dynamicCamera = Camera.main.GetComponent<DynamicCameraFollow>();
    }


    private void Update()
    {
        gemCount = collectedGems.Count;

        if (invincibleTimer < playerStats.invincibleDuration)
        {
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }

        invincibleTimer += Time.deltaTime;
    }

    // Death Triggers //----------------------------------------------------------------------------------------
    public void PlayerDie()
    {
        if (!isInvincible)
        {
            StartCoroutine(PlayerDeathSeq());
        }
    }

    //Ignore invincibility
    public void ForcePlayerDie()
    {
        StartCoroutine(PlayerDeathSeq());
    }


    // Death Sequence //----------------------------------------------------------------------------------------
    public IEnumerator PlayerDeathSeq()
    {
        StartCoroutine(DeathEffect());


        if (playerGamepad != null)
            playerGamepad.SetMotorSpeeds(1f, 1f);

        /* Switch rumble has to be set uniquely because poor Nintendo absolutely had to have some
        proprietary bullshit on top of HID. Even then, it only works over Bluetooth.*/

        if (playerGamepad is SwitchProControllerHID)
        {
            Debug.Log("Switch rumble");
            playerGamepad.SetMotorSpeeds(5f, 5f);
        }

        DisablePlayer();
        DropGems();

        gameObject.GetComponent<Animator>().applyRootMotion = true;
        
        //remove player
        transform.position = deathPos.transform.position;

        yield return new WaitForSeconds(playerStats.gemDropDelay);


        if (playerGamepad != null)
            playerGamepad.SetMotorSpeeds(0f, 0f);


        yield return new WaitForSeconds(playerStats.respawnDelay);
        EnablePlayer();
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<Animator>().applyRootMotion = false;

    }


    // Player Death Stages  //----------------------------------------------------------------------------------------
    IEnumerator DeathEffect()
    {
        GameObject currentSphere = Instantiate(impactSphere, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        Destroy(currentSphere);
    }

    void DisablePlayer()
    {
        isPlayerDead = true;
        dynamicCamera.RemovePlayer(transform);
        playerMesh.enabled = false;
        copperModel.SetActive(false);
        //playerCollider.enabled = false;

        playerLight.gameObject.SetActive(false);
        bombText.SetActive(false);
    }


    public void DropGems()
    {
        foreach (GameObject gems in collectedGems)
        {
            gems.GetComponent<Collider>().enabled = true;
            gems.transform.position = transform.position;
            gems.transform.rotation = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
            Debug.Log(gems.transform.rotation);
            gems.gameObject.SetActive(true);
            gems.gameObject.GetComponent<Rigidbody>().AddForce(gems.transform.forward * playerStats.scatterForce);
        }

        collectedGems.Clear();
        gemCount = 0;

    }


    void EnablePlayer()
    {
        gameObject.transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;

        //add player back
        dynamicCamera.AddPlayer(transform);
        isPlayerDead = false;

        playerMesh.enabled = true;
        copperModel.SetActive(true);
        //playerCollider.enabled = true;

        playerLight.gameObject.SetActive(true);
        bombText.SetActive(true);

        isInvincible = true;
        invincibleTimer = 0;

        //re-enable tracking with dynamic camera
    }
}
