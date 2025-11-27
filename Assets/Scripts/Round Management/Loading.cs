using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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

    public LevelRef sceneListSO;



    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        endMatchScene = SceneManager.GetSceneByName("End Match").buildIndex;

    }

    void Update()
    {
        Timer();
    }

    public void Timer()
    {
        loadingTime -= Time.deltaTime;

        int timeAsInt = Mathf.RoundToInt(loadingTime);
        countdownText.text = timeAsInt.ToString() + "...";


        if (loadingTime <= 0 && currentScene == endMatchScene)
            SceneManager.LoadScene(0);



        if (loadingTime <= 0 && currentScene != endMatchScene)
            LoadRandomScene();



    }

    void LoadRandomScene()
    {
        int randInd = Random.Range(0, sceneListSO.sceneList.Count);
        SceneManager.LoadScene(sceneListSO.sceneList[randInd]);
    }
}


