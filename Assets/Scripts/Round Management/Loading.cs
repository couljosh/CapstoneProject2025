using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public float loadingTime;

    void Start()
    {
        Invoke("LoadNextScene", loadingTime);
    }

    void Update()
    {
        
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
