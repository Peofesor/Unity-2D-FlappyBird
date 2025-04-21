using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicBackToMainMenu : MonoBehaviour
{
    public void ChangeSceneToGameplay()
    {
        SceneManager.LoadScene(0);
    }
}
