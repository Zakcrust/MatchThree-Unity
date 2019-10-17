using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance sebagai global access
    public static GameManager instance;
    public AchievementSystem achievementSystem;
    int playerScore;
    public Text scoreText;
    public Text timerText;
    public int time;
    private int currentTime;
    void Start()
    // singleton
    {
        currentTime = time;
        setTime(currentTime);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        StartCoroutine(timerCount());
    }

    public void SetScore(int point)
    //Update score dan ui
    {
        playerScore += point;
        scoreText.text = playerScore.ToString();
    }

   IEnumerator timerCount()
    {
        while(currentTime > 0)
        {
            currentTime--;
            setTime(currentTime);
            yield return new WaitForSeconds(1f);
        }

        gameOver();
        
    }

    private void setTime(int time)
    {
        timerText.text = time.ToString();
    }

    private void gameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0;
    }

}