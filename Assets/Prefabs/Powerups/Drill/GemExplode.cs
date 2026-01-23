using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GemExplode : MonoBehaviour
{
    public GameObject gemPrefab;
    private Quaternion randRot;
    public int gemAmt;
    public int scatterForce;

    //prevent exploding into more than one set of gems
    public bool hasExploded = false;


    public GameObject gemSparkle;
    public GemRef gemRef;
    private GameObject[] allGems;

    private void Start()
    {

        randRot = Quaternion.Euler(Random.Range(0, 360), 0, Random.Range(0, 360));
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Drill"))
        {
            ExplodeSequence();
        }

    }

    public void ExplodeSequence()
    {
        if (!hasExploded)
        {
            hasExploded = true;
            SpawnGems();
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_BigGems/GemShattering");
        }
        
    }

    public void SpawnGems()
    {
        //explosion particle
        GameObject particle = Instantiate(gemSparkle, transform.position, Quaternion.identity);

        int i;

        for (i = 0; i < gemAmt; i++)
        {
            
            randRot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            GameObject spawnedGem = Instantiate(gemPrefab, gameObject.transform.position, randRot);

            //force gem to be kinematic so that forces can be applied to it
            spawnedGem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            spawnedGem.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(randRot.x, randRot.y, randRot.z) * scatterForce);

        }

        if(i == gemAmt)
        {
            Destroy(gameObject);
        }
    }
}
