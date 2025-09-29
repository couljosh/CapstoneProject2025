using System.Collections;
using UnityEngine;

public class RepoMover : MonoBehaviour
{
    [Header("Stored Refenences")]
    public GameObject currentRepository;
    public GameObject[] respositories;
    private GameObject newRepository;

    [Header("Repository Customization")]
    public float moveInterval;
    private float elaspedTime;


    //Active Repository on Start
    void Start()
    {
        int i = 0;
        foreach (var repo in respositories)
        {
            respositories[i].GetComponent<SingleRepo>().active = false;
            i++;
        }
        currentRepository = respositories[Random.Range(0, respositories.Length)];
        currentRepository.GetComponent<SingleRepo>().active = true;
    }


    //Active Repository Switch
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
