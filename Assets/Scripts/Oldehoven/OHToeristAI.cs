using System.Collections;
using UnityEngine;

/// <summary>
/// The AI for the toeristen;
/// </summary>
public class OHToeristAI : MonoBehaviour
{
    private int _dir;
    private float _pos;
    private Rigidbody _rigidbody;
    private Rigidbody _rigidbodyParachute;
    private GameObject _ohCenter;
    private GameObject _debug;

    private GameObject _mainMesh;
    private GameObject _parachute;
    private bool _parachuteIsDeployed;
    private Animator _animator;
    private bool _isIdle;
    private int _speedModifier = 25;
    private LineRenderer _paraRope;

    public void Awake()
    {
        _mainMesh = transform.Find("Mesh").gameObject;
        _parachute = transform.Find("Parachute").gameObject;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbodyParachute = _parachute.GetComponent<Rigidbody>();
        _ohCenter = GameObject.Find("OHCenter");
        _animator = _mainMesh.GetComponent<Animator>();

        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        var mat = new Material(smr.material)
        {
            color = new Color(Random.value, Random.value, Random.value, 0.2f)
        };
        smr.material = mat;
        _parachute.GetComponent<MeshRenderer>().material = mat;

        _paraRope = _parachute.AddComponent<LineRenderer>();
        _paraRope.SetVertexCount(3);
        _paraRope.SetWidth(0.1f, 0.1f);
        _paraRope.material = Resources.Load<Material>("paraRopeMat");

        _parachute.SetActive(false);


        var rot = Random.value > 0.5f ? 90 : -90;
        _mainMesh.transform.eulerAngles = new Vector3(0, rot);

#if DEBUG
        _debug = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        _debug.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        DestroyImmediate(_debug.GetComponent<CapsuleCollider>());
        _debug.GetComponent<MeshRenderer>().material = mat;
        _debug.transform.parent = transform;
#endif
    }

    /// <summary>
    /// Updates the model and animation.
    /// </summary>
    private IEnumerator UpdateModel()
    {
        if (_parachuteIsDeployed) yield break;

        _isIdle = true;
        _animator.SetBool("IsIdle", _isIdle);

        yield return new WaitForSeconds(Random.Range(2, 4));

        if (_parachuteIsDeployed) yield break;

        _pos = Random.Range(-3, 3);
        var di = (_ohCenter.transform.position.x + _pos) - transform.position.x;
        _dir = di > 0 ? 1 : -1;
        var rot = di > 0 ? 90 : -90;
        _mainMesh.transform.eulerAngles = new Vector3(0, rot);
        _isIdle = false;
        _animator.SetBool("IsIdle", _isIdle);
        _speedModifier++;
    }



    private void Update()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 1.2f))
        {
            if (!_parachuteIsDeployed)
            {
                _parachuteIsDeployed = true;
                _parachute.SetActive(true);
                _parachute.transform.parent = null;
                _animator.SetBool("IsIdle", true);
                _dir = transform.position.x > 0 ? 1 : -1;
            }

            _paraRope.SetPosition(0, _parachute.transform.position + _parachute.transform.InverseTransformVector(Vector3.left));
            _paraRope.SetPosition(2, _parachute.transform.position + _parachute.transform.InverseTransformVector(Vector3.right));
            _paraRope.SetPosition(1, transform.position);

            _dir = transform.position.x > 0 ? 1 : -1;
            if (_rigidbodyParachute.angularVelocity.magnitude < 1)
                _rigidbodyParachute.AddForce(new Vector3(_dir, 0) * _speedModifier);
        }

        if (!_isIdle && !_parachuteIsDeployed)
        {
            if (_rigidbody.angularVelocity.magnitude < 1)
                _rigidbody.AddForce(new Vector3(_dir, 0) * _speedModifier);

            var dis = Mathf.Abs((_ohCenter.transform.position.x + _pos) - transform.position.x);
            if (dis < 0.2)
            {
                StartCoroutine(UpdateModel());
            }
#if DEBUG
            var tmp = new Vector3(_ohCenter.transform.position.x + _pos, transform.position.y + 2, transform.position.z);
            _debug.transform.position = tmp;
#endif
        }
    }
}
