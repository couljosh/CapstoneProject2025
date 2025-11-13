using UnityEngine;

public class BagGrowWithGemsHeld : MonoBehaviour
{
    [Header("Stored References")]
    private PlayerDeath playerDeath; //script where gems are kept track of

    [Header("Bag Customization")]
    public float sizeChangePerGem;
    public int allowedSizeChanges;
    private int sizeChanges;
    private Vector3 startingSize;
    private Vector3 startingPosition;
    private float gemsLastFrame;


    void Start()
    {
        playerDeath = gameObject.GetComponentInParent<PlayerDeath>();

        startingPosition = gameObject.transform.localPosition;
        startingSize = gameObject.transform.localScale;
    }


    void Update()
    {
        //if (sizeChanges <= allowedSizeChanges)
        //{
        //    if (playerDeath.gemCount > 0 && playerDeath.gemCount != gemsLastFrame) //gems are more than zero, and the gem count has changed since last frame
        //    {
        //        //change size based on gem count
        //        sizeChanges++;
        //        gameObject.transform.localScale += new Vector3(playerDeath.gemCount * sizeChangePerGem, playerDeath.gemCount * sizeChangePerGem, playerDeath.gemCount * sizeChangePerGem);
        //    }
                

        //    //ensure bag stays on back of player
        //    if (playerDeath.gemCount > 0 && playerDeath.gemCount != gemsLastFrame) //gems are more than zero, and the gem count has changed since last frame
        //    {
        //        //scale transform to stay on the back of the player
        //        sizeChanges++;
        //        gameObject.transform.localPosition -= new Vector3(0, 0, (playerDeath.gemCount * sizeChangePerGem) / 2);
        //    }
        //}     
            
        //if (playerDeath.gemCount == 0)
        //{
        //    gameObject.transform.localScale = startingSize;
        //    gameObject.transform.localPosition = startingPosition;
        //    sizeChanges = 0;
        //}
            


        //disable bag sprite if dead
        if (playerDeath.isPlayerDead == true)
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        
        else
            gameObject.GetComponent<MeshRenderer>().enabled = true;

        //save how many gems the player had this frame
        //gemsLastFrame = playerDeath.gemCount;
    }

    public void changeBagSize()
    {
        if (sizeChanges <= allowedSizeChanges)
        {
            print("size changed");
            sizeChanges++;
            gameObject.transform.localScale = new Vector3(playerDeath.gemCount * sizeChangePerGem, playerDeath.gemCount * sizeChangePerGem, playerDeath.gemCount * sizeChangePerGem);
            gameObject.transform.localPosition -= new Vector3(0, 0, (playerDeath.gemCount * sizeChangePerGem) / 2);
        }
    }

    public void resetBagSize()
    {
        gameObject.transform.localScale = startingSize;
        gameObject.transform.localPosition = startingPosition;
        sizeChanges = 0;
    }
}


