using UnityEngine;

public class Delete : MonoBehaviour
{
    private DragObject _dragObject;
    private ObjectPool _objectPool;

    private ScoreCounter _score;
    public bool Add;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        _dragObject = GameObject.Find("DragObject").GetComponent<DragObject>();
        _score = GameObject.Find("Score").GetComponent<ScoreCounter>();
    }


    public void OnTriggerEnter(Collider other)
    {
        _dragObject.Dragging = false;
        var o = other.gameObject;
        o.GetComponent<Rigidbody>().isKinematic = true;
        o.transform.position = new Vector3(Random.Range(-10, 10), -20);
        _objectPool.InActive.Add(o);
        if (Add)
            _score.Score++;
        else
            _score.Score--;
    }
}