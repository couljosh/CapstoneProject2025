using UnityEngine;

public class PitDeath : MonoBehaviour
{


    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.tag == "ObjectDestroyer")
        {
            collision.gameObject.GetComponent<PlayerDeath>().ForcePlayerDie();
        }
    }
}
