using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicScriptNameInput : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text resultText;
    public string m_name;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    void OnInputFieldValueChanged(string inputText)
    {
        m_name = inputText;

        if (resultText == null)
        {
            Debug.LogWarning("resultText ist immernoch nicht zugewiesen!");
            return;
        }

        if (string.IsNullOrEmpty(m_name) || m_name.Length > 10)
        {
            resultText.text = "Invalid";
            resultText.color = Color.red;
        }
        else
        {
            resultText.text = "Valid";
            resultText.color = Color.green;
        }
    }

    public void ChangeSceneToIngame()
    {
        // Speichere den Namen in PlayerPrefs, bevor die Szene gewechselt wird
        if (!string.IsNullOrEmpty(m_name))
        {
            PlayerPrefs.SetString("PlayerName", m_name);
            Debug.Log("NameInputScript: Player Name: " + m_name);
        }
        else
        {
            Debug.LogWarning("m_name ist leer!");
            PlayerPrefs.SetString("PlayerName", "Unknown"); // Fallback-Wert
        }
        SceneManager.LoadScene(3);
    }
}
