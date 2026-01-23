using UnityEngine;
using UnityEngine.TerrainUtils;

public class GroundDetection : MonoBehaviour
{
    public LayerMask groundMask;
    public float rayLength;
    public JackHammerLogic jackHammerLogicScript;

    public bool hasEmerged = false;

    void Start()
    {
        
    }

    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, rayLength, groundMask, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(transform.position, new Vector3(0, -rayLength, 0), Color.green);

        if (hits.Length == 0 && !hasEmerged)
        {
            jackHammerLogicScript.Emerge(new Vector3(0, -3, 0));
            hasEmerged = true;
        }

        print(hits.Length);
    }
}
