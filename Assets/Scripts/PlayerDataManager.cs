using UnityEngine;
using System.Runtime.Serialization;
using System.IO;

public enum LevelStatus
{
    Locked, // player is not allowed to play this level
    Available, // player could do this level, but has not played it before
    Attempted, // player has played level before, we load previous progress
    Completed // user has completed level, start from scratch if selected
}

/* Player specific data about a level, used for choose level interface */
public class LevelData
{
    public string Name;
    public LevelStatus Status;
    public float Height;

    public LevelData(string name, LevelStatus status)
    {
        Name = name;
        Status = status;
    }
}

public class PlayerData
{
    public LevelData[] LevelData
    {
        get; private set;
    }

    public int AtLevelIndex; // resume last level

    public LevelData AtLevel
    {
        get { return LevelData[AtLevelIndex]; }
    }

    public void Reset(LevelManager manager)
    {
        LevelData = manager.DefaultPlayerLevels();
        AtLevelIndex = 0;
    }
}

public class PlayerDataManager : MonoBehaviour {

    private static PlayerData player = null;

    public static PlayerData Player {
        get { return player; }
    }

	void Awake () {
	    if(player == null)
        {
            IFormatter formatter = UnityExtensions.Formatter;
            try
            {
                using (FileStream file = File.OpenRead(Application.persistentDataPath + "/PlayerData"))
                {
                    player = (PlayerData)formatter.Deserialize(file);
                }
            } catch(FileNotFoundException)
            {
                player = new PlayerData();
            }
        }       
        Destroy(gameObject);
	}
}
