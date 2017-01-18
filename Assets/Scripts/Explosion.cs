using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public float shockwaveSpeed;
    public float flux;
    public float threshold = 1000f;

    public static GameObject Create(Vector3 position, float shock, float flux)
    {
        GameObject obj = new GameObject("Explosion");
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0;
        collider.isTrigger = true;
        Explosion ex = obj.AddComponent<Explosion>();
        ex.shockwaveSpeed = shock;
        ex.flux = flux;
        obj.transform.position = position;
        return obj;
    }

    public float Force
    {
        get {
            return flux / (2 * Mathf.PI * GetComponent<CircleCollider2D>().radius);
        }
    }

	// Use this for initialization
	void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D body = other.GetComponent<Rigidbody2D>();
        Vector3 forceDirection = other.transform.position - transform.position;
        forceDirection.Normalize();
        body.AddForce(forceDirection * Force, ForceMode2D.Impulse);
    }

    void Update()
    {
        float radius = GetComponent<CircleCollider2D>().radius += shockwaveSpeed;
        if(radius > threshold)
        {
            Destroy(gameObject);
        }
    }
}
