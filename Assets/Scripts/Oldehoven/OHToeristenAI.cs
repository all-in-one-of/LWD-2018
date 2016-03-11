using System.Collections;
using UnityEngine;

/// <summary>
/// The AI for the toeristen;
/// </summary>
public class OHToeristenAI : MonoBehaviour
{
    private int _dir;
    private float _pos;
    private Rigidbody _rigidbody;
    private GameObject _ohCenter;
    private GameObject _debug;

    private GameObject _mainMesh;
    private Animator _animator;
    private bool _isIdle;

    private void Start()
    {
        _mainMesh = transform.GetChild(0).gameObject;
        _rigidbody = GetComponent<Rigidbody>();
        _ohCenter = GameObject.Find("OHCenter");
        _animator = _mainMesh.GetComponent<Animator>();

#if UNITY_EDITOR
        var mat = new Material(GetComponent<MeshRenderer>().material);
        mat.color = new Color(Random.value, Random.value, Random.value, 0.2f);
        GetComponent<MeshRenderer>().material = mat;

        _debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DestroyImmediate(_debug.GetComponent<SphereCollider>());
        _debug.GetComponent<MeshRenderer>().material = mat;
        _debug.transform.parent = transform;

#else
       Destroy(GetComponent<MeshRenderer>());
#endif
    }

    /// <summary>
    /// Updates the model.
    /// </summary>
    private IEnumerator updateModel()
    {
        _isIdle = true;
        _animator.SetBool("IsIdle", _isIdle);

        yield return new WaitForSeconds(Random.Range(2, 4)); 

        _pos = Random.Range(-3, 3);
        var di = _pos - transform.position.x;
        _dir = di > 0 ? 1 : -1;
        var rot = di > 0 ? 90 : -90;
        _mainMesh.transform.eulerAngles = new Vector3(0, rot);
        _isIdle = false;
        _animator.SetBool("IsIdle", _isIdle);
    }

    private void Update()
    {
        if (!_isIdle)
        {
            if (_rigidbody.angularVelocity.magnitude < 1)
                _rigidbody.AddForce(new Vector3(_dir, 0)*25);

            var dis = Mathf.Abs((_ohCenter.transform.position.x + _pos) - transform.position.x);
            if (dis < 1)
            {
                StartCoroutine(updateModel());
            }
#if UNITY_EDITOR
            var tmp = new Vector3(_ohCenter.transform.position.x + _pos, 38, transform.position.z);
            _debug.transform.position = tmp;
#endif
        }
    }
}