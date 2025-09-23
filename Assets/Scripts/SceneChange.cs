using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public Scene scene;
    public int sceneNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //change this to when the game is finished.
        {
            print("new scene");
            SceneManager.LoadScene(sceneNumber);
        }
    }



}
