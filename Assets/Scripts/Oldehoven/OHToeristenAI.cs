using UnityEngine;
using System.Collections;

public class OHToeristenAI : MonoBehaviour
{
    private float _pos;
    private int _dir;
    private Rigidbody _rigidbody;
    private float _waitDelay;

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_waitDelay > 0)
        {
            _waitDelay -= Time.deltaTime;
            return;
        }

        if (_rigidbody.angularVelocity.magnitude < 1)
            _rigidbody.AddForce(new Vector3(_dir, 0)*25);

        var d = Mathf.Abs(_pos - transform.position.x);
        if (d < 1)
        {
            _waitDelay = Random.Range(2, 4);
            _pos = Random.Range(-4, 4);
            var di = _pos - transform.position.x;
            _dir = di > 0 ? 1 : -1;
        }
    }
}
