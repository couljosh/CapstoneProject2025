using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayTeamScores : MonoBehaviour
{
    public TextMeshProUGUI redScore;
    public TextMeshProUGUI blueScore;
    public TextMeshProUGUI roundNum;

    public float elapsedTime;
    public float loadingTime;

    void Start()
    {
        roundNum.text = "Round: " + GameScore.roundNum.ToString() + " / 3";

        redScore.text = GameScore.redTotalScore.ToString();
        blueScore.text = GameScore.blueTotalScore.ToString();
    }
}
