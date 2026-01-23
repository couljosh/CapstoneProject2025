using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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

    public GameObject dirtModel;

    public Image emergeProgress;

    public float timeUntilEmergeIsAllowed;
    public float elapsedTimeBurrowed;
    public bool emergeDelayPassed = false;
    public bool hasEmerged = false;

    public GameObject underBedrockWarning;


    public bool underBedrock;

    void Start()
    {
        underBedrockWarning.SetActive(false);
        emergeProgress.fillAmount = 0;

        playerMoveScript = GetComponentInParent<PlayerMove>();

        dirtModel.SetActive(false); 

        Transform playerParent = this.transform.parent;

        playerCollider = gameObject.transform.parent.GetComponent<CapsuleCollider>();
        burrowCollider = gameObject.transform.parent.GetComponent<BoxCollider>();


        playerModel.Add(playerParent.GetComponentInChildren<BagSize>().gameObject); //gembag
        playerModel.Add(playerParent.GetComponentInChildren<SkinnedMeshRenderer>().gameObject); //model
        playerModel.Add(playerParent.GetComponentInChildren<Light>().gameObject); //spotlight

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
        dirtModel.SetActive(true);
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
        hasEmerged = true;

        foreach (GameObject piece in playerModel)
        {
            piece.SetActive(true);
        }

        burrowCollider.enabled = false;
        playerCollider.enabled = true;
        dirtModel.SetActive(false);
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
