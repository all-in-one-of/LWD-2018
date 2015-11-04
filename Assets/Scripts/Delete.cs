using UnityEngine;
using UnityEngine.UI;

public class Delete : MonoBehaviour
{
    private DragObject _dragObject;

    private ObjectPool _objectPool;
    public Text Text;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        _dragObject = GameObject.Find("DragObject").GetComponent<DragObject>();
    }

    public void Start()
    {
        Text.text = "";
    }

    public void OnTriggerEnter(Collider other)
    {
        _dragObject.Dragging = false;
        var o = other.gameObject;
        o.GetComponent<Rigidbody>().isKinematic = true;
        o.transform.position = new Vector3(-100, 0);
        _objectPool.InActive.Add(o);
    }
}