using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeBeforeDestroy;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeBeforeDestroy )
        {
            Destroy(this);
        }
    }
}
