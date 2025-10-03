using System.Collections;
using UnityEngine;

public class RepoMover : MonoBehaviour
{
    [Header("Stored Refenences")]
    public GameObject currentRepository;
    public GameObject[] respositories;
    private GameObject newRepository;

    [Header("Repository Customization")]
    public float switchInterval;
    public float switchDelayMin;
    public float switchDelayMax;
    public float elaspedTime;
    public int currentInd;


    void Start()
    {
        respositories = GameObject.FindGameObjectsWithTag("Repository");

        //Turn off all repositories
        foreach (var repo in respositories)
        {
            repo.gameObject.GetComponent<SingleRepo>().active = false;
        }

        //Activate Random Repository
        currentRepository = respositories[Random.Range(0, respositories.Length)];
        currentRepository.GetComponent<SingleRepo>().active = true;
 
    }


    //Active Repository Switch
    void Update()
    {
        elaspedTime += Time.deltaTime;

        if (elaspedTime > switchInterval)
        {
            currentRepository.GetComponent<SingleRepo>().active = false;
            StartCoroutine(SwitchRepo());
            elaspedTime = 0;

        }
    }

    public IEnumerator SwitchRepo()
    {
        yield return new WaitForSeconds(Random.Range(switchDelayMin, switchDelayMax));
        currentInd = System.Array.IndexOf(respositories, currentRepository);

        if(currentInd < respositories.Length - 1)
        {
            currentRepository = respositories[currentInd + 1];
            currentRepository.GetComponent<SingleRepo>().active = true;
            elaspedTime = 0;

        }
        else
        {
            currentRepository = respositories[0];
            currentRepository.GetComponent<SingleRepo>().active = true;
            elaspedTime = 0;

        }
    }
}
