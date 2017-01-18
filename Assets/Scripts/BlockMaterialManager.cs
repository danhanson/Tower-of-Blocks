using UnityEngine;

public enum MaterialType
{
    Wood,
    Brick,
    Balloon,
    Metal,
    Ice,
    Foundation,
    Grass
}

[System.Serializable]
public struct BlockMaterial
{
    public Material material;
    public PhysicsMaterial2D physics;
    public float density;
}

public class BlockMaterialManager : MonoBehaviour {

    private static BlockMaterialManager manager;

    public static BlockMaterialManager Instance
    {
        get { return manager; }
    }

    public BlockMaterial wood;
    public BlockMaterial brick;
    public BlockMaterial balloon;
    public BlockMaterial metal;
    public BlockMaterial ice;
    public BlockMaterial foundation;
    public BlockMaterial grass;

    public BlockMaterial fromType(MaterialType type)
    {
        switch (type)
        {
            case MaterialType.Wood: return wood;
            case MaterialType.Metal: return metal;
            case MaterialType.Brick: return brick;
            case MaterialType.Balloon: return balloon;
            case MaterialType.Ice: return ice;
            case MaterialType.Grass: return grass;
            case MaterialType.Foundation: return foundation;
        }
        throw new System.Exception("Invalid material type");
    }

    public BlockMaterialManager()
    {
        manager = this;
    }
}
