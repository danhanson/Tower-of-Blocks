using UnityEngine;
using System;

[ExecuteInEditMode]
public abstract class Block : MonoBehaviour
{
    public BlockType type;
    public MaterialType materialType;

    public BlockMaterial Material {
        get { return BlockMaterialManager.Instance.fromType(materialType);  }
    }

    // a transient block is a tower block being placed that is allowed to collide with terrain
    public bool IsTransient
    {
        get
        {
            return type == BlockType.Tower && gameObject.IsTransient();
        }
    }

    public abstract float Height
    {
        get;
    }

    public float Mass
    {
        get
        {
            return gameObject.GetComponent<Rigidbody2D>().mass;
        }
    }

    protected abstract Collider2D AddCollider();

    protected abstract Mesh MakeMesh();

    public void Awake()
    {
        gameObject.AddMissingComponent<Rigidbody2D>().useAutoMass = true;
        gameObject.AddMissingComponent<MeshFilter>();
        gameObject.AddMissingComponent<MeshRenderer>();
    }

    public void Start()
    {
        BlockMaterial mat = Material;
        Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
        Collider2D collider = AddCollider();
        if (type != BlockType.Tower)
        {
            body.isKinematic = true;
        }
        collider.sharedMaterial = mat.physics;
        if (body.useAutoMass)
        {
            collider.density = mat.density;
        }
        Mesh mesh = MakeMesh();
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.materials = new Material[] { mat.material };
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Block otherBlock = col.gameObject.GetComponent<Block>();
        // lose game if an active tower block collides with terrain block
        if (otherBlock != null && !otherBlock.IsTransient && type == BlockType.Terrain && otherBlock.type == BlockType.Tower && GameState.State != null)
        {
            GameState.State.OnLose();
        }
    }

    public abstract BlockData ToBlockData();
}
