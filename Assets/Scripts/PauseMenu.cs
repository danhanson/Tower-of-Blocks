using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void Continue()
    {
        GameState.State.Continue();
    }

    public void Restart()
    {
        LevelManager.Manager.RestartLevel();
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("menu").allowSceneActivation = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
