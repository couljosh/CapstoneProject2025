using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public float loadingTime;
    private float elapsedTime;

    public int sceneIdx;


    void Start()
    {
        
    }

    void Update()
    {
        Timer();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Timer()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime > loadingTime)
        {
            LoadNextScene();
        }
    }
}
