using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeath : MonoBehaviour
{

    public float respawnDelay;
    public BombExplode bombExplosionScript;
    public int spawnNum;

    private void Update()
    {

    }

    public IEnumerator PlayerDie()
    {
        gameObject.SetActive(false);
        transform.position = GameObject.Find("Spawn" + spawnNum).transform.position;
        yield return new WaitForSeconds(respawnDelay);
        gameObject.SetActive(true);
    }

}
