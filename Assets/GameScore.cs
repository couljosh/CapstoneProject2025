using UnityEngine;
using UnityEngine.SceneManagement;


public class GameScore : MonoBehaviour
{

    public static int redTotalScore;
    public static int blueTotalScore;
    public static int roundNum;



    public static void AddScore(int AddedRedPoints, int AddedBluePoints)
    {
        redTotalScore += AddedRedPoints;
        blueTotalScore += AddedBluePoints;
        roundNum++;
    }

    public static void ResetScore()
    {
        redTotalScore = 0;
        blueTotalScore = 0;
    }

    public static void DisplayScore()
    {

    }
    public static void DisplayFinalScore()
    {

    }
}
