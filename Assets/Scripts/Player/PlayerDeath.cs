using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeath : MonoBehaviour
{    
    public int spawnNum;

    [Header("Stored References")]
    //Player Respawn
    public MeshRenderer playerMesh;
    public PlayerInput playerInput;
    public Collider playerCollider;
    public Light playerLight;
    public GameObject bombText;
    public GameObject deathPos;
    public GameObject copperModel;
    public GameObject impactSphere;

    [Header("Invinciblity Variables")]
    private bool isInvincible = false;
    public float invincibleDuration = 3;
    public float invincibleTimer = 0;
    public float blinkInterval = 0.5f;    // How fast it blinks

    private MeshRenderer meshRenderer;

    //Gem
    public List<GameObject> collectedGems = new List<GameObject>();
    public GameObject gemPrefab;

    [Header("Respawn Customizaiton/Check")]
    public float respawnDelay;
    [HideInInspector] public bool isPlayerDead;

    [Header("Gem Drop Customization/Check")]
    public int gemCount;
    public float scatterForce;
    public float gemDropDelay;

    private void Start()
    {
        invincibleTimer = invincibleDuration;
        meshRenderer = GetComponent<MeshRenderer>();
        deathPos = GameObject.Find("DeathChamber");
    }


    private void Update()
    {
        gemCount = collectedGems.Count;

        if (invincibleTimer < invincibleDuration)
        {
            isInvincible = true;
            //StartCoroutine(BlinkEffect());
        }
        else
        {
            isInvincible = false;
        }
            invincibleTimer += Time.deltaTime;
    }


    public void PlayerDie()
    {
        if (!isInvincible)
        {
            StartCoroutine(PlayerDieOrder());
        }
    }


    public IEnumerator PlayerDieOrder()
    {
        StartCoroutine(DeathEffect());


        isPlayerDead = true;

        //Turn the player off
        playerMesh.enabled = false;
        //playerCollider.enabled = false;
        copperModel.SetActive(false);
        playerLight.gameObject.SetActive(false);
        bombText.SetActive(false);


        //Drop gems then respawn
        yield return new WaitForSeconds(gemDropDelay);
        DropGems();
        collectedGems.Clear();
        transform.position = deathPos.transform.position;
        yield return new WaitForSeconds(respawnDelay);

        //Turn player back on
        playerMesh.enabled = true;
        //playerCollider.enabled = true;
        playerLight.gameObject.SetActive(true);
        copperModel.SetActive(true);
        bombText.SetActive(true);
        isPlayerDead = false;
        isInvincible = true;
        invincibleTimer = 0;
        transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
    }

    //public IEnumerator BlinkEffect()
    //{
    //    float timer = 0f;
    //    while (timer < invincibleDuration)
    //    {
    //        copperModel.SetActive(false);
    //        yield return new WaitForSeconds(blinkInterval / 2f);
    //        copperModel.SetActive(true);  // Show
    //        yield return new WaitForSeconds(blinkInterval / 2f);

    //        timer += blinkInterval;
    //    }

    //    // Ensure character is visible at the end
    //    copperModel.SetActive(true);
    //}



    //Drop Gem Sequence
    public void DropGems()
    {
        foreach(GameObject gems in collectedGems)
        {
            gems.GetComponent<Collider>().enabled = true;
            gems.transform.position = transform.position;
            gems.transform.rotation = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
            //Debug.Log(gems.transform.rotation);
            gems.gameObject.SetActive(true);
            gems.gameObject.GetComponent<Rigidbody>().AddForce(gems.transform.forward * scatterForce);
        }
    }

    IEnumerator DeathEffect()
    {
        GameObject currentSphere = Instantiate(impactSphere, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        Destroy(currentSphere);
    }
}
