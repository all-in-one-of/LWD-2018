using UnityEngine;

public class BHPSpawner : MonoBehaviour
{
    private Animator _animator;
    private BHPObjectPool _objectPool;
    private float _timer;
    private int spawns;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<BHPObjectPool>();
        _animator = GetComponent<Animator>();
        _timer = Random.Range(0, 30);
    }

    private void Update()
    {
        _animator.SetBool("enabled", false);
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _objectPool.Spawns++;
            print(_objectPool.Spawns);
            _timer = Random.Range(5, 30);
            _animator.SetBool("enabled", true);
            if (_objectPool.InActive.Count > 0)
            {
                var o = _objectPool.InActive[0];
                o.transform.position = transform.position + new Vector3(0.5f, -0.5f);
                o.GetComponent<Rigidbody>().isKinematic = false;
                _objectPool.InActive.Remove(o);
            }
            else
            {
                var o =
                    Instantiate(Resources.Load("Inmate"), transform.position + new Vector3(0.5f, -0.5f),
                        Quaternion.identity) as GameObject;
                _objectPool.Active.Add(o);
                o.transform.name = Random.Range(1000, 9999) + "";
                o.transform.SetParent(_objectPool.transform);
            }
        }
    }
}