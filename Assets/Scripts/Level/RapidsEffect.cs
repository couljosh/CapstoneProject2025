using UnityEditor;
using UnityEngine;

public class RapidsEffect : MonoBehaviour
{
    public float rapidForce;
    public float gemBob;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
        if(other.GetComponent<Rigidbody>() != null)
        {
             other.GetComponent<Rigidbody>().AddForce(transform.forward * rapidForce, ForceMode.VelocityChange);

        }

        if (other.gameObject.tag == "Gem")
        {
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * gemBob, ForceMode.VelocityChange);
        }
        
    }





    
}
