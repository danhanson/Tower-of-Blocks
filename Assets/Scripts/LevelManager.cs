using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

/*
 * LevelManager manages saving and loading of levels.
 * 
 * Once a level is loaded, this class calls GameState.SetLevel with it.
 * GameState uses LevelManager to save checkpoints. The menus using this
 * class are responsible for catching exceptions caused by file io and
 * displaying a reasonable error messages.
 */
public class LevelManager : MonoBehaviour {
    private static LevelManager manager;
    private AsyncOperation loadingScene;
    private Task<Level> loadingLevel;
    private Task<object> saving;
    private Level checkpoint;
    public List<string> levels;

    public static LevelManager Manager
    {
        get { return manager; }
    }

    public bool IsSaving {
        get { return saving != null; }
    }

    public void NextLevel(PlayerData player)
    {
        player.AtLevelIndex++;
        if (player.AtLevelIndex >= levels.Count)
        {
            throw new NotImplementedException("Win Screen / Credits");
        }
        else
        {
            Continue(player);
        }
    }

    public void SetCheckpoint(Level checkpoint)
    {
        this.checkpoint = checkpoint;
    }

    public void SaveCheckPoint()
    {
        saving = checkpoint.SaveAsync(CheckpointPath(checkpoint.name));
    }

    public void RevertToCheckpoint()
    {
        if (checkpoint != null)
        {
            GameState.State.SetLevel(checkpoint);
        }
        else
        {
            loadingLevel = Level.LoadAsync(CheckpointPath(checkpoint.name));
        }
    }

    public void RestartLevel()
    {
        loadingLevel = Level.LoadAsync(LevelPath(checkpoint.name));
    }

    public void Continue(PlayerData data)
    {
        if(checkpoint != null)
        {
            SaveCheckPoint();
        }
        if (loadingScene == null || loadingScene.isDone)
        {   // prevents the menu screen or game from freezing
            loadingScene = SceneManager.LoadSceneAsync("game");
            loadingScene.allowSceneActivation = true;
            // TODO we should disable the other buttons
        }
        if (data.LevelData == null)
        {
            data.Reset(this);
        }
        else
        {
            switch (data.AtLevel.Status)
            {
                case LevelStatus.Locked:
                    throw new WTFException("user tried to continue locked level");
                case LevelStatus.Available:
                case LevelStatus.Completed:
                    loadingLevel = Level.LoadAsync(LevelPath(data.AtLevelIndex));
                    break;
                case LevelStatus.Attempted:
                    loadingLevel = Level.LoadAsync(CheckpointPath(data.AtLevelIndex));
                    break;
            }
        }
    }

    public string CheckpointPath(int levelIndex)
    {
        return CheckpointPath(levels[levelIndex]);
    }

    public string CheckpointPath(string name)
    {
        return Application.persistentDataPath + "/Resources/" + name + ".dat";
    }

    public string LevelPath(string name)
    {
        return Application.dataPath + "/Resources/" + name + ".dat";
    }

    public string LevelPath(int levelIndex)
    {
        return LevelPath(levels[levelIndex]);
    }

    public LevelData[] DefaultPlayerLevels()
    {
        return levels.Select(
            level => new LevelData(level, LevelStatus.Available)
        ).ToArray();
    }

    public void SelectLevel(int index, PlayerData data)
    {
        data.AtLevelIndex = index;
        Continue(data);
    }

    public void Update()
    {
        if (loadingLevel != null && loadingLevel.IsDone)
        {
            try
            {
                checkpoint = loadingLevel.Value;
            } catch(Exception e)
            {
                Debug.LogError(e);
            }
            loadingLevel = null;
            GameState.State.SetLevel(checkpoint);
        }
        if(saving != null && saving.IsDone)
        {
            saving = null;
        }
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("game"))
        {
            manager.Continue(PlayerDataManager.Player); // player should have been initialized in menu scene
        }
    }

    public void Awake()
    {
        if(manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
