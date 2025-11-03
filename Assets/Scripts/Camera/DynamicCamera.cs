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
    private float timer;
    public float maxLerpTime;
    public float zoomLimit;
    private float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set defaults
        defaultCameraSize = Camera.main.orthographicSize;
        defaultCameraPosition = gameObject.transform.position;

        //calculate offset because camera is tilted
        
    }

    // Update is called once per frame
    void Update()
    {
        if(players.Count == 0)
        {
            //this  needs to be an array so we have to convert it
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
                activeRepo = r;
                //print("Active Repository Found");
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
            //print("Attempting to focus in on active repository");
            
            //Vector3 destinationPosition;
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
            //print("Zooming back out");
            gameObject.transform.position = defaultCameraPosition;
            boundsCalculation.center = Vector3.zero;

            //preset, make sure to change this
            Camera.main.orthographicSize = defaultCameraSize;

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

        if(currentlyFocused)
        {
            timer += Time.deltaTime;
            //Mathf.Lerp(defaultCameraSize, boundsCalculation.size, timer)
            Vector3 boundSize = boundsCalculation.size;
            float destinationSize = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));

            print(destinationSize);

            float currentSize = Mathf.Lerp(defaultCameraSize, destinationSize, timer/maxLerpTime);
            currentSize = Mathf.Clamp(currentSize, zoomLimit, Mathf.Infinity);
            Camera.main.orthographicSize = currentSize;

            RaycastHit floor;
            if (Physics.Raycast(gameObject.transform.position, transform.forward, out floor, Mathf.Infinity))
            {
                offset = Vector3.Distance(floor.point, new Vector3(gameObject.transform.position.x, floor.point.y, gameObject.transform.position.z));
                print(offset);
            }

            transform.position = new Vector3(activeRepo.transform.position.x, transform.position.y, activeRepo.transform.position.z - offset);
        }
        else
        {
            timer = 0;
        }
    }
}
