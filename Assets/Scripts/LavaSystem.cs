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

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "ObjectDestroyer") //&& elapsedTime / lavaRisingDuration > lavaSafeLength)
        {
            
            other.gameObject.GetComponent<PlayerDeath>().PlayerDie();
        }
    }

}
