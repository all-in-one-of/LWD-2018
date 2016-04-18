// This file is part of Leeuwarden-2018
// 
// Copyright (c) 2016 sietze greydanus
// 
// Leeuwarden-2018 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3, as
// published by the Free Software Foundation.
// 
// Leeuwarden-2018 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Leeuwarden-2018. If not, see <http://www.gnu.org/licenses/>.
// 

using UnityEngine;

public class BHPDragObject : MonoBehaviour
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