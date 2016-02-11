using UnityEngine;

public class BHPInmateAI : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_rigidbody.isKinematic)
        {
            foreach (var contact in collision.contacts)
            {
                var p = contact.point;
                p += Random.onUnitSphere;
                if (transform.position.x > 0)
                    p += Vector3.left/4;
                else
                    p += Vector3.right/4;
                _rigidbody.AddExplosionForce(250, p, 50);
            }
        }

    }
}
