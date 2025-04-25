using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Für Text-Komponenten
using UnityEngine.SceneManagement;

[System.Serializable]
public class HighscoreEntry
{
    public string name;
    public int score;
}

[System.Serializable]
public class HighscoreList
{
    public List<HighscoreEntry> highscores;
}

public class HighScoreManager : MonoBehaviour
{
    public Text firstPlaceText;  // Textfeld für den 1. Platz
    public Text secondPlaceText; // Textfeld für den 2. Platz
    public Text thirdPlaceText;  // Textfeld für den 3. Platz

    private void Start()
    {
        // Highscores abrufen, wenn die Scene geladen wird
        StartCoroutine(GetHighscores());
    }

    IEnumerator GetHighscores()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://venushighscores.azurewebsites.net/api/VenusHighscores"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // JSON-Daten direkt als Liste deserialisieren
                string json = www.downloadHandler.text;
                HighscoreList highscoreList = JsonUtility.FromJson<HighscoreList>("{\"highscores\":" + json + "}");
                List<HighscoreEntry> highscores = highscoreList.highscores;

                // Textfelder aktualisieren
                if (highscores.Count > 0)
                {
                    firstPlaceText.text = $"{highscores[0].name} - {highscores[0].score}";
                }
                else
                {
                    firstPlaceText.text = "-";
                }

                if (highscores.Count > 1)
                {
                    secondPlaceText.text = $"{highscores[1].name} - {highscores[1].score}";
                }
                else
                {
                    secondPlaceText.text = "-";
                }

                if (highscores.Count > 2)
                {
                    thirdPlaceText.text = $"{highscores[2].name} - {highscores[2].score}";
                }
                else
                {
                    thirdPlaceText.text = "-";
                }
            }
            else
            {
                Debug.LogError("Fehler beim Abrufen der Highscores: " + www.error);
                firstPlaceText.text = "1st: Error";
                secondPlaceText.text = "2nd: Error";
                thirdPlaceText.text = "3rd: Error";
            }
        }
    }
}