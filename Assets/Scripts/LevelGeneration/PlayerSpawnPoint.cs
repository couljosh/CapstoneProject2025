using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public int spawnNum;

    //Finds Spawn for Player
    void Start()
    {
        transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
    }

}
