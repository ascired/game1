using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class StoryMenu : MonoBehaviour
{

    public bool StoryGame;
    public GameObject storyGameMenu;
    void Update()
    {
        //Pause();
    }

    public void Resume()
    {
        storyGameMenu.SetActive(false);
        StoryGame = false;
    }

    public void Pause()
    {
        storyGameMenu.SetActive(true);
        StoryGame = true;
    }
}
