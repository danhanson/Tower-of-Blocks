using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class UnityExtensions {

    public static Vector2 To2D(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 To3D(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 To3D(this Vector2 v)
    {
        return v.To3D(0);
    }

    public static T AddMissingComponent<T>(this GameObject obj)
    where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp == null)
        {
            return obj.AddComponent<T>();
        }
        return comp;
    }

    private static void SetLevelAlpha(this GameObject obj, float alpha)
    {
        Renderer r = obj.GetComponent<Renderer>();
        Color c = r.material.color;
        r.material.color = new Color(c.r, c.g, c.b, alpha);
    }

    public static void SetLevelPhysical(this GameObject obj)
    {
        obj.SetLevelAlpha(1f);
        Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
        body.useAutoMass = true;
        obj.GetComponent<Collider2D>().density = obj.GetComponent<Block>().Material.density;
        body.gravityScale = 1;
    }

    public static void SetLevelTransient(this GameObject obj)
    {
        obj.SetLevelAlpha(0.6f);
        Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
        body.useAutoMass = false;
        body.mass = 0;
        body.gravityScale = 0;
    }

    public static bool IsTransient(this GameObject obj)
    {
        return obj.GetComponent<Rigidbody2D>().mass <= 0.0001; // mass is changed to small number when set to 0
    }

    public class Vector2SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector2 v = (Vector2) obj;
            info.AddValue("x", v.x);
            info.AddValue("y", v.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector2 ret;
            ret.x = info.GetSingle("x");
            ret.y = info.GetSingle("y");
            return ret;
        }
    }

    static IFormatter formatter = MakeFormatter();

    public static IFormatter Formatter
    {
        get { return formatter; }
    }

    static IFormatter MakeFormatter()
    {
        IFormatter formatter = new BinaryFormatter();
        SurrogateSelector selector = new SurrogateSelector();
        selector.AddSurrogate(
            typeof(Vector2),
            new StreamingContext(StreamingContextStates.All),
            new Vector2SerializationSurrogate()
        );
        formatter.SurrogateSelector = selector;
        return formatter;
    }
}
