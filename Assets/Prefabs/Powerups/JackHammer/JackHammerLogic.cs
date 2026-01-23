using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class JackHammerLogic : MonoBehaviour
{
    [Header("References")]
    public PlayerMove playerMoveScript;
    public CapsuleCollider playerCollider;
    public BoxCollider burrowCollider;
    public GameObject dirtModel;
    public Image emergeProgress;
    public GameObject underBedrockWarning;
    private List<GameObject> playerModel = new List<GameObject>();

    [Header("Burrow Values")]
    public float moveSpeedMultiplier;

    [Header("Start Burrow")]
    public float startDelay;
    public bool isBurrowed = false;
    private float startElapsedTime;

    [Header("Start Emerge")]
    public float emergeDelay;
    public bool hasEmerged = false;
    private float emergeTimeElapsed;

    [Header("Delay Emerge")]
    public float timeUntilEmergeIsAllowed;
    public bool isMoving;
    public bool emergeDelayPassed = false;
    public bool underBedrock;
    private float elapsedTimeBurrowed;

    void Start()
    {
        playerMoveScript = GetComponentInParent<PlayerMove>();

        //Correct setting on start
        dirtModel.SetActive(false);
        underBedrockWarning.SetActive(false);
        emergeProgress.fillAmount = 0;

        //Collider switch
        playerCollider = gameObject.transform.parent.GetComponent<CapsuleCollider>();
        burrowCollider = gameObject.transform.parent.GetComponent<BoxCollider>();

        //Turn off player elements
        Transform playerParent = this.transform.parent;
        playerModel.Add(playerParent.GetComponentInChildren<BagSize>().gameObject); //gembag
        playerModel.Add(playerParent.GetComponentInChildren<SkinnedMeshRenderer>().gameObject); //model
        playerModel.Add(playerParent.GetComponentInChildren<Light>().gameObject); //spotlight
        playerModel.Add(playerParent.GetComponentInChildren<ObjectID>().gameObject); //pickaxe

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
        foreach (GameObject piece in playerModel)
        {
            piece.SetActive(false);
        }
        
        dirtModel.SetActive(true);
        playerCollider.enabled = false;
        burrowCollider.enabled = true;
        isBurrowed = true;        
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
