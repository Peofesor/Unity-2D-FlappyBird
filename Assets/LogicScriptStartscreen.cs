using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScriptStartscreen : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
}
