using System.Collections;
using TMPro;
using UnityEngine;

public class LavaSystem : MonoBehaviour
{
    [SerializeField] private float lavaRisingDuration;
    [SerializeField] public Transform lavaHeight;
    [SerializeField] private float lavaLinger;
    [SerializeField] private float lavaTimer;
    private float elapsedTime;
    private Vector3 newLavaLocation;
    private Vector3 startingLavaLocation;
    //private bool doneMoving = false;
    private bool goingUp = true;
    private float timer;
    private bool laveSafe;
    public float lavaSafeLength;
    //private float coyoteTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingLavaLocation = transform.position;

        newLavaLocation = new Vector3(transform.position.x, lavaHeight.position.y, transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        

        if (timer > lavaTimer)
        {
            if (goingUp)
            {
                LavaMove(startingLavaLocation, newLavaLocation);
            }
            else 
            {
                LavaMove(newLavaLocation, startingLavaLocation);
            }
        }
        
        if (elapsedTime / lavaRisingDuration > 1) //Finished Rising
        {
            goingUp = !goingUp;
            elapsedTime = 0;
            timer = 0;
        }
    }

    private void LavaMove(Vector3 startingLocation, Vector3 newLocation)
    {
        
        //doneMoving = false;
        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(startingLocation, newLocation, elapsedTime / lavaRisingDuration);
       // doneMoving = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ObjectDestroyer") //&& elapsedTime / lavaRisingDuration > lavaSafeLength)
        {
            PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();
            playerDeath.timeTouchingLava += Time.deltaTime;

            if(playerDeath.timeTouchingLava > lavaSafeLength)
            {
                playerDeath.timeTouchingLava = 0;
                playerDeath.PlayerDie();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ObjectDestroyer")
        {
            PlayerDeath playerDeath = other.gameObject.GetComponent<PlayerDeath>();
            playerDeath.timeTouchingLava = 0;
        }
    }
}
