using UnityEngine;
using System.Collections;

public class LevelSelector : MonoBehaviour {

    public GameObject LevelDisplay;

	// Use this for initialization
	void Start () {
        PlayerData data = PlayerDataManager.Player;
        foreach(LevelData level in data.LevelData)
        {

        }
	}
}
