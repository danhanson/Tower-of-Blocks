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

    public abstract float Height
    {
        get;
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
        if (type == BlockType.Terrain && GameState.State != null)
        {
            GameState.State.OnLose();
        }
    }

    public abstract BlockData ToBlockData();
}
