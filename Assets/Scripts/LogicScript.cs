using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LogicScript : MonoBehaviour
{
    public InputField nameInputField; 
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;
    public AudioSource scoreSound;
    public AudioSource gameOverSound;
    private DBManager dbManager;
    public GameObject startMenu;
    public GameObject nameInput;

    public GameObject pipeSpawner;
    public GameObject bird;
    public GameObject ground;
    void Start()
    {
        bird.SetActive(false);
        pipeSpawner.SetActive(false);
        startMenu.SetActive(true);
        ground.SetActive(true); // Boden soll sichtbar sein, aber nicht bewegen
        nameInput.SetActive(false);

        // Deaktiviere die GroundMoveScript-Komponente, damit der Boden sich nicht bewegt
        GroundScript groundMoveScript = ground.GetComponent<GroundScript>();
        if (groundMoveScript != null)
        {
            groundMoveScript.enabled = false;
        }

        playerScore = 0;
        scoreText.text = "0";
    }

    public void SavePlayerName()
    {
        string enteredName = nameInputField.text;
        PlayerPrefs.SetString("PlayerName", enteredName);
        PlayerPrefs.Save();
    }

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        scoreSound.Play();
    }

    public void restartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        StartGame(); // Wird nicht korrekt ausgeführt, bei Neustart des Spiels wird noch Menü angezeigt
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        gameOverSound.Play();

        // Send Name and Score to DB
        DBManager dbManager = DBManager.Instance;
        string playerName = PlayerPrefs.GetString("PlayerName");
        if (dbManager != null)
        {
            Debug.Log("LogicScript: Name sent to DB: " + playerName);
            dbManager.SendScoreToServer(playerName, playerScore);
        }
    }

    public void ChangeSceneToHighscore()
    {
        SceneManager.LoadScene(1);
    }

    public void DisplayNameInput()
    {
        nameInput.SetActive(true);
        startMenu.SetActive(false);
    }

    public void StartGame()
    {
        SavePlayerName(); // Speichere den Spielernamen aus dem Inputfeld

        startMenu.SetActive(false);
        nameInput.SetActive(false);
        bird.SetActive(true);
        pipeSpawner.SetActive(true);

        // Aktiviere die GroundMoveScript-Komponente, damit der Boden sich bewegt
        GroundScript groundMoveScript = ground.GetComponent<GroundScript>();
        if (groundMoveScript != null)
        {
            groundMoveScript.enabled = true;
        }

        // Spawne die erste Pipe sofort
        PipeSpawnScript pipeSpawnScript = pipeSpawner.GetComponent<PipeSpawnScript>();
        pipeSpawnScript.spawnPipe();
    }
}
