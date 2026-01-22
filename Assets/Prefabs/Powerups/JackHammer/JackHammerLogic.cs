using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JackHammerLogic : MonoBehaviour
{
    public PlayerMove playerMoveScript;

    public CapsuleCollider playerCollider;
    public BoxCollider burrowCollider;
    public List<GameObject> playerModel = new List<GameObject>();

    public float startElapsedTime;
    public float startDelay;
    public float emergeTimeElapsed;
    public float emergeDelay;
    public bool isBurrowed = false;
    public bool isMoving;

    public float moveSpeedMultiplier;

    private MeshRenderer dirtModel;


    public Image emergeProgress;

    public float timeUntilEmergeIsAllowed;
    public float elapsedTimeBurrowed;
    public bool emergeDelayPassed = false;

    public GameObject underBedrockWarning;


    public bool underBedrock;

    void Start()
    {
        underBedrockWarning.SetActive(false);
        emergeProgress.fillAmount = 0;

        playerMoveScript = GetComponentInParent<PlayerMove>();

        dirtModel = gameObject.GetComponent<MeshRenderer>();
        dirtModel.enabled = false; 

        Transform playerParent = this.transform.parent;

        playerCollider = gameObject.transform.parent.GetComponent<CapsuleCollider>();
        burrowCollider = gameObject.transform.parent.GetComponent<BoxCollider>();

        playerModel.Add(playerParent.GetComponentInChildren<BagSize>().gameObject); //gembag
        playerModel.Add(playerParent.GetComponentInChildren<SkinnedMeshRenderer>().gameObject); //model

    }

    void Update()
    {
        //Burrow Startup
        if (startElapsedTime < startDelay)
        {
            startElapsedTime += Time.deltaTime;

        }
        else //Triggering & During Burrow
        {
            if (!isBurrowed)
            {
                Burrow();
            }

           // This delay was added to avoid players emerging right after burrowing because of how quick the emerge happen
            elapsedTimeBurrowed += Time.deltaTime;
            if(elapsedTimeBurrowed > timeUntilEmergeIsAllowed)
            {            
                emergeDelayPassed = true;
            }
        }

        //Burrow Emerge Check
        if (emergeDelayPassed)
        {
            emergeProgress.fillAmount = emergeTimeElapsed / emergeDelay;

            if (isMoving || underBedrock)
            {
                emergeTimeElapsed = 0;
            }
            else
            {
                emergeTimeElapsed += Time.deltaTime;
            }

            if (emergeTimeElapsed > emergeDelay)
            {
                Emerge();
            }
        }
    }

    void Burrow()
    {

        dirtModel.enabled = true;

        foreach (GameObject piece in playerModel)
        {
            piece.SetActive(false);
        }

        playerCollider.enabled = false;
        burrowCollider.enabled = true;
        isBurrowed = true;

        //turn on special collider for burrow mode
        
    }

    void Emerge()
    {
        foreach (GameObject piece in playerModel)
        {
            piece.SetActive(true);
        }

        burrowCollider.enabled = false;
        playerCollider.enabled = true;
        dirtModel.enabled = false;

        StartCoroutine(destroyJackHammerAfterDelay(0.4f));


    }

    //if other scripts need to destroy the jackhammer (coroutines cannot be called from other scripts)
    public void destroyJackHammerFromOtherScript(float waitTime)
    {
        StartCoroutine(destroyJackHammerAfterDelay(waitTime));
    }

    public IEnumerator destroyJackHammerAfterDelay(float waitTime)
    {
        //do not move player until the delay ends
        //playerMoveScript.canAct = false;

        //yield return new WaitForSeconds(waitTime);

        //re-enable player movement when they're allowed to move again
        //playerMoveScript.canAct = true;

        //reset to basic movement
        playerMoveScript.powerUpPickupScript.activePowerup = null;

        CaveInManager.isPowerupInPlay = false;
        //engineStartInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);                        //Re-add with jackhamemr sfx
        //drillLogicScript.drillInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //destroy powerup object, so that you respawn normally
        Destroy(gameObject.gameObject);

        yield return null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Bedrock")
        {
            underBedrock = true;
            underBedrockWarning.SetActive(true);
        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bedrock")
        {
            underBedrock = false;
            underBedrockWarning.SetActive(false);
        }
    }

}
