using UnityEngine;
using UnityEngine.UI;

public class OHBalace : MonoBehaviour
{
    private float _force;
    private Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        _force += Input.GetAxis("Mouse X");
        if (Input.GetMouseButtonDown(0))
            _rigidbody.isKinematic = !_rigidbody.isKinematic;
#else
        _force = Input.acceleration.x * 50;
#endif
        _rigidbody.AddTorque(new Vector3(0, 0, -_force)*500);
    }
}