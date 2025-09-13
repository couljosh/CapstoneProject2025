using UnityEngine;

public class PreventChildRotation : MonoBehaviour
{
    public Vector3 rotationToMaintain;

    void Update()
    {
        
        this.transform.rotation = Quaternion.Euler(rotationToMaintain);
    }
}
