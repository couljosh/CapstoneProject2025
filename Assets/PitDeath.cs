using UnityEngine;

public class PitDeath : MonoBehaviour
{


    private void OnTriggerEnter(Collider collision)
    {

        if(collision.gameObject.tag == "ObjectDestroyer")
        {
            collision.gameObject.GetComponent<PlayerDeath>().ForcePlayerDie();
        }
    }
}
