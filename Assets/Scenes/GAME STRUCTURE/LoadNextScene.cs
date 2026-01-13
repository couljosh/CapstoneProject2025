using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public float loadingTime;
    public TextMeshProUGUI countdownText;

    void Update()
    {
        Timer();
    }
    public void Timer()
    {
        loadingTime -= Time.deltaTime;

        int timeAsInt = Mathf.RoundToInt(loadingTime);
        countdownText.text = timeAsInt.ToString() + "...";


        if (loadingTime <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }

    }
}
