using UnityEngine;

public class Spawner : MonoBehaviour
{
    private ObjectPool _objectPool;
    private float _timer;
    private Animator _animator;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        _animator = GetComponent<Animator>();
        _timer = Random.Range(5, 20);
    }

    private void Update()
    {
        _animator.SetBool("enabled",false);
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _timer = Random.Range(5, 20);
            _animator.SetBool("enabled", true);
            if (_objectPool.InActive.Count > 0)
            {
                var o = _objectPool.InActive[Random.Range(0, _objectPool.InActive.Count)];
                var v = Random.insideUnitCircle;
                o.transform.position = transform.position + new Vector3(v.x,v.y);
                o.GetComponent<Rigidbody>().isKinematic = false;
                _objectPool.InActive.Remove(o);
            }
            else
            {
                _objectPool.Active.Add(
                    Instantiate(Resources.Load("Inmate"), transform.position, Quaternion.identity) as GameObject);
            }
        }
    }
}