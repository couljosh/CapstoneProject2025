using UnityEngine;

public class Spin : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.Rotate(0, 1, 0);
    }
}
