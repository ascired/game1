using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class FinalMenu : MonoBehaviour
{
    public bool FinalGame;
    public GameObject finalGameMenu;
    private Action nextLevelCallback;

    void Update()
    {
        
    }

    public void Resume()
    {
        finalGameMenu.SetActive(false);
        FinalGame = false;
    }

    public void FinishPause(Action moveToReciever)
    {
        Time.timeScale = 0f;
        finalGameMenu.SetActive(true);
        nextLevelCallback = moveToReciever;
    }

    public void Pause()
    {
        finalGameMenu.SetActive(true);
        FinalGame = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        UnitySceneManager.LoadScene("Menu");
    }
}
