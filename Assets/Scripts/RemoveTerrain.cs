using UnityEngine;

public class RemoveTerrain : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Terrain")
        {
            Destroy(collision.gameObject);
        }

    }

}
