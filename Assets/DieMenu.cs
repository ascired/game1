using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class DieMenu : MonoBehaviour
{
    public bool DieGame;
    public GameObject dieGameMenu;
    public void Again()
    {
        Time.timeScale = 1f;
        UnitySceneManager.LoadScene("SampleScene");
    }

    public void DiePause()
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
