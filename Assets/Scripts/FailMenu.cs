using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMenu : MonoBehaviour {

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void CheckPoint()
    {
        LevelManager.Manager.RevertToCheckpoint();
    }

    public void StartOver()
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
