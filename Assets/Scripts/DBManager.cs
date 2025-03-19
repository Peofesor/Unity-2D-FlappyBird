using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.InputSystem;

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

        StartCoroutine(UploadScore(name, score));
    }

    IEnumerator UploadScore(string name, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_name", name);
        form.AddField("score", score.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post("http://ec2-3-73-118-68.eu-central-1.compute.amazonaws.com/save_score.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log("Score erfolgreich gesendet: " + www.downloadHandler.text);
                Debug.Log("Name: " + name + ",Score: " + score);
            }
            else
            {
                Debug.LogError("Fehler beim Senden: " + www.error);
            }
        }
    }
}
