using UnityEngine;

public class BagGrowWithGemsHeld : MonoBehaviour
{
    private PlayerDeath playerDeath; //script where gems are kept track of
    public float sizeChangePerGem;
    private float gemsLastFrame;
    public int allowedSizeChanges;
    private int sizeChanges;
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
        if (sizeChanges < allowedSizeChanges)
        {
            if (playerDeath.gemCount > 0 && playerDeath.gemCount != gemsLastFrame) //gems are more than zero, and the gem count has changed since last frame
            {
                //change size based on gem count
                sizeChanges++;
                gameObject.transform.localScale += new Vector3(playerDeath.gemCount * sizeChangePerGem, playerDeath.gemCount * sizeChangePerGem, playerDeath.gemCount * sizeChangePerGem);
            }
            else if (playerDeath.gemCount == 0)
                gameObject.transform.localScale = startingSize;

            //ensure bag stays on back of player
            if (playerDeath.gemCount > 0 && playerDeath.gemCount != gemsLastFrame) //gems are more than zero, and the gem count has changed since last frame
            {
                //scale transform to stay on the back of the player
                sizeChanges++;
                gameObject.transform.localPosition -= new Vector3(0, 0, (playerDeath.gemCount * sizeChangePerGem) / 2);
            }
        }
            
            

        else if (playerDeath.gemCount == 0)
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
