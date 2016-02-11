using UnityEngine;
using System.Collections;

public class OHBalace : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private float force;

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!Input.gyro.enabled)
        {
            Input.gyro.enabled = true;
            print(Input.gyro.enabled);
        }
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        force -= Input.GetAxis("Mouse X");
#else
        force -= Input.acceleration.x;
#endif

        _rigidbody.AddTorque(new Vector3(0, 0, force)*100);
    }


}