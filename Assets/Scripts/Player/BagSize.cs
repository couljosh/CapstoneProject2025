using UnityEngine;

public class BagSize : MonoBehaviour
{
    private Vector3 originalSize;
    private Vector3 originalPosition;

    private PlayerDeath playerDeath;

    public float sizeChangePerGem;
    public float sizeChangesAllowed;
    private int gemsHeldLastFrame;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //save the original size the bag
        originalPosition = transform.localPosition;
        originalSize = transform.localScale;

        playerDeath = gameObject.GetComponentInParent<PlayerDeath>();
    }

    private void Update()
    {

        //if gem count has changed
        if (playerDeath.gemCount != gemsHeldLastFrame)
        {
            //increase in size for each gem collected
            for (int i = 0; i <= playerDeath.gemCount; i++)
            {
                //break early if bag is at max size
                if (playerDeath.collectedGems.Count > sizeChangesAllowed)
                {
                    break;
                }


                //start at zero gem size
                transform.localPosition = originalPosition;
                transform.localScale = originalSize;

                //scale bag and adjust position to stay on back
                transform.localScale += new Vector3(sizeChangePerGem, sizeChangePerGem, sizeChangePerGem) * i;
                transform.localPosition -= new Vector3(0, 0, (playerDeath.gemCount * sizeChangePerGem) / 2);


            }
        }

        //update gem count
        gemsHeldLastFrame = playerDeath.gemCount;
    }
}

        


//    public void changeBagSize()
//    {
        
//        //increase in size for each gem collected
//        for (int i = 0; i <= playerDeath.gemCount; i++)
//        {
//            //break early if bag is at max size
//            if (playerDeath.collectedGems.Count > sizeChangesAllowed)
//            {
//                break;
//            }
                

//            //start at zero gem size
//            transform.localPosition = originalPosition;
//            transform.localScale = originalSize;

//            //scale bag and adjust position to stay on back
//            transform.localScale += new Vector3(sizeChangePerGem, sizeChangePerGem, sizeChangePerGem) * i;
//            transform.localPosition -= new Vector3(0, 0, (playerDeath.gemCount * sizeChangePerGem) / 2);


//        }
//    }
//}
