using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.Rendering;

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
    private GameObject deathPos;
    public GameObject copperModel;
    public GameObject impactSphere;
    public List<GameObject> collectedGems = new List<GameObject>();
     DynamicCameraFollow dynamicCamera;

    [Header("Invinciblity Variables")]
    private bool isInvincible = false;
    private float invincibleTimer = 0;
    private float timeSinceLastFlash = 0;
    public float blinkInterval = 0.2f;
    [HideInInspector] public float timeTouchingLava = 0f;

    [Header("Respawn Customizaiton/Check")]
    [HideInInspector] public bool isPlayerDead;
    public int spawnNum;

    [Header("Gem Drop Customization/Check")]
    public int gemCount;

    [Header("Gem Counter Checks UI")]
    public GemsHeldUI gemsHeldUI;


    private void Start()
    {
        invincibleTimer = playerStats.invincibleDuration;
        meshRenderer = GetComponent<MeshRenderer>();
        deathPos = GameObject.Find("DeathChamber");

        playerGamepad = (Gamepad)playerInput.devices[0];

        dynamicCamera = Camera.main.GetComponentInParent<DynamicCameraFollow>();

        //gameObject.transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
        EnablePlayer();
    }


    private void Update()
    {
        gemCount = collectedGems.Count;

        if (invincibleTimer < playerStats.invincibleDuration)
        {
            isInvincible = true;
            PlayerFlash();
        }
        else
        {
            isInvincible = false;
            copperModel.SetActive(true);
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
        //reset gem ui
        if (gemsHeldUI != null)
        {
            gemsHeldUI.AnimateToValue(0);
        }

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
        playerMesh.enabled = false;
        copperModel.SetActive(false);
        //playerCollider.enabled = false;

        playerLight.gameObject.SetActive(false);

        if(dynamicCamera)
        dynamicCamera.RemovePlayer(transform);
    }


    public void DropGems()
    {
        foreach (GameObject gems in collectedGems)
        {
            gems.GetComponent<Collider>().enabled = true;
            gems.transform.position = transform.position;
            gems.transform.rotation = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
            gems.gameObject.SetActive(true);
            gems.gameObject.GetComponent<Rigidbody>().AddForce(
                gems.transform.forward * playerStats.scatterForce
            );
        }

        collectedGems.Clear();
        gemCount = 0;

        //reset gem ui
        if (gemsHeldUI != null)
        {
            gemsHeldUI.AnimateToValue(0);
        }
    }

    void EnablePlayer()
    {
        
        //add player back
        if (dynamicCamera)
        dynamicCamera.AddPlayer(transform);

        isPlayerDead = false;

        playerMesh.enabled = true;
        copperModel.SetActive(true);
        //playerCollider.enabled = true;

        playerLight.gameObject.SetActive(true);

        isInvincible = true;
        invincibleTimer = 0;

        gameObject.transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
        //playerInput.transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
        playerInput.GetComponent<Rigidbody>().position = GameObject.Find("Spawn" + spawnNum).transform.position;
    }

    void PlayerFlash()
    {
        timeSinceLastFlash += Time.deltaTime;

        if(timeSinceLastFlash > blinkInterval)
        {
            copperModel.SetActive(!copperModel.activeSelf);
            timeSinceLastFlash = 0;
        }
    }
}
