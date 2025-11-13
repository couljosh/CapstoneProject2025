using UnityEngine;

public class LockUIScript : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2f, 0); // how high above player

    void LateUpdate()
    {
        if (!player) return;

        //keep above the player
        transform.position = player.position + offset;

        //facing upward test
        //transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
