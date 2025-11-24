using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public float loadingTime;

    public TextMeshProUGUI countdownText;

    private int currentScene;
    private int endMatchScene;

    public int minInd;
    public int maxInd;

    void Start()
    {

        currentScene = SceneManager.GetActiveScene().buildIndex;
        endMatchScene = SceneManager.GetSceneByName("End Match").buildIndex;

    }

    void Update()
    {
        Timer();
    }

    void LoadNextScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        int chosenInt = Random.Range(minInd, maxInd);

        SceneManager.LoadScene(chosenInt);
    }

    public void Timer()
    {
        loadingTime -= Time.deltaTime;

        int timeAsInt = Mathf.RoundToInt(loadingTime);
        countdownText.text = timeAsInt.ToString() + "...";


        if (loadingTime <= 0 && currentScene == endMatchScene)
            SceneManager.LoadScene(0);
        



        if (loadingTime <= 0 && currentScene != endMatchScene)
            LoadNextScene();
        

    }
}
