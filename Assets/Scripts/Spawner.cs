using UnityEngine;

public class Spawner : MonoBehaviour
{
    private ObjectPool _objectPool;
    private float _timer;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _timer = Random.Range(2, 10);
            if (_objectPool.InActive.Count > 0)
            {
                var o = _objectPool.InActive[Random.Range(0, _objectPool.InActive.Count)];
                o.transform.position = transform.position;
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