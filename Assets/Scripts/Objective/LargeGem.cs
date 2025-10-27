using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class LargeGem : MonoBehaviour
{
    public bool isReleased;
    public Rigidbody rb;
    public float radius;
    public LayerMask terrainMask;
    private float releasedTime;
    private Animator flashAnim;

    public bool isInDepositRadius;

    private void Start()
    {
        flashAnim = GetComponent<Animator>();
    }

    void Update()
    {
        //Release Gem on Check
        if (!isReleased)
        {
            Collider[] hitblocks = Physics.OverlapSphere(transform.position, radius, terrainMask, QueryTriggerInteraction.Ignore);

            if (hitblocks.Length < 7)
            {
                ReleaseGem();
                isReleased = true;
            }
        }

        if(isInDepositRadius)
        {
            print("reached in");
            gameObject.GetComponent<Animator>().enabled = true;
        }
        else if (!isInDepositRadius)
        {
            print("reached out");
            gameObject.GetComponent<Animator>().playbackTime = 0;
            gameObject.GetComponent<Animator>().enabled = false;
        }
    }




    //Release Gem
    public void ReleaseGem()
    {
        rb.isKinematic = false;
        releasedTime = Time.time;
    }

    

}
