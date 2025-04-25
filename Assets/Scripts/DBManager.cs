using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.InputSystem;
using System.Text;

public class DBManager : MonoBehaviour
{
    // Singleton-Instanz
    public static DBManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // DBManager bleibt zwischen Szenen bestehen
        }
        else
        {
            Destroy(gameObject); // Zerstört Duplikate
        }
    }

    public void SendScoreToServer(string name, int score)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Spielername ist leer!");
            return;
        }
        if (score < 0)
        {
            Debug.LogError("Score kann nicht negativ sein!");
            return;
        }

        StartCoroutine(PostScore(name, score));
    }

    IEnumerator PostScore(string name, int score)
    {
        string json = JsonUtility.ToJson(new HighscoreData { name = name, score = score });
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest("https://venushighscores.azurewebsites.net/api/VenusHighscores", "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Debug.Log($"POST Response: {request.downloadHandler.text}");
    }

    [System.Serializable]
    class HighscoreData
    {
        public string name;
        public int score;
    }
}
