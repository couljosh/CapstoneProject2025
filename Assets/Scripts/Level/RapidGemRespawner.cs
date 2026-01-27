using UnityEngine;

public class RapidGemRespawner : MonoBehaviour
{
    public Transform startPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            if (other.gameObject.tag == "Gem" || other.gameObject.tag == "LargeGem" || other.gameObject.tag == "Bombw")
            {
                other.gameObject.transform.position = startPos.position;
            }
        }
    }
}
