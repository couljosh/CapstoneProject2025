using UnityEngine;

public class RemoveTerrain : MonoBehaviour
{

    //Destory Terrain in bedrock (avoids gems getting stuck)
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "ActiveTerrain")
        {
            Destroy(collision.gameObject);
        }

    }

}
