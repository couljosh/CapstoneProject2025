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
    private float currentZoom; //before zooming out

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set defaults
        defaultCameraSize = Camera.main.orthographicSize;
        defaultCameraPosition = gameObject.transform.position;
        ZoomOutTimer = maxLerpTime; //so it starts already done in the lerp
    }

    // Update is called once per frame
    void Update()
    {
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
                if (Vector3.Distance(activeRepo.transform.position, p.transform.position) > repoFocusDistance)
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
                currentZoom = Camera.main.orthographicSize;
                currentRepoFocus = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            }

            ZoomOutTimer += Time.deltaTime;

            //lerping process
            if(ZoomOutTimer <= maxLerpTime)
            {
                Vector3 currentPosition = Vector3.Lerp(currentRepoFocus, defaultCameraPosition, ZoomOutTimer / maxLerpTime); //position in space
                float currentSize = Mathf.Lerp(currentZoom, defaultCameraSize, ZoomOutTimer / maxLerpTime); //camera zoom
                gameObject.transform.position = currentPosition;
                boundsCalculation.center = Vector3.zero;
                Camera.main.orthographicSize = currentSize;
            }
            else
            {
                gameObject.transform.position = defaultCameraPosition;
            }
            

        }



        //foreach (var r in repositories)
        //{
        //    if(Vector3.Distance(r.transform.position, gameObject.transform.position) <=  repoFocusDistance)
        //    {
        //        //force it to focus on the repository
        //        transform.position = new Vector3(r.transform.position.x, transform.position.y, r.transform.position.z);

        //        boundsCalculation.size = Vector3.zero;
        //        boundsCalculation.Encapsulate(r.transform.position);

        //        break;
        //    }
        //    else
        //    {
        //        transform.position = new Vector3(boundsCalculation.center.x, transform.position.y, boundsCalculation.center.z);
        //    }
        //}

        //zoom into a repository
        if(currentlyFocused)
        {
            ZoomInTimer += Time.deltaTime;

            //calculate the destination size to zoom into
            Vector3 boundSize = boundsCalculation.size;
            float destinationSize = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));

            //handle the lerping between current and destination
            float currentSize = Mathf.Lerp(defaultCameraSize, destinationSize, ZoomInTimer/maxLerpTime);
            currentSize = Mathf.Clamp(currentSize, zoomLimit, Mathf.Infinity);
            Camera.main.orthographicSize = currentSize;

            //calculate offset between where the camera is looking to center the repository on the screen
            RaycastHit floor;
            if (Physics.Raycast(gameObject.transform.position, transform.forward, out floor, Mathf.Infinity))
                offset = Vector3.Distance(floor.point, new Vector3(gameObject.transform.position.x, floor.point.y, gameObject.transform.position.z));
            
            Vector3 targetPosition = new Vector3(activeRepo.transform.position.x, transform.position.y, activeRepo.transform.position.z - offset);
            Vector3 currentPosition = Vector3.Lerp(defaultCameraPosition, targetPosition, ZoomInTimer/maxLerpTime);
            transform.position = currentPosition;
        }
        else
        {
            ZoomInTimer = 0;
        }
    }
}
