using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class NextMenu : MonoBehaviour
{
    public GameObject nextLevelMenu;

    private Action nextLevelCallback;
    public void Next()
    {
        Time.timeScale = 1f;
        MainManager.Instance.nextLevel();
        nextLevelMenu.SetActive(false);
        nextLevelCallback();
    }

    public void FinishPause(Action moveToReciever)
    {
        Time.timeScale = 0f;
        nextLevelMenu.SetActive(true);
        nextLevelCallback = moveToReciever;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        UnitySceneManager.LoadScene("Menu");
    }
}
