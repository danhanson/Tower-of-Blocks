using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

public enum BlockType
{
    Tower,
    Terrain,
    Foundation
}

[System.Serializable]
public class BlockData
{
    public MaterialType material;
    public Vector2[] polygon; // if polygon is null, we assume its a circle
    public float radius;
    public Vector2 position;
    public float angle;

    public GameObject ToGameObject(BlockType type)
    {
        GameObject obj = new GameObject("block");
        Block b;
        if (polygon == null)
        {
            Circle c = obj.AddComponent<Circle>();
            c.radius = radius;
            b = c;
        } else {
            Polygon p = obj.AddComponent<Polygon>();
            p.points = polygon;
            b = p;
        }
        b.materialType = material;
        b.type = type;
        obj.transform.eulerAngles = new Vector3(0, 0, angle);
        obj.transform.position = position;
        return obj;
    }
}

class LevelLoader : Task<Level>
{
    string path;

    public LevelLoader(string path)
    {
        this.path = path;
    }

    public override Level Compute(object sender, DoWorkEventArgs args)
    {
        return Level.Load(path);
    }
}

class LevelSaver : Task<object>
{
    Level level;
    string path;

    public LevelSaver(Level level, string path)
    {
        this.level = level;
        this.path = path;
    }

    public override object Compute(object sender, DoWorkEventArgs args)
    {
        level.Save(path);
        return null;
    }
}

[System.Serializable]
public class Level
{
    public string name;
    public System.Random rand; // SetLevel rand to null to use toPlace as a queue
    public BlockData[] toPlace;
    public BlockData[] tower;
    public BlockData[] foundation;
    public BlockData[] terrain;
    // add decorations

    public void Save(string path)
    {
        IFormatter formatter = UnityExtensions.Formatter;
        using (FileStream s = File.OpenWrite(path))
        {
            formatter.Serialize(s, this);
        }
    }

    public Task<object> SaveAsync(string path)
    {
        LevelSaver saver = new LevelSaver(this, path);
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += new DoWorkEventHandler(saver.DoWork);
        return saver;
    }

    /*
     * returns the specified level from the path, may throw an exception
     * if there is trouble reading the file
     */
    public static Level Load(string path)
    {
        IFormatter formatter = UnityExtensions.Formatter;
        using(FileStream s = File.OpenRead(path))
        {
            return (Level) formatter.Deserialize(s);
        }
    }

    public static Task<Level> LoadAsync(string path)
    {
        BackgroundWorker worker = new BackgroundWorker();
        LevelLoader loader = new LevelLoader(path);
        worker.DoWork += new DoWorkEventHandler(loader.DoWork);
        worker.RunWorkerAsync();
        return loader;
    }
}
