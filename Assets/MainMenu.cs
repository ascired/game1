using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() // Продолжить
    {
        UnitySceneManager.LoadScene("SampleScene");
    }

    public void NewGame() // Новая игра
    {
        PlayerPrefs.DeleteAll();
        PlayGame();
    }
}
