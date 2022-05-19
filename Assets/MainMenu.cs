using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() 
    {
        UnitySceneManager.LoadScene("SampleScene");
    }

    public void NewGame() 
    {
        PlayerPrefs.DeleteAll();
        PlayGame();
    }

    public void Exit(){
        Application.Quit();
    }
}
