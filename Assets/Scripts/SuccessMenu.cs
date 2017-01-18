using UnityEngine;
using UnityEngine.SceneManagement;

public class SuccessMenu : MonoBehaviour {

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        LevelManager.Manager.NextLevel(PlayerDataManager.Player);
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
