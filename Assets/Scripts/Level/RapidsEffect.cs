using UnityEditor;
using UnityEngine;

public class RapidsEffect : MonoBehaviour
{
    public float rapidForce;
    //public float gemBob;
    public float gemForce;
    public float largeGemForce;
    public float bombForce;

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
        if (other.GetComponent<Rigidbody>() != null)
        {
            if (other.gameObject.tag == "Gem" )
            {
                //other.GetComponent<Rigidbody>().AddForce(Vector3.up * (gemBob / 10), ForceMode.VelocityChange);
                other.GetComponent<Rigidbody>().AddForce(transform.forward * (gemForce / 1000), ForceMode.VelocityChange);
            }

            else if (other.gameObject.tag == "LargeGem")
            {
                other.GetComponent<Rigidbody>().AddForce(transform.forward * (largeGemForce / 1000), ForceMode.VelocityChange);
            }


            else if (other.gameObject.tag == "Bomb")
            {
                other.GetComponent<Rigidbody>().AddForce(transform.forward * (bombForce / 100), ForceMode.VelocityChange);
            }


            else
            {
                other.GetComponent<Rigidbody>().AddForce(transform.forward * (rapidForce / 100), ForceMode.VelocityChange);

            }
        }

         

        
        
    }





    
}
