    using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;
    public AudioSource scoreSound;
    public AudioSource gameOverSound;
    private DBManager dbManager;

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        scoreSound.Play();
    }

    public void ChangeSceneToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        gameOverSound.Play();

        // Send Name and Score to DB
        DBManager dbManager = DBManager.Instance;
        string playerName = PlayerPrefs.GetString("PlayerName");
        Debug.Log("Name sent to DB: " + playerName);
        if (dbManager != null)
        {
            Debug.Log("dbManager != null");
            dbManager.SendScoreToServer(playerName, playerScore);
        }
    }    
   
    public void QuitGame()
    {
        Application.Quit();
    }
}
