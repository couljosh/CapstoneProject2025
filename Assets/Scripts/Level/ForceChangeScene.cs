using UnityEngine;

public class ForceChangeScene : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gameObject.GetComponent<SceneChange>().checkScore();
        }
    }
}
