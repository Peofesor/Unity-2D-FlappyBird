using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class HighscoreEntry
{
    public string name;
    public int score;
    public string timestamp;

    [System.NonSerialized]
    public System.DateTime dateTime;
}

[System.Serializable]
public class HighscoreList
{
    public List<HighscoreEntry> highscores;
}

public class HighScoreManager : MonoBehaviour
{
    // Arrays of UI text elements for ranks 1-7
    public Text[] scoreTexts = new Text[7];
    public Text[] nameTexts = new Text[7];
    public Text[] dateTexts = new Text[7];

    private void Start()
    {
        // Get highscores when the scene loads
        StartCoroutine(GetHighscores());
    }
    IEnumerator GetHighscores()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://venushighscores.azurewebsites.net/api/VenusHighscores"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Deserialize JSON data directly as a list
                string json = www.downloadHandler.text;
                Debug.Log("Received JSON: " + json); // Log the received JSON for debugging

                HighscoreList highscoreList = JsonUtility.FromJson<HighscoreList>("{\"highscores\":" + json + "}");
                List<HighscoreEntry> highscores = highscoreList.highscores;

                // Parse timestamp strings to DateTime for each entry
                foreach (var entry in highscores)
                {
                    if (!string.IsNullOrEmpty(entry.timestamp))
                    {
                        if (System.DateTime.TryParseExact(
                            entry.timestamp,
                            "M/d/yyyy h:mm:ss tt", // Format for "4/25/2025 2:40:28 PM"
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out System.DateTime parsedDate))
                        {
                            entry.dateTime = parsedDate;
                        }
                        else
                        {
                            Debug.LogWarning($"Could not parse timestamp: {entry.timestamp}");
                            entry.dateTime = System.DateTime.MinValue; // Fallback value
                        }
                    }
                    else
                    {
                        entry.dateTime = System.DateTime.MinValue; // Fallback for empty timestamp
                    }
                }

                // Update UI elements for each rank
                UpdateHighscoreUI(highscores);
            }
            else
            {
                Debug.LogError("Error retrieving highscores: " + www.error);
                ClearHighscoreUI(); // Clear UI on error
            }
        }
    }

    private void UpdateHighscoreUI(List<HighscoreEntry> highscores)
    {
        // Clear the UI first
        ClearHighscoreUI();

        // Update UI with available highscores
        int entriesToShow = Mathf.Min(highscores.Count, 7);

        // Log the entries we're displaying
        for (int i = 0; i < entriesToShow; i++)
        {
            Debug.Log($"Entry {i}: {highscores[i].name}, Score: {highscores[i].score}, Time: {highscores[i].timestamp}");
        }

        for (int i = 0; i < entriesToShow; i++)
        {
            HighscoreEntry entry = highscores[i];

            if (scoreTexts[i] != null) scoreTexts[i].text = entry.score.ToString();
            if (nameTexts[i] != null) nameTexts[i].text = entry.name;
            if (dateTexts[i] != null) dateTexts[i].text = entry.dateTime.ToString("dd/MM/yy");
        }
    }
    private void ClearHighscoreUI()
    {
        // Set all text fields to "-"
        for (int i = 0; i < 7; i++)
        {
            if (scoreTexts[i] != null) scoreTexts[i].text = "-";
            if (nameTexts[i] != null) nameTexts[i].text = "-";
            if (dateTexts[i] != null) dateTexts[i].text = "-";
        }
    }
}