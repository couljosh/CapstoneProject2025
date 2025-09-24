using UnityEngine;

public class BagGrowWithGemsHeld : MonoBehaviour
{
    private PlayerDeath playerDeath; //script where gems are kept track of
    public float percentageChangePerGem;
    private float gemsLastFrame;
    private Vector3 startingSize;
    private Vector3 startingPosition;
    //public float growthSizeMaxPercent; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerDeath = gameObject.GetComponentInParent<PlayerDeath>();

        startingPosition = gameObject.transform.localPosition;
        startingSize = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        print(startingPosition.z * (1 + (playerDeath.gemCount * (percentageChangePerGem / 100) / 2)));

        if (playerDeath.gemCount > 0 && playerDeath.gemCount != gemsLastFrame) //gems are more than zero, and the gem count has changed since last frame
            //change size based on gem count
            gameObject.transform.localScale = new Vector3(startingSize.x * (1 + (playerDeath.gemCount * percentageChangePerGem / 100)), startingSize.y * (1 + (playerDeath.gemCount * percentageChangePerGem / 100)), startingSize.z * (1 + (playerDeath.gemCount * percentageChangePerGem / 100)));

        else if (playerDeath.gemCount == 0)
            gameObject.transform.localScale = startingSize;

        //ensure bag stays on back of player
        if (playerDeath.gemCount > 0 && playerDeath.gemCount != gemsLastFrame) //gems are more than zero, and the gem count has changed since last frame
            //scale transform to stay on the back of the player
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, startingPosition.z * (1 + (playerDeath.gemCount * (percentageChangePerGem / 100)/2)));

        else if(playerDeath.gemCount == 0)
            gameObject.transform.localPosition = startingPosition;


        //disable bag sprite if dead
        if (playerDeath.isPlayerDead == true)
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        
        else
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        

        //save how many gems the player had this frame
        gemsLastFrame = playerDeath.gemCount;
    }
}
