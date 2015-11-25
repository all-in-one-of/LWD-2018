using UnityEngine;
using UnityEngine.UI;

public class Delete : MonoBehaviour
{
    private DragObject _dragObject;

    private ObjectPool _objectPool;
    public Text Text;
    private int _score;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        _dragObject = GameObject.Find("DragObject").GetComponent<DragObject>();
    }

    public void Start()
    {
        if (Text != null)
            Text.text = "0";
    }

    public void OnTriggerEnter(Collider other)
    {
        _dragObject.Dragging = false;
        var o = other.gameObject;
        o.GetComponent<Rigidbody>().isKinematic = true;
        o.transform.position = new Vector3(-100, -100);
        _objectPool.InActive.Add(o);
        if (Text != null)
        {
            _score++;
            Text.text = "" + _score;
        }
    }
}