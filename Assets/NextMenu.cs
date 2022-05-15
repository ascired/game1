using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class NextMenu : MonoBehaviour
{
    public bool DieGame;
    public GameObject dieGameMenu;
    public void Next()
    {
        Time.timeScale = 1f;
        UnitySceneManager.LoadScene("SampleScene");
    }

    public void FinishPause()
    {
        dieGameMenu.SetActive(true);
        Time.timeScale = 0f;
        DieGame = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        UnitySceneManager.LoadScene("Menu");
    }
}
