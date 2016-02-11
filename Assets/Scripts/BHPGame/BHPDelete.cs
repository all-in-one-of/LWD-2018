using UnityEngine;

public class BHPDelete : MonoBehaviour
{
    private BHPDragObject _dragObject;
    private BHPObjectPool _objectPool;

    private BHPScoreCounter _score;
    public bool Add;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<BHPObjectPool>();
        _dragObject = GameObject.Find("DragObject").GetComponent<BHPDragObject>();
        _score = GameObject.Find("Score").GetComponent<BHPScoreCounter>();
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