using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScriptStartscreen : MonoBehaviour
{
    public void ChangeSceneToNameInput()
    {
        SceneManager.LoadScene(2);
    }    
    public void ChangeSceneToHighScore()
    {
        SceneManager.LoadScene(1);
    }
}
