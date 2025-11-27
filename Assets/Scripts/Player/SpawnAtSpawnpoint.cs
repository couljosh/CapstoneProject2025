using UnityEngine;

public class SpawnAtSpawnpoint : MonoBehaviour
{

    private int hasBeenMoved = 0;
    private PlayerDeath playerDeath;

    private void Start()
    {
        playerDeath = gameObject.GetComponent<PlayerDeath>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //try for x amount of frames
        if (hasBeenMoved < 15)
        {
            hasBeenMoved++;
            gameObject.transform.position = GameObject.Find("Spawn" + playerDeath.spawnNum).transform.position;
        }
    }
}
