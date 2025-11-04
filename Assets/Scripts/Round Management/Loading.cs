using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public float loadingTime;
    private float elapsedTime;

    public int sceneIdx;


    void Start()
    {
        Timer();
    }

    void Update()
    {
       
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
