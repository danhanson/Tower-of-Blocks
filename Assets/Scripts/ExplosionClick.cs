using UnityEngine;

public class ExplosionClick : MonoBehaviour {

    public float explosionSpeed;
    public float explosionStrength;

    virtual public void OnClick()
    {
        Explosion.Create(transform.position, explosionSpeed, explosionStrength);
    }
}
