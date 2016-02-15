using UnityEngine;

public class OHToeristenAI : MonoBehaviour
{
    private int _dir;
    private float _pos;
    private Rigidbody _rigidbody;
    private float _waitDelay;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
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