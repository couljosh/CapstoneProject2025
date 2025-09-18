using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeath : MonoBehaviour
{

    public float respawnDelay;
    public int spawnNum;

    public MeshRenderer playerMesh;
    public PlayerInput playerInput;
    public Collider playerCollider;
    public Light playerLight;
    public GameObject bombText;
    public List<GameObject> collectedGems = new List<GameObject>();

    public GameObject gemPrefab;
    private Vector3 deathPos;

    public int gemCount;

    public float scatterForce;
   

    private void Start()
    {

    }

    private void Update()
    {
        gemCount = collectedGems.Count;
    }

    public void PlayerDie()
    {
        StartCoroutine(PlayerDieOrder());   
    }

    public IEnumerator PlayerDieOrder()
    {
        //Turn the player off
        playerMesh.enabled = false;
        playerCollider.enabled = false;
        playerLight.gameObject.SetActive(false);
        bombText.SetActive(false);

        //Drop gems then respawn
        yield return new WaitForSeconds(0.5f);
        DropGems();
        collectedGems.Clear();
        yield return new WaitForSeconds(respawnDelay);

        //Turn player back on
        transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
        playerMesh.enabled = true;
        playerCollider.enabled = true;
        playerLight.gameObject.SetActive(true);
        bombText.SetActive(true);

    }

    public void DropGems()
    {
        foreach(GameObject gems in collectedGems)
        {
            gems.GetComponent<Collider>().enabled = true;
            gems.transform.position = transform.position;
            gems.transform.rotation = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
            Debug.Log(gems.transform.rotation);
            gems.gameObject.SetActive(true);
            gems.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * scatterForce);
        }
    }
}
