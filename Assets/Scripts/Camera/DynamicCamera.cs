using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class DynamicCamera : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> repositories = new List<GameObject>();
    public GameObject activeRepo;

    private Bounds boundsCalculation;
    public float repoFocusDistance;
    private bool currentlyFocused = false;
    
    private float defaultCameraSize;
    private Vector3 defaultCameraPosition;
    [HideInInspector] public float ZoomInTimer;
    [HideInInspector] public float ZoomOutTimer;
    public float maxLerpTime;
    public float zoomLimit;
    private float offset;
    private Vector3 currentRepoFocus;
    private float currentZoomOut; //before zooming out
    float currentSize; //camera size for zooming in
    private Vector3 currentZoomInPosition; //actively setting starting point for zooming into a repo. Makes it seamless instead of starting at a fixed point every time

    private float bufferTimer = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set defaults
        defaultCameraSize = Camera.main.orthographicSize;
        currentSize = Camera.main.orthographicSize;
        defaultCameraPosition = gameObject.transform.position;
        ZoomOutTimer = maxLerpTime; //so it starts already done in the lerp
    }

    // Update is called once per frame
    void Update()
    {
        bufferTimer += Time.deltaTime;

        if(players.Count == 0)
        {
            //this needs to be an array so we have to convert it
            var playersArray = GameObject.FindGameObjectsWithTag("ObjectDestroyer");
            foreach (var p in playersArray)
            {
                players.Add(p);
            }

            var repositoriesArray = GameObject.FindGameObjectsWithTag("Repository");
            foreach (var r in repositoriesArray)
            {
                repositories.Add(r);
            }
        }


        //find active repository, if any
        foreach (var r in repositories)
        {
            if (r.GetComponent<RepositoryLogic>().active)
            {
                //this one is active, set and stop search early
                activeRepo = r;
                currentRepoFocus = new Vector3(activeRepo.transform.position.x, transform.position.y, activeRepo.transform.position.z - offset);
                break;
            }
            else
            {
                activeRepo = null;
                currentlyFocused = false;
            }
        }

        //check if all players are close to an active repository
        if(activeRepo != null)
        {
            foreach (var p in players)
            {
                //if any are out of range
                if (Vector3.Distance(activeRepo.transform.position, p.transform.position) > repoFocusDistance * 2 && ZoomInTimer == 0)
                {
                   // print("At least one player is too far from the repository");
                    currentlyFocused = false;

                    //register which repo to back out of (for zooming)
                    currentRepoFocus = new Vector3(activeRepo.transform.position.x, transform.position.y, activeRepo.transform.position.z - offset);

                    break;
                }
                else
                {
                    currentlyFocused = true;
                }
            }
        }

        //if this is true, it means that all players are in range of an active repo
        if (currentlyFocused)
        {
            //zoom into the repo
            ZoomOutTimer = 0;
            boundsCalculation.size = Vector3.zero;
            boundsCalculation.center = activeRepo.transform.position;
            boundsCalculation.Encapsulate(activeRepo.transform.position);

            //make sure to include all players
            foreach (var p in players)
            {
                boundsCalculation.Encapsulate(p.transform.position);
            }

        }
        else
        {
            //zoom back out and reset to whole map
            if (ZoomOutTimer <= 0) //first frame of zooming out
            {
                print("leaving");
                currentZoomOut = Camera.main.orthographicSize;
                currentRepoFocus = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            }

            ZoomOutTimer += Time.deltaTime;

            //lerping process
            if(ZoomOutTimer <= maxLerpTime)
            {
                Vector3 currentPosition = Vector3.Slerp(currentRepoFocus, defaultCameraPosition, Mathf.SmoothStep(0, maxLerpTime, ZoomOutTimer/maxLerpTime)); //position in space
                float currentSize = Mathf.Lerp(currentZoomOut, defaultCameraSize, Mathf.SmoothStep(0, maxLerpTime, ZoomOutTimer/maxLerpTime)); //camera zoom
                gameObject.transform.position = currentPosition;
                boundsCalculation.center = Vector3.zero;
                Camera.main.orthographicSize = currentSize;
            }
            else
            {
                //gameObject.transform.position = defaultCameraPosition;
            }
            

        }

        //zoom into a repository
        if(currentlyFocused)
        {

            if (ZoomInTimer <= 0)
            {
                currentZoomInPosition = gameObject.transform.position;
                currentSize = Camera.main.orthographicSize;
            }

            ZoomInTimer += Time.deltaTime;

            //calculate the destination size to zoom into
            Vector3 boundSize = boundsCalculation.size;
            float destinationSize = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));

            //handle the lerping between current and destination
            float currentSize2 = Mathf.Lerp(currentSize, destinationSize, Mathf.SmoothStep(0, maxLerpTime, ZoomInTimer / maxLerpTime));
            currentSize2 = Mathf.Clamp(currentSize, zoomLimit, Mathf.Infinity);
            Camera.main.orthographicSize = currentSize2;

            //calculate offset between where the camera is looking to center the repository on the screen
            RaycastHit floor;
            if (Physics.Raycast(gameObject.transform.position, transform.forward, out floor, Mathf.Infinity))
                offset = Vector3.Distance(floor.point, new Vector3(gameObject.transform.position.x, floor.point.y, gameObject.transform.position.z));
            
            Vector3 targetPosition = new Vector3(activeRepo.transform.position.x, transform.position.y, activeRepo.transform.position.z - offset);
            Vector3 currentPosition = Vector3.Slerp(currentZoomInPosition, targetPosition, Mathf.SmoothStep(0, maxLerpTime, ZoomInTimer/maxLerpTime));
            transform.position = currentPosition;

            if (ZoomInTimer > maxLerpTime)
                ZoomInTimer = 0;
        }
        else
        {
            ZoomInTimer = 0;
        }
    }
}
