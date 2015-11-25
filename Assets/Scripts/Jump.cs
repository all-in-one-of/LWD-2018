using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour
{
    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!_rigidbody.isKinematic)
        foreach (ContactPoint contact in collision.contacts)
        {
            _rigidbody.AddExplosionForce(100, contact.point + Random.onUnitSphere/10,25);
        }
    }
}
