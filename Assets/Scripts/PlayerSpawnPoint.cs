using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public int spawnNum;

    void Start()
    {
        transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
    }

}
