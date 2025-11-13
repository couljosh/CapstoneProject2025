using UnityEngine;

public class RepoLight : MonoBehaviour
{
    public float rotSpeed;

    private void Start()
    {
    }

    void Update()
    {
        transform.Rotate(0, rotSpeed, 0);
    }
}
