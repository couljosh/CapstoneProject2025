using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public Scene scene;
    public int sceneNumber;

<<<<<<< Updated upstream
    // Start is called once before the first execution of Update after the MonoBehaviour is created
=======
    public float roundTime;
    public float endOfRoundDelay;

    public GameObject redRepository;
    public GameObject blueRepository;

    public GameObject singleRepository;

    public GameObject teamOneWinText;
    public GameObject teamTwoWinText;
    public GameObject drawText;
    public GameObject tint;

    public TextMeshProUGUI timerText;

    public TextMeshProUGUI redTeamPoints;
    public TextMeshProUGUI blueTeamPoints;


>>>>>>> Stashed changes
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        if (Input.GetKeyDown(KeyCode.Space)) //change this to when the game is finished.
=======
        redTeamPoints.text = singleRepository.GetComponent<SingleRepo>().depositTotalRed.ToString();
        blueTeamPoints.text = singleRepository.GetComponent<SingleRepo>().depositTotalBlue.ToString();


        //Timer Counts down
        roundTime -= Time.deltaTime;

        int min = Mathf.FloorToInt(roundTime / 60);
        int sec = Mathf.FloorToInt(roundTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", min,sec);

        if(roundTime <= 20)
>>>>>>> Stashed changes
        {
            print("new scene");
            SceneManager.LoadScene(sceneNumber);
        }
    }



}
