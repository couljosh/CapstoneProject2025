using System.Collections;
using UnityEngine;

public class RepoMover : MonoBehaviour
{
    public GameObject currentRepository;
    public GameObject[] respositories;
    
    private GameObject newRepository;

    public float moveInterval = 10;
    private float elaspedTime = 0;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int i = 0;
        foreach (var repo in respositories)
        {
            respositories[i].GetComponent<SingleRepo>().active = false;
            print("light off");
            i++;
        }
        currentRepository = respositories[Random.Range(0, respositories.Length)];
        currentRepository.GetComponent<SingleRepo>().active = true;
    }

    // Update is called once per frame
    void Update()
    {
        

        elaspedTime += Time.deltaTime;

        if (elaspedTime > moveInterval)
        {
            currentRepository.GetComponent<SingleRepo>().active = false;

            newRepository = respositories[Random.Range(0, respositories.Length)];

            while (newRepository == currentRepository)
            {
               newRepository = respositories[Random.Range(0, respositories.Length)];
            }

            currentRepository = newRepository;
            currentRepository.GetComponent<SingleRepo>().active = true;

            elaspedTime = 0;
        }



    }

 }
