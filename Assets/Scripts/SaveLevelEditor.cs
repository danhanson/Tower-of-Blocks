using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameState))]
public class SaveLevelEditor : Editor
{
    public void ClearWorld()
    {
        if (GameState.State == null)
            return;
        Transform world = GameState.State.transform;
        while (world.childCount > 0)
        {
            DestroyImmediate(world.GetChild(0).gameObject);
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Level Name");
        string levelName = GameState.State.levelName;
        if(levelName == null)
        {
            levelName = "";
        }
        base.OnInspectorGUI();
        if (GUILayout.Button("Save Level"))
        {
            string path = EditorUtility.SaveFilePanel("Save Level", Application.dataPath + "/Resources", levelName, "dat");
            GameObject level = GameState.State.gameObject;
            Block[] blocks = level.GetComponentsInChildren<Block>();
            List<BlockData> tower = new List<BlockData>();
            List<BlockData> terrain = new List<BlockData>();
            List<BlockData> foundation = new List<BlockData>();
            foreach(Block b in blocks)
            {
                switch(b.type)
                {
                    case BlockType.Tower: tower.Add(b.ToBlockData()); break;
                    case BlockType.Foundation: foundation.Add(b.ToBlockData()); break;
                    case BlockType.Terrain: terrain.Add(b.ToBlockData()); break;
                }
            }
            Level data = new Level();
            data.name = levelName;
            data.foundation = foundation.ToArray();
            data.terrain = terrain.ToArray();
            data.tower = tower.ToArray();
            data.toPlace = GameState.State.blocks;
            data.rand = GameState.State.random;
            try
            {
                data.Save(path);
            } catch(System.Exception e)
            {
                Debug.LogError(e);
            }
        }
        if(GUILayout.Button("Load Level"))
        {
            if (GameState.State == null)
                Debug.LogError("GameState component is required for level loading");
            string path = EditorUtility.OpenFilePanel(levelName, Application.dataPath + "/Resources", "dat");
            try
            {
                Level data = Level.Load(path);
                GameState.State.SetLevel(data);
            } catch(System.Exception e)
            {
                Debug.LogError(e);
            }
        }
        if(GUILayout.Button("Clear"))
        {
            ClearWorld();
        }
    }
}
