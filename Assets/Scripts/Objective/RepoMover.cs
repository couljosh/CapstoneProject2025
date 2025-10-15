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
            repo.gameObject.GetComponent<RepositoryLogic>().active = false;
        }

        //Activate Random Repository
        currentRepository = respositories[Random.Range(0, respositories.Length)];
        currentRepository.GetComponent<RepositoryLogic>().active = true;
 
    }


    //Active Repository Switch
    void Update()
    {
        elaspedTime += Time.deltaTime;

        if (elaspedTime > switchInterval)
        {
            currentRepository.GetComponent<RepositoryLogic>().active = false;
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
            currentRepository.GetComponent<RepositoryLogic>().active = true;
            elaspedTime = 0;

        }
        else
        {
            currentRepository = respositories[0];
            currentRepository.GetComponent<RepositoryLogic>().active = true;
            elaspedTime = 0;

        }
    }
}
