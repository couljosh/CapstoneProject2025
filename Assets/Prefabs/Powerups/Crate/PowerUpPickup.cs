using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public GameObject activePowerup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crate" && activePowerup == null)
        {
           CrateLogic crateData = other.gameObject.GetComponent<CrateLogic>();
           activePowerup = Instantiate(crateData.chosenPowerup, this.transform);
           
           Destroy(other.gameObject);


        }
    }
}
