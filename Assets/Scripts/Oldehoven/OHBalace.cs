using UnityEngine;
using UnityEngine.UI;

public class OHBalace : MonoBehaviour
{
    private float _force;
    private Rigidbody _rigidbody;

    public Text TextBD;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

    }


    private void Update()
    {
#if UNITY_EDITOR
        _force += Input.GetAxis("Mouse X");
#else
        _force = Input.acceleration.x * 50;
#endif
        TextBD.text = _force+"";
        _rigidbody.AddTorque(new Vector3(0, 0, -_force)*100);
    }
}