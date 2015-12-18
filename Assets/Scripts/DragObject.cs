using UnityEngine;

public class DragObject : MonoBehaviour
{
    private float _dist;
    private Vector3 _offset;
    private Transform _toDrag;
    public bool Dragging;

    private void Update()
    {
        Vector3 v3;

        if (Input.touchCount != 1)
        {
            Dragging = false;
            return;
        }

        var touch = Input.touches[0];
        Vector3 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Draggable"))
            {
                _toDrag = hit.transform;
                _toDrag.GetComponent<Rigidbody>().isKinematic = true;
                _dist = hit.transform.position.z - Camera.main.transform.position.z;
                v3 = new Vector3(pos.x, pos.y, _dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                _offset = _toDrag.position - v3;
                Dragging = true;
            }
        }
        if (Dragging && touch.phase == TouchPhase.Moved)
        {
            v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist);
            v3 = Camera.main.ScreenToWorldPoint(v3);
            _toDrag.position = v3 + _offset;
        }
        if (Dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            _toDrag.GetComponent<Rigidbody>().isKinematic = false;
            Dragging = false;
        }
    }
}