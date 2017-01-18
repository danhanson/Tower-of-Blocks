using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameState : MonoBehaviour {

    private static TimeSpan CHECKPOINT_TIME = TimeSpan.FromSeconds(5);

    public Vector2 blockSpawn;
    public string seed;
    public System.Random random;
    public string levelName;
    public GameObject failMenu;
    public GameObject successMenu;
    public GameObject pauseMenu;

    public enum GameStatus
    {
        Running,
        Paused,
        Won,
        Lost
    }

    public GameObject[] blockObjects; // used as a queue if random is null, otherwised used as a sample
    public BlockData[] blocks;
    private bool checkpointed = true;
    private IEnumerator blockEnumerator;
    private GameObject currentObject;
    private float height;
    private DateTime lastPlaced;

    private static GameState state;

    GameStatus status = GameStatus.Running;

    public GameStatus Status { get { return status;  }  }

    public int Score { set; get; }

    public void Pause()
    {
        if(status == GameStatus.Running)
        {
            Time.timeScale = 0;
            status = GameStatus.Paused;
            pauseMenu.SetActive(true);
        }
    }

    public void Continue()
    {
        if(status == GameStatus.Paused)
        {
            Time.timeScale = 1;
            status = GameStatus.Running;
            pauseMenu.SetActive(false);
        }
    }

    public void Awake()
    {
        state = this;
    }

    private GameState()
    {
        state = this;
        status = GameStatus.Running;
        Score = 0;
        height = 0;
    }

    public static GameState State {
        get
        {
            return state;
        }
    }

    public void OnLose()
    {
        if (status == GameStatus.Running)
        {
            status = GameStatus.Lost;
        }
    }

    public void Start()
    {
        if (blockObjects != null && blockObjects.Length > 0)
        {
            blocks = blockObjects.Select(b => b.GetComponent<Block>().ToBlockData()).ToArray();
        }
        if (seed.Length > 0)
        {
            random = new System.Random(seed.GetHashCode());
        }
        else
        {
            random = null;
        }
        blockEnumerator = blocks.GetEnumerator();
        nextBlock();
    }

    public void ClearWorld()
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void SetLevel(Level level)
    {
        ClearWorld();
        GameState state = GameState.State;
        state.blocks = level.toPlace;
        state.random = level.rand;
        GameObject tower = new GameObject("Tower");
        tower.transform.parent = state.transform;
        foreach (BlockData bd in level.tower)
        {
            bd.ToGameObject(BlockType.Tower).transform.parent = tower.transform;
        }
        GameObject foundation = new GameObject("Foundation");
        foundation.transform.parent = state.transform;
        foreach (BlockData bd in level.foundation)
        {
            bd.ToGameObject(BlockType.Foundation).transform.parent = foundation.transform;
        }
        GameObject terrain = new GameObject("Terrain");
        terrain.transform.parent = state.transform;
        foreach (BlockData bd in level.terrain)
        {
            bd.ToGameObject(BlockType.Terrain).transform.parent = terrain.transform;
        }
    }

    /*
     * Materializes the currently dragged object and prepares the next object.
     * currentObject is set to null  if there is no next object.
     */
    private void nextBlock()
    {
        if(currentObject != null)
        {
            currentObject.SetLevelPhysical();
            checkpointed = false;
            lastPlaced = DateTime.Now;
        }
        if (!blockEnumerator.MoveNext())
        {
            currentObject = null;
            return;
        }
        currentObject = ((BlockData)blockEnumerator.Current).ToGameObject(BlockType.Tower);
        currentObject.SetLevelTransient();
        currentObject.transform.position = new Vector3(blockSpawn.x, blockSpawn.y, 0);
    }

    /* returns a checkpoint for the game and updates height */
    Level CheckPoint()
    {
        Level checkpoint = new Level();
        checkpoint.name = levelName;
        List<BlockData> foundation = new List<BlockData>();
        foreach(Transform child in transform.Find("Foundation"))
        {
            foundation.Add(child.GetComponent<Block>().ToBlockData());
        }
        checkpoint.foundation = foundation.ToArray();
        List<BlockData> terrain = new List<BlockData>();
        foreach(Transform child in transform.Find("Terrain"))
        {
            terrain.Add(child.GetComponent<Block>().ToBlockData());
        }
        checkpoint.terrain = terrain.ToArray();
        List<BlockData> tower = new List<BlockData>();
        height = 0;
        foreach(Transform child in transform.Find("Tower"))
        {
            Block block = child.GetComponent<Block>();
            float blockHeight = block.Height;
            if(blockHeight > height)
            {
                height = blockHeight;
            }
            tower.Add(block.ToBlockData());
        }
        checkpoint.tower = tower.ToArray();
        return checkpoint;
    }

    public void Update()
    {
        switch(status)
        {
            case GameStatus.Won:
                successMenu.SetActive(true);
                break;
            case GameStatus.Lost:
                failMenu.SetActive(true);
                break;
            case GameStatus.Running:
                if(Input.GetMouseButtonUp(0))
                {
                    nextBlock();
                }
                if(!checkpointed && DateTime.Now - lastPlaced >= CHECKPOINT_TIME)
                {
                    LevelManager.Manager.SetCheckpoint(CheckPoint());
                    checkpointed = true;
                    Debug.Log("CHECKPOINT!");
                }
                if (currentObject != null)
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = 100;
                    mousePosition = Camera.main.ScreenToWorldPoint(
                        mousePosition
                    );
                    Vector3 blockPosition = currentObject.transform.position;
                    Vector3 velocity = mousePosition - blockPosition;
                    if(velocity.sqrMagnitude >= 1600)
                    {
                        velocity.Normalize();
                        velocity *= 40;
                    }
                    velocity.Scale(new Vector3(30f, 30f, 30f));
                    currentObject.GetComponent<Rigidbody2D>().velocity = velocity;
                }
                break;
        }
    }
}
