using UnityEngine;

public class GemExplode : MonoBehaviour
{
    public GameObject gemPrefab;
    private Quaternion randRot;
    public int gemAmt;
    public int scatterForce;

    private GameObject[] allGems;

    private void Start()
    {

        randRot = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Drill")
        {
            SpawnGems();
        }

    }

    void SpawnGems()
    {
        int i;

        for (i = 0; i < gemAmt; i++)
        {
            randRot = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
            GameObject spawnedGem = Instantiate(gemPrefab, gameObject.transform.position, randRot);
            spawnedGem.gameObject.GetComponent<Rigidbody>().AddForce(spawnedGem.transform.forward * scatterForce);
        }

        if(i == gemAmt)
        {
            Destroy(gameObject);
        }
    }
}
