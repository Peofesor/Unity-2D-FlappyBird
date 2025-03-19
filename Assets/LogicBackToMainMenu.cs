using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicBackToMainMenu : MonoBehaviour
{
    public void ChangeSceneToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
