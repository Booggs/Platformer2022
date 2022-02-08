using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBonesInstancer : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] _bones;

    [SerializeField]
    private float _explosionForce = 75f;

    public void Explode()
    {
        foreach(Rigidbody bone in _bones)
        {
            Rigidbody rigidbody = Instantiate<Rigidbody>(bone, new Vector3(transform.position.x, transform.position.y + Random.Range(2, 3), transform.position.z), Random.rotation);
            rigidbody.AddExplosionForce(_explosionForce, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), 2);
        }
    }
}
