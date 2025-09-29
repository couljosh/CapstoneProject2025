using Unity.VisualScripting;
using UnityEngine;

public class FollowAbovePlayer : MonoBehaviour
{
    public int PlayerNumToFollow;
    private GameObject objectToFollow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectToFollow = GameObject.Find("Player" + PlayerNumToFollow);
    }

    // Update is called once per frame
    void Update()
    {
        if (objectToFollow != null)
        gameObject.transform.position = new Vector3(objectToFollow.transform.position.x, gameObject.transform.position.y, objectToFollow.transform.position.z);
    }
}
