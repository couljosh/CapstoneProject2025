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
    public MeshRenderer gemMesh;
    public Material outOfRadiusMaterial;
    public Material inRadiusMaterial;

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


        //shining if in radius

        if (isInDepositRadius)
        {
            //not currently functioning
            gemMesh.material = inRadiusMaterial;
        }
        else if (!isInDepositRadius)
        {
            //not currently functioning
            gemMesh.material = outOfRadiusMaterial;
        }
    }




    //Release Gem
    public void ReleaseGem()
    {
        rb.isKinematic = false;
        releasedTime = Time.time;
    }

    

}
